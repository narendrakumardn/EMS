using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using BTEDiploma.Helper;


namespace BTEDiploma.Helper
{
    public class Institution
    {
        public int Inst_Code { get; set; }
        public string Inst_Name { get; set; }
    }

    public class InstitutionResult
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public List<Institution> Institutions { get; set; }
    }
    public class InstitutionDAO
    {
        public static InstitutionResult GetInstitutionList()
        {
            DataSet ds = SQLHelper.ExecuteStoredProcedureWithDataset("SP_Get_Institution_List");

            var result = new InstitutionResult();
            result.Institutions = new List<Institution>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (Convert.ToInt32(row["StatusCode"]) == 200)
                    {
                        result.StatusCode = 200;
                        result.Message = row["Message"].ToString();
                        result.Institutions.Add(new Institution
                        {
                            Inst_Code = Convert.ToInt32(row["Inst_Code"]),
                            Inst_Name = row["Inst_Name"].ToString()
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

    }
}
