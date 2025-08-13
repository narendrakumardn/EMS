using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using BTEDiploma.Helper;


namespace BTEDiploma.Helper
{
    public class ATSDao
    {
        public static DataTable GetExamDatesByCollege(string collegeCode)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@CollegeCode", collegeCode)
            };
            return SQLHelper.ExecuteStoredProcedure("spGetExamDatesByCollege", parameters);
        }

        public static DataTable GetCoursesByDateAndCollege(string examDate, string collegeCode)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@ExamDate", examDate),
                new SqlParameter("@CollegeCode", collegeCode)
            };
            return SQLHelper.ExecuteStoredProcedure("spGetCoursesByDateAndCollege", parameters);
        }


        /*  public static int GetStudentCount(string examDate, string courseCode, string collegeCode)
          {
             // System.Diagnostics.Debug.WriteLine($"Calling GetStudentCount with: ExamDate = {examDate}, CourseCode = {courseCode}, CollegeCode = {collegeCode}");

              SqlParameter[] parameters = {
          new SqlParameter("@ExamDate", examDate),
          new SqlParameter("@CourseCode", courseCode),
          new SqlParameter("@CollegeCode", collegeCode)
      };

              object result = SQLHelper.ExecuteStoredProcedure("spGetStudentCountForATS", parameters);

              return result != null ? Convert.ToInt32(result) : 0;
          }
        */
        public static int GetStudentCount(string examDate, string courseCode, string collegeCode)
        {
            SqlParameter[] parameters =
            {
        new SqlParameter("@ExamDate", examDate),
        new SqlParameter("@CourseCode", courseCode),
        new SqlParameter("@CollegeCode", collegeCode)
    };

            // Get DataTable from SP
            DataTable dt = SQLHelper.ExecuteStoredProcedure("spGetStudentCountForATS", parameters);

            if (dt != null && dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0]["StudentCount"]); // Use column name from SP
            }

            return 0;
        }


        public static DataTable GenerateOrFetchATS(string examDate, string courseCode, string collegeCode, int from, int to)
        {
            SqlParameter[] parameters = 
                {
                new SqlParameter("@ExamDate", examDate),
                new SqlParameter("@CourseCode", courseCode),
                new SqlParameter("@CollegeCode", collegeCode),
                new SqlParameter("@From", from),
                new SqlParameter("@To", to)
            };
            return SQLHelper.ExecuteStoredProcedure("spGenerateOrFetchATS", parameters);
        }
    }
}
