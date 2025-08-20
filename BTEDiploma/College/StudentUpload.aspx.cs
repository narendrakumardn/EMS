using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BTEDiploma.Helper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;

namespace BTEDiploma.admin
{
    public partial class StudentUpload : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }
        protected void btnDownloadTemplate_Click(object sender, EventArgs e)
        {
            ExcelHelper.GenerateStudentTemplateExcel(Response);
        }
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fuExcel.HasFile)
            {
                var students = ExcelToStudentList(fuExcel.FileContent);
                Session["UploadedStudents"] = students;
                gvPreview.DataSource = students;
                gvPreview.DataBind();
            }
        }

        private List<StudentUploadModel> ExcelToStudentList(Stream stream)
        {
            List<StudentUploadModel> students = new List<StudentUploadModel>();
            using (var workbook = new XLWorkbook(stream))
            {
                var sheet = workbook.Worksheet("StudentTemplate");
                var rows = sheet.RangeUsed().RowsUsed().Skip(1); // Skip header row
                foreach (var row in rows)
                {
                    var student = new StudentUploadModel
                    {
                        Institution_Code = int.Parse(row.Cell(1).GetString()),
                        Program_Code = row.Cell(2).GetString(),
                        AdharNumber = row.Cell(3).GetString(),
                        Application_Number = row.Cell(4).GetString(),
                        SATS_Number = row.Cell(5).GetString(),
                        Name = row.Cell(6).GetString(),
                        Father_Name = row.Cell(7).GetString(),
                        Mother_Name = row.Cell(8).GetString(),
                        Address = row.Cell(9).GetString(),
                        PINcode = row.Cell(10).GetString(),
                        TalukID = int.Parse(row.Cell(11).GetString()),
                        Cat_Code = row.Cell(12).GetString(),
                        Gender = row.Cell(13).GetString(),
                        Admission_Type = int.Parse(row.Cell(14).GetString()),
                        Date_Of_Birth = DateTime.Parse(row.Cell(15).GetString()),
                        Mobile_Number = row.Cell(16).GetString(),
                        Email_ID = row.Cell(17).GetString(),
                        SNQ = row.Cell(18).GetString() == "1"
                    };
                    students.Add(student);
                }
            }
            return students;
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var students = Session["UploadedStudents"] as List<StudentUploadModel>;
            if (students != null)
            {
                string academicYear = "2025-26";
                string yearPart = academicYear.Split('-')[0].Substring(2);

                var sortedStudents = students.OrderBy(s => s.Name).ToList();
                int i = 0;

                foreach (var student in sortedStudents)
                {
                    string roll = (++i).ToString("D3");
                    string registerNumber = $"{student.Institution_Code}{student.Program_Code}{yearPart}{roll}";
                    try
                    {
                        int studentId = StudentUploadDao.InsertToTbStudent(student);

                        int statusCode = StudentUploadDao.InsertToTbStudentEnrollment(studentId, student, registerNumber, academicYear);
                        // Show success message
                    }
                    catch (Exception ex)
                    {
                        lblErrorMessage.Text = ex.Message; // will display "Student already enrolled" if duplicate
                    }

                }

                // 🔄 Bind new GridView with DB-inserted data
                DataTable latestData = StudentUploadDao.GetStudentEnrollmentView();
                gvInserted.DataSource = latestData;
                gvInserted.DataBind();

                // Optional: hide old preview grid
                gvPreview.Visible = false;

                Response.Write("<script>alert('Student data saved and displayed successfully.');</script>");
            }
            else
            {
                Response.Write("<script>alert('No students to submit.');</script>");
            }
        }


        /* private int InsertToTbStudent(StudentUploadModel student)
         {
             using (var con = new SqlConnection(...))
             {
                 SqlCommand cmd = new SqlCommand("SP_Insert_Student", con);
                 cmd.CommandType = CommandType.StoredProcedure;
                 cmd.Parameters.AddWithValue("@Adhar", student.AdharNumber);
                 // other parameters...

                 SqlParameter outId = new SqlParameter("@Student_ID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                 cmd.Parameters.Add(outId);
                 con.Open();
                 cmd.ExecuteNonQuery();
                 return (int)outId.Value;
             }
         }*/

        /*private void InsertToTbStudentEnrollment(int studentId, StudentUploadModel s, string regNo, string year)
        {
            using (var con = new SqlConnection(...))
            {
                SqlCommand cmd = new SqlCommand("SP_Insert_StudentEnrollment", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Student_ID", studentId);
                cmd.Parameters.AddWithValue("@Register_Number", regNo);
                cmd.Parameters.AddWithValue("@Program_Code", s.Program_Code);
                cmd.Parameters.AddWithValue("@Institution_Code", s.Institution_Code);
                cmd.Parameters.AddWithValue("@Admission_Year", year);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }*/







    }
}
