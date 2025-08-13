using BTEDiploma.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BTEDiploma.forms
{
   
    
        public partial class DCB : System.Web.UI.Page
        {
            protected void Page_Load(object sender, EventArgs e)
            {
                if (!IsPostBack)
                {
                    if (Session["CollegeCode"] == null)
                    {
                        lblError.Text = "College code is missing. Please login again.";
                        return;
                    }

                    string collegeCode = Session["CollegeCode"].ToString();
                    LoadCollegeDCB(collegeCode);
                    LoadBranchwiseDCB(collegeCode);
                }
            }

            private void LoadCollegeDCB(string collegeCode)
            {
                DataTable dt = DCBDao.GetCollegeDCB_Summary(collegeCode);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    lblCollege.Text = $"College Code: {row["CollegeCode"]}";
                    lblStudents.Text = $"Students: {row["StudentCount"]}";
                    lblDemand.Text = "₹" + Convert.ToDecimal(row["TotalDemand"]).ToString("N2");
                    lblCollection.Text = "₹" + Convert.ToDecimal(row["TotalCollection"]).ToString("N2");
                    lblBalance.Text = "₹" + Convert.ToDecimal(row["Balance"]).ToString("N2");

                    // Call Chart
                    decimal demand = Convert.ToDecimal(row["TotalDemand"]);
                    decimal collection = Convert.ToDecimal(row["TotalCollection"]);
                    decimal balance = Convert.ToDecimal(row["Balance"]);

                    string chartScript = $"updateChart({demand}, {collection}, {balance});";
                    ScriptManager.RegisterStartupScript(this, GetType(), "updateChart", chartScript, true);
                }
                else
                {
                    lblError.Text = "No DCB summary found.";
                }
            }

            private void LoadBranchwiseDCB(string collegeCode)
            {
                DataTable dt = DCBDao.GetCollegeDCB_Branchwise(collegeCode);
                gvBranchDCB.DataSource = dt;
                gvBranchDCB.DataBind();
            }
        }
    }

