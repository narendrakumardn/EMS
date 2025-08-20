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
    public partial class StudentUploadBulk : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAcademicYears();
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



        protected void btnImport_Click(object sender, EventArgs e)
        {
            //StudentImportDAO dao = new StudentImportDAO();
            int academicYearId = Convert.ToInt32(ddlAcademicYear.SelectedValue);
            /*int academicYearId = 0;
            int.TryParse(ddlAcademicYear.SelectedValue, out academicYearId);

            if (academicYearId == 0)
            {
                lblResult.Text = "Please select a valid academic year.";
                lblResult.ForeColor = System.Drawing.Color.Red;
                return;
            }*/

            var result = StudentUploadDao.ImportStudentsFromSource(academicYearId);

            lblResult.Text = $"Status: {result.StatusCode} - {result.Message}, Students: {result.StudentsProcessed}";

            lblResult.ForeColor = result.StatusCode == 200 ? System.Drawing.Color.Green : System.Drawing.Color.Red;
        }





    }
}
