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

        public static void InsertExam(int month, int year, int academicYearId, int semId,
                              DateTime startDate, DateTime endDate, decimal fee, DateTime dueDate)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // 🔹 Check duplicates before insert
                using (SqlCommand checkCmd = new SqlCommand(@"
            SELECT COUNT(*) FROM tb_Academic_Calender 
            WHERE Academic_Year_ID = @Academic_Year_ID 
              AND Sem_ID = @Sem_ID"
              , con))
                {
                    checkCmd.Parameters.AddWithValue("@Academic_Year_ID", academicYearId);
                    checkCmd.Parameters.AddWithValue("@Sem_ID", semId);
                    checkCmd.Parameters.AddWithValue("@Exam_Month", month);
                    checkCmd.Parameters.AddWithValue("@Exam_Year", year);

                    int exists = (int)checkCmd.ExecuteScalar();
                    if (exists > 0)
                    {
                        throw new ApplicationException("⚠️ An exam with this Academic Year, Semester, Month, and Year already exists.");
                    }
                }

                // 🔹 Proceed with insert only if no duplicate
                using (SqlCommand cmd = new SqlCommand("sp_InsertExam", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Exam_Month", month);
                    cmd.Parameters.AddWithValue("@Exam_Year", year);
                    cmd.Parameters.AddWithValue("@Academic_Year_ID", academicYearId);
                    cmd.Parameters.AddWithValue("@Sem_Id", semId);
                    cmd.Parameters.AddWithValue("@Academic_Start_Date", startDate);
                    cmd.Parameters.AddWithValue("@Academic_End_Date", endDate);
                    cmd.Parameters.AddWithValue("@Regular_Fee", fee);
                    cmd.Parameters.AddWithValue("@Due_Date", dueDate);

                    cmd.ExecuteNonQuery();
                }
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

        public static DataTable GetSemesters()
        {
            return SQLHelper.ExecuteStoredProcedure("SP_Get_Sem_List", null);
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
        public static void UpdateExam(int examId, int academicYearId, int examMonth, int examYear, DateTime startDate, DateTime endDate, int semId, decimal fee, DateTime dueDate)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("sp_UpdateExam", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Exam_ID", examId);
                cmd.Parameters.AddWithValue("@Exam_Month", examMonth);
                cmd.Parameters.AddWithValue("@Exam_Year", examYear);
                cmd.Parameters.AddWithValue("@Academic_Year_ID", academicYearId);
                cmd.Parameters.AddWithValue("@Sem_Id", semId);
                cmd.Parameters.AddWithValue("@Academic_Start_Date", startDate);
                cmd.Parameters.AddWithValue("@Academic_End_Date", endDate);
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
        public DataTable GetAcademicYears1()
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            SqlCommand cmd = new SqlCommand("SELECT Academic_Year_ID, Academic_Year FROM Tb_Academic_Year_Master", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }

        public DataTable GetSemesters1()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SP_Get_Sem_List", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt); // First result set: Sem_Id, Name
                    return dt;
                }
            }
        }





        // Returns DataTable with Exam_ID (as before)
        public DataTable GetExamID(int academicYearID, int semID)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(@"SELECT Exam_ID FROM Tb_Exam 
                                      WHERE Academic_cal_id = (SELECT Academic_cal_id FROM tb_Academic_Calender 
                                      WHERE Academic_Year_ID=@YearID AND sem_ID=@SemID)", con);
                cmd.Parameters.AddWithValue("@YearID", academicYearID);
                cmd.Parameters.AddWithValue("@SemID", semID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // Helper to get single Exam_ID as int
        private int GetExamIDValue(int academicYearID, int semID)
        {
            DataTable dt = GetExamID(academicYearID, semID);
            if (dt.Rows.Count > 0)
                return Convert.ToInt32(dt.Rows[0]["Exam_ID"]);
            else
                throw new Exception("No Exam found for the given Academic Year and Semester.");
        }

        // Wrapper for Backlog Fee
        public void InsertOrUpdateBacklogFeeByYearSemester(int academicYearID, int semesterID, int backlogCount, decimal fee)
        {
            int examID = GetExamIDValue(academicYearID, semesterID); // <- fixed
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("SP_InsertOrUpdateBacklogFee", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Exam_ID", examID);
                cmd.Parameters.AddWithValue("@Backlog_Count", backlogCount);
                cmd.Parameters.AddWithValue("@Fee", fee);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Wrapper for Late Fine
        public void InsertOrUpdateLateFineByYearSemester(int academicYearID, int semesterID, DateTime lastDate, decimal fine)
        {
            int examID = GetExamIDValue(academicYearID, semesterID); // <- fixed
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("SP_InsertOrUpdateLateFine", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Exam_ID", examID);
                cmd.Parameters.AddWithValue("@Last_Date", lastDate);
                cmd.Parameters.AddWithValue("@Late_Fine", fine);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Grid fetch by Academic Year + Semester
        public DataTable GetGridDataByYearSemester(string operation, int academicYearID, int semesterID)
        {
            int examID = GetExamIDValue(academicYearID, semesterID); // <- fixed
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                if (operation == "Backlog")
                    cmd.CommandText = "SELECT Exam_ID,Backlog_Count AS ValueField, Fee AS FeeField FROM Tb_Exam_Backlog_Fee WHERE Exam_ID=@Exam_ID";
                else
                    cmd.CommandText = "SELECT Exam_ID,CONVERT(varchar, Last_Date, 23) AS ValueField, Late_Fine AS FeeField FROM Tb_Exam_Due_Date WHERE Exam_ID=@Exam_ID";

                cmd.Parameters.AddWithValue("@Exam_ID", examID);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }


        public void UpdateBacklogFee(int examID, int backlogCount, decimal fee)
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            SqlCommand cmd = new SqlCommand("UPDATE Tb_Exam_Backlog_Fee SET Fee=@Fee WHERE Exam_ID=@Exam_ID and Backlog_Count=@Count", con);
            cmd.Parameters.AddWithValue("@Count", backlogCount);
            cmd.Parameters.AddWithValue("@Fee", fee);
            cmd.Parameters.AddWithValue("@Exam_ID", examID);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public void UpdateLateFine(int examID, DateTime lastDate, decimal fine)
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            SqlCommand cmd = new SqlCommand("UPDATE Tb_Exam_Due_Date SET Last_Date=@LastDate, Late_Fine=@LateFine WHERE Exam_ID=@Exam_ID", con);
            cmd.Parameters.AddWithValue("@LastDate", lastDate);
            cmd.Parameters.AddWithValue("@LateFine", fine);
            cmd.Parameters.AddWithValue("@Exam_ID", examID);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }
        public DataTable GetSemestersByAcademicYear(int academicYearId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SELECT ac.Academic_cal_id, s.Name, ac.sem_ID FROM tb_Academic_Calender ac INNER JOIN Tb_Sem_Id s ON ac.sem_ID = s.Sem_Id WHERE ac.Academic_Year_ID = @AcademicYearId ORDER BY ac.sem_ID", con))
            {
                cmd.Parameters.AddWithValue("@AcademicYearId", academicYearId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public int GetExamIdByAcademicYearAndSemester(int academicYearId, int semId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(@"SELECT TOP 1 e.Exam_ID 
                                             FROM Tb_Exam e
                                             INNER JOIN tb_Academic_Calender ac ON e.Academic_cal_id = ac.Academic_cal_id
                                             WHERE ac.Academic_Year_ID = @AcademicYearId AND ac.sem_ID = @SemId", con))
            {
                cmd.Parameters.AddWithValue("@AcademicYearId", academicYearId);
                cmd.Parameters.AddWithValue("@SemId", semId);

                con.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }
        public DataTable GetBridgeCourses(string programCode, int semesterNo)
        {
            SqlParameter[] parameters = {
        new SqlParameter("@Program_Code", programCode),
        new SqlParameter("@Semester", semesterNo) // ✅ Correct name
    };
            return SQLHelper.ExecuteDataTable("SP_Get_Bridge_Courses", parameters);
        }


        // 2. Get Courses by Program & Semester (matches SP signature)
        public DataTable GetCoursesByProgramSemester(
            string programCode,
            int semesterNo,
            int sno,
            int admissionTypeId
        )
        {
            SqlParameter[] parameters = {
        new SqlParameter("@Program_Code", programCode),
        new SqlParameter("@Semester", semesterNo),
        new SqlParameter("@Sno", sno),
        new SqlParameter("@Admission_Type_ID", admissionTypeId)
    };
            return SQLHelper.ExecuteDataTable("SP_Get_Courses_By_Program_Semester", parameters);
        }

        // 3. NEW: Get Program Course Pivoted
        public DataTable GetProgramCoursePivoted(string programCode, int admissionTypeId)
        {
            SqlParameter[] parameters = {
        new SqlParameter("@ProgramCode", programCode),        // ✅ matches SP param
        new SqlParameter("@AdmissionTypeID", admissionTypeId) // ✅ matches SP param
    };
            return SQLHelper.ExecuteDataTable("SP_Get_Program_Course_Pivoted", parameters);
        }


        // 4. Insert Program Course (matching your current .aspx.cs usage)
        public int InsertProgramCourse(
            string programCode,
            int semesterNo,
            string courseCode,
            int admissionTypeId,
            int sno,
            out int statusCode,
            out string statusMessage
        )
        {
            SqlParameter[] parameters = {
        new SqlParameter("@Program_Code", programCode),
        new SqlParameter("@Semester", semesterNo),
        new SqlParameter("@Course_Code", courseCode),
        new SqlParameter("@Admission_Type_ID", admissionTypeId),
        new SqlParameter("@Sno", sno),
        new SqlParameter("@StatusCode", SqlDbType.Int) { Direction = ParameterDirection.Output },
        new SqlParameter("@StatusMessage", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output }
    };

            int rowsAffected = SQLHelper.ExecuteNonQuery1("SP_Insert_Program_Course", CommandType.StoredProcedure, parameters);

            statusCode = (int)parameters[5].Value;
            statusMessage = parameters[6].Value.ToString();

            return rowsAffected;
        }


        // 5. Update Program Course (with Table-Valued Parameter)
        public int UpdateProgramCourse_TVP(string programCode, int admissionTypeId, int semester, DataTable courses)
        {
            SqlParameter[] parameters = {
        new SqlParameter("@ProgramCode", programCode),
        new SqlParameter("@AdmissionTypeID", admissionTypeId),
        new SqlParameter("@Semester", semester),
        new SqlParameter("@Courses", SqlDbType.Structured)
        {
            TypeName = "dbo.CourseList",  // Must match your user-defined table type
            Value = courses
        }
    };

            return SQLHelper.ExecuteNonQuery1("SP_Update_Program_Course", CommandType.StoredProcedure, parameters);
        }


        public int UpdateBridgeCourse(string programCode, int semesterNo, int admissionTypeId, string course1Code, string course2Code)
        {
            SqlParameter[] parameters = {
        new SqlParameter("@Program_Code", programCode),
        new SqlParameter("@Semester", semesterNo),
        new SqlParameter("@AdmissionTypeID", admissionTypeId),
        new SqlParameter("@Course1_Code", (object)course1Code ?? DBNull.Value),
        new SqlParameter("@Course2_Code", (object)course2Code ?? DBNull.Value)
    };

            return SQLHelper.ExecuteNonQuery1("SP_Update_Bridge_Courses", CommandType.StoredProcedure, parameters);
        }


        // 7. Get Curriculum Scheme Details (no parameters)
        public DataTable GetCurriculumSchemeDetails()
        {
            return SQLHelper.ExecuteDataTable("SP_Get_Curriculum_Scheme_Details", null);
        }


        public DataTable GetAllCourses1()
        {
            string query = "SELECT Course_Code, Course_Code + ' - ' + Course_Name AS DisplayName FROM Tb_Course";
            return SQLHelper.ExecuteDataTableText(query);
        }



        // 8. Get Program, Course, and Admission Type dropdown data (no parameters)
        public DataSet GetProgramCourseDropdowns()
        {
            return SQLHelper.ExecuteDataSet("SP_Get_Program_Course_Dropdowns", null);
        }

        public static DataTable GetAllInstitutions1()
        {
            const string query = @"
SELECT 
    i.Inst_Code,
    i.Inst_Name,
    t.Taluk_ID,
    t.Taluk_Name,
    i.Address,
    i.Pincode,
    it.Institution_Type_ID,
    it.Institution_Type_Description,
    s.Inst_Shift_ID,
    s.Shift_Name,
    ez.Exam_Zone_ID,
    ez.Exam_Zone_Name,
    i.AICTE_ID,
    i.Is_Exam_Center,
    i.Gender,
    i.Principal_ID,
    i.Email_ID,
    i.Phone
FROM Tb_Institution AS i
LEFT JOIN Tb_Taluk AS t ON i.Taluk_ID = t.Taluk_ID
LEFT JOIN Tb_Institution_Shift AS s ON i.Inst_Shift = s.Inst_Shift_ID
LEFT JOIN Tb_Exam_Zone AS ez ON i.Inst_Exam_Zone = ez.Exam_Zone_ID
LEFT JOIN Tb_Institution_Type AS it ON i.Institution_Type_ID = it.Institution_Type_ID
ORDER BY i.Inst_Code DESC;";

            return SQLHelper.ExecuteDataTable1(query);
        }

        public static DataTable GetShifts()
        {
            return SQLHelper.ExecuteDataTable1("SELECT Inst_Shift_ID, Shift_Name FROM Tb_Institution_Shift");
        }

        public static DataTable GetTaluks()
        {
            return SQLHelper.ExecuteDataTable1("SELECT Taluk_ID, Taluk_Name FROM Tb_Taluk");
        }

        public static DataTable GetExamZones()
        {
            return SQLHelper.ExecuteDataTable1("SELECT Exam_Zone_ID, Exam_Zone_Name FROM Tb_Exam_Zone");
        }

        public static DataTable GetInstitutionTypes()
        {
            return SQLHelper.ExecuteDataTable1("SELECT Institution_Type_ID, Institution_Type_Description FROM Tb_Institution_Type");
        }

        // -------------------- CRUD via Stored Procedure --------------------

        public static bool InsertInstitution(
            int instCode, string instName, int talukID, string address, string pincode,
            int instTypeID, int instShift, int instExamZone, string aicteID, bool isExamCenter,
            string gender, int? principalID, string emailID, string phone,
            out int statusCode, out string message)
        {
            return ExecuteInstitutionSP(
                "INSERT", instCode, instName, talukID, address, pincode,
                instTypeID, instShift, instExamZone, aicteID, isExamCenter,
                gender, principalID, emailID, phone,
                out statusCode, out message
            );
        }

        public static bool UpdateInstitution(
            int instCode, string instName, int talukID, string address, string pincode,
            int instTypeID, int instShift, int instExamZone, string aicteID, bool isExamCenter,
            string gender, int? principalID, string emailID, string phone,
            out int statusCode, out string message)
        {
            return ExecuteInstitutionSP(
                "UPDATE", instCode, instName, talukID, address, pincode,
                instTypeID, instShift, instExamZone, aicteID, isExamCenter,
                gender, principalID, emailID, phone,
                out statusCode, out message
            );
        }

        public static bool DeleteInstitution(int instCode, out int statusCode, out string message)
        {
            var prms = new[]
            {
                new SqlParameter("@Action", SqlDbType.VarChar, 10) { Value = "DELETE" },
                new SqlParameter("@Inst_Code", SqlDbType.Int) { Value = instCode }
            };

            var dt = SQLHelper.ExecuteStoredProcedure("SP_Institution_CRUD", prms);
            return ParseStatus(dt, out statusCode, out message);
        }
        public static DataTable GetAllInstitutionTypes()
        {
            return SQLHelper.ExecuteStoredProcedure("spGetAllInstitutionTypes", null);
        }
        public static DataTable GetInstitutionsByType(int institutionTypeId)
        {
            SqlParameter[] param = {
        new SqlParameter("@Institution_Type_ID", institutionTypeId)
    };

            return SQLHelper.ExecuteStoredProcedure("spGetInstitutionsByType", param);
        }
        // -------------------- Internal SP executor --------------------

        private static bool ExecuteInstitutionSP(
            string action,
            int instCode, string instName, int talukID, string address, string pincode,
            int instTypeID, int instShift, int instExamZone, string aicteID, bool isExamCenter,
            string gender, int? principalID, string emailID, string phone,
            out int statusCode, out string message)
        {
            var prms = new[]
            {
                new SqlParameter("@Action", SqlDbType.VarChar, 10)   { Value = action },
                new SqlParameter("@Inst_Code", SqlDbType.Int)        { Value = instCode },

                new SqlParameter("@Inst_Name", SqlDbType.VarChar, 100) { Value = (object)instName ?? DBNull.Value },
                new SqlParameter("@Taluk_ID", SqlDbType.Int)           { Value = talukID },
                new SqlParameter("@Address", SqlDbType.VarChar, 100)   { Value = (object)address ?? DBNull.Value },
                new SqlParameter("@Pincode", SqlDbType.VarChar, 6)     { Value = (object)pincode ?? DBNull.Value },
                new SqlParameter("@Institution_Type_ID", SqlDbType.Int){ Value = instTypeID },
                new SqlParameter("@Inst_Shift", SqlDbType.Int)         { Value = instShift },
                new SqlParameter("@Inst_Exam_Zone", SqlDbType.Int)     { Value = instExamZone },
                new SqlParameter("@AICTE_ID", SqlDbType.VarChar, 20)   { Value = (object)aicteID ?? DBNull.Value },
                new SqlParameter("@Is_Exam_Center", SqlDbType.Bit)     { Value = isExamCenter },
                new SqlParameter("@Gender", SqlDbType.Char, 1)         { Value = (object)gender ?? DBNull.Value },
                new SqlParameter("@Principal_ID", SqlDbType.Int)       { Value = (object)principalID ?? DBNull.Value },
                new SqlParameter("@Email_ID", SqlDbType.VarChar, 100)  { Value = (object)emailID ?? DBNull.Value },
                new SqlParameter("@Phone", SqlDbType.VarChar, 10)      { Value = (object)phone ?? DBNull.Value }
            };

            var dt = SQLHelper.ExecuteStoredProcedure("SP_Institution_CRUD", prms);
            return ParseStatus(dt, out statusCode, out message);
        }

        // -------------------- Result parsing --------------------

        private static bool ParseStatus(DataTable dt, out int statusCode, out string message)
        {
            statusCode = 500;
            message = "Unknown error";

            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                statusCode = row.Table.Columns.Contains("StatusCode") ? Convert.ToInt32(row["StatusCode"]) : 500;
                message = row.Table.Columns.Contains("Message") ? Convert.ToString(row["Message"]) : "Unknown result";
                return true;
            }
            return false;
        }


    }
}