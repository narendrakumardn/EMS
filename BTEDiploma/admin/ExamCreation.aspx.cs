using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using BTEDiploma.Helper;
using BTEDiploma.sqlhelper;
using System.Globalization;

namespace BTEDiploma.admin
{
    public partial class ExamCreation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAcademicYears();
                BindSemesterDropdown();
                LoadGrid();
            }
        }

        protected string GetMonthName(object value)
        {
            if (value == null || value == DBNull.Value) return string.Empty;
            int m;
            if (!int.TryParse(value.ToString(), out m) || m < 1 || m > 12) return string.Empty;
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m);
        }

        private void LoadAcademicYears()
        {
            DataTable dt = ManagesystemdataDAO.GetAcademicYears();
            ddlAcademicYear.DataSource = dt;
            ddlAcademicYear.DataTextField = "Academic_Year";      // shows "2024-2025"
            ddlAcademicYear.DataValueField = "Academic_Year_ID"; // stores int ID
            ddlAcademicYear.DataBind();
            ddlAcademicYear.Items.Insert(0, new ListItem("Select", ""));
        }

        private void BindSemesterDropdown()
        {
            DataTable dt = ManagesystemdataDAO.GetSemesters();
            if (dt != null && dt.Rows.Count > 0)
            {
                ddlSem.DataSource = dt;
                ddlSem.DataTextField = "Name";      // Semester name
                ddlSem.DataValueField = "Sem_Id";   // ID
                ddlSem.DataBind();
            }
            ddlSem.Items.Insert(0, new ListItem("-- Select Semester --", "0"));
        }

        protected void ddlAcademicYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlYear.Items.Clear();

            string academicYearText = ddlAcademicYear.SelectedItem.Text; // e.g. "2024-2025"

            if (!string.IsNullOrEmpty(academicYearText))
            {
                string[] years = academicYearText.Split('-');

                if (years.Length == 2 && int.TryParse(years[0], out int startYear))
                {
                    int displayStartYear = startYear - 1;

                    for (int i = 0; i < 5; i++)
                    {
                        int yearToAdd = displayStartYear + i;
                        ddlYear.Items.Add(new ListItem(yearToAdd.ToString(), yearToAdd.ToString()));
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                int examMonth = int.Parse(ddlMonth.SelectedValue);
                int examYear = int.Parse(ddlYear.SelectedValue);
                int academicYearId = int.Parse(ddlAcademicYear.SelectedValue);
                int semId = int.Parse(ddlSem.SelectedValue);

                DateTime startDate = DateTime.Parse(txtStartDate.Text);
                DateTime endDate = DateTime.Parse(txtEndDate.Text);
                decimal fee = decimal.Parse(txtRegularFee.Text);
                DateTime dueDate = DateTime.Parse(txtDueDate.Text);

                // 🔹 DAO call (contains pre-check for duplicates)
                ManagesystemdataDAO.InsertExam(
                    examMonth, examYear, academicYearId, semId,
                    startDate, endDate, fee, dueDate
                );

                ShowMessage("✅ Record inserted successfully!", "success");
                ClearForm();
                LoadGrid();
            }
            catch (ApplicationException ex)  // expected validation errors (e.g. duplicates)
            {
                ShowMessage("⚠️ " + ex.Message, "warning");
            }
            catch (Exception ex)  // unexpected errors
            {
                ShowMessage("❌ Something went wrong while saving the exam. Please try again. " + ex.Message, "error");
            }
        }

        private bool ValidateInput()
        {
            if (ddlAcademicYear.SelectedValue == "")
            {
                ShowMessage("⚠️ Please select an academic year.", "warning");
                return false;
            }

            // ✅ force exact format for dates
            if (!DateTime.TryParseExact(txtStartDate.Text.Trim(), "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate) ||
                !DateTime.TryParseExact(txtEndDate.Text.Trim(), "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
            {
                ShowMessage("⚠️ Please enter valid Start Date and End Date in format yyyy-MM-dd.", "warning");
                return false;
            }

            // validate due date
            if (!DateTime.TryParseExact(txtDueDate.Text.Trim(), "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dueDate))
            {
                ShowMessage("⚠️ Please enter valid Due Date in format yyyy-MM-dd.", "warning");
                return false;
            }

            // academic year parsing
            string academicYearText = ddlAcademicYear.SelectedItem.Text;
            string[] academicYears = academicYearText.Split('-');

            if (academicYears.Length != 2 ||
                !int.TryParse(academicYears[0].Trim(), out int yearStart) ||
                !int.TryParse(academicYears[1].Trim(), out int yearEnd))
            {
                ShowMessage("⚠️ Academic year must be in format YYYY-YYYY.", "warning");
                return false;
            }

            // academic year range
           

            if (startDate >= endDate)
            {
                ShowMessage("⚠️ Start Date must be earlier than End Date.", "warning");
                return false;
            }

            // ✅ due date check
            if (dueDate < startDate || dueDate > endDate)
            {
                ShowMessage("⚠️ Due Date must be between Start Date and End Date.", "warning");
                return false;
            }

            return true;
        }

        protected void ShowMessage(string message, string type = "success")
        {
            // type can be "success", "danger", "warning", "info"
            phMessage.Controls.Clear();
            var div = new Literal
            {
                Text = $"<div class='alert alert-{type} alert-dismissible fade show' role='alert'>" +
                       $"{message}" +
                       "<button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button>" +
                       "</div>"
            };
            phMessage.Controls.Add(div);
            phMessage.Visible = true;
        }


        private void LoadGrid()
        {
            DataTable dt = ManagesystemdataDAO.GetAllExams();
            gvExams.DataSource = dt;
            gvExams.DataBind();
        }

        private void ClearForm()
        {
            ddlAcademicYear.SelectedIndex = 0;
            ddlYear.Items.Clear();
            ddlMonth.SelectedIndex = 0;
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            txtRegularFee.Text = "";
            txtDueDate.Text = "";
        }

        protected void gvExams_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvExams.EditIndex = e.NewEditIndex;
            LoadGrid(); // triggers RowDataBound for the edit row
        }

        protected void gvExams_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvExams.EditIndex = -1;
            LoadGrid();
        }

        protected void gvExams_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                var row = gvExams.Rows[e.RowIndex];
                int examId = Convert.ToInt32(gvExams.DataKeys[e.RowIndex].Value);

                int academicYearId = int.Parse(((DropDownList)row.FindControl("ddlAcademicYearEdit")).SelectedValue);
                int semId = int.Parse(((DropDownList)row.FindControl("ddlSemEdit")).SelectedValue);
                int examMonth = int.Parse(((DropDownList)row.FindControl("ddlMonthEdit")).SelectedValue);

                string yearText = ((TextBox)row.FindControl("txtExamYearEdit")).Text.Trim();
                string startDateText = ((TextBox)row.FindControl("txtStartDateEdit")).Text.Trim();
                string endDateText = ((TextBox)row.FindControl("txtEndDateEdit")).Text.Trim();
                string feeText = ((TextBox)row.FindControl("txtFeeEdit")).Text.Trim();
                string dueDateText = ((TextBox)row.FindControl("txtDueDateEdit")).Text.Trim();

                // ===== Validation =====
                if (!int.TryParse(yearText, out int examYear))
                {
                    ShowMessage("Invalid Exam Year.", "warning");
                    return;
                }

                if (!DateTime.TryParse(startDateText, out DateTime startDate) ||
                    !DateTime.TryParse(endDateText, out DateTime endDate))
                {
                    ShowMessage("Invalid Start or End date.", "warning");
                    return;
                }

                if (startDate >= endDate)
                {
                    ShowMessage("Start date must be earlier than End date.", "warning");
                    return;
                }

                if (!decimal.TryParse(feeText, out decimal fee))
                {
                    ShowMessage("Invalid fee amount.", "warning");
                    return;
                }

                if (!DateTime.TryParse(dueDateText, out DateTime dueDate))
                {
                    ShowMessage("Invalid Due Date.", "warning");
                    return;
                }

                if (dueDate < startDate || dueDate > endDate)
                {
                    ShowMessage("Due Date must be between Start Date and End Date.", "warning");
                    return;
                }

                // ===== Duplicate check before DB call =====
                DataTable existing = ManagesystemdataDAO.GetAllExams();
                DataRow[] dup = existing.Select(
                    $"Academic_Year_ID = {academicYearId} AND Sem_Id = {semId} AND Exam_Id <> {examId}"
                );
                if (dup.Length > 0)
                {
                    ShowMessage("This Academic Year + Semester already exists. Duplicate not allowed.", "error");
                    return;
                }

                // ===== Update in DB =====
                ManagesystemdataDAO.UpdateExam(
                    examId, academicYearId, examMonth, examYear,
                    startDate, endDate, semId, fee, dueDate
                );

                gvExams.EditIndex = -1;
                LoadGrid();
                ShowMessage("Record updated successfully!", "success");
            }
            catch (Exception ex)
            {
                ShowMessage("Update failed: " + ex.Message, "error");
            }
        }

        protected void gvExams_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            bool isEdit = (e.Row.RowState & DataControlRowState.Edit) == DataControlRowState.Edit;
            if (!isEdit) return;

            // Academic Year dropdown
            var ddlYear = e.Row.FindControl("ddlAcademicYearEdit") as DropDownList;
            if (ddlYear != null)
            {
                var dtYears = ManagesystemdataDAO.GetAcademicYears(); // must return Academic_Year_ID, Academic_Year
                ddlYear.DataSource = dtYears;
                ddlYear.DataTextField = "Academic_Year";
                ddlYear.DataValueField = "Academic_Year_ID";
                ddlYear.DataBind();

                var currentYearIdObj = DataBinder.Eval(e.Row.DataItem, "Academic_Year_ID");
                var currentYearId = (currentYearIdObj == null || currentYearIdObj == DBNull.Value) ? "" : currentYearIdObj.ToString();
                var li = ddlYear.Items.FindByValue(currentYearId);
                if (li != null) ddlYear.SelectedValue = currentYearId;
            }

            // Semester dropdown
            var ddlSem = e.Row.FindControl("ddlSemEdit") as DropDownList;
            if (ddlSem != null)
            {
                var dtSem = ManagesystemdataDAO.GetSemesters(); // must return Sem_Id, Name
                ddlSem.DataSource = dtSem;
                ddlSem.DataTextField = "Name";
                ddlSem.DataValueField = "Sem_Id";
                ddlSem.DataBind();

                var currentSemIdObj = DataBinder.Eval(e.Row.DataItem, "Sem_Id");
                var currentSemId = (currentSemIdObj == null || currentSemIdObj == DBNull.Value) ? "" : currentSemIdObj.ToString();
                var li2 = ddlSem.Items.FindByValue(currentSemId);
                if (li2 != null) ddlSem.SelectedValue = currentSemId;
            }

            // Month dropdown (preselect current month)
            var ddlMonth = e.Row.FindControl("ddlMonthEdit") as DropDownList;
            if (ddlMonth != null)
            {
                var currentMonthObj = DataBinder.Eval(e.Row.DataItem, "Exam_Month");
                var currentMonth = (currentMonthObj == null || currentMonthObj == DBNull.Value) ? "" : currentMonthObj.ToString();
                var li3 = ddlMonth.Items.FindByValue(currentMonth);
                if (li3 != null) ddlMonth.SelectedValue = currentMonth;
            }
        }
    }
}
