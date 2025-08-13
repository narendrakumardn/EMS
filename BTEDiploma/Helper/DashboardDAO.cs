using System;
using System.Data;
using System.Data.SqlClient;
using BTEDiploma.sqlhelper;

namespace BTEDiploma.Helper
{
    public static class DashboardDao
    {
        public static int GetTotalColleges()
        {
            DataTable dt = SQLHelper.ExecuteStoredProcedure("spGetTotalColleges", null);
            return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["TotalColleges"]) : 0;
        }

        public static int GetTotalStudents()
        {
            DataTable dt = SQLHelper.ExecuteStoredProcedure("spGetTotalStudents", null);
            return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["TotalStudents"]) : 0;
        }

        public static DataTable GetStudentFacultyStats()
        {
            return SQLHelper.ExecuteStoredProcedure("spGetStudentFacultyStats", null);
        }

        public static int GetFaculties()
        {
            DataTable dt = SQLHelper.ExecuteStoredProcedure("spGetFacultyStats", null);
            return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["TotalFaculty"]) : 0;
        }

        public static DataTable GetMenusByAccessLevel(int accessLevel)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@AccessLevel", accessLevel)
            };

            return SQLHelper.ExecuteStoredProcedure("usp_GetMenusByAccessLevel", parameters);
        }

        public static int GetExamFeeStatus()
        {
            DataTable dt = SQLHelper.ExecuteStoredProcedure("spGetExamFeeStatus", null);
            return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["TotalFeesPaid"]) : 0;
        }
    }
}
