using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using BTEDiploma.Helper;
using BTEDiploma.sqlhelper;

namespace BTEDiploma.admin
{
    public partial class Courses : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDropdowns();
                LoadCourses();
            }
        }

        private void BindDropdowns()
        {
            ddlPatternID.DataSource = ManagesystemdataDAO.GetAllPatterns();
            ddlPatternID.DataTextField = "Pattern_ID";
            ddlPatternID.DataValueField = "Pattern_ID";
            ddlPatternID.DataBind();
            ddlPatternID.Items.Insert(0, new ListItem("-- Select Pattern --", ""));

            ddlProgramCode.DataSource = ManagesystemdataDAO.GetAllPrograms1();
            ddlProgramCode.DataTextField = "Program_Code";
            ddlProgramCode.DataValueField = "Program_Code";
            ddlProgramCode.DataBind();
            ddlProgramCode.Items.Insert(0, new ListItem("-- Select Program Code --", ""));

            ddlSchemeCode.DataSource = ManagesystemdataDAO.GetAllSchemes();
            ddlSchemeCode.DataTextField = "Scheme_Code";
            ddlSchemeCode.DataValueField = "Scheme_Code";
            ddlSchemeCode.DataBind();
            ddlSchemeCode.Items.Insert(0, new ListItem("-- Select Scheme Code --", ""));

            ddlddlEval_Pattern_ID.DataSource = ManagesystemdataDAO.GetAllEvaluationPatterns();
            ddlddlEval_Pattern_ID.DataTextField = "Evaluation_Pattern_ID";
            ddlddlEval_Pattern_ID.DataValueField = "Evaluation_Pattern_ID";
            ddlddlEval_Pattern_ID.DataBind();
            ddlddlEval_Pattern_ID.Items.Insert(0, new ListItem("-- Select Evaluation Pattern --", ""));
        }

        private void LoadCourses()
        {
            gvCourses.DataSource = ManagesystemdataDAO.GetAllCourses();
            gvCourses.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int statusCode;
            string message;

            bool success = ManagesystemdataDAO.InsertCourse(
                txtCourseCode.Text.Trim(),
                txtCourseName.Text.Trim(),
                Convert.ToInt32(txtCredit.Text.Trim()),
                Convert.ToInt32(ddlPatternID.SelectedValue),
                ddlCoursesSEE_Type.SelectedItem.Text,
                ddlProgramCode.SelectedValue,
                ddlSchemeCode.SelectedValue,
                ddlIsElective.SelectedValue == "1" ? 1 : 0,
                Convert.ToInt32(ddlddlEval_Pattern_ID.SelectedValue),
                Convert.ToInt32(ddlCEEPatternID.SelectedValue),
                out statusCode, out message
            );

            lblMessage.Text = (success && statusCode == 200) ? "✅ " + message : "❌ " + message;
            LoadCourses();
            ClearFields();
        }

        /*protected void gvCourses_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string courseCode = gvCourses.DataKeys[e.RowIndex].Value.ToString();
            if (CoursesDao.DeleteCourse(courseCode))
            {
                lblMessage.Text = "✅ Course deleted.";
                LoadCourses();
            }
        }*/
        protected void gvCourses_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string courseCode = gvCourses.DataKeys[e.RowIndex].Value.ToString();

            int statusCode;
            string message;

            bool success = ManagesystemdataDAO.DeleteCourse(courseCode, out statusCode, out message);

            if (success && statusCode == 200)
            {
                lblMessage.Text = "✅ " + message;
                LoadCourses();
            }
            else
            {
                lblMessage.Text = "❌ Delete failed: " + message;
            }
        }
        protected void gvCourses_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvCourses.EditIndex = e.NewEditIndex;
            LoadCourses();
        }
        protected void gvCourses_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvCourses.EditIndex = -1;
            LoadCourses();
        }
        protected void gvCourses_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = gvCourses.Rows[e.RowIndex];

            string courseCode = gvCourses.DataKeys[e.RowIndex].Value.ToString();
            string courseName = ((TextBox)row.Cells[1].Controls[0]).Text.Trim();
            int credit = Convert.ToInt32(((TextBox)row.Cells[2].Controls[0]).Text.Trim());
            int patternId = Convert.ToInt32(((TextBox)row.Cells[3].Controls[0]).Text.Trim());
            string seeType = ((TextBox)row.Cells[4].Controls[0]).Text.Trim();
            string parentProgramCode = ((TextBox)row.Cells[5].Controls[0]).Text.Trim();
            string schemeCode = ((TextBox)row.Cells[6].Controls[0]).Text.Trim();
            int isElective = ((CheckBox)row.Cells[7].Controls[0]).Checked ? 1 : 0;
            int evalPatternId = Convert.ToInt32(((TextBox)row.Cells[8].Controls[0]).Text.Trim());
            int ciePatternId = Convert.ToInt32(((TextBox)row.Cells[9].Controls[0]).Text.Trim());

            int statusCode;
            string message;

            bool success = ManagesystemdataDAO.UpdateCourse(
                courseCode, courseName, credit, patternId, seeType, parentProgramCode,
                schemeCode, isElective, evalPatternId, ciePatternId, out statusCode, out message
            );

            if (success && statusCode == 200)
            {
                lblMessage.Text = "✅ " + message;
                gvCourses.EditIndex = -1;
                LoadCourses();
            }
            else
            {
                lblMessage.Text = "❌ Update failed: " + message;
            }
        }


        private void ClearFields()
        {
            txtCourseCode.Text = "";
            txtCourseName.Text = "";
            txtCredit.Text = "";
            ddlPatternID.SelectedIndex = 0;
            ddlCoursesSEE_Type.SelectedIndex = 0;
            ddlProgramCode.SelectedIndex = 0;
            ddlSchemeCode.SelectedIndex = 0;
            ddlIsElective.SelectedIndex = 0;
            ddlddlEval_Pattern_ID.SelectedIndex = 0;
            ddlCEEPatternID.SelectedIndex = 0;
        }
    }
}