using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beans;

namespace Daos
{



    public class StaffDAO
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        public List<DesignationBean> GetAllDesignations()
        {
            List<DesignationBean> list = new List<DesignationBean>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT Designation_Id, Designation_Name FROM dbo.MstDesignations ORDER BY Designation_Name", con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                DesignationBean bean = new DesignationBean();
             
                while (dr.Read())
                {
                    list.Add(new DesignationBean
                    {

                        DesignationId = Convert.ToInt32(dr["Designation_Id"]),
                        DesignationName = dr["Designation_Name"].ToString()
                    });
                    list.Add(bean);
                }
            }
            return list;
        }


        public List<CourseBean> GetAllCourses()
        {
            List<CourseBean> courses = new List<CourseBean>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT CourseCode, CourseName FROM dbo.Courses ORDER BY CourseName", con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    CourseBean course = new CourseBean
                    {
                        code = dr["CourseCode"].ToString(),
                        name = dr["CourseName"].ToString()
                    };
                    courses.Add(course);
                }
            }

            return courses;
        }


        public List<staffBean> GetStaffByCollegeCode(string collegeCode)
        {
            List<staffBean> list = new List<staffBean>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT 
                    ValuerCode, 
                    Fk_Clg_Code, 
                    Fk_Crs_Code AS Board,
                    Name, 
                    CASE Gender WHEN 'M' THEN 'Male' WHEN 'F' THEN 'Female' ELSE 'Other' END AS Gender, 
                    CAST(WrkngExp AS DECIMAL(5,1)) AS WrkngExp,
                    Mobile, 
                    EMail
                FROM dte.Mst_StaffDetails
                WHERE Fk_Clg_Code = @CollegeCode";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@CollegeCode", collegeCode);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new staffBean
                    {
                        ValuerCode = reader["ValuerCode"].ToString(),
                        Fk_Clg_Code = reader["Fk_Clg_Code"].ToString(),
                        Board = reader["Board"].ToString(),
                        Name = reader["Name"].ToString(),
                        Gender = reader["Gender"].ToString(),
                        WrkngExp = reader["WrkngExp"] != DBNull.Value ? Convert.ToDecimal(reader["WrkngExp"]) : 0,
                        Mobile = reader["Mobile"].ToString(),
                        EMail = reader["EMail"].ToString()
                    });
                }
                con.Close();
            }
            
            return list;

        }

        public List<staffBean> GetAllStaff()
        {
            List<staffBean> list = new List<staffBean>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT ValuerCode, Fk_Clg_Code,Fk_Crs_Code,Fk_Designation,Name,Gender,WrkngExp,Mobile, EMail,Fk_User_Updtd,Last_Upd_Dt,IpT FROM dte.Mst_StaffDetails ORDER BY Name", con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    staffBean staff = new staffBean
                    {
                        ValuerCode = dr["ValuerCode"].ToString(),
                     
                       
                        Fk_Clg_Code = dr["Fk_Clg_Code"].ToString(),
                        Fk_Crs_Code = dr["Fk_Crs_Code"].ToString(),
                        Fk_Designation = Convert.ToInt32(dr["Fk_Designation"]),
                        Name = dr["Name"].ToString(),
                        Gender = dr["Gender"].ToString(),
                        WrkngExp = dr["WrkngExp"] != DBNull.Value ? Convert.ToDecimal(dr["WrkngExp"]) : 0,
                        Mobile = dr["Mobile"].ToString(),
                        EMail = dr["EMail"].ToString(),

                    };
                    list.Add(staff);
                }
            }

            return list;
        }


        public string InsertOrUpdateStaff(staffBean staff)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
IF EXISTS (SELECT 1 FROM [dte].[Mst_StaffDetails] WHERE ValuerCode = @ValuerCode)
BEGIN
    UPDATE [dte].[Mst_StaffDetails]
    SET Name = @Name,
        DOB = @DOB,
        DOJ = @DOJ,
   
        Gender = @Gender,
        Mobile = @Mobile,
        EMail = @EMail,
        Fk_Clg_Code = @Fk_Clg_Code,
        Fk_Crs_Code = @Fk_Crs_Code,
        Fk_Designation = @Fk_Designation,
        Fk_User_Updtd = @Fk_User_Updtd,
        Last_Upd_Dt = @Last_Upd_Dt,
        IpT = @IpT
    WHERE ValuerCode = @ValuerCode
END
ELSE
BEGIN
    INSERT INTO [dte].[Mst_StaffDetails]
    (ValuerCode, Name, DOB, DOJ, Gender, Mobile, EMail, Fk_Clg_Code, Fk_Crs_Code, Fk_Designation, Fk_User_Updtd, Last_Upd_Dt, IpT)
    VALUES
    (@ValuerCode, @Name, @DOB, @DOJ, @Gender, @Mobile, @EMail, @Fk_Clg_Code, @Fk_Crs_Code, @Fk_Designation, @Fk_User_Updtd, @Last_Upd_Dt, @IpT)
END";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ValuerCode", staff.ValuerCode);
                    cmd.Parameters.AddWithValue("@Name", staff.Name);
                    cmd.Parameters.AddWithValue("@DOB", staff.DOB);
                    cmd.Parameters.AddWithValue("@DOJ", staff.DOJ);
                    //cmd.Parameters.AddWithValue("@WrkngExp", staff.WrkngExp); auto calculated
                    cmd.Parameters.AddWithValue("@Gender", staff.Gender);
                    cmd.Parameters.AddWithValue("@Mobile", staff.Mobile);
                    cmd.Parameters.AddWithValue("@EMail", staff.EMail);
                    cmd.Parameters.AddWithValue("@Fk_Clg_Code", staff.Fk_Clg_Code);
                    cmd.Parameters.AddWithValue("@Fk_Crs_Code", staff.Fk_Crs_Code);
                    cmd.Parameters.AddWithValue("@Fk_Designation", staff.Fk_Designation);
                    cmd.Parameters.AddWithValue("@Fk_User_Updtd", staff.Fk_User_Updtd ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Last_Upd_Dt", staff.Last_Upd_Dt);
                    cmd.Parameters.AddWithValue("@IpT", staff.IpT);

                    cmd.ExecuteNonQuery();
                }

                return "Staff details saved successfully.";
            }
        }
        public bool UpdateStaff(staffBean staff)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"UPDATE [dte].[Mst_StaffDetails]
                         SET Name = @Name,
                             DOB = @DOB,
                             DOJ = @DOJ,
                             Gender = @Gender,
                             Mobile = @Mobile,
                             EMail = @EMail,
                             Fk_Crs_Code = @Fk_Crs_Code,
                             Fk_Designation = @Fk_Designation,
                             Fk_User_Updtd = @Fk_User_Updtd,
                             Last_Upd_Dt = @Last_Upd_Dt,
                             IpT = @IpT
                         WHERE ValuerCode = @ValuerCode";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ValuerCode", staff.ValuerCode);
                cmd.Parameters.AddWithValue("@Name", staff.Name);
                cmd.Parameters.AddWithValue("@DOB", staff.DOB);
                cmd.Parameters.AddWithValue("@DOJ", staff.DOJ);
                cmd.Parameters.AddWithValue("@Gender", staff.Gender);
                cmd.Parameters.AddWithValue("@Mobile", staff.Mobile);
                cmd.Parameters.AddWithValue("@EMail", staff.EMail);
                cmd.Parameters.AddWithValue("@Fk_Crs_Code", staff.Fk_Crs_Code);
                cmd.Parameters.AddWithValue("@Fk_Designation", staff.Fk_Designation);
                cmd.Parameters.AddWithValue("@Fk_User_Updtd", staff.Fk_User_Updtd);
                cmd.Parameters.AddWithValue("@Last_Upd_Dt", staff.Last_Upd_Dt);
                cmd.Parameters.AddWithValue("@IpT", staff.IpT);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool DeleteStaff(string valuerCode)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM [dte].[Mst_StaffDetails] WHERE ValuerCode = @ValuerCode";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ValuerCode", valuerCode);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }


    }
}