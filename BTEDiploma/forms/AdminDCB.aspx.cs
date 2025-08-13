using BTEDiploma.Helper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BTEDiploma.forms
{

        public partial class AdminDCB : System.Web.UI.Page
        {
            protected void Page_Load(object sender, EventArgs e)
            {
                if (!IsPostBack)
                {
                    LoadAllCollegesDCB();
                }
            }

            private void LoadAllCollegesDCB()
            {
                DataTable dt = SQLHelper.ExecuteStoredProcedure("spGetAllCollegesDCB", null);
                gvColleges.DataSource = dt;
                gvColleges.DataBind();

                gvBranchDCB.DataSource = null;
                gvBranchDCB.DataBind();
            }

            protected void gvColleges_RowCommand(object sender, GridViewCommandEventArgs e)
            {
                if (e.CommandName == "ViewBranches")
                {
                    int rowIndex = Convert.ToInt32(e.CommandArgument);
                    string collegeCode = gvColleges.DataKeys[rowIndex].Value.ToString();
                    LoadBranchwiseDCB(collegeCode);
                }
            }

            private void LoadBranchwiseDCB(string collegeCode)
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter("@CollegeCode", collegeCode)
                };

            lblSelectedCollege.Text = collegeCode;
            DataTable dt = SQLHelper.ExecuteStoredProcedure("spGetCollegeDCB_Branchwise", parameters);
                gvBranchDCB.DataSource = dt;
                gvBranchDCB.DataBind();

            pnlBranchWise.Visible = true; // ✅ Show panel after loading

        }

            // Optional: add this to show no error on export
            public override void VerifyRenderingInServerForm(Control control) { }
        }
    }
