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

namespace BTEDiploma.admin
{
    public partial class CreateCurriculumScheme : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAcademicYears();
                LoadGridData();
            }
        }
        private void LoadAcademicYears()
        {
            ddlAcademicYear.Items.Clear();
            ddlAcademicYear.Items.Add(new ListItem("-- Select Year --", ""));

            string connectionString = ConfigurationManager.ConnectionStrings["DBC"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_Get_Academic_Years", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string yearName = reader["Academic_Year"].ToString();
                        string yearId = reader["Academic_Year_ID"].ToString();
                        ddlAcademicYear.Items.Add(new ListItem(yearName, yearId));
                    }

                    reader.Close();
                }
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DBC"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SP_Insert_Curriculum_Scheme", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Scheme_Code", txtCurriculumCode.Text);
                cmd.Parameters.AddWithValue("@Scheme_Name", txtName.Text);
                cmd.Parameters.AddWithValue("@Scheme_Academic_Year_ID", ddlAcademicYear.SelectedValue);
                cmd.Parameters.AddWithValue("@Attendance_Percentage", Convert.ToDecimal(txtAttendance.Text));
                cmd.Parameters.AddWithValue("@AT_Condonable_Percentage", Convert.ToDecimal(txtCondonation.Text));
                cmd.Parameters.AddWithValue("@Weeks", Convert.ToInt32(txtWeeks.Text));
                cmd.Parameters.AddWithValue("@Max_Backlogs", Convert.ToInt32(txtBacklogs.Text));
                con.Open();
                cmd.ExecuteNonQuery();
            }
            ClearFields(sender, e);
            LoadGridData();
        }
        protected void ClearFields(object sender, EventArgs e)
        {
            txtCurriculumCode.Text = "";
            txtName.Text = "";
            ddlAcademicYear.Text = "";
            txtAttendance.Text = "";
            txtCondonation.Text = "";
            txtWeeks.Text = "";
            txtBacklogs.Text = "";
        }
        protected void gvScheme_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvScheme.EditIndex = e.NewEditIndex;
            LoadGridData();
        }
        protected void gvScheme_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvScheme.EditIndex = -1;
            LoadGridData();
        }
        protected void gvScheme_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = gvScheme.Rows[e.RowIndex];

            string code = ((TextBox)row.Cells[0].Controls[0]).Text;
            string name = ((TextBox)row.Cells[1].Controls[0]).Text;
            string year = ((TextBox)row.Cells[2].Controls[0]).Text;
            decimal attendance = Convert.ToDecimal(((TextBox)row.Cells[3].Controls[0]).Text);
            bool condonation = ((TextBox)row.Cells[4].Controls[0]).Text.ToLower() == "true";
            int weeks = Convert.ToInt32(((TextBox)row.Cells[5].Controls[0]).Text);
            int backlogs = Convert.ToInt32(((TextBox)row.Cells[6].Controls[0]).Text);
            string connectionString = ConfigurationManager.ConnectionStrings["DBC"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SP_Update_Curriculum_Scheme", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Scheme_Code", code);
                cmd.Parameters.AddWithValue("@Scheme_Name", name);
                cmd.Parameters.AddWithValue("@Scheme_Academic_Year_ID", year);
                cmd.Parameters.AddWithValue("@Attendance_Percentage", attendance);
                cmd.Parameters.AddWithValue("@AT_Condonable_Percentage", condonation);
                cmd.Parameters.AddWithValue("@Weeks", weeks);
                cmd.Parameters.AddWithValue("@Max_Backlogs", backlogs);

                con.Open();
                cmd.ExecuteNonQuery();
            }
            gvScheme.EditIndex = -1;
            LoadGridData();
        }

        protected void gvScheme_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(gvScheme.DataKeys[e.RowIndex].Value);
            string connectionString = ConfigurationManager.ConnectionStrings["DBC"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeleteCurriculumScheme", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            LoadGridData();
        }
        private void LoadGridData()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DBC"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SP_Get_Curriculum_Scheme_Details", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvScheme.DataSource = dt;
                gvScheme.DataBind();
            }
        }
        protected void gvScheme_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowState.HasFlag(DataControlRowState.Edit))
            {
                DropDownList ddlYear = (DropDownList)e.Row.FindControl("ddlAcademicYear");

                if (ddlYear != null)
                {
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBC"].ConnectionString);
                    SqlCommand cmd = new SqlCommand("SP_Get_Academic_Years", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    ddlYear.DataSource = dr;
                    ddlYear.DataTextField = "Academic_Year_Name";
                    ddlYear.DataValueField = "Academic_Year_ID";
                    ddlYear.DataBind();

                    // Optional: Set selected value based on current value in grid
                    string currentValue = DataBinder.Eval(e.Row.DataItem, "Scheme_Academic_Year_ID").ToString();
                    ddlYear.SelectedValue = currentValue;

                    dr.Close();
                    con.Close();
                }
            }
        }

    }
}