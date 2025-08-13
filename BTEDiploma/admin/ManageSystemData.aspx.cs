using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BTEDiploma.admin
{
    public partial class ManageSystemData : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserType"] == null || Session["UserType"].ToString() != "10")
            {
                Response.Redirect("~/AccessDenied.aspx");
            }
        }
    }
}