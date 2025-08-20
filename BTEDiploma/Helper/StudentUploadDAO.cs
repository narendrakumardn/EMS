using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using BTEDiploma.Helper;
using DocumentFormat.OpenXml.Bibliography;
using System.Configuration;


namespace BTEDiploma.Helper
{
    public class ImportStudentResult
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public int StudentsProcessed { get; set; }
    }
    public class AcademicYear
    {
        public int AcademicYearID { get; set; }
        public string AcademicYearDesc { get; set; }
    }
    public class AcademicYearResultStatus
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public List<AcademicYear> AcademicYears { get; set; }
    }
    public class StudentUploadDao
    {
        public static int InsertToTbStudent(StudentUploadModel student)
        {
            try
            {
                SqlParameter[] parameters = {
            new SqlParameter("@AdharNumber", student.AdharNumber),
            new SqlParameter("@Application_Number", student.Application_Number),
            new SqlParameter("@SATS_Number", student.SATS_Number),
            new SqlParameter("@Name", student.Name),
            new SqlParameter("@Father_Name", student.Father_Name),
            new SqlParameter("@Mother_Name", student.Mother_Name),
            new SqlParameter("@Address", student.Address),
            new SqlParameter("@PINcode", student.PINcode),
            new SqlParameter("@TalukID", student.TalukID),
            new SqlParameter("@Gender", student.Gender),
            new SqlParameter("@Admission_Type", student.Admission_Type),
            new SqlParameter("@Date_Of_Birth", student.Date_Of_Birth),
            new SqlParameter("@Mobile_Number", student.Mobile_Number),
            new SqlParameter("@Email_ID", student.Email_ID),
            new SqlParameter("@SNQ", student.SNQ),
            new SqlParameter("@Cat_Code", student.Cat_Code)
        };

                var result = SQLHelper.ExecuteProcedureWithId("SP_Insert_Student", parameters, "Student_ID");

                if (result.StatusCode == 200 && result.ReturnedId.HasValue)
                {
                    return result.ReturnedId.Value;
                }
                else if (result.StatusCode == 409)
                    throw new Exception("Student already Exist, check duplicate value in adhar, Stas or applicatio nnumber.");
                else
                {
                    // Optional: log error here
                    //LogError("InsertToTbStudent", result.Message, result.StatusCode);
                    throw new Exception($"Student insert failed: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                // Optional: log unexpected exceptions
                //LogError("InsertToTbStudent Exception", ex.Message, 500);
                throw;
                //new Exception("Unexpected error while inserting student: " + ex.Message, ex);
            }
        }

        public static int InsertToTbStudentEnrollment(int studentId, StudentUploadModel student, String registerNumber, String academicYear, bool isApprovedByACM = false)
        {
            try
            {
                SqlParameter[] parameters = {
                new SqlParameter("@Student_ID ", studentId),
                new SqlParameter("@Register_Number ", registerNumber),
                new SqlParameter("@Program_Code ", student.Program_Code),
                new SqlParameter("@Institution_Code", student.Institution_Code),
                new SqlParameter("@Admission_Year", academicYear),
                new SqlParameter("@Is_Approved_By_ACM ", isApprovedByACM)
            };
                //return SQLHelper.ExecuteStoredProcedure("spGetCoursesByDateAndCollege", parameters);
                // return SQLHelper.ExecuteNonQuery("SP_Insert_Student_Enrollment", CommandType.StoredProcedure, parameters);
                var result = SQLHelper.ExecuteProcedureWithId("SP_Insert_Student_Enrollment", parameters, "Enrollment_ID");

                if (result.StatusCode == 200 && result.ReturnedId.HasValue)
                    return result.ReturnedId.Value;
                else if (result.StatusCode == 409)
                    throw new Exception("Student already enrolled.");
                else
                    throw new Exception($"Student enrollment  failed: {result.Message}");
            }
            catch (Exception ex)
            {
                // Optional: log the exception here
                throw;
            }
        }
        public static DataTable GetStudentEnrollmentView()
        {
            return SQLHelper.ExecuteStoredProcedure("SP_Get_Student_Enrollment_View", null);
        }
        // Updated method using existing SQLHelper.ExecuteStoredProcedureWithDataset
        public static List<StudentEnrollmentViewModel> GetStudentsForApproval(int institutionCode, string programCode)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@Institution_Code", institutionCode),
        new SqlParameter("@Program_Code", programCode)
            };

            DataSet ds = SQLHelper.ExecuteStoredProcedureWithDataset("SP_Get_Students_For_Approval", parameters);
            List<StudentEnrollmentViewModel> students = new List<StudentEnrollmentViewModel>();

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    students.Add(new StudentEnrollmentViewModel
                    {
                        Student_ID = Convert.ToInt32(row["Student_ID"]),
                        Student_Enrollment_ID = Convert.ToInt32(row["Student_Enrollment_ID"]),
                        Student_Name = row["Name"].ToString(),
                        Register_Number = row["Register_Number"].ToString(),
                        Institution_Code = Convert.ToInt32(row["Inst_Code"]),
                        Program_Code = row["Program_Code"].ToString(),
                        Is_Approved_By_ACM = Convert.ToBoolean(row["Is_Approved_By_ACM"]),
                        Admission_Type = Convert.ToInt32(row["Admission_Type"]),
                        Admission_Type_Description = row["Admission_Type_Description"].ToString(),
                        CatCode = row["Cat_Code"].ToString(),
                        Gender = row["Gender"].ToString()
                    });
                }
            }

            return students;
        }

        public static void UpdateACMApproval(int studentId, int studentEnrollmentId, bool isApproved)
        {
            SqlParameter[] parameters = {
        new SqlParameter("@Enrollment_ID", studentEnrollmentId),
        new SqlParameter("@Is_Approved_By_ACM", isApproved)
    };

            SQLHelper.ExecuteNonQuery("SP_Update_IsApproved_By_ACM", CommandType.StoredProcedure, parameters);
        }
        public static AcademicYearResultStatus GetAcademicYearList()
        {
            DataSet ds = SQLHelper.ExecuteStoredProcedureWithDataset("SP_Get_Academic_Years");

            var result = new AcademicYearResultStatus();
            result.AcademicYears = new List<AcademicYear>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (Convert.ToInt32(row["StatusCode"]) == 200)
                    {
                        result.StatusCode = 200;
                        result.Message = row["Message"].ToString();
                        result.AcademicYears.Add(new AcademicYear
                        {
                            AcademicYearID = Convert.ToInt32(row["Academic_Year_ID"]),
                            AcademicYearDesc = row["Academic_Year"].ToString()
                        });
                    }
                    else
                    {
                        result.StatusCode = Convert.ToInt32(row["StatusCode"]);
                        result.Message = row["Message"].ToString();
                    }
                }
            }
            else
            {
                result.StatusCode = 400;
                result.Message = "No data returned.";
            }

            return result;
        }
        public static ImportStudentResult ImportStudentsFromSource(int academicYearId)
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Academic_Year_ID", academicYearId)
            };

            DataTable resultTable = SQLHelper.ExecuteStoredProcedure("SP_Import_Students_From_Source", parameters);

            if (resultTable.Rows.Count > 0)
            {
                DataRow row = resultTable.Rows[0];
                return new ImportStudentResult
                {
                    StatusCode = Convert.ToInt32(row["StatusCode"]),
                    Message = row["Message"].ToString(),
                    StudentsProcessed = Convert.ToInt32(row["StudentsProcessed"])
                };
            }

            return new ImportStudentResult
            {
                StatusCode = 500,
                Message = "No data returned from procedure.",
                StudentsProcessed = 0
            };
        }
        public static List<StudentEnrollmentViewModel> GetStudentsForApproval(int institutionCode, string programCode, int academicyearID)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@Institution_Code", institutionCode),
        new SqlParameter("@Program_Code", programCode),
        new SqlParameter("@Academic_Year_ID", academicyearID)
            };

            DataSet ds = SQLHelper.ExecuteStoredProcedureWithDataset("SP_Get_Students_For_Approval", parameters);
            List<StudentEnrollmentViewModel> students = new List<StudentEnrollmentViewModel>();

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var isApproved = Convert.ToBoolean(row["Is_Approved_By_ACM"]);
                    students.Add(new StudentEnrollmentViewModel
                    {
                        Student_ID = Convert.ToInt32(row["Student_ID"]),
                        Student_Enrollment_ID = Convert.ToInt32(row["Student_Enrollment_ID"]),
                        Student_Name = row["Name"].ToString(),
                        Register_Number = row["Register_Number"].ToString(),
                        Institution_Code = Convert.ToInt32(row["Inst_Code"]),
                        Program_Code = row["Program_Code"].ToString(),
                        Is_Approved_By_ACM = isApproved,
                        ACM_Approval_Status_Text = isApproved ? "Approved" : "Not Approved",
                        Admission_Type = Convert.ToInt32(row["Admission_Type"]),
                        Admission_Type_Description = row["Admission_Type_Description"].ToString(),
                        CatCode = row["Cat_Code"].ToString(),
                        Gender = row["Gender"].ToString()
                    });
                }
            }

            return students;
        }
        public static DataTable GetStudentEnrollmentDrillDown(int level, int? institutionCode, string programCode, int? studyYear)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Level", level),
                new SqlParameter("@Institution_Code", institutionCode.HasValue ? institutionCode.Value : (object)DBNull.Value),
                new SqlParameter("@Program_Code", string.IsNullOrEmpty(programCode) ? (object)DBNull.Value : programCode),
                new SqlParameter("@Study_Year", studyYear.HasValue ? studyYear.Value : (object)DBNull.Value)
            };

            return SQLHelper.ExecuteStoredProcedureWithDataset("SP_Get_Student_Enrollment_DrillDown", parameters).Tables[0];
        }
    }

}

