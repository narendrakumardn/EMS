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
using WebGrease.Activities;

namespace BTEDiploma.admin
{
    public partial class ViewStudent : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAcademicYears();
               // BindInstitutions();
                BindPrograms();
            }
        }
        private void LoadAcademicYears()
        {
            var result = StudentUploadDao.GetAcademicYearList();
            if (result.StatusCode == 200)
            {
                ddlAcademicYear.DataSource = result.AcademicYears;
                ddlAcademicYear.DataTextField = "AcademicYearDesc";
                ddlAcademicYear.DataValueField = "AcademicYearID";
                ddlAcademicYear.DataBind();
            }
            else
            {
                // Optionally show error message or log: result.Message
            }

        }
       /* private void BindInstitutions()
        {
            var result = InstitutionDAO.GetInstitutionList();
            if (result.StatusCode == 200)
            {
                ddlInstitution.DataSource = result.Institutions;
                ddlInstitution.DataTextField = "Inst_Name";
                ddlInstitution.DataValueField = "Inst_Code";
                ddlInstitution.DataBind();
            }
            else
            {
                // Optionally show error message or log: result.Message
            }

        }*/

        private void BindPrograms()
        {
            int instituteCode;

            // Validate session variable
            if (Session["CollegeCode"] == null ||
                !int.TryParse(Session["CollegeCode"].ToString(), out instituteCode))
            {
                // Session expired or invalid → handle gracefully
                // Example: redirect or show error
                lblErrorMessage.Text = "Session expired. Please login again.";
                return;
            }

            // Pass session variable into DAO
            var result = ProgramDAO.GetProgramList(instituteCode);

            if (result.StatusCode == 200)
            {
                ddlProgram.DataSource = result.Programs;
                ddlProgram.DataTextField = "Program_Name";
                ddlProgram.DataValueField = "Program_Code";
                ddlProgram.DataBind();
            }
            else
            {
                // Show error
                lblErrorMessage.Text = result.Message;
            }
        }

        protected void ddlFilter_Changed(object sender, EventArgs e)
        {
            BindGrid();
        }
        private void BindGrid()
        {
            int academicYearId = Convert.ToInt32(ddlAcademicYear.SelectedValue);
            /* int.TryParse(ddlAcademicYear.SelectedValue, out academicYearId);

             if (academicYearId == 0)
             {
                 lblResult.Text = "Please select a valid academic year.";
                 lblResult.ForeColor = System.Drawing.Color.Red;
                 return;
             }*/

           // int instituteCode = 0;
            if (Session["CollegeCode"] == null || !int.TryParse(Session["CollegeCode"].ToString(), out int instituteCode))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Session expired or Institute Code missing/invalid in session.');", true);
                return;
            }

            instituteCode = Convert.ToInt32(Session["CollegeCode"]);

            //int institutionCode = Convert.ToInt32(ddlInstitution.SelectedValue);
            string programCode = ddlProgram.SelectedValue;

            gvViewStudent.DataSource = StudentUploadDao.GetStudentsForApproval(instituteCode, programCode, academicYearId);
            gvViewStudent.DataBind();
        }









    }
}
