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
    public class ExceptionLoggerDao
    {
        public static void InsertExceptionsIntoDB(string exceptionMessage)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("insert into ExceptionLogger (Date,ExceptionMessage) values(getDate(),@ExceptionMessage)", con);
                cmd.Parameters.AddWithValue("@ExceptionMessage", exceptionMessage);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
        public static void InsertExceptionsIntoDB(StringBuilder exceptionMessage)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("insert into ExceptionLogger (Date,ExceptionMessage) values(getDate(),@ExceptionMessage)", con);
                cmd.Parameters.AddWithValue("@ExceptionMessage", exceptionMessage.ToString());
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
        public static void LogException(Exception ex)
        {
            if (ex != null)
            {
                StringBuilder sbExceptionMessage = new StringBuilder();
                do
                {
                    sbExceptionMessage.Append("Exception Type:" + Environment.NewLine);
                    sbExceptionMessage.Append(ex.GetType().Name);
                    sbExceptionMessage.Append("Exception message:" + Environment.NewLine);
                    sbExceptionMessage.Append(ex.Message + Environment.NewLine);
                    sbExceptionMessage.Append("Stack trace:" + Environment.NewLine);
                    sbExceptionMessage.Append(ex.StackTrace + Environment.NewLine);
                    ex = ex.InnerException;
                } while (ex != null);
                ExceptionLoggerDao.InsertExceptionsIntoDB(sbExceptionMessage);
            }

        }
        public void InsertExceptionsIntoDB(StringBuilder exceptionMessage, LoginBean loginBO)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("spExceptionLogger", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ExceptionMessage", exceptionMessage.ToString());
                cmd.Parameters.AddWithValue("@UserName", loginBO.UserName);
                cmd.Parameters.AddWithValue("@ClgCode", loginBO.College.code);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
        public static void InsertExceptionsIntoDB(StringBuilder exceptionMessage, Object obj)
        {
            string username = null;
            string clgCode = null;
            LoginBean lbo = null;
            if (obj is LoginBean)
            {
                lbo = obj as LoginBean;
                username = lbo.UserName;
                clgCode = lbo.College.code;
            }
            //else
            //{
            //    stdlbo = obj as StdLoginBean;
            //    username = stdlbo.Beans.BteNo;
            //    clgCode = stdlbo.Beans.Clg.code;
            //}
            
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString()))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("spExceptionLogger", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ExceptionMessage", exceptionMessage.ToString());
                cmd.Parameters.AddWithValue("@UserName", username);
                cmd.Parameters.AddWithValue("@ClgCode", clgCode);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
        public void Log(Exception ex, LoginBean loginBO, string Url)
        {
            if (ex != null)
            {
                StringBuilder sbExceptionMessage = new StringBuilder();
                do
                {
                    sbExceptionMessage.Append("Exception Type:" + Environment.NewLine);
                    sbExceptionMessage.Append(ex.GetType().Name);
                    sbExceptionMessage.Append(Environment.NewLine + Environment.NewLine);
                    sbExceptionMessage.Append("Exception message:" + Environment.NewLine);
                    sbExceptionMessage.Append(ex.Message + Environment.NewLine);
                    sbExceptionMessage.Append("Stack trace:" + Environment.NewLine);
                    sbExceptionMessage.Append(ex.StackTrace + Environment.NewLine);
                    sbExceptionMessage.Append(Environment.NewLine + Environment.NewLine);
                    sbExceptionMessage.Append("Error At:" + Url);
                    ex = ex.InnerException;
                } while (ex != null);
                new ExceptionLoggerDao().InsertExceptionsIntoDB(sbExceptionMessage, loginBO);
            }

        }
        public static void LogException(Exception ex, Object obj)
        {
            if (ex != null)
            {
                StringBuilder sbExceptionMessage = new StringBuilder();
                do
                {
                    sbExceptionMessage.Append("Exception Type:" + Environment.NewLine);
                    sbExceptionMessage.Append(ex.GetType().Name);
                    sbExceptionMessage.Append("Exception message:" + Environment.NewLine);
                    sbExceptionMessage.Append(ex.Message + Environment.NewLine);
                    sbExceptionMessage.Append("Stack trace:" + Environment.NewLine);
                    sbExceptionMessage.Append(ex.StackTrace + Environment.NewLine);
                    ex = ex.InnerException;
                } while (ex != null);
                ExceptionLoggerDao.InsertExceptionsIntoDB(sbExceptionMessage, obj);
            }

        }
        
    }
}
