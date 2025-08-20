using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using ClosedXML.Excel;



namespace BTEDiploma.Helper
{
    public class ExcelHelper
    {
        public static void GenerateStudentTemplateExcel(HttpResponse response)
        {
            using (var workbook = new XLWorkbook())
            {
                // Fetch dropdowns from DB
                var ds = SQLHelper.ExecuteStoredProcedureWithDataset("SP_Get_StudentUploadDropdowns", null);

                var mainSheet = workbook.Worksheets.Add("StudentTemplate");
                var listSheet = workbook.Worksheets.Add("Lists");
                listSheet.Visibility = XLWorksheetVisibility.VeryHidden;

                // === Lists from DB ===
                DataTable categoryList = ds.Tables[0];
                DataTable admissionTypeList = ds.Tables[1];
                DataTable talukList = ds.Tables[2];
                DataTable programList = ds.Tables[3];
                DataTable institutionList = ds.Tables[4];

                int totalRows = 1000;

                // === 1. Lists sheet ===
                listSheet.Cell("A1").InsertTable(categoryList);
                listSheet.Cell("F1").InsertTable(admissionTypeList);
                listSheet.Cell("K1").InsertTable(talukList);
                listSheet.Cell("P1").InsertTable(programList);
                listSheet.Cell("U1").InsertTable(institutionList);
                listSheet.Cell("Z1").InsertData(new[] { "M", "F" });
                listSheet.Cell("AA1").InsertData(new[] { "0", "1" });

                // === 2. Headers for template ===
                var headers = new string[] {
                "Institution_Code", "Program_Code", "AdharNumber", "Application_Number", "SATS_Number",
                "Name", "Father_Name", "Mother_Name", "Address", "PINcode", "TalukID", "Cat_Code",
                "Gender", "Admission_Type", "Date_Of_Birth", "Mobile_Number", "Email_ID", "SNQ"
            };

                for (int i = 0; i < headers.Length; i++)
                    mainSheet.Cell(1, i + 1).Value = headers[i];

                // === 3. Data Validations (Dropdowns) ===
                for (int r = 2; r <= totalRows; r++)
                {
                    // Institution_Code → column A
                    mainSheet.Range($"A{r}").CreateDataValidation()
                             .List(listSheet.Range("U2:U" + (institutionList.Rows.Count + 1)));

                    // Program_Code → column B
                    mainSheet.Range($"B{r}").CreateDataValidation()
                             .List(listSheet.Range("P2:P" + (programList.Rows.Count + 1)));

                    // TalukID → column K
                    mainSheet.Range($"K{r}").CreateDataValidation()
                             .List(listSheet.Range("K2:K" + (talukList.Rows.Count + 1)));

                    // Cat_Code → column L
                    mainSheet.Range($"L{r}").CreateDataValidation()
                             .List(listSheet.Range("A2:A" + (categoryList.Rows.Count + 1)));

                    // Gender (M/F) → column M
                    mainSheet.Range($"M{r}").CreateDataValidation()
                             .List(listSheet.Range("Z1:Z2"));

                    // Admission_Type → column N
                    mainSheet.Range($"N{r}").CreateDataValidation()
                             .List(listSheet.Range("F2:F" + (admissionTypeList.Rows.Count + 1)));

                    // SNQ (0 or 1) → column R
                    mainSheet.Range($"R{r}").CreateDataValidation()
                             .List(listSheet.Range("AA1:AA2"));                             // SNQ (0/1)
                }

                // === 4. Sample Row ===
                mainSheet.Cell("A2").Value = "102";
                mainSheet.Cell("B2").Value = "CS";
                mainSheet.Cell("C2").Value = "123456789012";
                mainSheet.Cell("D2").Value = "APP123";
                mainSheet.Cell("E2").Value = "SATS123";
                mainSheet.Cell("F2").Value = "John Student";
                mainSheet.Cell("G2").Value = "Father";
                mainSheet.Cell("H2").Value = "Mother";
                mainSheet.Cell("I2").Value = "Sample Address";
                mainSheet.Cell("J2").Value = "560001";
                mainSheet.Cell("K2").Value = "5442";
                mainSheet.Cell("L2").Value = "3A";
                mainSheet.Cell("M2").Value = "M";
                mainSheet.Cell("N2").Value = "1";
                mainSheet.Cell("O2").Value = DateTime.Today.AddYears(-18).ToString("yyyy-MM-dd");
                mainSheet.Cell("P2").Value = "9876543210";
                mainSheet.Cell("Q2").Value = "john@example.com";
                mainSheet.Cell("R2").Value = "0";

                // === 5. Download File ===
                response.Clear();
                response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                response.AddHeader("content-disposition", "attachment;filename=StudentUploadTemplate.xlsx");

                using (var ms = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(ms);
                    ms.WriteTo(response.OutputStream);
                }

                response.Flush();
                response.End();
            }
        }
    }
}



