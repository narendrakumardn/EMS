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

        private const string VS_Hierarchy = "VS_ExamHierarchy";
        private const string VS_PaymentModes = "VS_PaymentModes";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    // Load hierarchy dataset
                    var dsHierarchy = dao.GetExamHierarchyDropdown();
                    ViewState[VS_Hierarchy] = dsHierarchy;

                    // Academic Years
                    ddlAcademicYear.DataSource = dsHierarchy.Tables[0];
                    ddlAcademicYear.DataTextField = "Academic_Year";
                    ddlAcademicYear.DataValueField = "Academic_Year_ID";
                    ddlAcademicYear.DataBind();
                    ddlAcademicYear.Items.Insert(0, new ListItem("--Select--", ""));

                    // Odd/Even sessions initially blank
                    ddlExamYear.Items.Clear();
                    ddlExamYear.Items.Insert(0, new ListItem("--Select--", ""));
                    var instituteCode = Session["CollegeCode"] != null 
                        ? Convert.ToInt32(Session["CollegeCode"])
                           : 0;

                    if (instituteCode == 0)
                    {
                        ShowError ("Session expired or invalid institute code.");
                        return;
                    }

                    // Programmes
                    var dtProg = dao.GetProgrammeList(instituteCode);
                    ddlProgramme.DataSource = dtProg;
                    ddlProgramme.DataTextField = "Program_Name";
                    ddlProgramme.DataValueField = "Program_Code";
                    ddlProgramme.DataBind();
                    ddlProgramme.Items.Insert(0, new ListItem("--Select--", ""));

                    // Payment modes (cache in viewstate for grid)
                    var dtModes = dao.GetPaymentModes();
                    ViewState[VS_PaymentModes] = dtModes;

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
                ddlExamYear.Items.Insert(0, new ListItem("--Select--", ""));
                gvStudents.DataSource = null;
                gvStudents.DataBind();
                lblMessage.Text = string.Empty;

                if (string.IsNullOrEmpty(ddlAcademicYear.SelectedValue))
                    return;

                var ds = ViewState[VS_Hierarchy] as DataSet;
                if (ds == null || ds.Tables.Count < 2)
                {
                    ShowError("Exam hierarchy not found in ViewState.");
                    return;
                }

                int selectedYearId;
                if (!int.TryParse(ddlAcademicYear.SelectedValue, out selectedYearId))
                {
                    ShowError("Invalid Academic Year selected.");
                    return;
                }

                // 🔹 Bind Odd/Even sessions for this Academic Year
                var rows = ds.Tables[1]
                    .AsEnumerable()
                    .Where(r => r.Field<int>("Academic_Year_ID") == selectedYearId);

                if (rows.Any())
                {
                    DataTable dtSessions = rows.CopyToDataTable();

                    ddlExamYear.DataSource = dtSessions;
                    ddlExamYear.DataTextField = "Sem_Name";   // Odd / Even
                    ddlExamYear.DataValueField = "sem_ID";  // could also use Exam_ID if you want
                    ddlExamYear.DataBind();
                }
                else
                {
                    ShowWarn("No Exam sessions found for the selected Academic Year.");
                }

                ddlExamYear.Items.Insert(0, new ListItem("--Select--", ""));
            }
            catch (Exception ex)
            {
                ShowError("Could not load Exam sessions. " + ex.Message);
            }
        }
        protected void ddlExamYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                gvStudents.DataSource = null;
                gvStudents.DataBind();
                lblMessage.Text = string.Empty;

                if (string.IsNullOrEmpty(ddlAcademicYear.SelectedValue) ||
                    string.IsNullOrEmpty(ddlExamYear.SelectedValue)) return;

                // Use SelectedItem.Text (OddSem/EvenSem) instead of DataTextField
                string oddEven = ddlExamYear.SelectedItem.Text;

                // 🔹 Rebind semester dropdown based on Odd/Even
                BindSemesterDropdown(oddEven);
            }
            catch (Exception ex)
            {
                ShowError("Could not filter semesters. " + ex.Message);
            }
        }

        private void BindSemesterDropdown(string oddEven)
        {
            ddlSemester.Items.Clear();
            ddlSemester.Items.Insert(0, new ListItem("--Select--", ""));

            if (oddEven.Equals("OddSem", StringComparison.OrdinalIgnoreCase))
            {
                ddlSemester.Items.Add(new ListItem("1", "1"));
                ddlSemester.Items.Add(new ListItem("3", "3"));
                ddlSemester.Items.Add(new ListItem("5", "5"));
            }
            else if (oddEven.Equals("EvenSem", StringComparison.OrdinalIgnoreCase))
            {
                ddlSemester.Items.Add(new ListItem("2", "2"));
                ddlSemester.Items.Add(new ListItem("4", "4"));
                ddlSemester.Items.Add(new ListItem("6", "6"));
            }
        }




        protected void ddlSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                gvStudents.DataSource = null;
                gvStudents.DataBind();
                lblMessage.Text = string.Empty;

                if (string.IsNullOrEmpty(ddlAcademicYear.SelectedValue) ||
                    string.IsNullOrEmpty(ddlExamYear.SelectedValue)) return;

                var ds = ViewState[VS_Hierarchy] as DataSet;
                if (ds == null || ds.Tables.Count < 3) return;

                // Step 1: Find Semester row in Table[1]
                var semRow = ds.Tables[1]
                    .AsEnumerable()
                    .FirstOrDefault(r =>
                        r["Academic_Year_ID"].ToString() == ddlAcademicYear.SelectedValue &&
                        r["Sem_ID"].ToString() == ddlExamYear.SelectedValue);

                if (semRow == null)
                {
                    ShowError("Could not find matching Semester for Academic Year.");
                    return;
                }

                string semId = semRow["Sem_ID"].ToString();
                string acadCalId = semRow["Academic_cal_id"].ToString();

                // Step 2: Lookup Exam in Table[2]
                var examRow = ds.Tables[2]
                    .AsEnumerable()
                    .FirstOrDefault(r =>
                        r["Sem_ID"].ToString() == semId &&
                        r["Academic_cal_id"].ToString() == acadCalId);
               
                if (examRow == null)
                {
                    ShowWarn("No Exam found for this Academic Year & Session.");
                    return;
                }

                // ✅ Found Exam
                int examId = Convert.ToInt32(examRow["Exam_ID"]);
                ViewState["SelectedExamId"] = examId;
            }
            catch (Exception ex)
            {
                ShowError("Could not find exam. " + ex.Message);
            }
        }

        protected void ddlProgramme_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadStudentsGrid();
        }

        private void LoadStudentsGrid()
        {
            lblMessage.Text = string.Empty;
            gvStudents.DataSource = null;
            gvStudents.DataBind();

            try
            {
                if (!ValidateSelections(out string message))
                {
                    ShowWarn(message);
                    return;
                }

                lblCaution.Text = "Caution: Please verify the transaction reference number in " +
                "<a href='https://k2.karnataka.gov.in/wps/portal/Khajane-II/Scope/Remittance/DoubleVerification/' " +
                "target='_blank' class='btn btn-sm btn-warning text-dark fw-bold ms-2'>Click Here to Verify in K2</a>. " +
                "If it’s invalid, do not enter it. Otherwise the case worker and principal will be responsible for errors.";

                lblCaution.ForeColor = System.Drawing.Color.Red;

                var instituteCode = Convert.ToString(Session["CollegeCode"]);
                if (string.IsNullOrWhiteSpace(instituteCode))
                {
                    ShowError("Session expired or Institute Code missing in session.");
                    return;
                }

                int examId = (int)ViewState["SelectedExamId"];
                int sem = int.Parse(ddlSemester.SelectedValue);
                var dt = dao.GetStudentsForFee(ddlProgramme.SelectedValue, instituteCode, sem,examId);

                gvStudents.DataKeyNames = new[] { "Sem_History_ID" };
                gvStudents.DataSource = dt;
                gvStudents.DataBind();

                gvStudents.RowDataBound -= gvStudents_RowDataBound;
                gvStudents.RowDataBound += gvStudents_RowDataBound;
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
                // set amount (default 350 if missing)
                if (txtAmount != null)
                {
                    if (drv["Fee_Amount"] == DBNull.Value || string.IsNullOrEmpty(drv["Fee_Amount"].ToString()))
                        txtAmount.Text = "350";
                    else
                        txtAmount.Text = Convert.ToDecimal(drv["Fee_Amount"]).ToString("F2");
                }


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
                    int examId = (int)ViewState["SelectedExamId"];
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
                int examId = (int)ViewState["SelectedExamId"];
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

        private bool ValidateSelections(out string message)
        {
            message = string.Empty;

            if (string.IsNullOrEmpty(ddlAcademicYear.SelectedValue))
            {
                message = "Please select Academic Year."; return false;
            }
            if (string.IsNullOrEmpty(ddlExamYear.SelectedValue))
            {
                message = "Please select Exam Session (Odd/Even)."; return false;
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
