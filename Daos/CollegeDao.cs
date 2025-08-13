using Beans;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daos
{
    public class CollegeDao
    {
        public SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString());
        }
        public DataSet Get_Govt_Dte_Letter_Data(string reg_id)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter("select LetterFileData,LetterNo,LetterContentType from [dbo].[CircularLetters] where Id=@reg_id", con);
                    da.SelectCommand.Parameters.AddWithValue("@reg_id", reg_id);
                    da.Fill(ds);
                }
                finally
                {
                    con.Close();
                }

            }
            return ds;
        }
        public string GetCollegeGroup(string clgCode)
        {
            string appNo1 = string.Empty;
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from dbo.Colleges where CollegeCode = @clgCode", con);
                    cmd.Parameters.AddWithValue("@clgCode", clgCode);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        appNo1 = rdr["CollegeGroup"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                }
                return appNo1;
            }
        }
        public List<CollegeBean> CollegeList()
        {
            List<CollegeBean> beans = new List<CollegeBean>();
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from Colleges order by CollegeCode", con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        beans.Add(new CollegeBean(rdr["CollegeGroup"].ToString(), rdr["CollegeCode"].ToString(), "(" + rdr["CollegeCode"].ToString() + ") " + rdr["CollegeName"].ToString(),
                                                  rdr["CollegeType"].ToString(), new DistrictDao().DistrictData(rdr["DistrictCode"].ToString()),
                                                  rdr["CollegeGender"].ToString(), rdr["SecondShift"].ToString(), rdr["RkHKCollege"].ToString(), new TalukDao().TalukData(rdr["Fk_TalukCode"].ToString()), rdr["IsNodal"].ToString()));
                    }
                }
                finally
                {
                    con.Close();
                }

            }
            return beans;
        }


        public List<CollegeBean> GetAllColleges()
        {
            List<CollegeBean> list = new List<CollegeBean>();

            // Always use a fresh SqlConnection inside a using block
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                string query = "SELECT CollegeCode, CollegeName FROM Colleges ORDER BY CollegeCode";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open(); // This is safe here since it's a new connection
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            CollegeBean college = new CollegeBean
                            {
                                code = dr["CollegeCode"].ToString(),
                                name = dr["CollegeName"].ToString()
                            };
                            list.Add(college);
                        }
                    }
                }
            }

            return list;
        }



        public List<CollegeBean> CollegeListOnlyEngineeringColleges()
        {
            List<CollegeBean> beans = new List<CollegeBean>();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from Colleges WHERE COLLEGECODE <> '100' and CollegeGroup='E' order by CollegeCode,DistrictCode", con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        beans.Add(new CollegeBean(rdr["CollegeGroup"].ToString(), rdr["CollegeCode"].ToString(), "(" + rdr["CollegeCode"].ToString() + ") " + rdr["CollegeName"].ToString(),
                                                  rdr["CollegeType"].ToString(), new DistrictDao().DistrictData(rdr["DistrictCode"].ToString()),
                                                  rdr["CollegeGender"].ToString(), rdr["SecondShift"].ToString(), rdr["RkHKCollege"].ToString(), new TalukDao().TalukData(rdr["Fk_TalukCode"].ToString()), rdr["IsNodal"].ToString()));
                    }
                }
                finally
                {
                    con.Close();
                }

            }
            return beans;
        }
        public DataSet CollegeListBasedOnCollegeGroup(string cGroup)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter("select CollegeCode as CCODE,CollegeName as NAME from dbo.Colleges where CollegeGroup  = @cGroup and CollegeType = 'G'", con);
                    da.SelectCommand.Parameters.AddWithValue("@cGroup", cGroup);
                    da.Fill(ds);
                }
                finally
                {
                    con.Close();
                }
            }
            return ds;
        }
        public CollegeBean CollegeData(string clgCode)
        {
            CollegeBean bean = null;
            if (string.IsNullOrEmpty(clgCode))
                return null;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from Colleges where CollegeCode = @clgCode order by CollegeCode,DistrictCode", con);
                    cmd.Parameters.AddWithValue("@clgCode", clgCode);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        bean = new CollegeBean(rdr["CollegeGroup"].ToString(), rdr["CollegeCode"].ToString(), rdr["CollegeName"].ToString(),
                                                  rdr["CollegeType"].ToString(), new DistrictDao().DistrictData(rdr["DistrictCode"].ToString()),
                                                  rdr["CollegeGender"].ToString(), rdr["SecondShift"].ToString(), rdr["RkHKCollege"].ToString(), new TalukDao().TalukData(rdr["Fk_TalukCode"].ToString()), rdr["IsNodal"].ToString());
                    }
                }
                finally
                {
                    con.Close();
                }

            }
            return bean;
        }
        public List<CollegeBean> CollegeDataBasedOnType(string type)
        {
            List<CollegeBean> beans = new List<CollegeBean>();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from Colleges where CollegeType = @type and CollegeCode in (select CollegeCode from dbo.CollegeCourses where Intake > 0) order by CollegeCode,DistrictCode", con);
                    cmd.Parameters.AddWithValue("@type", type);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        beans.Add(new CollegeBean(rdr["CollegeGroup"].ToString(), rdr["CollegeCode"].ToString(), rdr["CollegeName"].ToString(),
                                                  rdr["CollegeType"].ToString(), new DistrictDao().DistrictData(rdr["DistrictCode"].ToString()),
                                                  rdr["CollegeGender"].ToString(), rdr["SecondShift"].ToString(), rdr["RkHKCollege"].ToString(), new TalukDao().TalukData(rdr["Fk_TalukCode"].ToString()), rdr["IsNodal"].ToString()));
                    }
                }
                finally
                {
                    con.Close();
                }

            }
            return beans;
        }

        public List<CourseBean> ListOfAllCoursesInACollege(string clgcode)
        {
            List<CourseBean> beans = new List<CourseBean>();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT z.CourseCode,y.CourseName FROM [dbo].[Colleges] x,dbo.CollegeCourses z,dbo.Courses y where z.CourseCode = y.CourseCode and z.CollegeCode = x.CollegeCode and x.CollegeCode = @clgCode", con);
                    cmd.Parameters.AddWithValue("@clgCode", clgcode);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        beans.Add(new CourseBean(rdr["CourseCode"].ToString(), rdr["CourseName"].ToString()));
                    }
                }
                finally
                {
                    con.Close();
                }

            }
            return beans;
        }
    }
}
