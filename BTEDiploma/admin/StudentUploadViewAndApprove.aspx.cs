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
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;

namespace BTEDiploma.admin
{
    public partial class StudentUploadViewAndApprove : System.Web.UI.Page
    {

        int currentLevel
        {
            get { return (int)(ViewState["CurrentLevel"] ?? 1); }
            set { ViewState["CurrentLevel"] = value; }
        }
        int? selectedInstitution
        {
            get { return (int?)(ViewState["InstitutionCode"]); }
            set { ViewState["InstitutionCode"] = value; }
        }
        string selectedProgram
        {
            get { return (string)(ViewState["ProgramCode"]); }
            set { ViewState["ProgramCode"] = value; }
        }
        int? selectedYear
        {
            get { return (int?)(ViewState["StudyYear"]); }
            set { ViewState["StudyYear"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadLevel1();
            }

            else
            {
                // Recreate GridView on every postback so dynamic controls exist
                if (currentLevel == 4) LoadLevel4();
                else if (currentLevel == 3) LoadLevel3();
                else if (currentLevel == 2) LoadLevel2();
                else LoadLevel1();
            }
        }

        private void LoadLevel1()
        {
            gvData.Columns.Clear();
            DataTable dt = StudentUploadDao.GetStudentEnrollmentDrillDown(1, null, null, null);
            gvData.DataSource = dt;

            gvData.DataKeyNames = new string[] { "KeyValue" }; // 🔹 Important
            TemplateField slNoCol = new TemplateField();
            slNoCol.HeaderText = "Sl. No";
            gvData.Columns.Add(slNoCol);
            gvData.Columns.Add(new BoundField { DataField = "Inst_Name", HeaderText = "Institution Name" });
            gvData.Columns.Add(new BoundField { DataField = "Admission_Type", HeaderText = "Admission Type" });
            gvData.Columns.Add(new BoundField { DataField = "Approved_Count", HeaderText = "Approved Students" });
            gvData.Columns.Add(new BoundField { DataField = "Not_Approved_Count", HeaderText = "Not Approved Students" });
            gvData.Columns.Add(new BoundField { DataField = "Total_Count", HeaderText = "Total Students" });
            gvData.Columns.Add(new ButtonField { Text = "View Programs", CommandName = "SelectRow" });

            gvData.DataBind();
            currentLevel = 1;
            btnApprove.Visible = false;
        }


        private void LoadLevel2()
        {
            gvData.Columns.Clear();
            DataTable dt = StudentUploadDao.GetStudentEnrollmentDrillDown(2, selectedInstitution, null, null);
            gvData.DataSource = dt;

            gvData.DataKeyNames = new string[] { "KeyValue" }; // 🔹 Important
            TemplateField slNoCol = new TemplateField();
            slNoCol.HeaderText = "Sl. No";
            gvData.Columns.Add(slNoCol);
            gvData.Columns.Add(new BoundField { DataField = "Program_Name", HeaderText = "Program" });
            gvData.Columns.Add(new BoundField { DataField = "Admission_Type", HeaderText = "Admission Type" });
            gvData.Columns.Add(new BoundField { DataField = "Approved_Count", HeaderText = "Approved Students" });
            gvData.Columns.Add(new BoundField { DataField = "Not_Approved_Count", HeaderText = "Not Approved Students" });
            gvData.Columns.Add(new BoundField { DataField = "Total_Count", HeaderText = "Total Students" });
            gvData.Columns.Add(new ButtonField { Text = "View Study Years", CommandName = "SelectRow" });

            gvData.DataBind();
            currentLevel = 2;
            btnApprove.Visible = false;
        }

        protected void GvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Add serial number
                int serialNo = e.Row.RowIndex + 1;
                e.Row.Cells[0].Text = serialNo.ToString();

                // ✅ Optional highlight for approved
                CheckBox chk = e.Row.FindControl("chkApproved") as CheckBox;
                if (chk != null && chk.Checked)
                {
                    e.Row.CssClass = "table-success"; // green row for approved
                }
            }
        }

        private void LoadLevel3()
        {
            gvData.Columns.Clear();
            DataTable dt = StudentUploadDao.GetStudentEnrollmentDrillDown(3, selectedInstitution, selectedProgram, null);

            foreach (DataRow row in dt.Rows)
                // row["Study_Year_Text"] = "Year " + row["Study_Year_No"].ToString();

                gvData.DataSource = dt;

            gvData.DataKeyNames = new string[] { "KeyValue" }; // 🔹 Important
            TemplateField slNoCol = new TemplateField();
            slNoCol.HeaderText = "Sl. No";
            gvData.Columns.Add(slNoCol);
            gvData.Columns.Add(new BoundField { DataField = "KeyValue", HeaderText = "Study Year" });
            gvData.Columns.Add(new BoundField { DataField = "Admission_Type", HeaderText = "Admission Type" });
            gvData.Columns.Add(new BoundField { DataField = "Approved_Count", HeaderText = "Approved Students" });
            gvData.Columns.Add(new BoundField { DataField = "Not_Approved_Count", HeaderText = "Not Approved Students" });
            gvData.Columns.Add(new BoundField { DataField = "Total_Count", HeaderText = "Total Students" });
            gvData.Columns.Add(new ButtonField { Text = "View Students", CommandName = "SelectRow" });

            gvData.DataBind();
            currentLevel = 3;
            btnApprove.Visible = false;
        }


        private void LoadLevel4()
        {
            gvData.Columns.Clear();
            DataTable dt = StudentUploadDao.GetStudentEnrollmentDrillDown(4, selectedInstitution, selectedProgram, selectedYear);
            gvData.DataSource = dt;

            gvData.DataKeyNames = new string[] { "KeyValue" };
            gvData.DataKeyNames = new string[] { "KeyValue", "Student_Enrollment_ID" };
            TemplateField slNoCol = new TemplateField();
            slNoCol.HeaderText = "Sl. No";
            gvData.Columns.Add(slNoCol);
            gvData.Columns.Add(new BoundField { DataField = "KeyValue", HeaderText = "Student ID" });
            gvData.Columns.Add(new BoundField { DataField = "Student_Enrollment_ID", HeaderText = "Student Enrollment No" });
            gvData.Columns.Add(new BoundField { DataField = "Register_Number", HeaderText = "Register No" });
            gvData.Columns.Add(new BoundField { DataField = "Name", HeaderText = "Name" });
            gvData.Columns.Add(new BoundField { DataField = "Admission_Type", HeaderText = "Admission Type" });

            TemplateField chkCol = new TemplateField { HeaderText = "Approved By ACM" };
            chkCol.ItemTemplate = new CheckBoxTemplate("Is_Approved_By_ACM");
            gvData.Columns.Add(chkCol);

            gvData.DataBind();
            currentLevel = 4;
            btnApprove.Visible = true;
        }

        protected void gvData_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectRow")
            {
                int index = Convert.ToInt32(e.CommandArgument);

                if (currentLevel == 1)
                {
                    selectedInstitution = Convert.ToInt32(gvData.DataKeys[index].Value);
                    LoadLevel2();
                }
                else if (currentLevel == 2)
                {
                    selectedProgram = gvData.DataKeys[index].Value.ToString();
                    LoadLevel3();
                }
                else if (currentLevel == 3)
                {
                    selectedYear = Convert.ToInt32(gvData.DataKeys[index].Value);
                    LoadLevel4();
                }
            }
        }


        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // First column is our Sl. No.
                e.Row.Cells[0].Text = (e.Row.RowIndex + 1).ToString();

                // ✅ Highlight approved rows
                CheckBox chk = e.Row.FindControl("chkApproved") as CheckBox;
                if (chk != null && chk.Checked)
                {
                    e.Row.CssClass = "table-success";
                }
            }
        }


        protected void btnApprove_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gvData.Rows)
            {
                int studentId = Convert.ToInt32(gvData.DataKeys[row.RowIndex].Values["KeyValue"]);

                int StudentEnrollmentID = Convert.ToInt32(gvData.DataKeys[row.RowIndex].Values["Student_Enrollment_ID"]);
                CheckBox chk = row.FindControl("chkApproved") as CheckBox;
                bool isApproved = chk != null && chk.Checked;

                StudentUploadDao.UpdateACMApproval(studentId, StudentEnrollmentID, isApproved);
                //  dao.UpdateApprovalStatus(studentId, isApproved);
            }
            lblMessage.Text = "Approval status updated.";
            LoadLevel4();
        }



    }
}
