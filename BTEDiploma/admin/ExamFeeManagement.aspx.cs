using System;
using System.Data;
using System.Web.UI.WebControls;
using BTEDiploma.sqlhelper;

namespace BTEDiploma.admin
{
    public partial class ExamFeeManagement : System.Web.UI.Page
    {
        ManagesystemdataDAO dao = new ManagesystemdataDAO();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlAcademicYear.DataSource = dao.GetAcademicYears1();
                ddlAcademicYear.DataTextField = "Academic_Year";
                ddlAcademicYear.DataValueField = "Academic_Year_ID";
                ddlAcademicYear.DataBind();
                ddlAcademicYear.Items.Insert(0, new ListItem("--Select--", ""));
            }
        }

        protected void ddlOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlBacklog.Visible = ddlOperation.SelectedValue == "Backlog";
            pnlLateFine.Visible = ddlOperation.SelectedValue == "LateFine";
            LoadGrid();
        }

        protected void ddlAcademicYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlSemester.DataSource = dao.GetSemesters1();
            ddlSemester.DataTextField = "Name";   // Display name
            ddlSemester.DataValueField = "Sem_Id"; // Backend ID
            ddlSemester.DataBind();
            ddlSemester.Items.Insert(0, new ListItem("--Select--", ""));
            LoadGrid();
        }


        protected void ddlSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadGrid();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlAcademicYear.SelectedValue) || string.IsNullOrEmpty(ddlSemester.SelectedValue))
            {
                lblMessage.Text = "Please select Academic Year and Semester";
                lblMessage.CssClass = "alert alert-danger";
                lblMessage.Visible = true;
                return;
            }

            if (ddlOperation.SelectedValue == "Backlog")
                dao.InsertOrUpdateBacklogFeeByYearSemester(Convert.ToInt32(ddlAcademicYear.SelectedValue),
                                                            Convert.ToInt32(ddlSemester.SelectedValue),
                                                            Convert.ToInt32(ddlBacklogCount.SelectedValue),
                                                            Convert.ToDecimal(txtAmount.Text.Trim()));
            else if (ddlOperation.SelectedValue == "LateFine")
                dao.InsertOrUpdateLateFineByYearSemester(Convert.ToInt32(ddlAcademicYear.SelectedValue),
                                                          Convert.ToInt32(ddlSemester.SelectedValue),
                                                          Convert.ToDateTime(txtLastDate.Text),
                                                          Convert.ToDecimal(txtAmount.Text.Trim()));

            lblMessage.Text = "Saved successfully!";
            lblMessage.CssClass = "alert alert-success";
            lblMessage.Visible = true;
            LoadGrid();
        }

        private void LoadGrid()
        {
            if (!string.IsNullOrEmpty(ddlAcademicYear.SelectedValue) && !string.IsNullOrEmpty(ddlSemester.SelectedValue))
            {
                gvData.DataSource = dao.GetGridDataByYearSemester(ddlOperation.SelectedValue,
                                                                   Convert.ToInt32(ddlAcademicYear.SelectedValue),
                                                                   Convert.ToInt32(ddlSemester.SelectedValue));
                gvData.DataBind();
            }
        }

        protected void gvData_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvData.EditIndex = e.NewEditIndex;
            LoadGrid();
        }

        protected void gvData_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvData.EditIndex = -1;
            LoadGrid();
        }

        protected void gvData_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int examID = Convert.ToInt32(gvData.DataKeys[e.RowIndex].Value);
            GridViewRow row = gvData.Rows[e.RowIndex];
            TextBox txtValue = row.FindControl("txtEditValue") as TextBox;
            TextBox txtFee = row.FindControl("txtEditFee") as TextBox;

            if (ddlOperation.SelectedValue == "Backlog")
                dao.UpdateBacklogFee(examID, Convert.ToInt32(txtValue.Text.Trim()), Convert.ToDecimal(txtFee.Text.Trim()));
            else if (ddlOperation.SelectedValue == "LateFine")
                dao.UpdateLateFine(examID, Convert.ToDateTime(txtValue.Text.Trim()), Convert.ToDecimal(txtFee.Text.Trim()));

            gvData.EditIndex = -1;
            LoadGrid();
        }
    }
}
