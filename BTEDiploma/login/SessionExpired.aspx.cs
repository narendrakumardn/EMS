using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace BTEDiploma.login
{
    public partial class SessionExpired : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        { 

            Session.RemoveAll();
            Session.Abandon();
            FormsAuthentication.SignOut();
            HttpCookie cookie = new HttpCookie("ASP.NET_SessionId", "");
             cookie.Secure = true;
            Response.Cookies.Add(cookie);

        }
    }
}

