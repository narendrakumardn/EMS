using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daos;

namespace Daos
{

    public static class DCBDAO
    {
        public static DataTable GetCollegeDCB(string clgCode)
        {
            string sql = @"
            SELECT 
                CollegeCode,
                SUM([Demand-Amount]) AS Demand,
                SUM([Regular-Amount] + [Arrear-Amount]) AS Collection,
                SUM([Demand-Amount] - ([Regular-Amount] + [Arrear-Amount])) AS Balance
            FROM DCB
            WHERE (@ClgCode = '0' OR CollegeCode = @ClgCode)
            GROUP BY CollegeCode";

            SqlParameter[] parameters = {
            new SqlParameter("@ClgCode", clgCode)
             };

            return new DatabaseHelper().ExecuteSelectQuery(sql, parameters);
        }
    }
}