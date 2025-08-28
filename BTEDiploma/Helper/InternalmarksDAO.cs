using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using BTEDiploma.Helper;

namespace BTEDiploma.sqlhelper
{
    public class IntrnalMarksDAO
    {
        static string connStr = ConfigurationManager.ConnectionStrings["DBC"].ConnectionString;
        public static (int StatusCode, DataTable AdmissionTypes) GetAdmissionTypes()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand("usp_GetAdmissionTypes", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                return (200, dt);
            }
            catch (Exception ex)
            {
                // log error if needed
                return (500, null);
            }
        }
        public static DataTable GetIAData(int institution, string program, int semester)
        {
            using (SqlConnection con = new SqlConnection(
                System.Configuration.ConfigurationManager.ConnectionStrings["DBC"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.SP_Get_AllCourses_Internal_Assessments_Long", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Institution_Code", institution);
                cmd.Parameters.AddWithValue("@Program_Code", program);
                cmd.Parameters.AddWithValue("@Semester", semester);

                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
                return dt;
            }
        }
        public static bool UpsertFinalizeIA(int institutionCode, string programCode, int semester)
        {
            using (SqlConnection con = new SqlConnection(
                System.Configuration.ConfigurationManager.ConnectionStrings["DBC"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.SP_Upsert_Process_State_FinalizeIA", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Institution_Code", institutionCode);
                cmd.Parameters.AddWithValue("@Program_Code", programCode);
                cmd.Parameters.AddWithValue("@Semester", semester);

                con.Open();
                object result = cmd.ExecuteScalar();
                return (result != null && Convert.ToInt32(result) == 1);
            }
        }
        public static bool IsIAFinalized(int institutionCode, string programCode, int semester)
        {
            using (SqlConnection con = new SqlConnection(
                System.Configuration.ConfigurationManager.ConnectionStrings["DBC"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.SP_Get_Process_State_ForCombo_Finalized", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Institution_Code", institutionCode);
                cmd.Parameters.AddWithValue("@Program_Code", programCode);
                cmd.Parameters.AddWithValue("@Semester", semester);

                con.Open();
                object o = cmd.ExecuteScalar();
                // If no row, treat as not finalized (editable)
                return (o != null && o != DBNull.Value && Convert.ToInt32(o) == 1);
            }
        }
        public static  double  GetCourseTotalMaxMarks(string courseCode)
        {
            double maxMarks = 0;

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand("sp_GetCourseTotalMaxMarks", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CourseCode", courseCode);

                    con.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null && double.TryParse(result.ToString(), out double val))
                    {
                        maxMarks = val;
                    }
                }

                return (maxMarks);
            }
            catch (Exception ex)
            {
                // Log error if needed
                return ( 0);
            }
        }


        public static (int StatusCode, DataTable Courses) GetCoursesByProgramcurrentyear(string programCode, int semesterNo, int admissionType)
        {
            DataTable dt = new DataTable();
            int statusCode = 500;

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand("SP_Get_Program_Course_By_CurrentState", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // all 3 mandatory params
                    cmd.Parameters.AddWithValue("@ProgramCode", programCode);
                    cmd.Parameters.AddWithValue("@Semester", semesterNo);
                    cmd.Parameters.AddWithValue("@AdmissionType", admissionType);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        statusCode = Convert.ToInt32(dt.Rows[0]["StatusCode"]);
                    }
                }

                return (statusCode, dt);
            }
            catch (Exception ex)
            {
                // optional: log exception
                return (500, null);
            }
        }
        public static string CalculateFinalIAMarks(int studentSemId, string courseCode)
        {
            string resultMsg = string.Empty;

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand("SP_Calculate_Final_IA", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Student_Semester_ID", studentSemId);
                    cmd.Parameters.AddWithValue("@Course_Code", courseCode);

                    SqlParameter msgParam = new SqlParameter("@Msg", SqlDbType.NVarChar, 200)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(msgParam);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    resultMsg = msgParam.Value?.ToString() ?? "No message returned from SP.";
                }
            }
            catch (Exception ex)
            {
                resultMsg = $"Error: {ex.Message}";
            }

            return resultMsg;
        }

        public static (int StatusCode, DataTable Data) GetStudentAssessmentMarks(
        int institutionCode, string programCode, int semester, int admissionType, string courseCode)
            {
                DataTable dt = new DataTable();
                try
                {
                    using (SqlConnection con = new SqlConnection(connStr))
                    using (SqlCommand cmd = new SqlCommand("SP_Get_Student_Internal_Assessments", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Institution_Code", institutionCode);
                        cmd.Parameters.AddWithValue("@Program_Code", programCode);
                        cmd.Parameters.AddWithValue("@Semester", semester);
                        cmd.Parameters.AddWithValue("@AdmissionType_ID", admissionType);
                        cmd.Parameters.AddWithValue("@Course_Code", courseCode);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                    return (200, dt);
                }
                catch
                {
                    return (500, null);
                }
            }

        public static string InsertStudentIAMarks(int studentId, string courseCode, string assessmentName, double marks)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SP_Insert_IA_Marks", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Student_Semester_ID", studentId);
                cmd.Parameters.AddWithValue("@Course_Code", courseCode);
                cmd.Parameters.AddWithValue("@Assessment_Name", assessmentName);
                cmd.Parameters.AddWithValue("@Marks_Scored", marks);

                SqlParameter msgParam = new SqlParameter("@Msg", SqlDbType.NVarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(msgParam);

                con.Open();
                cmd.ExecuteNonQuery();

                return msgParam.Value?.ToString() ?? "No message returned";
            }
        }


      

    }
}