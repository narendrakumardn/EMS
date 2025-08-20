using BTEDiploma.Helper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace BTEDiploma.Helper
{



    public class LoginDao
    {
        public static bool AuthenticateUser(string username, string password, HttpSessionState session)
        {
            DataTable dt = GetUserDetails(username, password);  // Call the SP

            if (dt != null && dt.Rows.Count > 0)
            {
                // Set session values from the result
                session["Username"] = dt.Rows[0]["username"].ToString();
                session["UserType"] = dt.Rows[0]["UserTypeCode"].ToString();  // int or string
                session["CollegeCode"] = dt.Rows[0]["collegecode"].ToString();
                session["CollegeName"] = dt.Rows[0]["collegename"].ToString();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Fetches the user row (with salt + hash) using stored procedure dbo.spGetUserByUsername.
        /// Returns a DataTable (0 or 1 row).
        /// </summary>
        public static DataTable GetUserByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("username is required", nameof(username));

            SqlParameter[] parameters = {
                new SqlParameter("@Username", SqlDbType.VarChar, 100) { Value = username }
            };

            return SQLHelper.ExecuteStoredProcedure("dbo.spGetUserByUsername", parameters);
        }

        public static DataTable GetUserLogin(string userType, string username, string ip)
        {
            List<SqlParameter> parameters = new List<SqlParameter>
        {
            new SqlParameter("@UserType", userType),
            new SqlParameter("@LoginName", username)
        };


            return SQLHelper.ExecuteStoredProcedure("spGetUserLogin", parameters.ToArray());
        }

       

        public static bool CheckFirstLoginStudent(string spName, string username)
        {
            DataTable dt = SQLHelper.ExecuteStoredProcedure(
                spName,
                new SqlParameter[] { new SqlParameter("@Username", username) }
            );

            return (dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0]["Is_FirstLogin"]));
        }

        public static bool CheckFirstLoginEmployee(string spName, string username)
        {
            DataTable dt = SQLHelper.ExecuteStoredProcedure(
                spName,
                new SqlParameter[] { new SqlParameter("@Username", username) }
            );

            return (dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0]["Is_FirstLogin"]));
        }
        public static string GetInstitutionCodeByLoginName(string loginName)
        {
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@LoginName", loginName)
            };

            DataTable dt = SQLHelper.ExecuteStoredProcedure("spGetInstitutionCodeByLogin", parameters.ToArray());

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["InstitutionCode"].ToString();
            }

            return null; // or throw an exception / return "" as needed
        }
        public static bool CheckFirstLoginInstitution(string spName, string username)
        {
            DataTable dt = SQLHelper.ExecuteStoredProcedure(
                spName,
                new SqlParameter[] { new SqlParameter("@Username", username) }
            );

            if (dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0]["Is_FirstLogin"]))
                return true;
            return false;
        }
        public static DataTable GetInstitutionDetails(string institutionCode)
        {
            List<SqlParameter> parameters = new List<SqlParameter>
    {
        new SqlParameter("@InstitutionCode", institutionCode)
    };

            return SQLHelper.ExecuteStoredProcedure("spGetInstitutionDetailsByLogin", parameters.ToArray());
        }
        public static string GetInstitutionCodeByEmployeeLogin(string loginName)
        {
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@LoginName", loginName)
            };

            DataTable dt = SQLHelper.ExecuteStoredProcedure("spGetInstitutionCodeByEmployeeLogin", parameters.ToArray());

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["Current_Institute_ID"].ToString();
            }

            return null; // or throw an exception / return "" as needed
        }
        public static string GetInstitutionCodeByStudentEnrollment(string loginName)
        {
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@LoginName", loginName)
            };

            DataTable dt = SQLHelper.ExecuteStoredProcedure("spGetInstitutionCodeByStudentEnrollment", parameters.ToArray());

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["Institution_Code"].ToString();
            }

            return null; // or throw an exception / return "" as needed
        }

            public static DataTable GetStudentDetails(string institutionCode, string username)
        {
            List<SqlParameter> parameters = new List<SqlParameter>
        {
            new SqlParameter("@inst_code", institutionCode),
            new SqlParameter("@Username", username)
        };


            return SQLHelper.ExecuteStoredProcedure("spGetStudentCollegeDetails", parameters.ToArray());
        }


        public static DataTable GetEmployeeDetails(string institutionCode, string username)
        {
            List<SqlParameter> parameters = new List<SqlParameter>
        {
            new SqlParameter("@inst_code", institutionCode),
            new SqlParameter("@Username", username)
        };


            return SQLHelper.ExecuteStoredProcedure("spGetEmployeeCollegeDetails", parameters.ToArray());
        }
        public static DataTable GetUserDetails(string username, string password)
        {
            try
            {
                string query = "GetUserTypeByUsn";  // Make sure this SP exists

                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@Username", username),
            new SqlParameter("@Password", password)
                };

                return SQLHelper.ExecuteStoredProcedure(query, parameters);
            }
            catch (SqlException ex)
            {
                // Log the error or rethrow it
                throw new Exception("SQL Error in GetUserDetails: " + ex.Message);
            }
        }





    }
}

