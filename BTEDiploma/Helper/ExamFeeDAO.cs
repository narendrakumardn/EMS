using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace BTEDiploma.sqlhelper
{
    public class ExamFeeDetailsDAO
    {
        private static readonly string connStr = ConfigurationManager.ConnectionStrings["DBC"].ConnectionString;

        // 1) Exam hierarchy dropdowns (Academic Years, Exam Years, Exam Months/Exam_ID)
        public DataSet GetExamHierarchyDropdown()
        {
            using (var con = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("usp_GetExamHierarchy", con))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var ds = new DataSet();
                da.Fill(ds);
                return ds; // ds.Tables[0] AY, ds.Tables[1] EY, ds.Tables[2] Months with Exam_ID
            }
        }

        // Utility filters from the hierarchy dataset (optional convenience)
        public DataTable FilterExamYears(DataSet ds, string academicYear)
        {
            if (ds == null || ds.Tables.Count < 2) return new DataTable();
            var rows = ds.Tables[1].Select("Academic_Year = " + Quote(academicYear));
            return rows.Length > 0 ? rows.CopyToDataTable() : new DataTable();
        }
        public DataTable FilterExamMonths(DataSet ds, string academicYear, string examYear)
        {
            if (ds == null || ds.Tables.Count < 3) return new DataTable();
            var rows = ds.Tables[2].Select($"Academic_Year = {Quote(academicYear)} AND Exam_Year = {Quote(examYear)}");
            return rows.Length > 0 ? rows.CopyToDataTable() : new DataTable();
        }
        private static string Quote(string s) => "'" + s.Replace("'", "''") + "'";

        // 2) Programmes list (per your instruction: use SP_Get_Program_List)
        public DataTable GetProgrammeList(int instituteCode)
        {
            using (var con = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("SP_Get_Program_List_ByInstitution", con))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Pass instituteCode as parameter
                cmd.Parameters.AddWithValue("@Inst_Code", instituteCode);

                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }


        // 3) Payment Modes
        public DataTable GetPaymentModes()
        {
            using (var con = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("SELECT Payment_Mode_ID, Description FROM Tb_Payment_Mode ORDER BY Description", con))
            using (var da = new SqlDataAdapter(cmd))
            {
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // 4) Students for the selected Programme + Institute + Semester
        // Uses base tables you listed.
        public DataTable GetStudentsForFee(string programmeCode, string instituteCode, int sem, int examId)
        {
            const string sql = @"
SELECT 
    s.Student_ID,
    s.Name AS Student_Name,
    e.Register_Number,
    se.Student_Semeter_ID AS Sem_History_ID,
    p.Payment_ID,
    p.Exam_ID,
    p.Payment_Date,
    p.Fee_Amount,
    p.Payment_Mode_ID,
    p.Transaction_Ref_No
FROM dbo.Tb_Student s
JOIN dbo.Tb_Student_Enrollment e 
    ON e.Student_ID = s.Student_ID
JOIN dbo.Tb_Student_Semester se 
    ON se.Enrollment_ID = e.Enrollment_ID
LEFT JOIN dbo.Tb_Exam_Fee_Payment p
    ON p.Student_Semester_ID = se.Student_Semeter_ID
    AND p.Exam_ID = @Exam_ID
WHERE e.Program_Code = @Program_Code
  AND e.Institution_Code = @Institution_Code
  AND se.Sem = @Sem
ORDER BY s.Name;";


            using (var con = new SqlConnection(connStr))
            using (var cmd = new SqlCommand(sql, con))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@Program_Code", programmeCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Institution_Code", instituteCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Sem", sem);
                cmd.Parameters.AddWithValue("@Exam_ID", examId);
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // 5) Insert Exam Fee (sequence-based Payment_ID)
        // Uses a stored proc so you can enforce validations and reuse it.
        public (int PaymentId, string Message) InsertExamFee(
    int examId,
    int semHistoryId,
    DateTime paymentDate,
    decimal feeAmount,
    int paymentModeId,
    string transactionRefNo,
    int? existingPaymentId = null)
        {
            using (var con = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("sp_Insert_Exam_Fee_Payment", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Input parameters
                cmd.Parameters.AddWithValue("@Exam_ID", examId);
                cmd.Parameters.AddWithValue("@Sem_History_ID", semHistoryId);
                cmd.Parameters.AddWithValue("@Payment_Date", paymentDate);
                cmd.Parameters.AddWithValue("@Fee_Amount", feeAmount);
                cmd.Parameters.AddWithValue("@Payment_Mode_ID", paymentModeId);
                cmd.Parameters.AddWithValue("@Transaction_Ref_No",
                    string.IsNullOrEmpty(transactionRefNo) ? DBNull.Value : (object)transactionRefNo);

                // Add payment ID only if it exists (for updates)
                if (existingPaymentId.HasValue)
                    cmd.Parameters.AddWithValue("@Payment_ID", existingPaymentId.Value);
                else
                    cmd.Parameters.AddWithValue("@Payment_ID", DBNull.Value);

                // Output parameters
                var outMsgParam = new SqlParameter("@Out_Message", SqlDbType.NVarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outMsgParam);

                var outPaymentIdParam = new SqlParameter("@Out_Payment_ID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outPaymentIdParam);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();

                    string message = outMsgParam.Value?.ToString() ?? "Unknown result";
                    int paymentId = outPaymentIdParam.Value != DBNull.Value
                        ? Convert.ToInt32(outPaymentIdParam.Value)
                        : 0;

                    return (paymentId, message);
                }
                catch (Exception ex)
                {
                    return (0, "Database error: " + ex.Message);
                }
            }
        }


        // 6) Update Exam Fee (if you allow editing)
        public int UpdateExamFee(int paymentId, DateTime paymentDate, decimal feeAmount, int paymentModeId, string transactionRefNo)
        {
            using (var con = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("sp_Update_Exam_Fee_Payment", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Payment_ID", paymentId);
                cmd.Parameters.AddWithValue("@Payment_Date", paymentDate);
                cmd.Parameters.AddWithValue("@Fee_Amount", feeAmount);
                cmd.Parameters.AddWithValue("@Payment_Mode_ID", paymentModeId);
                cmd.Parameters.AddWithValue("@Transaction_Ref_No", (object)transactionRefNo ?? DBNull.Value);

                var rows = new SqlParameter("@RowsAffected", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(rows);

                con.Open();
                cmd.ExecuteNonQuery();
                return (rows.Value == DBNull.Value) ? 0 : Convert.ToInt32(rows.Value);
            }
        }
        // 7) Optional: get existing fee payments for a set of Sem_History_IDs (useful for pre-fill)
        public DataTable GetPaymentsByExamAndSemHist(int examId, string semHistIdCsv)
        {
            // semHistIdCsv should be a CSV of ints like "101,102,103"
            // Split in SQL using STRING_SPLIT (SQL 2016+) or use a TVP if preferred.
            const string sql = @"
SELECT p.Payment_ID, p.Exam_ID, p.Sem_History_ID, p.Payment_Date, p.Fee_Amount, p.Payment_Mode_ID, p.Transaction_Ref_No
FROM dbo.Tb_Exam_Fee_Payment p
JOIN STRING_SPLIT(@SemHistCsv, ',') s ON TRY_CAST(s.value AS INT) = p.Student_Semester_ID
WHERE p.Exam_ID = @Exam_ID;";

            using (var con = new SqlConnection(connStr))
            using (var cmd = new SqlCommand(sql, con))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@Exam_ID", examId);
                cmd.Parameters.AddWithValue("@SemHistCsv", semHistIdCsv ?? "");

                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
    }
}
