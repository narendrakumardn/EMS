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
                var ds = dao.GetExamHierarchy();
                ddlAcademicYear.DataSource = ds.Tables[0];
                ddlAcademicYear.DataTextField = "Academic_Year";
                ddlAcademicYear.DataValueField = "Academic_Year";
                ddlAcademicYear.DataBind();
                ddlAcademicYear.Items.Insert(0, new ListItem("Select", ""));

                // Populate Programme
                var dtProgrammes = dao.GetProgrammeList();
                ddlProgramme.DataSource = dtProgrammes;
                //ddlProgramme.DataTextField = "Program_Name";  // Make sure this column exists
                ddlProgramme.DataValueField = "Program_Code";
                ddlProgramme.DataBind();
                ddlProgramme.Items.Insert(0, new ListItem("Select", ""));
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine($"LoadDropdowns error: {ex.Message}");
            }
        }

        protected void DropdownsChanged(object sender, EventArgs e)
        {
            string academic = ddlAcademicYear.SelectedValue;

            // When Academic Year changes: Populate Exam Year and reset Month/Course Grid
            if (sender == ddlAcademicYear)
            {
                var dtYears = dao.GetExamYears(academic);
                ddlExamYear.DataSource = dtYears;
                ddlExamYear.DataTextField = "Exam_Year";
                ddlExamYear.DataValueField = "Exam_Year";
                ddlExamYear.DataBind();
                ddlExamYear.Items.Insert(0, new ListItem("Select", ""));

                ddlExamMonth.Items.Clear();
                ddlExamMonth.Items.Insert(0, new ListItem("Select", ""));

                gvCourses.DataSource = null;
                gvCourses.DataBind();
            }

            // When Exam Year or Academic Year changes: Populate Exam Months as "1 - Jan", ..., "12 - Dec"
            if ((sender == ddlExamYear || sender == ddlAcademicYear) &&
                !string.IsNullOrEmpty(ddlExamYear.SelectedValue) &&
                ddlExamYear.SelectedValue != "Select")
            {
                if (int.TryParse(ddlExamYear.SelectedValue, out int year))
                {
                    var dtMonths = dao.GetExamMonths(academic, year);
                    ddlExamMonth.Items.Clear();
                    ddlExamMonth.Items.Insert(0, new ListItem("Select", ""));

                    foreach (DataRow row in dtMonths.Rows)
                    {
                        if (int.TryParse(row["Exam_Month"].ToString(), out int month))
                        {
                            string monthName = new DateTime(2000, month, 1).ToString("MMM"); // "Jan", "Feb", etc.
                            ddlExamMonth.Items.Add(new ListItem($"{month} - {monthName}", month.ToString()));
                        }
                    }
                }
                else
                {
                    ddlExamMonth.Items.Clear();
                    ddlExamMonth.Items.Insert(0, new ListItem("Select", ""));
                }
            }
        }


        protected void ddlProgramme_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(ddlProgramme.SelectedValue) &&
                    ddlProgramme.SelectedValue != "Select" &&
                    !string.IsNullOrEmpty(ddlAcademicYear.SelectedValue) &&
                    !string.IsNullOrEmpty(ddlExamYear.SelectedValue) &&
                    !string.IsNullOrEmpty(ddlExamMonth.SelectedValue))
                {
                    int examId = dao.GetExamId(ddlAcademicYear.SelectedValue, ddlExamYear.SelectedValue, ddlExamMonth.SelectedValue);
                    var dt = dao.GetCoursesByProgrammeWithTimetable(ddlProgramme.SelectedValue, examId);

                    // Verify the data structure
                    if (dt != null && dt.Columns.Contains("CourseCode")) // Check if column exists
                    {
                        gvCourses.DataKeyNames = new string[] { "CourseCode" }; // Set DataKey
                        gvCourses.DataSource = dt;
                        gvCourses.DataBind();
                    }
                    else
                    {
                        // Handle missing column
                        System.Diagnostics.Debug.WriteLine("Course_Code column not found in data source");
                        gvCourses.DataSource = null;
                        gvCourses.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine($"ddlProgramme_SelectedIndexChanged error: {ex.Message}");
            }
        }


        protected void btnSubmitAll_Click(object sender, EventArgs e)
        {
            try
            {
                int examId = dao.GetExamId(ddlAcademicYear.SelectedValue, ddlExamYear.SelectedValue, ddlExamMonth.SelectedValue);

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