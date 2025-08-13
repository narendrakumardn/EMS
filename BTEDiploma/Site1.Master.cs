using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BTEDiploma.Helper;

namespace BTEDiploma
{
    
    public partial class Site1 : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)

        {    //to Prevent Browser Caching
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.MinValue);

            if (!IsPostBack)
            {
                if (Session["UserType"] == null || Session["Username"] == null || Session["CollegeName"] == null)
                {
                    Response.Redirect("Login.aspx");

                    string currentPage = Request.Url.AbsolutePath.ToLower();
                    if (!currentPage.Contains("sessionexpired.aspx") && !currentPage.Contains("login.aspx"))
                    {
                        Response.Redirect("~/SessionExpired.aspx");
                    }
                }
                else
                {
                    // Display user info   
                    lblUserName.Text = "User: " + Session["CollegeCode"].ToString();
                    lblCollegeName.Text = " | College: " + Session["CollegeName"].ToString();

                    // Set menu visibility
                    ConfigureMenu();
                }
            }
        }



        private void ConfigureMenu()
        {
            int userType = Convert.ToInt32(Session["UserType"]);

            DataTable menuItems = DashboardDao.GetMenusByAccessLevel(userType);

            if (menuItems == null || menuItems.Rows.Count == 0)
                return;

            Dictionary<int, List<DataRow>> groupedMenus = new Dictionary<int, List<DataRow>>();

            foreach (DataRow row in menuItems.Rows)
            {
                int parentId = Convert.ToInt32(row["ParentID"]);
                if (!groupedMenus.ContainsKey(parentId))
                {
                    groupedMenus[parentId] = new List<DataRow>();
                }
                groupedMenus[parentId].Add(row);
            }

            navDynamicMenus.Controls.Clear();

            string currentUrl = Request.Url.AbsolutePath.ToLower();

            if (groupedMenus.ContainsKey(0)) // Top-level menus
            {
                foreach (DataRow topLevelMenu in groupedMenus[0])
                {
                    string topMenuUrl = topLevelMenu["MenuUrl"]?.ToString()?.ToLower() ?? "";
                    string topMenuText = topLevelMenu["MenuName"].ToString();

                    HtmlGenericControl li = new HtmlGenericControl("li");
                    li.Attributes["class"] = "nav-item dropdown";

                    HtmlAnchor a = new HtmlAnchor
                    {
                        InnerText = topMenuText,
                        HRef = "#"
                    };
                    a.Attributes["class"] = "nav-link dropdown-toggle";
                    a.Attributes["data-bs-toggle"] = "dropdown";
                    a.Attributes["role"] = "button";

                    HtmlGenericControl dropdownMenu = new HtmlGenericControl("ul");
                    dropdownMenu.Attributes["class"] = "dropdown-menu";

                    int menuId = Convert.ToInt32(topLevelMenu["MenuID"]);

                    bool isActive = false;

                    if (groupedMenus.ContainsKey(menuId)) // Has children
                    {
                        foreach (DataRow childMenu in groupedMenus[menuId])
                        {
                            string childUrl = childMenu["MenuUrl"].ToString().ToLower();
                            string childText = childMenu["MenuName"].ToString();

                            HtmlGenericControl childLi = new HtmlGenericControl("li");
                            HtmlAnchor childA = new HtmlAnchor
                            {
                                HRef = ResolveUrl(childUrl),
                                InnerText = childText
                            };

                            childA.Attributes["class"] = "dropdown-item";

                            // Active check
                            if (currentUrl.Contains(childUrl))
                            {
                                childA.Attributes["class"] += " active";
                                isActive = true;
                            }

                            childLi.Controls.Add(childA);
                            dropdownMenu.Controls.Add(childLi);
                        }

                        // Apply active class to parent if any child is active
                        if (isActive)
                        {
                            a.Attributes["class"] += " active";
                        }

                        li.Controls.Add(a);
                        li.Controls.Add(dropdownMenu);
                    }
                    else
                    {
                        // No children, render as single link
                        a.HRef = ResolveUrl(topMenuUrl);
                        a.Attributes["class"] = "nav-link";

                        // Active check for top-level menu
                        if (currentUrl.Contains(topMenuUrl))
                        {
                            a.Attributes["class"] += " active";
                        }

                        li.Controls.Clear();
                        li.Controls.Add(a);
                    }

                    navDynamicMenus.Controls.Add(li);
                }
            }
        }



        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon(); // fully ends the session
            Response.Redirect("~/login/Login.aspx");
        }
    }
}
