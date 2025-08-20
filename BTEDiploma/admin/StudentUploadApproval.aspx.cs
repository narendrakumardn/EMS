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
    public partial class StudentUploadApproval : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindInstitutions();
                BindPrograms();
                LoadAcademicYears();
                //BindGrid();
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
        protected void ddlFilter_Changed(object sender, EventArgs e)
        {
            BindGrid();
        }

        private void BindInstitutions()
        {
            var result = InstitutionDAO.GetInstitutionList();
            if (result.StatusCode == 200)
            {
                // Project into a new list with "Inst_Code - Inst_Name" as DisplayText
                var institutions = result.Institutions
                    .Select(i => new
                    {
                        Inst_Code = i.Inst_Code,
                        DisplayText = i.Inst_Code + " - " + i.Inst_Name
                    })
                    .ToList();

                ddlInstitution.DataSource = institutions;
                ddlInstitution.DataTextField = "DisplayText";  // what user sees
                ddlInstitution.DataValueField = "Inst_Code";   // actual value
                ddlInstitution.DataBind();

                ddlInstitution.Items.Insert(0, new ListItem("--Select Institution--", ""));
            }
            else
            {
                // Optionally show error message
            }
        }
    //ddlInstitution.DataSource = InstitutionDAO.GetInstitutionList();
            // ddlInstitution.DataTextField = "Inst_Name";
            //ddlInstitution.DataValueField = "Inst_Code";
            //ddlInstitution.DataBind();
        

        private void BindPrograms()
        {
            if (string.IsNullOrEmpty(ddlInstitution.SelectedValue))
                return; // No institution selected yet

            int instituteCode = Convert.ToInt32(ddlInstitution.SelectedValue);

            var result = ProgramDAO.GetProgramList(instituteCode);
            if (result.StatusCode == 200)
            {
                ddlProgram.DataSource = result.Programs;
                ddlProgram.DataTextField = "Program_Name";
                ddlProgram.DataValueField = "Program_Code";
                ddlProgram.DataBind();

                ddlProgram.Items.Insert(0, new ListItem("--Select Program--", ""));
            }
            else
            {
                // Optionally show error message or log
                lblMessage.Text = result.Message;
            }
        }


        //ddlProgram.DataSource = ProgramDAO.GetProgramList();
        //ddlProgram.DataTextField = "Program_Name";
        //ddlProgram.DataValueField = "Program_Code";
        //ddlProgram.DataBind();

        protected void ddlInstitution_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindPrograms();
        }

        private void BindGrid()
        {
            int academicYearId = Convert.ToInt32(ddlAcademicYear.SelectedValue);
            int institutionCode = Convert.ToInt32(ddlInstitution.SelectedValue);
            string programCode = ddlProgram.SelectedValue;

            gvStudents.DataSource = StudentUploadDao.GetStudentsForApproval(institutionCode, programCode, academicYearId);
            gvStudents.DataBind();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gvStudents.Rows)
            {
                int studentId = Convert.ToInt32(gvStudents.DataKeys[row.RowIndex].Values["Student_ID"]);
                int studentEnrollmentID = Convert.ToInt32(gvStudents.DataKeys[row.RowIndex].Values["Student_Enrollment_ID"]);
                CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");

                if (chkApproved != null)
                {
                    StudentUploadDao.UpdateACMApproval(studentId, studentEnrollmentID, chkApproved.Checked);
                }
            }

            lblMessage.Text = "Approval statuses updated successfully.";
            BindGrid();
        }





    }
}
