using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using BTEDiploma.Helper;



using Microsoft.Reporting.WebForms;


namespace BTEDiploma.admin
{
    public partial class ViewAndApproveExamEligibleStudent : System.Web.UI.Page
    {
        private int instituteCode;
        protected void Page_Load(object sender, EventArgs e)
        {
            instituteCode = Session["CollegeCode"] != null ? Convert.ToInt32(Session["CollegeCode"]) : 0;
            if (instituteCode == 0)
            {
                ShowError("Session expired or invalid institute code.");
                return;
            }
            if (!IsPostBack)
            {
                LoadExamMonthYear();
               // BindInstitutions();
                BindPrograms();
                LoadSemester();

                //LoadAttendanceGrid();
            }
            pnlEligibleList.Visible = true;
        }
        private void LoadExamMonthYear()
        {
            try
            {
                DataTable dt = AttendanceDAO.GetExamMonthYear();

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    lblExamMonth.Text = row["ExamMonth"].ToString();
                    lblExamYear.Text = row["ExamYear"].ToString();
                    Session["CurrentExamID"] = row["Exam_ID"];
                    lblMessage.Text = "✔ Exam session loaded successfully.";
                    lblMessage.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblExamMonth.Text = "";
                    lblExamYear.Text = "";
                    lblMessage.Text = "⚠ No exam session found.";
                    lblMessage.ForeColor = System.Drawing.Color.OrangeRed;
                }
            }
            catch (Exception ex)
            {
                lblExamMonth.Text = "";
                lblExamYear.Text = "";
                lblMessage.Text = "❌ Error: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }
        private void LoadSemester()
        {
           
            var semesterResult = InstitutionDAO.GetSemesterListByInst(instituteCode);
            if (semesterResult.StatusCode == 200)
            {
                ddlSemester.DataSource = semesterResult.Semesters;
                ddlSemester.DataTextField = "SemesterNo";
                ddlSemester.DataValueField = "SemesterNo";
                ddlSemester.DataBind();
                ddlSemester.Items.Insert(0, new ListItem("-- Select Semester --", "0"));
            }
        }
        private void ShowError(string msg)
        {
            lblMessage.CssClass = "text-danger fw-semibold";
            lblMessage.Text = msg;
        }

      
        private void BindPrograms()
        {
            var result = ProgramDAO.GetProgramList(instituteCode);
            if (result.StatusCode == 200)
            {
                ddlProgram.DataSource = result.Programs;
                ddlProgram.DataTextField = "Program_Name";
                ddlProgram.DataValueField = "Program_Code";
                ddlProgram.DataBind();
                
            }
        }

        protected void ddlFilter_Changed(object sender, EventArgs e)
        {
            //RenderAttendanceTable();
        }
        protected void btnApproveEligibleList_Click(object sender, EventArgs e)
        {
            try
            {
                // ✅ Check Exam ID
                if (Session["CurrentExamID"] == null || !int.TryParse(Session["CurrentExamID"].ToString(), out int examId))
                {
                    lblMessage.Text = "⚠ Exam session is not set. Please load Exam Month & Year first.";
                    lblMessage.ForeColor = System.Drawing.Color.OrangeRed;
                    return;
                }

                // ✅ Use dt directly from Session
                DataTable dt = Session["EligibleList"] as DataTable;
                if (dt == null || dt.Rows.Count == 0)
                {
                    lblMessage.Text = "⚠ No eligible students found to approve.";
                    lblMessage.ForeColor = System.Drawing.Color.OrangeRed;
                    return;
                }

                int successCount = 0, failCount = 0;

                foreach (DataRow row in dt.Rows)
                {
                    int studentSemesterId = Convert.ToInt32(row["Student_Semester_ID"]);
                    string courseCode = row["Course_Code"].ToString();

                    // Call DAO → returns ProcedureResult now
                    var result = AttendanceDAO.InsertExamEligibility(examId, studentSemesterId, courseCode);

                    if (result.StatusCode == 200)
                        successCount++;
                    else
                        failCount++;
                }

                lblMessage.Text = $"✔ Approved successfully. Inserted: {successCount}, Failed: {failCount}";
                lblMessage.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblMessage.Text = "❌ Error approving eligible list: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }
        protected void btnLoad_Click(object sender, EventArgs e)
        {
            int institutionCode = instituteCode;
            string programCode = ddlProgram.SelectedValue;
            int semester = Convert.ToInt32(ddlSemester.SelectedValue);
            if (institutionCode == 0 || string.IsNullOrEmpty(programCode) || semester == 0)
            {
                lblMessage.Text = "Please select Institution, Program and Semester.";
                return;
            }

            if (institutionCode > 0 && !string.IsNullOrEmpty(programCode) && semester > 0)
            {
                // Check if attendance finalized
                bool CheckAttendancePreProcess = AttendanceDAO.IsCondonaleListFinalized(institutionCode, programCode, semester, 1); // Process_ID = 1 (Attendance)
                                                                                                                                    // var status = AttendanceDAO.GetAttendanceStatus(instCode, programCode, semester, 1); // Process ID = 1
                bool CheckIAPreProcess = AttendanceDAO.IsIAFinalized(institutionCode, programCode, semester, 2);

                if (!CheckAttendancePreProcess || !CheckIAPreProcess)
                {
                    pnlEligibleList.Visible = false;
                    lblError.Text = "Please Finalize IA and Attendance";
                    return;
                }
            }

            DataTable dt = AttendanceDAO.GetEligibleListFromSP(institutionCode, programCode, semester);
            Session["EligibleList"] = dt;
            DataTable pivoted = PivotEligibleList(dt);

            grExamEligibleStudents.DataSource = pivoted;
            grExamEligibleStudents.DataBind();

            // store for later PDF export
            Session["PivotedAttendance"] = pivoted;
        }

        private DataTable PivotEligibleList(DataTable dt)
        {
            DataTable pivot = new DataTable();
            pivot.Columns.Add("Register_Number", typeof(string));
            pivot.Columns.Add("Name", typeof(string));

            // Collect distinct courses (order by Sno for consistency)
            var courses = dt.AsEnumerable()
                            .OrderBy(r => Convert.ToInt32(r["Sno"]))
                            .Select(r => r["Course_Code"].ToString())
                            .Distinct()
                            .ToList();

            // Add one column per course
            foreach (string c in courses)
            {
                pivot.Columns.Add(c, typeof(string));
            }

            // Group by student
            var students = dt.AsEnumerable()
                             .GroupBy(r => new
                             {
                                 Register_Number = r["Register_Number"].ToString(),
                                 Name = r["Name"].ToString()
                             });

            foreach (var s in students)
            {
                DataRow newRow = pivot.NewRow();
                newRow["Register_Number"] = s.Key.Register_Number;
                newRow["Name"] = s.Key.Name;

                // Initialize all course columns as "NE" (not eligible)
                foreach (string c in courses)
                {
                    newRow[c] = "NE";
                }

                // For each student–course row from DB
                foreach (DataRow r in s)
                {
                    string courseCode = r["Course_Code"].ToString();

                    // Check eligibility if column exists
                    if (pivot.Columns.Contains(courseCode))
                    {



                        newRow[courseCode] = "Eligible";
                    }
                }

                pivot.Rows.Add(newRow);
            }

            return pivot;
        }



    }
}
