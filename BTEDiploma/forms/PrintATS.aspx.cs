using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BTEDiploma.Helper;



namespace BTEDiploma.forms
{
    public partial class PrintATS : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadExamDates();
            }
        }

        private void LoadExamDates()
        {
            string collegeCode = Session["CollegeCode"]?.ToString();
            DataTable dt = ATSDao.GetExamDatesByCollege(collegeCode);
            ddlExamDate.DataSource = dt;
            ddlExamDate.DataTextField = "ExamDate";
            ddlExamDate.DataValueField = "ExamDate";
            ddlExamDate.DataBind();
            ddlExamDate.Items.Insert(0, new ListItem("-- Select Date --", ""));
        }

        protected void ddlExamDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlCourseCode.Items.Clear();
            pnlATSPreview.Visible = false;
            string selectedDate = ddlExamDate.SelectedValue;
            string collegeCode = Session["CollegeCode"]?.ToString();
            DataTable dt = ATSDao.GetCoursesByDateAndCollege(selectedDate, collegeCode);
            ddlCourseCode.DataSource = dt;
            ddlCourseCode.DataTextField = "CourseCode";
            ddlCourseCode.DataValueField = "CourseCode";
            ddlCourseCode.DataBind();
            ddlCourseCode.Items.Insert(0, new ListItem("-- Select Course --", ""));
        }

        protected void ddlCourseCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedDate = ddlExamDate.SelectedValue;
            string courseCode = ddlCourseCode.SelectedValue;
            string collegeCode = Session["CollegeCode"]?.ToString();
            int count = ATSDao.GetStudentCount(selectedDate, courseCode, collegeCode);
            lblStudentCount.Text = "Total Students: " + count.ToString();
            pnlATSPreview.Visible = false;
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            string selectedDate = ddlExamDate.SelectedValue;
            string courseCode = ddlCourseCode.SelectedValue;
            string collegeCode = Session["CollegeCode"]?.ToString();
            string fromStr = txtFrom.Text.Trim();
            string toStr = txtTo.Text.Trim();

            if (!int.TryParse(fromStr, out int from) || !int.TryParse(toStr, out int to) || from < 1 || to > 25 || from > to)
            {
                lblStudentCount.Text = "Invalid range. Must be between 1 and 25.";
                return;
            }

            DataTable dt = ATSDao.GenerateOrFetchATS(selectedDate, courseCode, collegeCode, from, to);
            gvATSPreview.DataSource = dt;
            gvATSPreview.DataBind();

            // Display header info
            lblCollegeInfo.Text = collegeCode + " - " + Session["CollegeName"];
            lblExamInfo.Text = "Course: " + courseCode + ", Exam Date: " + selectedDate;

            pnlATSPreview.Visible = true;
        }
    }
}
