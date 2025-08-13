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

