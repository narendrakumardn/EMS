using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using BTEDiploma.Helper;
using Newtonsoft.Json;
using System.Web.UI.WebControls;

namespace BTEDiploma.admin
{
    public partial class StudentAttendance : System.Web.UI.Page
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
                //BindInstitutions();
                BindPrograms();
                LoadSemester();
            }
            else
            {
                // Recreate table from hidden fields on every postback
                if (!string.IsNullOrEmpty(hfStudents.Value) && !string.IsNullOrEmpty(hfCourses.Value))
                {
                    var students = JsonConvert.DeserializeObject<List<StudentModel>>(hfStudents.Value);
                    var courses = JsonConvert.DeserializeObject<List<CourseModel>>(hfCourses.Value);
                    var attendanceDict = JsonConvert.DeserializeObject<Dictionary<string, (int, int)>>(hfAttendance.Value ?? "{}");
                    bool isFinalized = !string.IsNullOrEmpty(hfIsFinalized.Value) && Convert.ToBoolean(hfIsFinalized.Value);
                    RenderAttendanceTable(students, courses, attendanceDict, isFinalized);
                    btnSave.Enabled = !isFinalized;
                    btnSubmit.Enabled = !isFinalized;
                    //btnDownloadReport.Enabled = isFinalized;
                    lblStatus.Text = "Attendance finalized. You can now view students in condonable range.";
                    lblStatus.Visible = isFinalized;

                    btnViewCondonable.Visible = isFinalized;
                }
            }
        }
        private void ShowError(string msg)
        {
            lblMessage.CssClass = "text-danger fw-semibold";
            lblMessage.Text = msg;
        }
        private void BindPrograms()
        {
            

            // Pass session variable into DAO
            var result = ProgramDAO.GetProgramList(instituteCode);

            if (result.StatusCode == 200)
            {
                ddlProgram.DataSource = result.Programs;
                ddlProgram.DataTextField = "Program_Name";
                ddlProgram.DataValueField = "Program_Code";
                ddlProgram.DataBind();
            }
            else
            {
                // Show error
                lblErrorMessage.Text = result.Message;
            }
        }
        // ---------------- Dropdown binding -------------------
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

        private bool CheckIfFinalized(int instituteCode, string programCode, int semester)
        {

            bool result = AttendanceDAO.IsAttendanceFinalized(instituteCode, programCode, semester, 1); // Process_ID = 1 (Attendance)
            return result;
        }

  
        /*private void BindCondStudents()
        {
            var result = AttendanceDAO.GetCondonableStudents();
            if (result.StatusCode == 200)
            {
                ddlInstitution.DataSource = result.Institutions;
                ddlInstitution.DataTextField = "Inst_Name";
                ddlInstitution.DataValueField = "Inst_Code";
                ddlInstitution.DataBind();
            }
        }*/

   
        protected void ddlFilter_Changed(object sender, EventArgs e)
        {
            //RenderAttendanceTable();
        }
        protected void btnViewCondonable_Click(object sender, EventArgs e)
        {
            try
            {
                // Assuming you have dropdowns or variables to get selected values
                int institutionCode = instituteCode; // replace with your control
                string programCode = ddlProgram.SelectedValue;                 // replace with your control
                int semester = int.Parse(ddlSemester.SelectedValue);           // replace with your control

                // Store in session
                Session["Institution_Code"] = institutionCode;
                Session["Program_Code"] = programCode;
                Session["Semester"] = semester;

                // Redirect to condonable students page
                Response.Redirect("~/College/CondonableStudents.aspx");
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = "Error: " + ex.Message;
            }
        }

        // ---------------- Utility -------------------
        private static string MakeSafeColumnName(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            string safe = Regex.Replace(s.Trim(), @"\s+", "");
            safe = Regex.Replace(safe, @"[^A-Za-z0-9_]", "_");
            return safe;
        }

        // ---------------- Load Button -------------------
        protected void btnLoad_Click(object sender, EventArgs e)
        {
            int instCode = instituteCode;
            string programCode = ddlProgram.SelectedValue;
            int semester = Convert.ToInt32(ddlSemester.SelectedValue);

            if (instCode == 0 || string.IsNullOrEmpty(programCode) || semester == 0)
            {
                lblError.Text = "Please select Institution, Program and Semester.";
                return;
            }
            bool isFinalized = CheckIfFinalized(instCode, programCode, semester);
            if (isFinalized)
            {
                btnSave.Enabled = false;
                btnSubmit.Enabled = false;
            }
            else
            {
                btnSave.Enabled = true;
                btnSubmit.Enabled = true;
            }
            var studentResult = AttendanceDAO.GetStudentsByInstProgramSemester(instCode, programCode, semester);
            var courseResult = AttendanceDAO.GetCoursesByProgramAndSemester(programCode, semester);
            DataSet attendanceDS = AttendanceDAO.GetAttendanceSummary(instCode, programCode, semester);

            var students = studentResult.Students;
            var courses = courseResult.Courses;

            if (students.Count == 0 || courses.Count == 0)
            {
                lblError.Text = "No students or courses found.";
                return;
            }

            // Build attendance dictionary
            Dictionary<string, (int, int)> attendanceDict = new Dictionary<string, (int, int)>();
            if (attendanceDS.Tables.Count > 0)
            {
                var dt = attendanceDS.Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    int semId = Convert.ToInt32(dr["Sem_History_ID"]);
                    string courseCode = dr["Course_Code"].ToString();
                    int attended = Convert.ToInt32(dr["Classes_Attended"]);
                    int conducted = Convert.ToInt32(dr["Classes_Conducted"]);
                    attendanceDict[$"{semId}_{courseCode}"] = (attended, conducted);
                }
            }

            // Save JSON to hidden fields
            hfStudents.Value = JsonConvert.SerializeObject(students);
            hfCourses.Value = JsonConvert.SerializeObject(courses);
            hfAttendance.Value = JsonConvert.SerializeObject(attendanceDict);
            hfIsFinalized.Value = isFinalized.ToString().ToLower();
            lblStatus.Text = "Attendance finalized. You can now view students in condonable range.";
            lblStatus.Visible = isFinalized;

            btnViewCondonable.Visible = isFinalized;
            RenderAttendanceTable(students, courses, attendanceDict, isFinalized);
        }

        // ---------------- Render Table -------------------
        private void RenderAttendanceTable(List<StudentModel> students, List<CourseModel> courses,
                                    Dictionary<string, (int, int)> attendanceDict, bool isFinalized)
        {
            phAttendance.Controls.Clear();

            HtmlTable table = new HtmlTable
            {
                Border = 1,
                CellPadding = 5,
                CellSpacing = 0,
                Width = "100%"
            };
            table.Attributes["style"] = "border-collapse: collapse; border:1px solid black;";

            // Header row
            HtmlTableRow header = new HtmlTableRow();
            header.Cells.Add(new HtmlTableCell { InnerText = "Student Name", Attributes = { ["style"] = "border:1px solid black;" } });
            header.Cells.Add(new HtmlTableCell { InnerText = "Register No.", Attributes = { ["style"] = "border:1px solid black;" } });

            foreach (var course in courses)
            {
                header.Cells.Add(new HtmlTableCell
                {
                    InnerText = course.Course_Code,
                    Attributes = { ["style"] = "border:1px solid black; text-align:center;" }
                });
            }
            table.Rows.Add(header);

            // Student rows
            foreach (var student in students)
            {
                HtmlTableRow row = new HtmlTableRow();
                row.Attributes["data-semid"] = student.Student_Semeter_ID.ToString();

                row.Cells.Add(new HtmlTableCell { InnerText = student.Name, Attributes = { ["style"] = "border:1px solid black;" } });
                row.Cells.Add(new HtmlTableCell { InnerText = student.Register_Number, Attributes = { ["style"] = "border:1px solid black;" } });

                foreach (var course in courses)
                {
                    // 👇 unique per student + course
                    string safeAttended = $"{student.Student_Semeter_ID}_{MakeSafeColumnName(course.Course_Code)}_Attended";
                    string safeConducted = $"{student.Student_Semeter_ID}_{MakeSafeColumnName(course.Course_Code)}_Conducted";

                    int attended = 0;
                    int conducted = AttendanceDAO.GetClassesConductedForCourse(course.Course_Code);

                    string key = $"{student.Student_Semeter_ID}_{course.Course_Code}";
                    if (attendanceDict.ContainsKey(key))
                    {
                        var val = attendanceDict[key];
                        attended = val.Item1;
                        conducted = val.Item2;
                    }

                    HtmlTableCell cell = new HtmlTableCell();
                    cell.Attributes["style"] = "border:1px solid black; text-align:center;";
                    if (isFinalized)
                    {
                        // Show readonly text with percentage
                        double pct = conducted > 0 ? (attended * 100.0 / conducted) : 0;
                        cell.InnerText = $"{attended}/{conducted} ({pct:F1}%)";
                    }
                    else
                    {
                        HtmlInputText txtAttended = new HtmlInputText
                        {
                            ID = safeAttended,
                            Value = attended.ToString()
                        };
                        txtAttended.Style.Add("width", "40px");

                        HtmlInputText txtConducted = new HtmlInputText
                        {
                            ID = safeConducted,
                            Value = conducted.ToString()
                        };
                        txtConducted.Style.Add("width", "40px");

                        cell.Controls.Add(txtAttended);
                        cell.Controls.Add(new LiteralControl(" / "));
                        cell.Controls.Add(txtConducted);
                    }
                    row.Cells.Add(cell);
                }

                table.Rows.Add(row);
            }

            phAttendance.Controls.Add(table);
        }


        // ---------------- Save Button -------------------
        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblResult.Text = "";
            lblError.Text = "";

            var students = JsonConvert.DeserializeObject<List<StudentModel>>(hfStudents.Value);
            var courses = JsonConvert.DeserializeObject<List<CourseModel>>(hfCourses.Value);

            int successCount = 0;
            int failCount = 0;
            List<string> errors = new List<string>();

            foreach (Control ctrl in phAttendance.Controls)
            {
                if (ctrl is HtmlTable table)
                {
                    foreach (HtmlTableRow row in table.Rows)
                    {
                        if (row == table.Rows[0]) continue; // skip header
                        int semId = Convert.ToInt32(row.Attributes["data-semid"]);

                        foreach (var course in courses)
                        {
                            string safeAttended = $"{semId}_{MakeSafeColumnName(course.Course_Code)}_Attended";
                            string safeConducted = $"{semId}_{MakeSafeColumnName(course.Course_Code)}_Conducted";

                            HtmlInputText txtAttended = row.FindControl(safeAttended) as HtmlInputText;
                            HtmlInputText txtConducted = row.FindControl(safeConducted) as HtmlInputText;


                            int attended = 0;
                            int conducted = 0;

                            if (txtAttended != null) int.TryParse(txtAttended.Value, out attended);
                            if (txtConducted != null) int.TryParse(txtConducted.Value, out conducted);

                            var result = AttendanceDAO.SaveAttendance(semId, course.Course_Code, attended, conducted);
                            if (result.StatusCode == 200)
                                successCount++;
                            else
                            {
                                failCount++;
                                errors.Add(result.StatusMessage ?? $"Save failed for {course.Course_Code} (SemHistory {semId})");
                            }
                        }
                    }
                }
            }

            lblResult.Text = $"{successCount} records saved successfully.";
            if (failCount > 0)
                lblResult.Text += $" {failCount} failed. Errors: {string.Join("; ", errors.Distinct())}";


            btnLoad_Click(sender, e);
        }

        // ---------------- Submit Button -------------------
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int instCode = instituteCode;
            string programCode = ddlProgram.SelectedValue;
            int semester = Convert.ToInt32(ddlSemester.SelectedValue);

            if (instCode == 0 || string.IsNullOrEmpty(programCode) || semester == 0)
            {
                lblError.Text = "Please select Institution, Program and Semester.";
                return;
            }

            int processId = 1;
            int employeeId = 2568884;  // to be replaced from session var

            var result = AttendanceDAO.FinalizeAttendance(instCode, programCode, semester, processId, employeeId);
            if (result.StatusCode == 200)
                lblResult.Text = result.StatusMessage;
            else
                lblError.Text = result.StatusMessage;
            bool isFinalized = CheckIfFinalized(instCode, programCode, semester);
            if (isFinalized)
            {
                btnSave.Enabled = false;
                btnSubmit.Enabled = false;
            }
            else
            {
                btnSave.Enabled = true;
                btnSubmit.Enabled = true;
            }
            btnLoad_Click(sender, e);
        }
    }
}
