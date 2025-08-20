using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BTEDiploma.Helper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;

namespace BTEDiploma.admin
{
    public partial class Attendance : System.Web.UI.Page
    {
        AttendanceDAO dao = new AttendanceDAO();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindPrograms();
        }

        private void BindPrograms()
        {
            string instCode = Session["InstituteCode"].ToString();
            ddlProgram.DataSource = dao.GetProgramsByInstitute(instCode);
            ddlProgram.DataTextField = "Program_Name";
            ddlProgram.DataValueField = "Program_Code";
            ddlProgram.DataBind();
            ddlProgram.Items.Insert(0, "-- Select Program --");
        }

        protected void ddlProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlCourse.DataSource = dao.GetCoursesDropdown().Tables[1]; // Courses are 2nd table
            ddlCourse.DataTextField = "Course_Display";
            ddlCourse.DataValueField = "Course_Code";
            ddlCourse.DataBind();
            ddlCourse.Items.Insert(0, "-- Select Course --");
        }

        protected void ddlCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindStudents();
        }

        private void BindStudents()
        {
            string instCode = Session["InstituteCode"].ToString();
            string courseCode = ddlCourse.SelectedValue;
            gvAttendance.DataSource = dao.GetStudentsAttendance(instCode, courseCode);
            gvAttendance.DataBind();
        }

        protected void gvAttendance_RowEditing(object sender, System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            gvAttendance.EditIndex = e.NewEditIndex;
            BindStudents();
        }

        protected void gvAttendance_RowCancelingEdit(object sender, System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
        {
            gvAttendance.EditIndex = -1;
            BindStudents();
        }

        protected void gvAttendance_RowUpdating(object sender, System.Web.UI.WebControls.GridViewUpdateEventArgs e)
        {
            int semHistoryId = Convert.ToInt32(gvAttendance.DataKeys[e.RowIndex].Values["Sem_History_ID"]);
            string courseCode = gvAttendance.DataKeys[e.RowIndex].Values["Course_Code"].ToString();

            var row = gvAttendance.Rows[e.RowIndex];
            int conducted = Convert.ToInt32(((System.Web.UI.WebControls.TextBox)(row.Cells[3].Controls[0])).Text);
            int attended = Convert.ToInt32(((System.Web.UI.WebControls.TextBox)(row.Cells[4].Controls[0])).Text);

            var (status, message) = dao.InsertOrUpdateAttendance(semHistoryId, courseCode, conducted, attended);

            lblMessage.Text = message;
            lblError.Text = status == 200 ? "" : message;

            gvAttendance.EditIndex = -1;
            BindStudents();
        }
    }
}
