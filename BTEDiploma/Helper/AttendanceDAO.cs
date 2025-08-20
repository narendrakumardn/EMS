using System;
using System.Data;
using System.Data.SqlClient;
using BTEDiploma.sqlhelper;
using DocumentFormat.OpenXml.Spreadsheet;

namespace BTEDiploma.Helper
{
    public class AttendanceDAO
    {
        public DataTable GetProgramsByInstitute(string instCode)
        {
            SqlParameter[] prms = {
        new SqlParameter("@InstCode", instCode)
    };
            return SQLHelper.ExecuteStoredProcedure("SP_Get_Program_Institution_Mappings", prms);
        }

        public DataSet GetCoursesDropdown()
        {
            return SQLHelper.ExecuteStoredProcedureWithDataset("SP_Get_Program_Course_Dropdowns");
        }

        public DataTable GetStudentsAttendance(string instCode, string courseCode)
        {
            SqlParameter[] prms = {
        new SqlParameter("@InstCode", instCode),
        new SqlParameter("@CourseCode", courseCode)
    };
            return SQLHelper.ExecuteStoredProcedure("SP_Get_Student_Attendance_ByCourse", prms);
        }

        public (int status, string message) InsertOrUpdateAttendance(int semHistoryId, string courseCode, int conducted, int attended)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@Sem_History_ID", semHistoryId),
                new SqlParameter("@Course_Code", courseCode),
                new SqlParameter("@Classes_Conducted", conducted),
                new SqlParameter("@Classes_Attended", attended),
                new SqlParameter("@StatusCode", SqlDbType.Int) { Direction = ParameterDirection.Output },
                new SqlParameter("@StatusMessage", SqlDbType.VarChar, 200) { Direction = ParameterDirection.Output }
            };

            SQLHelper.ExecuteNonQuery(
      "SP_InsertAttendance",
      CommandType.StoredProcedure,
      parameters
  );


            int status = Convert.ToInt32(parameters[4].Value);
            string message = parameters[5].Value.ToString();

            return (status, message);
        }
    }
}
