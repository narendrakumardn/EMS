using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daos
{
    public class SubjectsDAO
    {
        public SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["DBCS"].ToString());
        }
        public DataSet GetSubjectCodeDetails(string SubjectCode)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT [FK_Curriculum] as CURR,[SubjectName] AS SUB_NAME,[SubjectType] AS SUB_TYPE,[IsWorkshopLab] AS IS_WORK_LAP,[CIE_Week_3_Max] AS WEEK_3_MAX,[CIE_Week_4_Max] AS WEEK_4_MAX,[CIE_Week_5_Max] AS WEEK_5_MAX,[CIE_Week_6_Max] AS WEEK_6_MAX,[CIE_Week_7_Max] AS WEEK_7_MAX,[CIE_Week_8_Max] AS WEEK_8_MAX,[CIE_Week_9_Max] AS WEEK_9_MAX,[CIE_Week_11_Max] AS WEEK_11_MAX,[CIE_Week_12_Max] AS WEEK_12_MAX,[CIE_Week_13_Max] AS WEEK_13_MAX,[CIE_Week_13_Std_Activity_Max] AS WEEK_13_ACT_MAX,[EndExamMax] AS END_EXM_MAX,[EndExamMin] AS END_EXAM_MIN,[IAMax] AS IA_MAX,[IAMin] AS IA_MIN,[TotalMaxMarks] AS TOT_MAX,[PassMinMarks] AS PASS_MIN,[EndExamTime] AS END_TIME_EXM,[EquivStatus] AS EQ_STATUS,[Elective] AS IS_ELECT,[Credit] AS CRD_HOURS FROM [dbo].[MstSubjects] WHERE SubjectCode = @SubjectCode", con);
                    da.SelectCommand.Parameters.AddWithValue("@SubjectCode", SubjectCode);
                    da.Fill(ds);
                }
                finally
                {
                    con.Close();
                }

            }
            return ds;
        }
    }
}
