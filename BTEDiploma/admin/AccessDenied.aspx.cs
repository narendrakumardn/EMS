using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BTEDiploma.admin
{
    public partial class AccessDenied : System.Web.UI.Page
    {
     
            protected void Page_Load(object sender, EventArgs e)
            {
                // Optionally log the unauthorized access attempt or show user info
            }

            protected void btnBack_Click(object sender, EventArgs e)
            {
                // Redirect based on user type, if needed
                if (Session["UserType"] != null)
                {
                    int userType = Convert.ToInt32(Session["UserType"]);
                    switch (userType)
                    {
                        case 0:
                            Response.Redirect("~/admin/DashboardAdmin.aspx");
                            break;
                        case 1:
                            Response.Redirect("~/principal/Dashboard.aspx");
                            break;
                        case 10:
                            Response.Redirect("~/admin/SuperAdminDashboard.aspx");
                            break;
                        default:
                            Response.Redirect("~/Login.aspx");
                            break;
                    }
                }
                else
                {
                    Response.Redirect("~/Login.aspx");
                }
            }
        }
    }
