using BTEDiploma.Helper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace BTEDiploma.Helper
{
   
        public  class DCBDao
        {
            /// <summary>
            /// Calls the stored procedure to refresh college-wise DCB data.
            /// </summary>
            public static void RefreshCollegeDCB()
            {
                SQLHelper.ExecuteNonQuery("spRefreshCollegeDCB", CommandType.StoredProcedure);
            }

        /// <summary>
        /// Gets the DCB summary for a specific college from the CollegeDCB_Summary table.
        /// </summary>
        public static DataTable GetCollegeDCB_Summary(string collegeCode)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@CollegeCode", collegeCode)
            };

            return SQLHelper.ExecuteStoredProcedure("spGetCollegeDCB_Summary", parameters);
        }

        /// <summary>
        /// Gets the branch-wise DCB summary for a specific college.
        /// </summary>
        public static DataTable GetCollegeDCB_Branchwise(string collegeCode)
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter("@CollegeCode", collegeCode)
                };

                return SQLHelper.ExecuteStoredProcedure("spGetCollegeDCB_Branchwise", parameters);
            }
        }
    }
