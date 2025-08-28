using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using BTEDiploma.Helper;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
//using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Reporting.WebForms;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BTEDiploma.admin
{
    public partial class ViewStudentAttendanceReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAttendanceGrid();
            }
        }
        private void GenerateAttendancePDF(DataTable pivotedData)
        {
            // Set QuestPDF license
            QuestPDF.Settings.License = LicenseType.Community;

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Text("Student Attendance Report")
                        .SemiBold()
                        .FontSize(16)
                        .AlignCenter();

                    page.Content().Table(table =>
                    {
                        int colCount = pivotedData.Columns.Count;

                        // Define columns once
                        table.ColumnsDefinition(columns =>
                        {
                            for (int i = 0; i < colCount; i++)
                                columns.RelativeColumn();
                        });

                        // Table header
                        table.Header(header =>
                        {
                            foreach (DataColumn col in pivotedData.Columns)
                                header.Cell().Background(Colors.Grey.Lighten2)
                                .Border(1) // 1 point border
                                      .Text(col.ColumnName)
                                      .SemiBold()
                                      .FontSize(10)
                                      .AlignCenter();
                        });

                        // Table rows
                        foreach (DataRow row in pivotedData.Rows)
                        {
                            foreach (DataColumn col in pivotedData.Columns)
                            {
                                string text = row[col] != DBNull.Value ? row[col].ToString() : "";

                                table.Cell()
                                     .Element(cellContainer =>
                                     {
                                         cellContainer.Border(1)       // Row cell border
                                                      .Padding(2)      // optional padding
                                                      .AlignCenter()
                                                      .Text(text)
                                                      .FontSize(9);
                                     });
                            }
                        }
                    });
                });
            }).GeneratePdf();

            // Return PDF to browser
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment;filename=AttendanceReport.pdf");
            Response.BinaryWrite(pdfBytes);
            Response.End();
        }
        protected void LoadAttendanceGrid()
        {
            int institutionCode = Convert.ToInt32(Session["Institution_Code"]);
            string programCode = Session["Program_Code"].ToString();
            int semester = Convert.ToInt32(Session["Semester"]);

            DataTable dt = AttendanceDAO.GetAttendanceFromSP(institutionCode, programCode, semester);

            DataTable pivoted = PivotAttendance(dt);

            GridView1.DataSource = pivoted;
            GridView1.DataBind();

            // store for later PDF export
            Session["PivotedAttendance"] = pivoted;
        }
        protected void btnExport_Click(object sender, EventArgs e)
        {
            // Generate PDF
            DataTable pivotedData = Session["PivotedAttendance"] as DataTable;

            if (pivotedData == null || pivotedData.Rows.Count == 0)
            {
                // Optionally show message
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('No data to export!');", true);
                return;
            }

            // Call the PDF generation method
            GenerateAttendancePDF(pivotedData);
        }

        private DataTable PivotAttendance(DataTable dt)
        {
            DataTable pivot = new DataTable();
            pivot.Columns.Add("Register_Number", typeof(string));
            pivot.Columns.Add("Name", typeof(string));

            // Order courses by Sno
            var courses = dt.AsEnumerable()
                            .OrderBy(r => Convert.ToInt32(r["Sno"]))
                            .Select(r => r["Course_Code"].ToString())
                            .Distinct()
                            .ToList();

            // Add one column per course
            var courseColumnMap = new Dictionary<string, string>();
            foreach (string c in courses)
            {
                string columnName = $"{c}\nAT | Total | % | Status";
                pivot.Columns.Add(columnName, typeof(string));
                courseColumnMap[c] = columnName; // store mapping
            }
            pivot.Columns.Add("Eligibility", typeof(string));

            // Group by student
            var students = dt.AsEnumerable()
                             .GroupBy(r => new
                             {
                                 Register_Number = r["Register_Number"],
                                 Name = r["Name"],
                                 Overall_Status = r["Overall_Status"]
                             });

            foreach (var s in students)
            {
                DataRow newRow = pivot.NewRow();
                newRow["Register_Number"] = s.Key.Register_Number;
                newRow["Name"] = s.Key.Name;
                newRow["Eligibility"] = s.Key.Overall_Status;

                foreach (DataRow r in s)
                {
                    string courseCode = r["Course_Code"].ToString();
                    string pivotColumn = courseColumnMap[courseCode];
                    // Map Course_Status to single letter
                    string status = r["Course_Status"].ToString();
                    string shortStatus;
                    switch (status)
                    {
                        case "Eligible":
                            shortStatus = "E";
                            break;
                        case "Not Eligible":
                            shortStatus = "NE";
                            break;
                        case "Condonable":
                            shortStatus = "C";
                            break;
                        default:
                            shortStatus = status;
                            break;
                    }


                    // Combine values in one string
                    string cellValue = $"{r["Classes_Attended"]} | {r["Classes_Conducted"]} | {r["Attendance_Percentage"]}% | {shortStatus}";
                    newRow[pivotColumn] = cellValue;
                }

                pivot.Rows.Add(newRow);
            }

            return pivot;
        }

    }
}
