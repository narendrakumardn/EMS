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
    public class SSP
    {
        private SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString());
        }
        public string CheckWhetherSSPStudentIdAlreadyExists(String SSPId)
        {
            string appNo1 = string.Empty;
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select case when count([FK_USN]) is null then 0 else count(BTE_REG_NO) end as Tot from [dte].[Candidate_SSP_Details] where [SSP_STD_ID] = @SSPId", con);
                    cmd.Parameters.AddWithValue("@SSPId", SSPId);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        appNo1 = rdr["Tot"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                }
                return appNo1;
            }
        }
        public string CheckWhetherDataIsPushedToSSP(String regNo)
        {
            string appNo1 = string.Empty;
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select case when count([FK_USN]) is null then 0 else count([FK_USN]) end as Tot from [dte].[Candidate_SSP_Details] where [FK_USN] = @regNo and [BONAFIDE_DATA] = 0", con);
                    cmd.Parameters.AddWithValue("@regNo", regNo);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        appNo1 = rdr["Tot"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                }
                return appNo1;
            }
        }
        //Since UID is hashed we couldn't check what is the exact value of UID
        public string CheckWhetherUIDAlreadyExists(String bteRegNo)
        {
            string appNo1 = string.Empty;
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select case when count([UDIH]) is null then 0 else count([UDIH]) end as Tot from [dte].[Candidate_SSP_Details] where [FK_USN] = @bteRegNo", con);
                    cmd.Parameters.AddWithValue("@bteRegNo", bteRegNo);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        appNo1 = rdr["Tot"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                }
                return appNo1;
            }
        }
        public Boolean UpdateStudentSSPIdForFirstYearAdmission(String regNo, long studentSSPID, String name, int gender, LoginBean bean, String userIp, String id, String admnstat, String stype, String admnfee, String examfee)
        {
            String gen = String.Empty;
            bool res = false;
            switch (gender)
            {
                case 1:
                    gen = "M";
                    break;
                case 2:
                    gen = "F";
                    break;
                case 3:
                    gen = "T";
                    break;
            }
            SqlTransaction dbTransaction = null;
            String query1 = "spUpdateStudentSSPID";
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    dbTransaction = con.BeginTransaction();
                    SqlCommand cmd1 = new SqlCommand(query1, con, dbTransaction);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@studentSSPID", studentSSPID.ToString());
                    cmd1.Parameters.AddWithValue("@name", name);
                    cmd1.Parameters.AddWithValue("@username", bean.UserName);
                    cmd1.Parameters.AddWithValue("@gen", gen);
                    cmd1.Parameters.AddWithValue("@userIp", userIp);
                    cmd1.Parameters.AddWithValue("@regNo", regNo);
                    cmd1.Parameters.AddWithValue("@id", id);
                    cmd1.Parameters.AddWithValue("@admnfee", admnfee);
                    cmd1.Parameters.AddWithValue("@examfee", examfee);
                    cmd1.Parameters.AddWithValue("@stype", stype);
                    cmd1.Parameters.AddWithValue("@admnstat", admnstat);

                    SqlParameter outParamter = new SqlParameter();
                    outParamter.ParameterName = "@ReturnValue";
                    outParamter.SqlDbType = SqlDbType.Int;
                    outParamter.Direction = ParameterDirection.Output;
                    cmd1.Parameters.Add(outParamter);
                    int result = Convert.ToInt32(outParamter.Value.ToString());
                    if (result > 0)
                    {
                       res = true;
                    }
                    dbTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                    res = false;
                }
                finally
                {
                    con.Close();
                }
            }
            if (!res)
                dbTransaction.Rollback();
            return res;
        }
        public DataSet RetreiveOnlyPendingApprovedListCandidatesForUpdation(String opCode, String acaYear)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter("select USN as REGNO,L.NAME + ' (' + USN + ')' +  ' (FATHER NAME ' + L.FNAME + ')' as NAME from [dte].[CollegeCourses] R,DTE.CandidateList L where R.OptionCode=@opCode and L.FkAcaYear = @acaYear AND R.OptionCode = L.OptionCode AND R.AcademicYear=@acaYear and R.CollegeCode = L.ALLOTTEDCOLLEGE and L.USN not in (select FK_USN from [dte].[Candidate_SSP_Details] B where  [SSP_STD_ID] is NOT null and (ADMN_DATA = 1 or [BONAFIDE_DATA] = 1)) order by USN", con);
                    da.SelectCommand.Parameters.AddWithValue("@opCode", opCode);
                    da.SelectCommand.Parameters.AddWithValue("@acaYear", acaYear);
                    da.Fill(ds);
                }
                finally
                {
                    con.Close();
                }
            }
            return ds;
        }
    }
    public class StatusMsg
    {
        public String Message_Status;
        public String Message;
    }
}
