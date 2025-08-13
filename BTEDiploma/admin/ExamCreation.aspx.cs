using System;
using System.Data;
using System.Web.UI.WebControls;
using BTEDiploma.sqlhelper;

namespace BTEDiploma.admin
{
    public partial class ExamCreation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAcademicYears();
                LoadGrid();
            }
        }

        private void LoadAcademicYears()
        {
            DataTable dt = ManagesystemdataDAO.GetAcademicYears();
            ddlAcademicYear.DataSource = dt;
            ddlAcademicYear.DataTextField = "Academic_Year";
            ddlAcademicYear.DataValueField = "Academic_Year";
            ddlAcademicYear.DataBind();
            ddlAcademicYear.Items.Insert(0, new ListItem("Select", ""));
        }

        protected void ddlAcademicYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlYear.Items.Clear();

            if (!string.IsNullOrEmpty(ddlAcademicYear.SelectedValue))
            {
                string[] years = ddlAcademicYear.SelectedValue.Split('-');

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

            ManagesystemdataDAO.InsertExam(
                ddlMonth.SelectedValue,
                ddlYear.SelectedValue,
                ddlAcademicYear.SelectedValue,
                txtStartDate.Text,
                txtEndDate.Text,
                txtRegularFee.Text,
                txtDueDate.Text
            );

            ClearForm();
            LoadGrid();
        }

        private bool ValidateInput()
        {
            if (ddlAcademicYear.SelectedValue == "")
            {
                ShowMessage("Please select academic year.");
                return false;
            }

            if (!DateTime.TryParse(txtStartDate.Text, out DateTime startDate) ||
                !DateTime.TryParse(txtEndDate.Text, out DateTime endDate))
            {
                ShowMessage("Invalid start or end date.");
                return false;
            }

            string[] academicYears = ddlAcademicYear.SelectedValue.Split('-');
            if (academicYears.Length != 2 || !int.TryParse(academicYears[0], out int yearStart))
            {
                ShowMessage("Invalid academic year format.");
                return false;
            }

            DateTime academicStartThreshold = new DateTime(yearStart - 1, 6, 1);
            if (startDate < academicStartThreshold)
            {
                ShowMessage($"Start date should not be before {academicStartThreshold:yyyy-MM-dd}");
                return false;
            }

            if (startDate >= endDate)
            {
                ShowMessage("Start date must be earlier than end date.");
                return false;
            }

            return true;
        }

        private void ShowMessage(string message)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('{message}');", true);
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
            LoadGrid();
        }

        protected void gvExams_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvExams.EditIndex = -1;
            LoadGrid();
        }

        protected void gvExams_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = gvExams.Rows[e.RowIndex];
            int examId = Convert.ToInt32(gvExams.DataKeys[e.RowIndex].Value);

            string academicYear = ((TextBox)row.Cells[1].Controls[0]).Text.Trim();
            int examMonth = int.Parse(((TextBox)row.Cells[2].Controls[0]).Text.Trim());
            int examYear = int.Parse(((TextBox)row.Cells[3].Controls[0]).Text.Trim());
            DateTime startDate = DateTime.Parse(((TextBox)row.Cells[4].Controls[0]).Text.Trim());
            DateTime endDate = DateTime.Parse(((TextBox)row.Cells[5].Controls[0]).Text.Trim());
            decimal fee = decimal.Parse(((TextBox)row.Cells[6].Controls[0]).Text.Trim());
            DateTime dueDate = DateTime.Parse(((TextBox)row.Cells[7].Controls[0]).Text.Trim());

            ManagesystemdataDAO.UpdateExam(examId, academicYear, examMonth, examYear, startDate, endDate, fee, dueDate);

            gvExams.EditIndex = -1;
            LoadGrid();
        }
    }
}
