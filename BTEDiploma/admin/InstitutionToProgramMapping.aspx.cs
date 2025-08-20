using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BTEDiploma.sqlhelper;

namespace BTEDiploma.admin
{
    public partial class InstituteProgramMapping : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindInstitutionTypes();

                BindInstitutions();

                BindPrograms();
                pnlAddNew.Visible = false;
            }
        }

        private void BindInstitutionTypes()
        {
            DataTable dt = ManagesystemdataDAO.GetAllInstitutionTypes(); // Should return Institution_Type_Name

            ddlInstitutionType.DataSource = dt;
            ddlInstitutionType.DataTextField = "Institution_Type_Description";
            ddlInstitutionType.DataValueField = "Institution_Type_ID"; // Use name as value too
            ddlInstitutionType.DataBind();

            ddlInstitutionType.Items.Insert(0, new ListItem("--Select Type--", ""));
        }



        protected void ddlInstitutionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedTypeId;
            if (int.TryParse(ddlInstitutionType.SelectedValue, out selectedTypeId))
            {
                BindInstitutionsByType(selectedTypeId);
            }
            else
            {
                ddlInstitution.Items.Clear();
                ddlInstitution.Items.Insert(0, new ListItem("--Select--", ""));
            }
        }


        private void BindInstitutionsByType(int institutionTypeId)
        {
            DataTable dt = ManagesystemdataDAO.GetInstitutionsByType(institutionTypeId);

            dt.Columns.Add("InstDisplay", typeof(string));
            foreach (DataRow row in dt.Rows)
            {
                row["InstDisplay"] = $"{row["Inst_Code"]} - {row["Inst_Name"]}";
            }

            ddlInstitution.DataSource = dt;
            ddlInstitution.DataTextField = "InstDisplay";
            ddlInstitution.DataValueField = "Inst_Code";
            ddlInstitution.DataBind();
            ddlInstitution.Items.Insert(0, new ListItem("--Select--", ""));
        }



        private void BindInstitutions()
        {

            var dao = new ManagesystemdataDAO();
            DataTable dt = dao.GetAllInstitutions();


            // Add a new column to combine name and code
            dt.Columns.Add("InstDisplay", typeof(string));
            foreach (DataRow row in dt.Rows)
            {
                row["InstDisplay"] = $"{row["Inst_Code"]} - {row["Inst_Name"]}";
            }

            ddlInstitution.DataSource = dt;
            ddlInstitution.DataTextField = "InstDisplay"; // Show both code and name
            ddlInstitution.DataValueField = "Inst_Code";  // Value is still just the code
            ddlInstitution.DataBind();
            ddlInstitution.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void BindPrograms()
        {
            var dao = new ManagesystemdataDAO();

            DataTable dt = dao.GetAllPrograms();
            ddlProgram.DataSource = dt;
            ddlProgram.DataTextField = "Program_Name";
            ddlProgram.DataValueField = "Program_Code";
            ddlProgram.DataBind();
        }


        protected void ddlInstitution_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = ""; // ✅ also clear previous message
            if (!string.IsNullOrEmpty(ddlInstitution.SelectedValue))
            {
                BindGrid();
                pnlAddNew.Visible = true;

            }
        }

        private void BindGrid()
        {
            int instCode = Convert.ToInt32(ddlInstitution.SelectedValue);
            var dao = new ManagesystemdataDAO();
            DataTable dt = dao.GetProgramMappingsByInstitution(instCode);
            gvMappings.DataSource = dt;
            gvMappings.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            int instCode = Convert.ToInt32(ddlInstitution.SelectedValue);
            string progId = ddlProgram.SelectedValue;
            int year = int.Parse(txtYear.Text.Trim());
            int intake = Convert.ToInt32(ddlIntake.SelectedValue);

            bool isActive = chkActive.Checked;
            bool isAided = chkAided.Checked;

            try
            {
                var dao = new ManagesystemdataDAO();
                bool success = dao.InsertInstitutionProgramMapping(
                    instCode, progId, year, intake, isActive, isAided);

                if (success)
                {
                    lblMessage.Text = "✅ Mapping added successfully.";
                    BindGrid();
                    ClearFields();
                }
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Mapping already exists"))
                {
                    lblMessage.Text = "⚠️ Mapping already exists for this institution and program.";
                    ClearFields();
                }
                else
                {
                    lblMessage.Text = "❌ Database error: " + ex.Message;
                    ClearFields();
                }
            }

        }
        private void ClearFields()
        {
            ddlInstitution.SelectedIndex = 0;
            ddlProgram.SelectedIndex = 0;
            txtYear.Text = "";
            ddlIntake.SelectedIndex = 0;
            chkActive.Checked = false;
            chkAided.Checked = false;
        }

        protected void gvMappings_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvMappings.EditIndex = e.NewEditIndex;
            BindGrid(); // reload the data again
        }
        protected void gvMappings_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvMappings.EditIndex = -1;
            BindGrid();
        }
        protected void gvMappings_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = gvMappings.Rows[e.RowIndex];


            int instCode = Convert.ToInt32(gvMappings.DataKeys[e.RowIndex].Values["Inst_Code"]);

            string progId = gvMappings.DataKeys[e.RowIndex].Values["Program_Code"].ToString();

            int affiliationYear = int.Parse(((TextBox)row.Cells[1].Controls[0]).Text);
            int intake = int.Parse(((TextBox)row.Cells[2].Controls[0]).Text);
            bool isActive = ((CheckBox)row.Cells[3].Controls[0]).Checked;
            bool isAided = ((CheckBox)row.Cells[4].Controls[0]).Checked;

            // Call your DAO method to update the row
            var dao = new ManagesystemdataDAO();
            bool updated = dao.UpdateInstitutionProgramMapping(instCode, progId, affiliationYear, intake, isActive, isAided);

            if (updated)
            {
                gvMappings.EditIndex = -1;
                BindGrid(); // refresh data
            }
            else
            {
                // show error
                lblMessage.Text = "Update failed.";
            }
        }



    }
}