using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace BTEDiploma.Helper
{
    public class ForgotPasswordDao
    {
        public static bool UserExists(string usertype, string username)
        {
            string query = "";

            // Choose query based on user type
            switch (usertype)
            {
                case "I": // Institute
                    query = "SELECT COUNT(*) FROM Tb_InstituteLogin WHERE Institute_Login_ID = @username";
                    break;
                case "S": // Student
                    query = "SELECT COUNT(*) FROM Tb_StudentLogin WHERE User_Login_Name = @username";
                    break;
                case "E": // Employee
                    query = "SELECT COUNT(*) FROM Tb_EmployeeLogin WHERE Employee_Login_Name = @username";
                    break;
                default:
                    throw new ArgumentException("Invalid user type.");
            }

            object result = SQLHelper.ExecuteScalar(query, new[] {
        new SqlParameter("@username", username)
    });

            return Convert.ToInt32(result) > 0;
        }



        public static bool UpdatePassword(string userType, string username, string newPassword)
        {
            // Generate salt & hash
            SecurityHelper.CreatePasswordHash(newPassword, out byte[] saltBytes, out byte[] hashBytes);

            string query = string.Empty;

            // Choose table & key column based on user type
            switch (userType)
            {
                case "I": // Institute
                    query = "UPDATE Tb_InstituteLogin SET PasswordHash = @hash, PasswordSalt = @salt WHERE Institute_Login_ID = @username";
                    break;
                case "S": // Student
                    query = "UPDATE Tb_StudentLogin SET PasswordHash = @hash, PasswordSalt = @salt WHERE User_Login_Name = @username";
                    break;
                case "E": // Employee
                    query = "UPDATE Tb_EmployeeLogin SET PasswordHash = @hash, PasswordSalt = @salt WHERE Employee_Login_Name = @username";
                    break;
                default:
                    throw new ArgumentException("Invalid user type");
            }

            // Run update
            int rows = SQLHelper.ExecuteNonQuery(query, CommandType.Text, new[]
            {
        new SqlParameter("@hash", SqlDbType.VarBinary) { Value = hashBytes },
        new SqlParameter("@salt", SqlDbType.VarBinary) { Value = saltBytes },
        new SqlParameter("@username", username)
    });

            return rows > 0;
        }





        public static string GetEmailByUsername(string usertype, string username)
        {
            SqlParameter[] param = {
                new SqlParameter("@usertype", usertype),
        new SqlParameter("@username", username)
    };

            DataTable dt = SQLHelper.ExecuteStoredProcedure("spGetEmailByUsername", param);

            if (dt.Rows.Count > 0)
                return dt.Rows[0]["Email_ID"].ToString();

            return null;
        }


        public static bool UpdateFirstLoginFlag(string username, string userType)
        {
            string sql = "";
            if (userType == "S")
                sql = "UPDATE Tb_StudentLogin SET Is_FirstLogin = 0 WHERE User_Login_Name = @Username";
            else if (userType == "E")
                sql = "UPDATE Tb_EmployeeLogin SET Is_FirstLogin = 0 WHERE Employee_Login_Name = @Username";
            else if (userType == "I")
                sql = "UPDATE Tb_InstituteLogin SET Is_FirstLogin = 0 WHERE Institute_Login_ID = @Username";

            return SQLHelper.ExecuteNonQuery(sql, CommandType.Text, new SqlParameter[] { new SqlParameter("@Username", username) }
            ) > 0;
        }





    }
}
