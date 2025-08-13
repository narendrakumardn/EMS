using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using BTEDiploma.Helper;

namespace BTEDiploma.sqlhelper
{
    public class ManagesystemdataDAO
    {
        static string connStr = ConfigurationManager.ConnectionStrings["DBC"].ConnectionString;

        public static void InsertExam(string month, string year, string acadYear, string startDate, string endDate, string fee, string dueDate)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("sp_InsertExam", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Exam_Month", month);
                cmd.Parameters.AddWithValue("@Exam_Year", year);
                cmd.Parameters.AddWithValue("@Academic_Year", acadYear);
                cmd.Parameters.AddWithValue("@Academic_Start_Date", startDate);
                cmd.Parameters.AddWithValue("@Academic_End_Date", endDate);
                cmd.Parameters.AddWithValue("@Regular_Fee", fee);
                cmd.Parameters.AddWithValue("@Due_Date", dueDate);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public static DataTable GetAcademicYears()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SP_Get_Academic_Years", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
        public static DataTable GetAllExams()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("sp_GetAllExams", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }

            }
        }
        public static DataTable GetAllPatterns()
        {
            return ExecuteSelectQuery("SELECT Pattern_ID FROM Tb_Teaching_Scheme_Pattern");
        }

        public static DataTable GetAllPrograms1()
        {
            return ExecuteSelectQuery("SELECT Program_Code FROM Tb_Program");
        }

        public static DataTable GetAllSchemes()
        {
            return ExecuteSelectQuery("SELECT Scheme_Code FROM Tb_Curriculum_Scheme");
        }

        public static DataTable GetAllEvaluationPatterns()
        {
            return ExecuteSelectQuery("SELECT Evaluation_Pattern_ID FROM Tb_Evaluation_Scheme_Pattern");
        }

        public static DataTable GetAllCourses()
        {
            return ExecuteSelectQuery("SELECT * FROM Tb_Course");
        }

        public static bool InsertCourse(string courseCode, string courseName, int credit, int patternID,
            string seeType, string parentProgramCode, string schemeCode, int isElective, int evalPatternID, int ciePatternID,
            out int statusCode, out string message)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SP_Insert_Course", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "INSERT");
                cmd.Parameters.AddWithValue("@Course_Code", courseCode);
                cmd.Parameters.AddWithValue("@Course_Name", courseName);
                cmd.Parameters.AddWithValue("@Credit", credit);
                cmd.Parameters.AddWithValue("@Pattern_ID", patternID);
                cmd.Parameters.AddWithValue("@See_Type", seeType);
                cmd.Parameters.AddWithValue("@Parent_Program_Code", parentProgramCode);
                cmd.Parameters.AddWithValue("@Scheme_Code", schemeCode);
                cmd.Parameters.AddWithValue("@Is_Elective", isElective);
                cmd.Parameters.AddWithValue("@Evaluation_Pattern_ID", evalPatternID);
                cmd.Parameters.AddWithValue("@CIE_Pattern_ID", ciePatternID);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    statusCode = Convert.ToInt32(reader["StatusCode"]);
                    message = reader["Message"].ToString();
                    return true;
                }
                statusCode = 500;
                message = "Unknown error";
                return false;
            }
        }
        public static bool UpdateCourse(string courseCode, string courseName, int credit, int patternID,
                string seeType, string parentProgramCode, string schemeCode, int isElective, int evalPatternID, int ciePatternID,
                out int statusCode, out string message)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SP_Insert_Course", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "UPDATE");
                cmd.Parameters.AddWithValue("@Course_Code", courseCode);
                cmd.Parameters.AddWithValue("@Course_Name", courseName);
                cmd.Parameters.AddWithValue("@Credit", credit);
                cmd.Parameters.AddWithValue("@Pattern_ID", patternID);
                cmd.Parameters.AddWithValue("@See_Type", seeType);
                cmd.Parameters.AddWithValue("@Parent_Program_Code", parentProgramCode);
                cmd.Parameters.AddWithValue("@Scheme_Code", schemeCode);
                cmd.Parameters.AddWithValue("@Is_Elective", isElective);
                cmd.Parameters.AddWithValue("@Evaluation_Pattern_ID", evalPatternID);
                cmd.Parameters.AddWithValue("@CIE_Pattern_ID", ciePatternID);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    statusCode = Convert.ToInt32(reader["StatusCode"]);
                    message = reader["Message"].ToString();
                    return true;
                }
                statusCode = 500;
                message = "Unknown error";
                return false;
            }
        }

        /*public static bool DeleteCourse(string courseCode)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SP_Insert_Course", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@Course_Code", courseCode);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }*/
        public static bool DeleteCourse(string courseCode, out int statusCode, out string message)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SP_Insert_Course", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@Course_Code", courseCode);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    statusCode = Convert.ToInt32(reader["StatusCode"]);
                    message = reader["Message"].ToString();
                    return true;
                }
                statusCode = 500;
                message = "Unknown error";
                return false;
            }
        }
        private static DataTable ExecuteSelectQuery(string query)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlDataAdapter da = new SqlDataAdapter(query, con))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
        public static void UpdateExam(int examId, string acadYear, int month, int year, DateTime start, DateTime end, decimal fee, DateTime dueDate)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("sp_UpdateExam", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Exam_ID", examId);
                cmd.Parameters.AddWithValue("@Academic_Year", acadYear);
                cmd.Parameters.AddWithValue("@Exam_Month", month);
                cmd.Parameters.AddWithValue("@Exam_Year", year);
                cmd.Parameters.AddWithValue("@Academic_Start_Date", start);
                cmd.Parameters.AddWithValue("@Academic_End_Date", end);
                cmd.Parameters.AddWithValue("@Regular_Fee", fee);
                cmd.Parameters.AddWithValue("@Due_Date", dueDate);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public DataSet GetExamHierarchy()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("usp_GetExamHierarchy", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
        }

        public DataTable GetExamYears(string academicYear)
        {
            var ds = GetExamHierarchy();
            var filteredRows = ds.Tables[1].Select($"Academic_Year='{academicYear}'");
            return filteredRows.Length > 0 ? filteredRows.CopyToDataTable() : new DataTable();
        }


        public DataTable GetExamMonths(string academicYear, int examYear)
        {
            var ds = GetExamHierarchy();
            var filteredRows = ds.Tables[2].Select($"Academic_Year='{academicYear}' AND Exam_Year={examYear}");
            return filteredRows.Length > 0 ? filteredRows.CopyToDataTable() : new DataTable();
        }


        public DataTable GetProgrammeList()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("SP_Get_Program_List", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public int GetExamId(string academicYear, string examYear, string examMonth)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 Exam_ID FROM Tb_Exam WHERE Academic_Year = @AY AND Exam_Year = @EY AND Exam_Month = @EM", con);
                cmd.Parameters.AddWithValue("@AY", academicYear);
                cmd.Parameters.AddWithValue("@EY", examYear);
                cmd.Parameters.AddWithValue("@EM", examMonth);
                con.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        public DataTable GetCoursesByProgrammeWithTimetable(string programmeCode, int examId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("usp_GetCoursesByProgrammeCodes_WithTimetable", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProgrammeCodes", programmeCode);
                cmd.Parameters.AddWithValue("@ExamID", examId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public void InsertOrUpdateTimeTable(int examId, string courseCode, DateTime date, string session)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_InsertOrUpdate_Timetable", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ExamID", examId);
                cmd.Parameters.AddWithValue("@CourseCode", courseCode);
                cmd.Parameters.AddWithValue("@ExamDate", date);
                cmd.Parameters.AddWithValue("@Session", session);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public  DataTable GetAllInstitutions()
        {
            return SQLHelper.ExecuteStoredProcedure("spGetAllInstitutions", null);
        }

        public  DataTable GetAllPrograms()
        {
            return SQLHelper.ExecuteStoredProcedure("spGetAllPrograms", null);
        }

        public  DataTable GetProgramMappingsByInstitution(int institutionCode)
        {
            SqlParameter[] param = {
            new SqlParameter("@Institution_Code", institutionCode)
             };
            return SQLHelper.ExecuteStoredProcedure("spGetInstitutionProgramMappings", param);
        }

        public  bool InsertInstitutionProgramMapping(int instCode, string progId, int year, int intake, bool isActive, bool isAided)
        {
            SqlParameter[] param = {
            new SqlParameter("@Institution_Code", instCode),
            new SqlParameter("@Program_ID", progId),
            new SqlParameter("@Affiliation_Year", year),
            new SqlParameter("@Current_Approved_Intake", intake),
            new SqlParameter("@Is_Active", isActive),
            new SqlParameter("@Is_Aided",isAided)


        };

            return SQLHelper.ExecuteNonQuery("spInsertInstitutionProgramMapping", CommandType.StoredProcedure, param) > 0;

        }
        public static DataTable GetProgramTypeList()
        {
            return SQLHelper.ExecuteStoredProcedure("Sp_Get_Program_Type_List", null);
        }
        public static DataTable GetProgramList()
        {
            return SQLHelper.ExecuteStoredProcedure("SP_Get_Program_List", null);
        }
        public static bool AddPrograms(String Program_Code, String Program_Name, int Program_Type_ID, decimal credit, String Diploma_Title)
        {
            SqlParameter[] param = {
     new SqlParameter("@Program_Code", Program_Code),
     new SqlParameter("@Program_Name", Program_Name),
     new SqlParameter("@Program_Type_ID", Program_Type_ID),
     new SqlParameter("@credit", credit),
     new SqlParameter("@Diploma_Title", Diploma_Title)
     };
            return SQLHelper.ExecuteNonQuery("SP_Insert_Program", CommandType.StoredProcedure, param) > 0;
        }

        public  bool UpdateInstitutionProgramMapping(int instCode, string progId, int year, int intake, bool isActive, bool isAided)
        {
            SqlParameter[] param = {
        new SqlParameter("@Institution_Code", instCode),
        new SqlParameter("@Program_ID", progId),
        new SqlParameter("@Affiliation_Year", year),
        new SqlParameter("@Current_Approved_Intake", intake),
        new SqlParameter("@Is_Active", isActive),
        new SqlParameter("@Is_Aided",isAided)
    };

            return SQLHelper.ExecuteNonQuery("spUpdateInstitutionProgramMapping", CommandType.StoredProcedure, param) > 0;
        }



    }
}