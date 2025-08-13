using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using BTEDiploma.sqlhelper;

namespace BTEDiploma.admin
{
    public partial class AddExamFeeDetailscollege : System.Web.UI.Page
    {
        private readonly ExamFeeDetailsDAO dao = new ExamFeeDetailsDAO();

        // Cache in ViewState so we don't keep hitting DB for hierarchy and modes
        private const string VS_Hierarchy = "VS_ExamHierarchy";
        private const string VS_PaymentModes = "VS_PaymentModes";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    // Load Exam hierarchy (AY, EY, Months/Exam_ID)
                    var dsHierarchy = dao.GetExamHierarchyDropdown();
                    ViewState[VS_Hierarchy] = dsHierarchy;

                    ddlAcademicYear.DataSource = dsHierarchy.Tables[0];
                    ddlAcademicYear.DataTextField = "Academic_Year";
                    ddlAcademicYear.DataValueField = "Academic_Year";
                    ddlAcademicYear.DataBind();
                    ddlAcademicYear.Items.Insert(0, new ListItem("--Select--", ""));

                    // Programmes
                    var dtProg = dao.GetProgrammeList();
                    ddlProgramme.DataSource = dtProg;
                    ddlProgramme.DataTextField = "Program_Name";
                    ddlProgramme.DataValueField = "Program_Code";
                    ddlProgramme.DataBind();
                    ddlProgramme.Items.Insert(0, new ListItem("--Select--", ""));

                    // Payment modes (for RowDataBound)
                    var dtModes = dao.GetPaymentModes();
                    ViewState[VS_PaymentModes] = dtModes;

                    // Clear dependent controls
                    ddlExamYear.Items.Clear();
                    ddlExamYear.Items.Insert(0, new ListItem("--Select--", ""));
                    ddlExamMonth.Items.Clear();
                    ddlExamMonth.Items.Insert(0, new ListItem("--Select--", ""));

                    lblMessage.Text = string.Empty;
                }
                catch (Exception ex)
                {
                    ShowError("Failed to initialize page. " + ex.Message);
                }
            }
        }

        protected void ddlAcademicYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ddlExamYear.Items.Clear();
                ddlExamMonth.Items.Clear();
                ddlExamYear.Items.Insert(0, new ListItem("--Select--", ""));
                ddlExamMonth.Items.Insert(0, new ListItem("--Select--", ""));
                gvStudents.DataSource = null;
                gvStudents.DataBind();
                lblMessage.Text = string.Empty;

                if (string.IsNullOrEmpty(ddlAcademicYear.SelectedValue)) return;

                var ds = ViewState[VS_Hierarchy] as DataSet;
                var dtYears = FilterExamYears(ds, ddlAcademicYear.SelectedValue);

                ddlExamYear.DataSource = dtYears;
                ddlExamYear.DataTextField = "Exam_Year";
                ddlExamYear.DataValueField = "Exam_Year";
                ddlExamYear.DataBind();
                ddlExamYear.Items.Insert(0, new ListItem("--Select--", ""));
            }
            catch (Exception ex)
            {
                ShowError("Could not load exam years. " + ex.Message);
            }
        }

        protected void ddlExamYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ddlExamMonth.Items.Clear();
                ddlExamMonth.Items.Insert(0, new ListItem("--Select--", ""));
                gvStudents.DataSource = null;
                gvStudents.DataBind();
                lblMessage.Text = string.Empty;

                if (string.IsNullOrEmpty(ddlAcademicYear.SelectedValue) ||
                    string.IsNullOrEmpty(ddlExamYear.SelectedValue)) return;

                var ds = ViewState[VS_Hierarchy] as DataSet;
                var dtMonths = FilterExamMonths(ds, ddlAcademicYear.SelectedValue, ddlExamYear.SelectedValue);

                // Build display like "1 - Jan"
                var dtBind = dtMonths.Clone();
                if (!dtBind.Columns.Contains("DisplayText"))
                    dtBind.Columns.Add("DisplayText", typeof(string));

                foreach (DataRow r in dtMonths.Rows)
                {
                    var row = dtBind.NewRow();
                    row["Exam_ID"] = r["Exam_ID"];
                    row["Academic_Year"] = r["Academic_Year"];
                    row["Exam_Year"] = r["Exam_Year"];

                    int month = Convert.ToInt32(r["Exam_Month"]);
                    string monShort = new DateTime(2000, month, 1).ToString("MMM");
                    row["Exam_Month"] = month;
                    row["DisplayText"] = $"{month} - {monShort}";
                    dtBind.Rows.Add(row);
                }

                ddlExamMonth.DataSource = dtBind;
                ddlExamMonth.DataTextField = "DisplayText";
                ddlExamMonth.DataValueField = "Exam_ID"; // IMPORTANT: value = Exam_ID
                ddlExamMonth.DataBind();
                ddlExamMonth.Items.Insert(0, new ListItem("--Select--", ""));
            }
            catch (Exception ex)
            {
                ShowError("Could not load exam months. " + ex.Message);
            }
        }

        // We reuse this handler for Semester too (aspx wired OnSelectedIndexChanged="ddlProgramme_SelectedIndexChanged" for both)
        protected void ddlProgramme_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadStudentsGrid();
        }

        private void LoadStudentsGrid()
        {
            lblMessage.Text = string.Empty;
            lblCaution.Text = string.Empty;
            gvStudents.DataSource = null;
            gvStudents.DataBind();

            try
            {
                if (!ValidateSelections(out string msg))
                {
                    ShowWarn(msg);
                    return;
                }

                // Show caution above grid
                lblCaution.Text = "Caution: Please verify the transaction reference number in " +
                                  "<a href='https://k2.karnataka.gov.in/wps/portal/Khajane-II/Scope/Remittance/DoubleVerification/' target='_blank'>K2</a>. " +
                                  "If it’s invalid, do not enter it. Otherwise the case worker and principal will be responsible for errors.";
                lblCaution.ForeColor = System.Drawing.Color.Red;

                var instituteCode = Convert.ToString(Session["CollegeCode"]);
                if (string.IsNullOrWhiteSpace(instituteCode))
                {
                    ShowError("Session expired or Institute Code missing in session.");
                    return;
                }

                // safe parse semester and examId
                if (!int.TryParse(ddlSemester.SelectedValue, out int sem))
                {
                    ShowWarn("Please select a valid Semester.");
                    return;
                }
                if (!int.TryParse(ddlExamMonth.SelectedValue, out int examId))
                {
                    ShowWarn("Please select a valid Exam Month.");
                    return;
                }

                // get students (LEFT JOIN with payments will happen in DAO)
                var dt = dao.GetStudentsForFee(ddlProgramme.SelectedValue, instituteCode, sem, examId);

                // Build semHistCsv safely (handle DBNull)
                var semHistIds = dt.AsEnumerable()
                    .Select(r => r["Sem_History_ID"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["Sem_History_ID"]))
                    .Where(id => id.HasValue)
                    .Select(id => id.Value.ToString())
                    .ToArray();

                string semHistCsv = semHistIds.Length > 0 ? string.Join(",", semHistIds) : string.Empty;

                DataTable dtPayments = new DataTable();
                if (!string.IsNullOrWhiteSpace(semHistCsv))
                {
                    dtPayments = dao.GetPaymentsByExamAndSemHist(examId, semHistCsv); // should return 0..n rows or empty dt
                }

                // Ensure columns exist on dt for binding (prevents missing column errors)
                if (!dt.Columns.Contains("Payment_ID")) dt.Columns.Add("Payment_ID", typeof(int));
                if (!dt.Columns.Contains("Fee_Amount")) dt.Columns.Add("Fee_Amount", typeof(decimal));
                if (!dt.Columns.Contains("Payment_Mode_ID")) dt.Columns.Add("Payment_Mode_ID", typeof(int));
                if (!dt.Columns.Contains("Transaction_Ref_No")) dt.Columns.Add("Transaction_Ref_No", typeof(string));
                if (!dt.Columns.Contains("Payment_Date")) dt.Columns.Add("Payment_Date", typeof(DateTime));

                foreach (DataRow row in dt.Rows)
                {
                    int semHistId = row["Sem_History_ID"] == DBNull.Value ? 0 : Convert.ToInt32(row["Sem_History_ID"]);
                    DataRow[] match = (dtPayments == null || dtPayments.Rows.Count == 0)
                        ? Array.Empty<DataRow>()
                        : dtPayments.Select($"Sem_History_ID = {semHistId}");

                    if (match.Length > 0)
                    {
                        var p = match[0];
                        row["Payment_ID"] = p["Payment_ID"] == DBNull.Value ? 0 : Convert.ToInt32(p["Payment_ID"]);
                        row["Fee_Amount"] = p["Fee_Amount"] == DBNull.Value ? 0m : Convert.ToDecimal(p["Fee_Amount"]);
                        row["Payment_Mode_ID"] = p["Payment_Mode_ID"] == DBNull.Value ? 0 : Convert.ToInt32(p["Payment_Mode_ID"]);
                        row["Transaction_Ref_No"] = p["Transaction_Ref_No"] == DBNull.Value ? "" : Convert.ToString(p["Transaction_Ref_No"]);
                        row["Payment_Date"] = p["Payment_Date"] == DBNull.Value ? (object)DBNull.Value : Convert.ToDateTime(p["Payment_Date"]);
                    }
                    else
                    {
                        row["Payment_ID"] = 0;
                        row["Fee_Amount"] = DBNull.Value;
                        row["Payment_Mode_ID"] = DBNull.Value;
                        row["Transaction_Ref_No"] = DBNull.Value;
                        row["Payment_Date"] = DBNull.Value;
                    }
                }

                gvStudents.DataKeyNames = new[] { "Sem_History_ID", "Payment_ID" };
                gvStudents.DataSource = dt;
                gvStudents.DataBind();

                if (dt.Rows.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "NoRecordsAlert",
                        "alert('No records present for selected exam and programme');", true);
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to load students. " + ex.Message);
            }
        }


        protected void gvStudents_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            var ddlMode = (DropDownList)e.Row.FindControl("ddlMode");
            var txtPayDate = (TextBox)e.Row.FindControl("txtPayDate");
            var txtAmount = (TextBox)e.Row.FindControl("txtAmount");
            var txtRefNo = (TextBox)e.Row.FindControl("txtRefNo");
            var btnSave = (Button)e.Row.FindControl("btnSave");
            var btnEdit = (Button)e.Row.FindControl("btnEdit");
            var hfPaymentID = (HiddenField)e.Row.FindControl("hfPaymentID");

            // bind payment modes
            var dtModes = ViewState[VS_PaymentModes] as DataTable;
            if (ddlMode != null && dtModes != null)
            {
                ddlMode.DataSource = dtModes;
                ddlMode.DataTextField = "Description";
                ddlMode.DataValueField = "Payment_Mode_ID";
                ddlMode.DataBind();
                ddlMode.Items.Insert(0, new ListItem("--Select--", ""));
            }

            var drv = e.Row.DataItem as DataRowView;
            if (drv != null)
            {
                // Payment ID determines whether this row already has a payment
                int paymentId = drv["Payment_ID"] == DBNull.Value ? 0 : Convert.ToInt32(drv["Payment_ID"]);

                // set amount if present
                if (txtAmount != null)
                    txtAmount.Text = drv["Fee_Amount"] == DBNull.Value ? "" : Convert.ToDecimal(drv["Fee_Amount"]).ToString("F2");

                // set payment mode selected value if exists
                if (ddlMode != null && drv["Payment_Mode_ID"] != DBNull.Value)
                {
                    string val = Convert.ToString(drv["Payment_Mode_ID"]);
                    if (!string.IsNullOrEmpty(val) && ddlMode.Items.FindByValue(val) != null)
                        ddlMode.SelectedValue = val;
                }

                // set ref no
                if (txtRefNo != null)
                    txtRefNo.Text = drv["Transaction_Ref_No"] == DBNull.Value ? "" : Convert.ToString(drv["Transaction_Ref_No"]);

                // set payment date: if existing payment use it, otherwise default to today (date picker)
                if (txtPayDate != null)
                {
                    if (drv["Payment_Date"] == DBNull.Value)
                    {
                        txtPayDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        txtPayDate.Text = Convert.ToDateTime(drv["Payment_Date"]).ToString("yyyy-MM-dd");
                    }
                }

                // Show Save only for rows WITHOUT payment; show Edit only when payment exists
                if (btnSave != null && btnEdit != null)
                {
                    btnSave.Visible = (paymentId == 0);
                    btnEdit.Visible = (paymentId > 0);
                }
            }
        }

        protected void gvStudents_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "SaveRow" && e.CommandName != "EditRow") return;

            // parse row index defensively
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out int rowIndex))
            {
                ShowError("Invalid row index.");
                return;
            }
            if (rowIndex < 0 || rowIndex >= gvStudents.Rows.Count)
            {
                ShowError("Row index out of range.");
                return;
            }

            GridViewRow row = gvStudents.Rows[rowIndex];

            var hfSemHistoryID = row.FindControl("hfSemHistoryID") as HiddenField;
            var hfPaymentID = row.FindControl("hfPaymentID") as HiddenField;
            var txtAmount = row.FindControl("txtAmount") as TextBox;
            var ddlMode = row.FindControl("ddlMode") as DropDownList;
            var txtRefNo = row.FindControl("txtRefNo") as TextBox;
            var txtPayDate = row.FindControl("txtPayDate") as TextBox;

            if (hfSemHistoryID == null)
            {
                ShowError("Missing Sem_History reference.");
                return;
            }
            if (!int.TryParse(hfSemHistoryID.Value, out int semHistId))
            {
                ShowError("Invalid Sem_History_ID in row.");
                return;
            }

            if (!int.TryParse(ddlExamMonth.SelectedValue, out int examId))
            {
                ShowWarn("Please select a valid Exam Month.");
                return;
            }

            // validate user inputs
            if (ddlMode == null || string.IsNullOrWhiteSpace(ddlMode.SelectedValue))
            {
                ShowWarn("Please select payment mode.");
                return;
            }
            if (txtAmount == null || !decimal.TryParse(txtAmount.Text, out decimal amount) || amount < 0)
            {
                ShowWarn("Enter a valid amount.");
                return;
            }
            if (txtPayDate == null || !DateTime.TryParse(txtPayDate.Text, out DateTime payDate))
            {
                ShowWarn("Enter a valid payment date.");
                return;
            }

            int paymentModeId = int.Parse(ddlMode.SelectedValue);
            string refNo = (txtRefNo?.Text ?? string.Empty).Trim();

            try
            {
                if (e.CommandName == "SaveRow")
                {
                    // if hfPaymentID is present and >0, it's already saved
                    if (hfPaymentID != null && int.TryParse(hfPaymentID.Value, out int existingId) && existingId > 0)
                    {
                        ShowWarn("Payment already exists for this student. Use Edit.");
                        return;
                    }

                    // Insert and expect an inserted id (adjust DAO to return new id)
                    var result1 = dao.InsertExamFee(examId, semHistId, payDate, amount, paymentModeId, refNo);
                    int newPaymentId = result1.PaymentId;
                    string msg = result1.Message;
                    if (e.CommandName == "SaveRow")
                    {
                        var result = dao.InsertExamFee(examId, semHistId, payDate, amount, paymentModeId, refNo);
                        if (result.PaymentId > 0)
                            ShowSuccess(result.Message ?? "Saved successfully.");
                        else
                            ShowError(result.Message ?? "Save failed.");
                    }

                    if (newPaymentId > 0)
                        ShowSuccess("Saved successfully.");
                    else
                        ShowError("Save failed (no id returned).");
                }
                else // EditRow
                {
                    if (hfPaymentID == null || !int.TryParse(hfPaymentID.Value, out int paymentId) || paymentId <= 0)
                    {
                        ShowWarn("No existing payment found to edit. Please Save first.");
                        return;
                    }
                    int rows = dao.UpdateExamFee(paymentId, payDate, amount, paymentModeId, refNo);
                    if (rows > 0)
                        ShowSuccess("Updated successfully.");
                    else
                        ShowWarn("No rows updated (maybe unchanged).");
                }

                // refresh
                LoadStudentsGrid();
            }
            catch (Exception ex)
            {
                ShowError("Operation failed. " + ex.Message);
            }
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlExamMonth.SelectedValue))
                {
                    ShowWarn("Please select Exam Month.");
                    return;
                }

                int examId = int.Parse(ddlExamMonth.SelectedValue);
                int success = 0, failed = 0;

                foreach (GridViewRow row in gvStudents.Rows)
                {
                    var hfSemHistoryID = (HiddenField)row.FindControl("hfSemHistoryID");
                    var txtAmount = (TextBox)row.FindControl("txtAmount");
                    var ddlMode = (DropDownList)row.FindControl("ddlMode");
                    var txtRefNo = (TextBox)row.FindControl("txtRefNo");
                    var txtPayDate = (TextBox)row.FindControl("txtPayDate");

                    // Row-level validation
                    if (hfSemHistoryID == null || string.IsNullOrWhiteSpace(hfSemHistoryID.Value)) { failed++; continue; }
                    if (ddlMode == null || string.IsNullOrWhiteSpace(ddlMode.SelectedValue)) { failed++; continue; }
                    if (txtAmount == null || string.IsNullOrWhiteSpace(txtAmount.Text)) { failed++; continue; }
                    if (txtPayDate == null || string.IsNullOrWhiteSpace(txtPayDate.Text)) { failed++; continue; }

                    if (!decimal.TryParse(txtAmount.Text, out var amount) || amount < 0) { failed++; continue; }
                    if (!DateTime.TryParse(txtPayDate.Text, out var payDate)) { failed++; continue; }

                    int semHistId = int.Parse(hfSemHistoryID.Value);
                    int paymentModeId = int.Parse(ddlMode.SelectedValue);
                    string refNo = (txtRefNo?.Text ?? string.Empty).Trim();

                    try
                    {
                        dao.InsertExamFee(examId, semHistId, payDate, amount, paymentModeId, refNo);
                        success++;
                    }
                    catch
                    {
                        failed++;
                    }
                }

                if (success > 0 && failed == 0)
                {
                    ShowSuccess($"Saved {success} payment record(s) successfully.");
                }
                else if (success > 0 && failed > 0)
                {
                    ShowWarn($"Partially saved. Success: {success}, Failed: {failed}. Check invalid rows.");
                }
                else
                {
                    ShowError("No records saved. Please fix validation errors and try again.");
                }
            }
            catch (Exception ex)
            {
                ShowError("Error while saving payments. " + ex.Message);
            }
        }

        // ------- helpers -------

        private bool ValidateSelections(out string message)
        {
            message = string.Empty;

            if (string.IsNullOrEmpty(ddlAcademicYear.SelectedValue))
            {
                message = "Please select Academic Year."; return false;
            }
            if (string.IsNullOrEmpty(ddlExamYear.SelectedValue))
            {
                message = "Please select Exam Year."; return false;
            }
            if (string.IsNullOrEmpty(ddlExamMonth.SelectedValue))
            {
                message = "Please select Exam Month."; return false;
            }
            if (string.IsNullOrEmpty(ddlProgramme.SelectedValue))
            {
                message = "Please select Programme."; return false;
            }
            if (string.IsNullOrEmpty(ddlSemester.SelectedValue))
            {
                message = "Please select Semester."; return false;
            }

            return true;
        }

        private static DataTable FilterExamYears(DataSet ds, string academicYear)
        {
            if (ds == null || ds.Tables.Count < 2) return new DataTable();
            var rows = ds.Tables[1].Select($"Academic_Year = '{academicYear.Replace("'", "''")}'");
            return rows.Length > 0 ? rows.CopyToDataTable() : new DataTable();
        }

        private static DataTable FilterExamMonths(DataSet ds, string academicYear, string examYear)
        {
            if (ds == null || ds.Tables.Count < 3) return new DataTable();
            var rows = ds.Tables[2].Select(
                $"Academic_Year = '{academicYear.Replace("'", "''")}' AND Exam_Year = '{examYear.Replace("'", "''")}'");
            return rows.Length > 0 ? rows.CopyToDataTable() : new DataTable();
        }

        private void ShowSuccess(string msg)
        {
            lblMessage.CssClass = "text-success fw-semibold";
            lblMessage.Text = msg;
        }

        private void ShowWarn(string msg)
        {
            lblMessage.CssClass = "text-warning fw-semibold";
            lblMessage.Text = msg;
        }

        private void ShowError(string msg)
        {
            lblMessage.CssClass = "text-danger fw-semibold";
            lblMessage.Text = msg;
        }
    }
}
