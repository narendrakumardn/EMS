using System;
using System.Web.UI.WebControls;
using System.Data;
using BTEDiploma.sqlhelper;

namespace BTEDiploma.admin
{
    public partial class ExamTimetableadd : System.Web.UI.Page
    {
        ManagesystemdataDAO dao = new ManagesystemdataDAO();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadDropdowns();
        }

        private void LoadDropdowns()
        {
            try
            {
                // Populate Academic Year
                var ds = dao.GetExamHierarchy(); // This calls your SP usp_GetExamHierarchy
                ddlAcademicYear.DataSource = ds.Tables[0];
                ddlAcademicYear.DataTextField = "Academic_Year";
                ddlAcademicYear.DataValueField = "Academic_Year_ID"; // Use ID internally
                ddlAcademicYear.DataBind();
                ddlAcademicYear.Items.Insert(0, new ListItem("Select", ""));

                // Populate Programme
                var dtProgrammes = dao.GetProgrammeList();
                ddlProgramme.DataSource = dtProgrammes;
                ddlProgramme.DataTextField = "Program_Name"; // Make sure this exists
                ddlProgramme.DataValueField = "Program_Code";
                ddlProgramme.DataBind();
                ddlProgramme.Items.Insert(0, new ListItem("Select", ""));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadDropdowns error: {ex.Message}");
            }
        }

        protected void DropdownsChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender == ddlAcademicYear && !string.IsNullOrEmpty(ddlAcademicYear.SelectedValue))
                {
                    // Populate Semester based on selected Academic Year
                    var dtSem = dao.GetSemestersByAcademicYear(Convert.ToInt32(ddlAcademicYear.SelectedValue));
                    ddlSemester.DataSource = dtSem;
                    ddlSemester.DataTextField = "Name";    // Display Name
                    ddlSemester.DataValueField = "sem_ID"; // Use sem_ID internally
                    ddlSemester.DataBind();
                    ddlSemester.Items.Insert(0, new ListItem("Select", ""));
                }

                // Reset Grid when either dropdown changes
                gvCourses.DataSource = null;
                gvCourses.DataBind();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DropdownsChanged error: {ex.Message}");
            }
        }


        protected void ddlProgramme_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(ddlProgramme.SelectedValue) &&
                    !string.IsNullOrEmpty(ddlAcademicYear.SelectedValue) &&
                    !string.IsNullOrEmpty(ddlSemester.SelectedValue))
                {
                    // Get exam_ID for AcademicYear + Semester
                    int examId = dao.GetExamIdByAcademicYearAndSemester(
                        Convert.ToInt32(ddlAcademicYear.SelectedValue),
                        Convert.ToInt32(ddlSemester.SelectedValue)
                    );

                    var dtCourses = dao.GetCoursesByProgrammeWithTimetable(ddlProgramme.SelectedValue, examId);

                    gvCourses.DataKeyNames = new string[] { "CourseCode" };
                    gvCourses.DataSource = dtCourses;
                    gvCourses.DataBind();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ddlProgramme_SelectedIndexChanged error: {ex.Message}");
            }
        }


        protected void btnSubmitAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlAcademicYear.SelectedValue) || string.IsNullOrEmpty(ddlSemester.SelectedValue))
                {
                    lblMessage.Text = "Please select Academic Year and Semester.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                // Get examId using AcademicYear + Semester
                int examId = dao.GetExamIdByAcademicYearAndSemester(
                    Convert.ToInt32(ddlAcademicYear.SelectedValue),
                    Convert.ToInt32(ddlSemester.SelectedValue)
                );

                if (examId <= 0)
                {
                    lblMessage.Text = "Invalid Exam ID. Please check your selections.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                foreach (GridViewRow row in gvCourses.Rows)
                {
                    string courseCode = gvCourses.DataKeys[row.RowIndex]?.Value?.ToString();
                    TextBox txtDate = (TextBox)row.FindControl("txtExamDate");
                    DropDownList ddlSession = (DropDownList)row.FindControl("ddlSession");

                    if (txtDate == null || ddlSession == null || string.IsNullOrWhiteSpace(txtDate.Text))
                        continue;

                    if (!DateTime.TryParse(txtDate.Text, out DateTime examDate))
                        continue;

                    string session = ddlSession.SelectedValue;

                    dao.InsertOrUpdateTimeTable(examId, courseCode, examDate, session);
                }

                lblMessage.Text = "Timetable saved successfully for all courses.";
                lblMessage.ForeColor = System.Drawing.Color.Green;

                // Refresh grid if needed
                ddlProgramme_SelectedIndexChanged(null, null);
            }
            catch (Exception ex)
            {
                lblMessage.Text = "An error occurred while saving: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }


    }
}