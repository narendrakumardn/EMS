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
    public class LoginDao
    {
        public SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString());
        }
        public String GetEmail(String mobile)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select [EMail] as Mail from [dbo].[DteOneUsers] where [MobileNo] = @mobile", con);
                    cmd.Parameters.AddWithValue("@mobile", mobile);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        return rdr["Mail"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLoggerDao.LogException(ex.GetBaseException());
                    return null;
                }
                finally
                {
                    con.Close();
                }
            }
            return null;
        }
        public int LoginDataExistsOrNot(string UserName, string collegeCode, string userType)
        {
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select case when count(Username) = '' then 0 else 1 end as CountU from dte.UserLogin where Username = @username and UserTypeCode=@userType and [CollegeCode]=@collegeCode", con);
                    cmd.Parameters.AddWithValue("@username", UserName);
                    cmd.Parameters.AddWithValue("@collegeCode", collegeCode);
                    cmd.Parameters.AddWithValue("@userType", userType);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        return Convert.ToInt32(rdr["CountU"].ToString());
                    }
                }
                finally
                {
                    con.Close();
                }

            }
            return -1;
        }
        public string CheckMemberExists(string mobileNo, string email, string insCode)
        {
            string appNo1 = string.Empty;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select count([MobileNo]) as Tot from [dbo].[DteOneUsers] where MobileNo = @mobileNo and [Email]=@email and [Fk_InsCode] = @insCode", con);
                    cmd.Parameters.AddWithValue("@mobileNo", mobileNo);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@insCode", insCode);
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
        public string GetRandomStringForPassowrdRecovery(string rand, String new_registration)
        {
            string appNo1 = "0";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT TOP 1 case when Mobile = '' then '0' else Mobile end as Data FROM [dbo].[RANDOM_STR] where ComputedField = @rand and New_Registration = @new_registration order by upd_dt desc", con);
                    cmd.Parameters.AddWithValue("@rand", rand);
                    cmd.Parameters.AddWithValue("@new_registration", new_registration);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        appNo1 = rdr["Data"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                }
                return appNo1;
            }
        }
        public string CheckForLinkActivation(string randString)
        {
            string appNo1 = "0";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT case when  DATEDIFF(minute, [Upd_Dt], GETDATE()) > 31 then 0 else 1 end as Result from [dbo].[RANDOM_STR] where [ComputedField] = @randString", con);
                    cmd.Parameters.AddWithValue("@randString", randString);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        appNo1 = rdr["Result"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                }
                return appNo1;
            }
        }
        public string CheckWhetherPasswordHasBeenChanged(string randString, String new_regsitration)
        {
            string appNo1 = "0";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT Used as Status from [dbo].[RANDOM_STR] where [ComputedField] = @randString and cast([Upd_Dt] as Date) = cast(Getdate() as Date) and New_Registration=@new_regsitration", con);
                    cmd.Parameters.AddWithValue("@randString", randString);
                    cmd.Parameters.AddWithValue("@new_regsitration", new_regsitration);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        appNo1 = rdr["Status"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                }
                return appNo1;
            }
        }
        public string CheckWhetherPasswordHasBeenChangedBasedOnMobile(string mobile, String new_registration)
        {
            string appNo1 = "0";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT Used as Status from [dbo].[RANDOM_STR] where [Mobile] = @mobile and cast([Upd_Dt] as Date) = cast(Getdate() as Date) and New_Registration=@new_registration", con);
                    cmd.Parameters.AddWithValue("@mobile", mobile);
                    cmd.Parameters.AddWithValue("@new_registration", new_registration);

                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        appNo1 = rdr["Status"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                }
                return appNo1;
            }
        }
        public string CheckForEmailDeliveredForADay(string mobile, String new_registration)
        {
            string appNo1 = "0";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT count(Mobile) as Result from [dbo].[RANDOM_STR] where [Mobile] = @mobile and cast([Upd_Dt] as date) = CAST( GETDATE() AS Date ) and New_Registration = @new_registration", con);
                    cmd.Parameters.AddWithValue("@mobile", mobile);
                    cmd.Parameters.AddWithValue("@new_registration", new_registration);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        appNo1 = rdr["Result"].ToString();
                    }
                }
                finally
                {
                    con.Close();
                }
                return appNo1;
            }
        }
        public int RequestForPasswordReset(String mobile, String ip, String randomString, String new_registration)
        {
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("[spRandomString]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@mobile", mobile);
                    cmd.Parameters.AddWithValue("@new", new_registration);
                    cmd.Parameters.AddWithValue("@randomString", randomString);
                    cmd.Parameters.AddWithValue("@ip", ip);
                    SqlParameter outParamter = new SqlParameter();
                    outParamter.ParameterName = "@ReturnValue";
                    outParamter.SqlDbType = SqlDbType.Int;
                    outParamter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outParamter);
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    ExceptionLoggerDao.InsertExceptionsIntoDB(ex.GetBaseException().Message);
                    return 0;
                }
                finally
                {
                    con.Close();
                }
            }
        }
        public int ResetPassword(String password, String ip, String uType, String clgCode, String username)
        {
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("[spAdminResetPass]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@clgCode", clgCode);
                    cmd.Parameters.AddWithValue("@uType", uType);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@ip", ip);
                    SqlParameter outParamter = new SqlParameter();
                    outParamter.ParameterName = "@ReturnValue";
                    outParamter.SqlDbType = SqlDbType.Int;
                    outParamter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outParamter);
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    ExceptionLoggerDao.InsertExceptionsIntoDB(ex.GetBaseException().Message);
                    return 0;
                }
                finally
                {
                    con.Close();
                }
            }
        }
        public int NewUser1(String username, String password, String mobile, String inscode, String userType)
        {
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("[spAddUser]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserName", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Mobile", mobile);
                    cmd.Parameters.AddWithValue("@InsCode", inscode);
                    cmd.Parameters.AddWithValue("@userType", userType);
                    SqlParameter outParamter = new SqlParameter();
                    outParamter.ParameterName = "@ReturnValue";
                    outParamter.SqlDbType = SqlDbType.Int;
                    outParamter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outParamter);
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    ExceptionLoggerDao.InsertExceptionsIntoDB(ex.GetBaseException().Message);
                    return 0;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public List<LoginBean> AllUserList(string approvedFlag, string clgCode)
        {
            List<LoginBean> beans = new List<LoginBean>();
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from dte.UserLogin where Approved = @approvedFlag and CollegeCode = @clgCode and UserTypeCode > 1", con);
                    cmd.Parameters.AddWithValue("@approvedFlag", approvedFlag);
                    cmd.Parameters.AddWithValue("@clgCode", clgCode);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        beans.Add(new LoginBean(rdr["Username"].ToString(), rdr["Passwd"].ToString(), rdr["Mobile"].ToString(),
                                  (UserType)Enum.Parse(typeof(UserType)
                                  , new UserTypeDao().UserTypeData(Convert.ToInt32(rdr["UserTypeCode"].ToString())).name, true)
                                  , new CollegeDao().CollegeData(rdr["CollegeCode"].ToString()), rdr["Approved"].ToString(), rdr["Status"].ToString(), rdr["Name"].ToString(), Convert.ToInt32(rdr["IsLocked"].ToString())));
                    }
                }
                finally
                {
                    con.Close();
                }

            }
            return beans;
        }
        public LoginBean LoginData(string UserName)
        {
            LoginBean bean = null;
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from dte.UserLogin where Username = @username", con);
                    cmd.Parameters.AddWithValue("@username", UserName);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        bean = new LoginBean(rdr["Username"].ToString(), rdr["Passwd"].ToString(), rdr["Mobile"].ToString(),
                                  (UserType)Enum.Parse(typeof(UserType)
                                  , new UserTypeDao().UserTypeData(Convert.ToInt32(rdr["UserTypeCode"].ToString())).name, true)
                                  , new CollegeDao().CollegeData(rdr["CollegeCode"].ToString()), rdr["Approved"].ToString(), rdr["Status"].ToString(), rdr["Name"].ToString(), Convert.ToInt32(rdr["IsLocked"].ToString()));
                    }
                }
                finally
                {
                    con.Close();
                }

            }
            return bean;
        }
        public LoginBean LoginData(string UserName, bool flag)
        {
            LoginBean bean = null;
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from dte.UserLogin where Username = @username and Approved = 'Y'", con);
                    cmd.Parameters.AddWithValue("@username", UserName);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        bean = new LoginBean(rdr["Username"].ToString(), rdr["Passwd"].ToString(), rdr["Mobile"].ToString(),
                                  (UserType)Enum.Parse(typeof(UserType)
                                  , new UserTypeDao().UserTypeData(Convert.ToInt32(rdr["UserTypeCode"].ToString())).name, true)
                                  , new CollegeDao().CollegeData(rdr["CollegeCode"].ToString()), rdr["Approved"].ToString(), rdr["Status"].ToString(), rdr["Name"].ToString(), Convert.ToInt32(rdr["IsLocked"].ToString()));
                    }
                }
                finally
                {
                    con.Close();
                }

            }
            return bean;
        }
        public int NewUser(String username, String password, String mobile, String inscode, String name, String userType)
        {
            SqlTransaction dbTransaction = null;
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    dbTransaction = con.BeginTransaction();
                    SqlCommand cmd = new SqlCommand("insert into dte.UserLogin values (@UserName,@Password,@Name,@InsCode,@userType,@Mobile,getdate(),'Y',getdate(),NULL)", con, dbTransaction);                    
                    cmd.Parameters.AddWithValue("@UserName", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Mobile", mobile);
                    cmd.Parameters.AddWithValue("@InsCode", inscode);
                    cmd.Parameters.AddWithValue("@userType", userType);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.ExecuteNonQuery();
                    dbTransaction.Commit();
                    return 2;
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                    ExceptionLoggerDao.InsertExceptionsIntoDB(ex.GetBaseException().Message);
                    return 0;
                }
                finally
                {
                    con.Close();
                }
            }
        }
        public int UpdateIsLocked(String username, bool flag)
        {
            int id = -1;
            String sqlQuery = String.Empty;
            if (flag)
                sqlQuery = "update dte.[UserLogin] set IsLocked += 1 output inserted.IsLocked where Username=@uname";
            else
                sqlQuery = "update dte.[UserLogin] set IsLocked = 0 output inserted.IsLocked where Username=@uname";
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(sqlQuery, con);
                    cmd.Parameters.AddWithValue("@uname", username);
                    SqlDataReader rdr = cmd.ExecuteReader(); ;
                    if (rdr.Read())
                    {
                        return Convert.ToInt32(rdr["IsLocked"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLoggerDao.LogException(ex.GetBaseException());
                    return id;
                }
                finally
                {
                    con.Close();
                }
            }
            return -1;
        }
        public Boolean UpdateUserLoginHistory(LoginBean login, String ip)
        {
            SqlTransaction dbTransaction = null;
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    dbTransaction = con.BeginTransaction();
                    SqlCommand cmd = new SqlCommand("insert into dte.UserLoginHistory (UserName,LogInTime,IpAddress) values (@UserName,GETDATE(),@Ip)", con, dbTransaction);
                    cmd.Parameters.AddWithValue("@UserName", login.UserName);
                    cmd.Parameters.AddWithValue("@Ip", ip);
                    cmd.ExecuteNonQuery();
                    dbTransaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    ExceptionLoggerDao.LogException(ex.GetBaseException(), login);
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public Boolean UpdateUserLogOutHistory(LoginBean login)
        {
            SqlTransaction dbTransaction = null;
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    dbTransaction = con.BeginTransaction();
                    SqlCommand cmd = new SqlCommand("update dte.UserLoginHistory set LogOutTime = GETDATE() where LogInTime = (select max(LogInTime) from dte.UserLoginHistory) and Username = @Username", con, dbTransaction);
                    cmd.Parameters.AddWithValue("@Username", login.UserName);
                    cmd.ExecuteNonQuery();
                    dbTransaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    ExceptionLoggerDao.LogException(ex.GetBaseException(), login);
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }
        }
        public DataSet GetUserLoginHistory(string username)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter("select [LogInTime],[LogOutTime],[IpAddress] from [dte].[UserLoginHistory] where [UserName]=@username order by LogInTime DESC", con);
                    da.SelectCommand.Parameters.AddWithValue("@username", username);
                    da.Fill(ds);
                }
                finally
                {
                    con.Close();
                }

            }
            return ds;
        }

        public int CP(String username, String password, string ip)
        {
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("[spChangePass]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserName", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@ip", ip);
                    SqlParameter outParamter = new SqlParameter();
                    outParamter.ParameterName = "@ReturnValue";
                    outParamter.SqlDbType = SqlDbType.Int;
                    outParamter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outParamter);
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    ExceptionLoggerDao.LogException(ex.GetBaseException());
                    return 0;
                }
                finally
                {
                    con.Close();
                }
            }
        }
        public int AU(String username, String password, string ip)
        {
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("[spCreatePass]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserName", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@ip", ip);
                    SqlParameter outParamter = new SqlParameter();
                    outParamter.ParameterName = "@ReturnValue";
                    outParamter.SqlDbType = SqlDbType.Int;
                    outParamter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outParamter);
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    ExceptionLoggerDao.LogException(ex.GetBaseException());
                    return 0;
                }
                finally
                {
                    con.Close();
                }
            }
        }
        public int ChangePasswordForUser(LoginBean lbo, String newPassword, string ipAddress)
        {
            SqlTransaction dbTransaction = null;
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    dbTransaction = con.BeginTransaction();
                    SqlCommand cmd = new SqlCommand("update [UserLogin] set [Passwd]=@Password where [Username]=@Username", con, dbTransaction);
                    SqlCommand cmd1 = new SqlCommand("insert into [dte].[UserChangePasswordLog] values (@Username,getdate(),@ipAddress)", con, dbTransaction);
                    cmd.Parameters.AddWithValue("@Username", lbo.UserName);
                    cmd.Parameters.AddWithValue("@Password", newPassword);
                    cmd1.Parameters.AddWithValue("@Username", lbo.UserName);
                    cmd1.Parameters.AddWithValue("@ipAddress", ipAddress);
                    cmd.ExecuteNonQuery();
                    cmd1.ExecuteNonQuery();
                    dbTransaction.Commit();
                    return 1;
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                    ExceptionLoggerDao.LogException(ex.GetBaseException(), lbo);
                    return -1;
                }
                finally
                {
                    con.Close();
                }
            }
        }
    }
}
