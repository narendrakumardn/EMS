using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using BTEDiploma.Helper;


namespace BTEDiploma.Helper
{
    public class AttendanceSaveResult
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
    }

    public class AttendanceFinalizeResult
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
    }
    public class StudentModel
    {
        public int Student_Semeter_ID { get; set; }
        public string Register_Number { get; set; }
        public string Name { get; set; }
    }

    public class CourseModel
    {
        public string Course_Code { get; set; }
        public string Course_Name { get; set; }
    }
    public class CondonableReasonModel
    {
        public int reason_id { get; set; }
        public string reason_desc { get; set; }
    }
    public class CondonableStudentModel
    {
        public int Student_Semeter_ID { get; set; }
        public string Name { get; set; }
        public string Course_Name { get; set; }
        public string Course_Code { get; set; }
    }
    public class AttendanceDAO
    {
        // 1️⃣ Get condonable students (from sp_GetCondonableStudents)
        public static List<CondonableStudentModel> GetCondonableStudents(int? instCode, string programCode, int? semester, int? studentId = null)
        {
            SqlParameter[] parameters = {
        new SqlParameter("@Institution_Code", instCode ?? (object)DBNull.Value),
        new SqlParameter("@Program_Code", programCode ?? (object)DBNull.Value),
        new SqlParameter("@Semester", semester ?? (object)DBNull.Value),
        new SqlParameter("@Student_Semester_ID", studentId ?? (object)DBNull.Value)
         };
            var ds = SQLHelper.ExecuteStoredProcedureWithDataset("sp_GetCondonableStudents", parameters);
            return ds.Tables[0].AsEnumerable().Select(r => new CondonableStudentModel
            {
                Student_Semeter_ID = Convert.ToInt32(r["Sem_History_ID"]),
                Name = r["Name"].ToString(),
                Course_Code = r.Table.Columns.Contains("Course_Code") ? r["Course_Code"].ToString() : null,
                Course_Name = r.Table.Columns.Contains("Course_Name") ? r["Course_Name"].ToString() : null
            }).ToList();
        }
        public static DataTable GetAttendanceFromSP(int institutionCode, string programCode, int semester)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Institution_Code", institutionCode),
                new SqlParameter("@Program_Code", programCode),
                new SqlParameter("@Semester", semester)
            };

            return SQLHelper.ExecuteStoredProcedure("sp_GetAttendanceReport", parameters);
        }
        public static DataTable GetEligibleListFromSP(int institutionCode, string programCode, int semester)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Institution_Code", institutionCode),
                new SqlParameter("@Program_Code", programCode),
                new SqlParameter("@Sem", semester)
            };

            return SQLHelper.ExecuteStoredProcedure("sp_GetEligibleCandidates", parameters);
        }

        public static DataTable GetExamMonthYear()
        {
            SqlParameter[] parameters = new SqlParameter[]
            {

            };

            return SQLHelper.ExecuteStoredProcedure("sp_GetCurrentExamMonthYear", parameters);
        }


        public static List<CondonableReasonModel> GetCondonableReasons()
        {
            var ds = SQLHelper.ExecuteStoredProcedureWithDataset("sp_GetCondonableReasons");
            return ds.Tables[0].AsEnumerable().Select(r => new CondonableReasonModel
            {
                reason_id = Convert.ToInt32(r["reason_id"]),
                reason_desc = r["reason_desc"].ToString()
            }).ToList();
        }

        public static (int StatusCode, string StatusMessage) SaveCondonableStudent(int semId, string courseCode, int reasonId, DateTime dateOfCert, string remark)
        {
            SqlParameter[] parameters = {
        new SqlParameter("@Student_Semester_ID", semId),
        new SqlParameter("@Course_Code", courseCode),
        new SqlParameter("@Reason_ID", reasonId),
        new SqlParameter("@DateOfCertificate", dateOfCert),
        new SqlParameter("@Remark", remark)
    };
            var ds = SQLHelper.ExecuteStoredProcedureWithDataset("sp_SaveCondonableStudent", parameters);
            int code = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            string msg = ds.Tables[0].Rows[0][1].ToString();
            return (code, msg);
        }

        public static DataTable GetCondonableStudentList(int instCode, string programCode, int semester)
        {
            SqlParameter[] parameters = {
        new SqlParameter("@Institution_Code", instCode),
        new SqlParameter("@Program_Code", programCode),
        new SqlParameter("@Semester", semester)
    };
            return SQLHelper.ExecuteStoredProcedureWithDataset("sp_GetCondonableStudentList", parameters).Tables[0];
        }

        public static int GetClassesConductedForCourse(string courseCode)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@Course_Code", courseCode),
                  };

            DataTable dt = SQLHelper.ExecuteStoredProcedure("sp_GetClassesConductedForCourse", parameters);

            if (dt.Rows.Count > 0 && dt.Rows[0]["Classes_Conducted"] != DBNull.Value)
            {
                return Convert.ToInt32(dt.Rows[0]["Classes_Conducted"]);
            }

            return 0;
        }


        public static AttendanceSaveResult SaveAttendance(int semHistoryId, string courseCode, int classesAttended, int classesConducted)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@Sem_History_ID", SqlDbType.Int) { Value = semHistoryId },
        new SqlParameter("@Course_Code", SqlDbType.Char, 8) { Value = courseCode },
        new SqlParameter("@Classes_Attended", SqlDbType.Int) { Value = classesAttended },
        new SqlParameter("@Classes_Conducted", SqlDbType.Int) { Value = classesConducted }
            };

            DataSet ds = SQLHelper.ExecuteStoredProcedureWithDataset("sp_SaveAttendance", parameters);

            AttendanceSaveResult result = new AttendanceSaveResult();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result.StatusCode = Convert.ToInt32(ds.Tables[0].Rows[0]["StatusCode"]);
                result.StatusMessage = ds.Tables[0].Rows[0]["Message"].ToString();
            }
            else
            {
                result.StatusCode = -1;
                result.StatusMessage = "Unknown error while saving attendance.";
            }

            return result;
        }
        public static DataSet GetAttendanceSummary(int instCode, string programCode, int semester)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@InstCode", SqlDbType.Int) { Value = instCode },
                new SqlParameter("@ProgramCode", SqlDbType.VarChar, 4) { Value = programCode },
                new SqlParameter("@Semester", SqlDbType.Int) { Value = semester }
            };

            return SQLHelper.ExecuteStoredProcedureWithDataset("SP_Get_Attendance_Summary", parameters);
        }
        public static bool IsAttendanceFinalized(int institutionCode, string programCode, int semester, int processId)
        {
            SqlParameter[] parameters = new SqlParameter[]
    {
        new SqlParameter("@Institution_Code", institutionCode),
        new SqlParameter("@Program_Code", programCode),
        new SqlParameter("@Semester", semester),
        new SqlParameter("@Process_ID", processId),
        new SqlParameter("@Is_Finalized", SqlDbType.Bit) { Direction = ParameterDirection.Output },
        new SqlParameter("@StatusCode", SqlDbType.Int) { Direction = ParameterDirection.Output },
        new SqlParameter("@StatusMessage", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output }
    };

            SQLHelper.ExecuteStoredProcedure("sp_GetAttendanceStatus", parameters);

            bool isFinalized = parameters[4].Value != DBNull.Value && Convert.ToBoolean(parameters[4].Value);
            return isFinalized;
        }

        public static bool IsCondonaleListFinalized(int institutionCode, string programCode, int semester, int processId)
        {
            SqlParameter[] parameters = new SqlParameter[]
    {
        new SqlParameter("@Institution_Code", institutionCode),
        new SqlParameter("@Program_Code", programCode),
        new SqlParameter("@Semester", semester),
        new SqlParameter("@Process_ID", processId),
        new SqlParameter("@Is_Finalized", SqlDbType.Bit) { Direction = ParameterDirection.Output },
        new SqlParameter("@StatusCode", SqlDbType.Int) { Direction = ParameterDirection.Output },
        new SqlParameter("@StatusMessage", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output }
    };

            SQLHelper.ExecuteStoredProcedure("sp_GetAttendanceStatus", parameters);

            bool isFinalized = parameters[4].Value != DBNull.Value && Convert.ToBoolean(parameters[4].Value);
            return isFinalized;
        }
        public static bool IsIAFinalized(int institutionCode, string programCode, int semester, int processId)
        {
            SqlParameter[] parameters = new SqlParameter[]
    {
        new SqlParameter("@Institution_Code", institutionCode),
        new SqlParameter("@Program_Code", programCode),
        new SqlParameter("@Semester", semester),
        new SqlParameter("@Process_ID", processId),
        new SqlParameter("@Is_Finalized", SqlDbType.Bit) { Direction = ParameterDirection.Output },
        new SqlParameter("@StatusCode", SqlDbType.Int) { Direction = ParameterDirection.Output },
        new SqlParameter("@StatusMessage", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output }
    };

            SQLHelper.ExecuteStoredProcedure("sp_GetAttendanceStatus", parameters);

            bool isFinalized = parameters[4].Value != DBNull.Value && Convert.ToBoolean(parameters[4].Value);
            return isFinalized;
        }

        public static AttendanceFinalizeResult FinalizeCondonable(
            int instCode,
            string programCode,
            int semester,
            int processId,
            int employeeId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Institution_Code", SqlDbType.Int) { Value = instCode },
                new SqlParameter("@Program_Code", SqlDbType.VarChar, 4) { Value = programCode },
                new SqlParameter("@Semester", SqlDbType.Int) { Value = semester },
                new SqlParameter("@Process_ID", SqlDbType.Int) { Value = processId },
                new SqlParameter("@Employee_ID", SqlDbType.Int) { Value = employeeId },

                // Output params
                new SqlParameter("@StatusCode", SqlDbType.Int) { Direction = ParameterDirection.Output },
                new SqlParameter("@StatusMessage", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output }
            };

            SQLHelper.ExecuteNonQuery("SP_FinalizeCondonableList", CommandType.StoredProcedure, parameters);

            return new AttendanceFinalizeResult
            {
                StatusCode = Convert.ToInt32(parameters[5].Value),       // @StatusCode
                StatusMessage = parameters[6].Value.ToString()           // @StatusMessage
            };
        }
        public static AttendanceFinalizeResult FinalizeAttendance(
            int instCode,
            string programCode,
            int semester,
            int processId,
            int employeeId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Institution_Code", SqlDbType.Int) { Value = instCode },
                new SqlParameter("@Program_Code", SqlDbType.VarChar, 4) { Value = programCode },
                new SqlParameter("@Semester", SqlDbType.Int) { Value = semester },
                new SqlParameter("@Process_ID", SqlDbType.Int) { Value = processId },
                new SqlParameter("@Employee_ID", SqlDbType.Int) { Value = employeeId },

                // Output params
                new SqlParameter("@StatusCode", SqlDbType.Int) { Direction = ParameterDirection.Output },
                new SqlParameter("@StatusMessage", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output }
            };

            SQLHelper.ExecuteNonQuery("sp_FinalizeAttendance", CommandType.StoredProcedure, parameters);

            return new AttendanceFinalizeResult
            {
                StatusCode = Convert.ToInt32(parameters[5].Value),       // @StatusCode
                StatusMessage = parameters[6].Value.ToString()           // @StatusMessage
            };
        }

        public static SQLHelper.ProcedureResult InsertExamEligibility(int examId, int studentSemesterId, string courseCode)
        {
            SqlParameter[] prms = new SqlParameter[]
            {
        new SqlParameter("@Exam_ID", examId),
        new SqlParameter("@Student_Semester_ID", studentSemesterId),
        new SqlParameter("@Course_Code", courseCode)
            };

            DataTable dt = SQLHelper.ExecuteStoredProcedure("sp_InsertExamEligibilityForStudent", prms);

            if (dt != null && dt.Rows.Count > 0)
            {
                return new SQLHelper.ProcedureResult
                {
                    StatusCode = Convert.ToInt32(dt.Rows[0]["StatusCode"]),
                    Message = dt.Rows[0]["StatusMessage"].ToString()
                };
            }
            else
            {
                return new SQLHelper.ProcedureResult
                {
                    StatusCode = 500,
                    Message = "No response from stored procedure."
                };
            }
        }
        public static AttendanceFinalizeResult MarkEndAcademicYearForEligibleStudents(
    int? institutionCode,
    string programCode,
    int? semester)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
    new SqlParameter("@Institution_Code", SqlDbType.Int) { Value = (object)institutionCode ?? DBNull.Value },
    new SqlParameter("@Program_Code", SqlDbType.VarChar, 4) { Value = (object)programCode ?? DBNull.Value },
    new SqlParameter("@Sem", SqlDbType.Int) { Value = (object)semester ?? DBNull.Value },

    // Output params
    new SqlParameter("@StatusCode", SqlDbType.Int) { Direction = ParameterDirection.Output },
    new SqlParameter("@StatusMessage", SqlDbType.NVarChar, 200) { Direction = ParameterDirection.Output }
            };

            SQLHelper.ExecuteNonQuery("sp_MarkEndAcademicYearForEligibleStudents", CommandType.StoredProcedure, parameters);

            return new AttendanceFinalizeResult
            {
                StatusCode = parameters[3].Value != DBNull.Value ? Convert.ToInt32(parameters[3].Value) : -1,
                StatusMessage = parameters[4].Value != DBNull.Value ? parameters[4].Value.ToString() : "No message returned"
            };
        }

        public static (List<StudentModel> Students, int StatusCode, string StatusMessage)
           GetStudentsByInstProgramSemester(int instCode, string programCode, int semester)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Institution_Code", SqlDbType.Int) { Value = instCode },
                new SqlParameter("@Program_Code", SqlDbType.VarChar, 4) { Value = programCode },
                new SqlParameter("@Semester", SqlDbType.Int) { Value = semester }
            };

            DataSet ds = SQLHelper.ExecuteStoredProcedureWithDataset("sp_GetStudentsByInstProgramSemester", parameters);

            List<StudentModel> students = new List<StudentModel>();
            int statusCode = -1;
            string statusMessage = "Unknown error";

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    students.Add(new StudentModel
                    {
                        Student_Semeter_ID = row["Student_Semeter_ID"] != DBNull.Value ? Convert.ToInt32(row["Student_Semeter_ID"]) : 0,
                        Register_Number = row["Register_Number"].ToString(),
                        Name = row["Name"].ToString()
                    });
                }
            }

            // If SP returns status in second table
            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                statusCode = Convert.ToInt32(ds.Tables[1].Rows[0]["StatusCode"]);
                statusMessage = ds.Tables[1].Rows[0]["StatusMessage"].ToString();
            }
            else
            {
                statusCode = 200;
                statusMessage = "Success";
            }

            return (students, statusCode, statusMessage);
        }

        // ✅ Get Courses
        public static (List<CourseModel> Courses, int StatusCode, string StatusMessage)
            GetCoursesByProgramAndSemester(string programCode, int semester)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Program_Code", SqlDbType.VarChar, 4) { Value = programCode },
                new SqlParameter("@Semester", SqlDbType.Int) { Value = semester }
            };

            DataSet ds = SQLHelper.ExecuteStoredProcedureWithDataset("sp_GetCoursesByProgramAndSemesterforAttendance", parameters);

            List<CourseModel> courses = new List<CourseModel>();
            int statusCode = -1;
            string statusMessage = "Unknown error";

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    courses.Add(new CourseModel
                    {
                        Course_Code = row["Course_Code"].ToString(),
                        Course_Name = row["Course_Name"].ToString()
                    });
                }
            }

            // If SP returns status in second table
            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                statusCode = Convert.ToInt32(ds.Tables[1].Rows[0]["StatusCode"]);
                statusMessage = ds.Tables[1].Rows[0]["StatusMessage"].ToString();
            }
            else
            {
                statusCode = 200;
                statusMessage = "Success";
            }

            return (courses, statusCode, statusMessage);
        }
    }
}
