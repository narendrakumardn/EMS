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
    public class DistrictDao
    {
        public SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString());
        }
        public List<DistrictBean> DistrictsList()
        {
            List<DistrictBean> beans = new List<DistrictBean>();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from Districts", con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        beans.Add(new DistrictBean(rdr["DistrictCode"].ToString(), rdr["DistrictName"].ToString()));
                    }
                }
                finally
                {
                    con.Close();
                }

            }
            return beans;
        }

        public DistrictBean DistrictData(string disCode)
        {
            DistrictBean bean = null;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from Districts where DistrictCode = @disCode", con);
                    cmd.Parameters.AddWithValue("@disCode", disCode);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        bean = new DistrictBean(rdr["DistrictCode"].ToString(), rdr["DistrictName"].ToString());
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
