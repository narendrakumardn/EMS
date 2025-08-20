using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using BTEDiploma.Helper;


namespace BTEDiploma.Helper
{
    public class ProgramModel
    {
        public string Program_Code { get; set; }
        public string Program_Name { get; set; }
        public int Program_Type { get; set; }
        public string Program_Type_Name { get; set; }
        public int Credit { get; set; }
        public string Diploma_Title { get; set; }
    }

    public class ProgramResult
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public List<ProgramModel> Programs { get; set; }
    }
    public class ProgramDAO
    {
        public static ProgramResult GetProgramList(int instituteCode)
        {
            // Pass parameter to stored procedure
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@Inst_Code", instituteCode)
            };

            DataSet ds = SQLHelper.ExecuteStoredProcedureWithDataset("SP_Get_Program_List_ByInstitution", parameters);

            var result = new ProgramResult();
            result.Programs = new List<ProgramModel>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (Convert.ToInt32(row["StatusCode"]) == 200)
                    {
                        result.StatusCode = 200;
                        result.Message = row["Message"].ToString();
                        result.Programs.Add(new ProgramModel
                        {
                            Program_Code = row["Program_Code"].ToString(),
                            Program_Name = row["Program_Name"].ToString(),
                            Program_Type = Convert.ToInt32(row["Program_Type"]),
                            Program_Type_Name = row["Program_Type_Name"].ToString(),
                            Credit = Convert.ToInt32(row["Credit"]),
                            Diploma_Title = row["Diploma_Title"].ToString()
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

        public static ProgramResult GetProgramList() 
        { 
            DataSet ds = SQLHelper.ExecuteStoredProcedureWithDataset("SP_Get_Program_List"); 
            var result = new ProgramResult(); 
            result.Programs = new List<ProgramModel>();
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0) 
            { 
                foreach (DataRow row in ds.Tables[0].Rows) 
                { 
                    if (Convert.ToInt32(row["StatusCode"]) == 200)
                    {
                        result.StatusCode = 200; 
                        result.Message = row["Message"].ToString();
                        result.Programs.Add(new ProgramModel
                        {
                            Program_Code = row["Program_Code"].ToString(), 
                            Program_Name = row["Program_Name"].ToString(),
                            Program_Type = Convert.ToInt32(row["Program_Type"]),
                            Program_Type_Name = row["Program_Type_Name"].ToString(),
                            Credit = Convert.ToInt32(row["Credit"]), Diploma_Title = row["Diploma_Title"].ToString() });
                    } else { result.StatusCode = Convert.ToInt32(row["StatusCode"]); 
                        result.Message = row["Message"].ToString();
                    } } } 
            else {
                result.StatusCode = 400; result.Message = "No data returned."; } return result; 
        }
    }
}
