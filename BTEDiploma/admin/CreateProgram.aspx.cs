using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using BTEDiploma.Helper;
using BTEDiploma.sqlhelper;
namespace BTEDiploma.admin
{
    public partial class CreateProgram : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadProgramTypeList();
                LoadGridData();
            }
        }
        private void LoadGridData()
        {
            {
                DataTable dt = ManagesystemdataDAO.GetProgramList();
                gvProgram.DataSource = dt;
                gvProgram.DataBind();
            }
        }
        private void LoadProgramTypeList()
        {
            ddlProgramType.Items.Clear();
            ddlProgramType.Items.Add(new ListItem("-- Select Program Type --", ""));
            DataTable dt = ManagesystemdataDAO.GetProgramTypeList();
            foreach (DataRow row in dt.Rows)
            {
                string name = row["Program_Type_Name"].ToString();
                string id = row["Program_Type_ID"].ToString();
                ddlProgramType.Items.Add(new ListItem(name, id));
            }
        }
        protected void btnAddProgram_Click(object sender, EventArgs e)
        {
            string Program_Code = txtProgramCode.Text;
            string Program_Name = txtProgramName.Text;
            int Program_Type_ID = int.Parse(ddlProgramType.SelectedValue);
            decimal credit = Convert.ToDecimal(txtminCredit.Text);
            string Diploma_Title = txtDiplomaAwardTitle.Text;
            ManagesystemdataDAO.AddPrograms(Program_Code, Program_Name, Program_Type_ID, credit, Diploma_Title);
            ClearFields(sender, e);
            LoadGridData();
        }
        protected void ClearFields(object sender, EventArgs e)
        {
            txtProgramCode.Text = "";
            txtProgramName.Text = "";
            ddlProgramType.Text = "";
            txtminCredit.Text = "";
            txtDiplomaAwardTitle.Text = "";
        }
        protected void gvProgram_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvProgram.EditIndex = e.NewEditIndex;
            LoadGridData();
        }
        protected void gvProgram_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvProgram.EditIndex = -1;
            LoadGridData();
        }
        protected void gvProgram_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            gvProgram.PageIndex = e.NewPageIndex;
            LoadGridData();
        }
        protected void gvProgram_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = gvProgram.Rows[e.RowIndex];
            string Program_Code = ((Label)row.FindControl("lblProgramCode")).Text;
            string Program_Name = ((TextBox)row.FindControl("txtProgramName")).Text;
            DropDownList ddlProgramTypeGrid = (DropDownList)row.FindControl("ddlProgramTypeGrid");
            int Program_Type_ID = Convert.ToInt32(ddlProgramTypeGrid.SelectedValue);
            string creditText = ((TextBox)row.FindControl("txtCredit"))?.Text;
            decimal Credit = 0;
            decimal.TryParse(creditText, out Credit);  // Safer than Convert.ToDecimal
            string Diploma_Title = ((TextBox)row.FindControl("txtDiplomaTitle"))?.Text;
            ManagesystemdataDAO.AddPrograms(Program_Code, Program_Name, Program_Type_ID, Credit, Diploma_Title);
            gvProgram.EditIndex = -1;
            ClearFields(sender, e);
            LoadGridData();
        }
        protected void gvProgram_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && (e.Row.RowState & DataControlRowState.Edit) > 0)
            {
                DropDownList ddlProgramTypeGrid = (DropDownList)e.Row.FindControl("ddlProgramTypeGrid");
                if (ddlProgramTypeGrid != null)
                {
                    ddlProgramTypeGrid.DataSource = ManagesystemdataDAO.GetProgramTypeList();
                    ddlProgramTypeGrid.DataTextField = "Program_Type_Name";
                    ddlProgramTypeGrid.DataValueField = "Program_Type_ID";
                    ddlProgramTypeGrid.DataBind();
                    // Select the current value
                    string selectedValue = DataBinder.Eval(e.Row.DataItem, "Program_Type_Name").ToString();
                    ddlProgramTypeGrid.SelectedValue = selectedValue;
                }
            }
        }
    }
}
