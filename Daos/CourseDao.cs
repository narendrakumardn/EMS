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
    public class CourseDao
    {
        public SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString());
        }
        public int GetSumOfIntake(string clgCode)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select sum(Intake) as Intake from [dbo].[MstCollegeCourses] where CollegeCode = @clgCode", con);
                    cmd.Parameters.AddWithValue("@clgCode", clgCode);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        return Convert.ToInt32(rdr["Intake"].ToString());
                    }
                }
                finally
                {
                    con.Close();
                }
            }
            return 0;
        }
        public DataSet AllCoursesInACollegeWithZeroIntakeAlso(string clgCode, String year, string cGroup)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter("select R.CourseCode,R.CourseName,S.OptionCode as optionCode,S.CollegeCourseCode as CrsCode from dte.CollegeCourses S,dbo.Colleges C,dbo.Courses R where S.CollegeCode = @clgCode and S.CollegeCode = C.CollegeCode and S.AcademicYear in (select AcaYear from MstAcamedicYear where [AcaYear]=@year) ", con);
                    da.SelectCommand.Parameters.AddWithValue("@clgCode", clgCode);
                    da.SelectCommand.Parameters.AddWithValue("@year", year);
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
        public DataSet GetAllCoursesLabDeatials(string clgCode, string acaYear)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter("select * from dte.CrsWiseLabDetails D,MstCourseSubjects S,MstSubjects M,MstCollegeCourses R where D.FkSubMapCode = S.MappedCode and S.FkSubjectCode = M.SubjectCode and R.CollegeCourseCode = D.FkOptionCode and R.CollegeCode = @clgCode and D.FkAcaYear = @acaYear order by R.CourseCode,FkSubjectCode", con);
                    da.SelectCommand.Parameters.AddWithValue("@clgCode", clgCode.ToUpper());
                    da.SelectCommand.Parameters.AddWithValue("@acaYear", acaYear.ToUpper());
                    da.Fill(ds);
                }
                finally
                {
                    con.Close();
                }

            }
            return ds;
        }
        public DataSet GetAllCoursesResultsDeatials(string clgCode, string acaYear)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT M.CourseCode,T.SubjectCode,T.SubjectName,T.SubjectType,S.FkSemId as SemId,R.No_Of_Stud_Atten as TotStudents,R.No_Of_Stud_Passed as PassStudents,R.Percentage_passed as PassPercent FROM [dte].[CrsWiseResult] R,MstCollegeCourses M,MstCourseSubjects S,MstSubjects T where R.FkOptionCode = M.CollegeCourseCode and R.FkSubMapCode = S.MappedCode and S.FkSubjectCode = T.SubjectCode and M.CollegeCode = @clgCode and FkAcaYear = @acaYear order by S.FkSemId,M.CourseCode,T.SubjectCode", con);
                    da.SelectCommand.Parameters.AddWithValue("@clgCode", clgCode.ToUpper());
                    da.SelectCommand.Parameters.AddWithValue("@acaYear", acaYear.ToUpper());
                    da.Fill(ds);
                }
                finally
                {
                    con.Close();
                }

            }
            return ds;
        }
        public DataSet GetEachCoursesLabEquipmentDeatials(string crsCode, string submapcode, string semId)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT MappedId as Id, EquipmentName FROM [dbo].[MstEquipments] where FkCourseCode = @crsCode and FkSemId=@semId and FkSubjectMapCode = @submapcode", con);
                    da.SelectCommand.Parameters.AddWithValue("@crsCode", crsCode.ToUpper());
                    da.SelectCommand.Parameters.AddWithValue("@submapcode", submapcode.ToUpper());
                    da.SelectCommand.Parameters.AddWithValue("@semId", semId);
                    da.Fill(ds);
                }
                finally
                {
                    con.Close();
                }

            }
            return ds;
        }
        public DataSet GetEachCoursesLabDeatials(string crsCode, string acaYear, string subCode, string clgCode)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter("select * from dte.CrsWiseLabDetails D,MstCourseSubjects S,MstSubjects M,MstCollegeCourses R where D.FkSubMapCode = S.MappedCode and S.FkSubjectCode = M.SubjectCode and R.CollegeCourseCode = D.FkOptionCode and R.CourseCode = @crsCode and R.CollegeCode = @clgCode and D.FkAcaYear = @acaYear and D.FkSubMapCode = @subCode order by R.CourseCode", con);
                    da.SelectCommand.Parameters.AddWithValue("@crsCode", crsCode.ToUpper());
                    da.SelectCommand.Parameters.AddWithValue("@acaYear", acaYear.ToUpper());
                    da.SelectCommand.Parameters.AddWithValue("@clgCode", clgCode.ToUpper());
                    da.SelectCommand.Parameters.AddWithValue("@subCode", subCode);
                    da.Fill(ds);
                }
                finally
                {
                    con.Close();
                }

            }
            return ds;
        }
        public DataSet GetEachCoursesResultDeatials(string crsCode, string acaYear, string subCode, string clgCode)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT M.CourseCode,T.SubjectCode,T.SubjectName,T.SubjectType,S.FkSemId,R.No_Of_Stud_Atten,R.No_Of_Stud_Passed,R.Percentage_passed FROM [dte].[CrsWiseResult] R,MstCollegeCourses M,MstCourseSubjects S,MstSubjects T where R.FkOptionCode = M.CollegeCourseCode and R.FkSubMapCode = S.MappedCode and S.FkSubjectCode = T.SubjectCode and M.CollegeCode = @clgCode and FkAcaYear = @acaYear and M.CourseCode = @crsCode and R.FkSubMapCode = @subCode", con);
                    da.SelectCommand.Parameters.AddWithValue("@crsCode", crsCode.ToUpper());
                    da.SelectCommand.Parameters.AddWithValue("@acaYear", acaYear.ToUpper());
                    da.SelectCommand.Parameters.AddWithValue("@clgCode", clgCode.ToUpper());
                    da.SelectCommand.Parameters.AddWithValue("@subCode", subCode);
                    da.Fill(ds);
                }
                finally
                {
                    con.Close();
                }

            }
            return ds;
        }
        public DataSet GetEachCoursesLabQuantityDeatials(string crsCode, string acaYear, string subCode, string clgCode, string equiId)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter("select * from dte.CrsWiseLabEquipQuantity D,MstCourseSubjects S,MstSubjects M,MstCollegeCourses R where D.FkSubMapCode = S.MappedCode and S.FkSubjectCode = M.SubjectCode and R.CollegeCourseCode = D.FkOptionCode and R.CourseCode = @crsCode and R.CollegeCode = @clgCode and D.FkAcaYear = @acaYear and D.FkSubMapCode = @subCode and D.FkEquipMapId = @equiId order by R.CourseCode", con);
                    da.SelectCommand.Parameters.AddWithValue("@crsCode", crsCode.ToUpper());
                    da.SelectCommand.Parameters.AddWithValue("@acaYear", acaYear.ToUpper());
                    da.SelectCommand.Parameters.AddWithValue("@clgCode", clgCode.ToUpper());
                    da.SelectCommand.Parameters.AddWithValue("@subCode", subCode);
                    da.SelectCommand.Parameters.AddWithValue("@equiId", equiId);
                    da.Fill(ds);
                }
                finally
                {
                    con.Close();
                }

            }
            return ds;
        }
        public DataSet GetEachCoursesAllLabQuantityDeatials(string clgCode, string acaYear)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter("select * from dte.CrsWiseLabEquipQuantity D,MstCourseSubjects S,MstSubjects M,MstCollegeCourses R,MstEquipments Q where Q.MappedId = D.FkEquipMapId and D.FkSubMapCode = S.MappedCode and S.FkSubjectCode = M.SubjectCode and R.CollegeCourseCode = D.FkOptionCode and R.CollegeCode = @clgCode and D.FkAcaYear = @acaYear order by R.CourseCode,FkSubMapCode", con);
                    da.SelectCommand.Parameters.AddWithValue("@acaYear", acaYear.ToUpper());
                    da.SelectCommand.Parameters.AddWithValue("@clgCode", clgCode.ToUpper());
                    da.Fill(ds);
                }
                finally
                {
                    con.Close();
                }

            }
            return ds;
        }
        public bool UpdateLabDeatailsForEachCourse(string crsCode, int toStdCnt, int totHours, int totArea, string subCode, int totExper, LoginBean bean)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("[spUpdateCrsLab]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@crsCode", crsCode.ToUpper());
                    cmd.Parameters.AddWithValue("@clgCode", bean.College.code.ToUpper());
                    cmd.Parameters.AddWithValue("@submapcode", subCode.ToUpper());
                    cmd.Parameters.AddWithValue("@exprCnt", totExper);
                    cmd.Parameters.AddWithValue("@totStds", toStdCnt);
                    cmd.Parameters.AddWithValue("@totHours", totHours);
                    cmd.Parameters.AddWithValue("@username", bean.UserName.ToUpper());
                    cmd.Parameters.AddWithValue("@labdimen", totArea);
                    SqlParameter outParamter = new SqlParameter();
                    outParamter.ParameterName = "@ReturnValue";
                    outParamter.SqlDbType = SqlDbType.Int;
                    outParamter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outParamter);
                    cmd.ExecuteNonQuery();
                    int result = Convert.ToInt32(outParamter.Value.ToString());
                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    con.Close();
                }

            }
        }
        public bool UpdateResultsForEachCourse(string crsCode, int toStdCnt, int studPassed, string subMapCode, LoginBean bean)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("[dte].[spCrsWiseResultUpdate]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@crsCode", crsCode.ToUpper());
                    cmd.Parameters.AddWithValue("@clgCode", bean.College.code.ToUpper());
                    cmd.Parameters.AddWithValue("@FkSubMapCode", subMapCode.ToUpper());
                    cmd.Parameters.AddWithValue("@No_Of_Stud_Atten", toStdCnt);
                    cmd.Parameters.AddWithValue("@No_Of_Stud_Passed", studPassed);
                    cmd.Parameters.AddWithValue("@Username", bean.UserName.ToUpper());
                    SqlParameter outParamter = new SqlParameter();
                    outParamter.ParameterName = "@ReturnValue";
                    outParamter.SqlDbType = SqlDbType.Int;
                    outParamter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outParamter);
                    cmd.ExecuteNonQuery();
                    int result = Convert.ToInt32(outParamter.Value.ToString());
                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    con.Close();
                }

            }
        }
        public bool UpdateCrsWiseLabEquipCount(string crsCode, int quantity, string subCode, string equipId, LoginBean bean)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("[dte].[spUpdateLabCrsEquipCnt]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@crsCode", crsCode.ToUpper());
                    cmd.Parameters.AddWithValue("@clgCode", bean.College.code.ToUpper());
                    cmd.Parameters.AddWithValue("@submapcode", subCode.ToUpper());
                    cmd.Parameters.AddWithValue("@username", bean.UserName.ToUpper());
                    cmd.Parameters.AddWithValue("@equipMapcode", equipId);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    SqlParameter outParamter = new SqlParameter();
                    outParamter.ParameterName = "@ReturnValue";
                    outParamter.SqlDbType = SqlDbType.Int;
                    outParamter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outParamter);
                    cmd.ExecuteNonQuery();
                    int result = Convert.ToInt32(outParamter.Value.ToString());
                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    con.Close();
                }

            }
        }

        public bool InsertCourseSubjectMappings(string crs, string circulimm, string subjcode, string sem, string mapCode, string suborder)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd1 = new SqlCommand("spUpdateMapCrsSubjectMaster", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@FkCourseCode", crs);
                    cmd1.Parameters.AddWithValue("@FkCurriculumCode", circulimm);
                    cmd1.Parameters.AddWithValue("@FkSubjectCode", subjcode);
                    cmd1.Parameters.AddWithValue("@FkSemId", sem);
                    cmd1.Parameters.AddWithValue("@MappedCode", mapCode);
                    cmd1.Parameters.AddWithValue("@OrderOfSubjectCoder", suborder);
                    SqlParameter outParamter = new SqlParameter();
                    outParamter.ParameterName = "@ReturnValue";
                    outParamter.SqlDbType = SqlDbType.Int;
                    outParamter.Direction = ParameterDirection.Output;
                    cmd1.Parameters.Add(outParamter);
                    cmd1.ExecuteNonQuery();
                    int result = Convert.ToInt32(outParamter.Value.ToString());
                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }
        }
        public bool AddNewCourseDetails(string crsCode, string crsName)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("spAddCourse", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@crsCode", crsCode.ToUpper());
                    cmd.Parameters.AddWithValue("@crsName", crsName.ToUpper());
                    SqlParameter outParamter = new SqlParameter();
                    outParamter.ParameterName = "@ReturnValue";
                    outParamter.SqlDbType = SqlDbType.Int;
                    outParamter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outParamter);
                    cmd.ExecuteNonQuery();
                    int result = Convert.ToInt32(outParamter.Value.ToString());
                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    con.Close();
                }

            }
        }
        public bool AddNewSubjectDetails(string subCode, string subName, string subType, int endexamax, int endexamin, int iamax, int iamin, int passmrks, int time, int totmarks)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("spAddSubject", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@subCode", subCode.ToUpper());
                    cmd.Parameters.AddWithValue("@subName", subName.ToUpper());
                    cmd.Parameters.AddWithValue("@subType", subType.ToUpper());
                    cmd.Parameters.AddWithValue("@endexamax", endexamax);
                    cmd.Parameters.AddWithValue("@endexamin", endexamin);
                    cmd.Parameters.AddWithValue("@iamax", iamax);
                    cmd.Parameters.AddWithValue("@iamin", iamin);
                    cmd.Parameters.AddWithValue("@passmrks", passmrks);
                    cmd.Parameters.AddWithValue("@time", time);
                    cmd.Parameters.AddWithValue("@totmarks", totmarks);
                    SqlParameter outParamter = new SqlParameter();
                    outParamter.ParameterName = "@ReturnValue";
                    outParamter.SqlDbType = SqlDbType.Int;
                    outParamter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outParamter);
                    cmd.ExecuteNonQuery();
                    int result = Convert.ToInt32(outParamter.Value.ToString());
                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    con.Close();
                }

            }
        }
        public List<CourseBean> CourseList()
        {
            List<CourseBean> beans = new List<CourseBean>();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from Courses WHERE CourseCode <> 'SV' and CourseCode <> 'AM' and CourseCode <> 'PM' order by CourseCode", con);
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

        public List<CourseBean> CourseListBasedOnCollegeCode(string clgCode)
        {
            List<CourseBean> beans = new List<CourseBean>();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select R.CourseCode,R.CourseName from MstCollegeCourses M,Courses R where M.CollegeCode = @clgCode and R.CourseCode = M.CourseCode order by CourseCode", con);
                    cmd.Parameters.AddWithValue("@clgCode", clgCode.ToUpper());
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

        public CourseBean CourseData(string courseCode)
        {
            CourseBean bean = null;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from Courses where CourseCode = @courseCode order by CourseCode", con);
                    cmd.Parameters.AddWithValue("@courseCode", courseCode);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        bean = new CourseBean(rdr["CourseCode"].ToString(), rdr["CourseName"].ToString());
                    }
                }
                finally
                {
                    con.Close();
                }

            }
            return bean;
        }
        public CourseBean CourseDataFromOptionCode(string courseCode)
        {
            CourseBean bean = null;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["APPRCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from Courses where CourseCode in (select CourseCode from dte.CollegeCourses where OptionCode = @courseCode and AcademicYear in (select AcaYear from MstAcamedicYear where Active='Y')) order by CourseCode", con);
                    cmd.Parameters.AddWithValue("@courseCode", courseCode);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        bean = new CourseBean(rdr["CourseCode"].ToString(), rdr["CourseName"].ToString());
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
