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
    public class TalukDao
    {
        public SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString());
        }
        public List<TalukBean> TaluksList()
        {
            List<TalukBean> beans = new List<TalukBean>();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from MstTaluk", con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        beans.Add(new TalukBean(rdr["Code"].ToString(), rdr["TalukName"].ToString()));
                    }
                }
                finally
                {
                    con.Close();
                }

            }
            return beans;
        }

        public TalukBean TalukData(string Code)
        {
            TalukBean bean = null;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from MstTaluk where Code = @Code", con);
                    cmd.Parameters.AddWithValue("@Code", Code);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        bean = new TalukBean(rdr["Code"].ToString(), rdr["TalukName"].ToString());
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
