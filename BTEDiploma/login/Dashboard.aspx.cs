using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BTEDiploma.Helper;

namespace BTEDiploma.login
{
    public partial class Dashboard : System.Web.UI.Page
    {


        protected void Page_Load(object sender, EventArgs e)

        {
            if (!IsPostBack)
            {
              
                LoadDashboard();
            }
            
        }

        
        private void LoadDashboard()
        {
            lblColleges.Text = DashboardDao.GetTotalColleges().ToString();
           lblStudents.Text = DashboardDao.GetTotalStudents().ToString();
            lblFaculties.Text = DashboardDao.GetFaculties().ToString();
            lblFeesPaid.Text = DashboardDao.GetExamFeeStatus().ToString();

            

           
        }




    }
}