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
            if (Session["Institute_Type"] == null || Session["Institute_Type"].ToString() != "20")
            {
                Response.Redirect("~/AccessDenied.aspx");
            }
        }
    }
}