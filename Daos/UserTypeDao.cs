using Beans;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daos
{
    public class UserTypeDao
    {
        public SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString());
        }
        public List<UserTypeBean> CourseList()
        {
            List<UserTypeBean> beans = new List<UserTypeBean>();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from UserType order by Code", con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        beans.Add(new UserTypeBean(Convert.ToInt32(rdr["Code"].ToString()), rdr["Name"].ToString()));
                    }
                }
                finally
                {
                    con.Close();
                }

            }
            return beans;
        }
        public UserTypeBean UserTypeData(int code)
        {
            UserTypeBean bean = null;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from UserType where Code = @code", con);
                    cmd.Parameters.AddWithValue("@code", code);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        bean = new UserTypeBean(Convert.ToInt32(rdr["Code"].ToString()), rdr["Name"].ToString());
                    }
                }
                finally
                {
                    con.Close();
                }

            }
            return bean;
        }
    }
}
