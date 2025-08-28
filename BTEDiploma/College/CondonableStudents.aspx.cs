using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using BTEDiploma.Helper;

namespace BTEDiploma.admin
{
    public partial class CondonableStudents : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int instCode = Convert.ToInt32(Session["Institution_Code"] ?? "0");
                string programCode = Session["Program_Code"]?.ToString() ?? "";
                int semester = Convert.ToInt32(Session["Semester"] ?? "0");

                if (instCode > 0 && !string.IsNullOrEmpty(programCode) && semester > 0)
                {
                    // Check if attendance finalized
                    bool AttendanceFinalisedStatus = AttendanceDAO.IsAttendanceFinalized(instCode, programCode, semester, 1); // Process_ID = 1 (Attendance)
                                                                                                                              // var status = AttendanceDAO.GetAttendanceStatus(instCode, programCode, semester, 1); // Process ID = 1
                    if (AttendanceFinalisedStatus)
                    {
                        pnlCondonable.Visible = true;
                        PnlEditCondonable.Visible = true;
                        bool isCondonableFinalized = CheckIfCondFinalized(instCode, programCode, semester);
                        if (isCondonableFinalized)
                        {
                            lblError.Text = "Condonable entries are finalized Click below to view and download report";
                            PnlViewReport.Visible = true;
                            pnlCondonable.Visible = false;
                            btnSubmitCondList.Enabled = !isCondonableFinalized;
                        }
                        else
                        {

                            // Load students in condonable range
                            var students = AttendanceDAO.GetCondonableStudents(instCode, programCode, semester);
                            ddlCondonableStudents.DataSource = students;
                            ddlCondonableStudents.DataTextField = "Name";
                            ddlCondonableStudents.DataValueField = "Student_Semeter_ID";
                            ddlCondonableStudents.DataBind();
                            ddlCondonableStudents.Items.Insert(0, new ListItem("-- Select Student --", "0"));

                            // Load condonable reasons
                            var reasons = AttendanceDAO.GetCondonableReasons();
                            ddlCondonableReason.DataSource = reasons;
                            ddlCondonableReason.DataTextField = "reason_desc";
                            ddlCondonableReason.DataValueField = "reason_id";
                            ddlCondonableReason.DataBind();
                            ddlCondonableReason.Items.Insert(0, new ListItem("-- Select Reason --", "0"));
                            // Load grid
                            LoadCondonableList(instCode, programCode, semester);
                        }
                    }

                    else
                    {
                        pnlCondonable.Visible = false;

                        PnlEditCondonable.Visible = false;
                        lblError.Text = "Attendance not finalized yet. Condonable entries will be available after finalization.";
                    }
                }
            }
        }

        protected void ddlFilter_Cond_Student_Changed(object sender, EventArgs e)
        {
            int studentId = Convert.ToInt32(ddlCondonableStudents.SelectedValue);
            if (studentId > 0)
            {
                var courses = AttendanceDAO.GetCondonableStudents(null, null, null, studentId);
                ddlCondonableCourses.DataSource = courses;
                ddlCondonableCourses.DataTextField = "Course_Name";
                ddlCondonableCourses.DataValueField = "Course_Code";
                ddlCondonableCourses.DataBind();
                ddlCondonableCourses.Items.Insert(0, new ListItem("-- Select Course --", "0"));
            }
        }

        protected void btnSaveCondonable_Click(object sender, EventArgs e)
        {
            int semId = Convert.ToInt32(ddlCondonableStudents.SelectedValue);
            string courseCode = ddlCondonableCourses.SelectedValue;
            int reasonId = Convert.ToInt32(ddlCondonableReason.SelectedValue);
            DateTime dateOfCertificate = Convert.ToDateTime(txtCondonableDate.Text);
            string remark = txtRemarks.Text.Trim();

            var result = AttendanceDAO.SaveCondonableStudent(semId, courseCode, reasonId, dateOfCertificate, remark);

            if (result.StatusCode == 200)
            {
                lblResult.Text = result.StatusMessage;
                // Refresh grid
                LoadCondonableList(Convert.ToInt32(Session["Institution_Code"]),
                                   Session["Program_Code"].ToString(),
                                   Convert.ToInt32(Session["Semester"]));
            }
            else
            {
                lblError.Text = result.StatusMessage;
            }
        }

        private void LoadCondonableList(int instCode, string programCode, int semester)
        {
            var dt = AttendanceDAO.GetCondonableStudentList(instCode, programCode, semester);
            gvCondonableList.DataSource = dt;
            gvCondonableList.DataBind();
        }
        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                // Retrieve from session
                int institutionCode = Convert.ToInt32(Session["Institution_Code"]);
                string programCode = Session["Program_Code"].ToString();
                int semester = Convert.ToInt32(Session["Semester"]);

                // (if needed, set them back again or forward to another page)
                Session["Institution_Code"] = institutionCode;
                Session["Program_Code"] = programCode;
                Session["Semester"] = semester;
                // Redirect to condonable students page
                Response.Redirect("~/College/ViewStudentAttendanceReport.aspx");
            }
            catch (Exception ex)
            {
                //  lblErrorMessage.Text = "Error: " + ex.Message;
            }
        }
        protected void btnSubmit_ClickCondList(object sender, EventArgs e)
        {
            int institutionCode = Convert.ToInt32(Session["Institution_Code"]);
            string programCode = Session["Program_Code"].ToString();
            int semester = Convert.ToInt32(Session["Semester"]);


            if (institutionCode == 0 || string.IsNullOrEmpty(programCode) || semester == 0)
            {
                lblError.Text = "Please select Institution, Program and Semester.";
                return;
            }

            int processId = 4;
            int employeeId = 2568884;  // to be replaced from session var

            var result = AttendanceDAO.FinalizeCondonable(institutionCode, programCode, semester, processId, employeeId);
            if (result.StatusCode == 200)
                lblResult.Text = result.StatusMessage;
            else
                lblError.Text = result.StatusMessage;

            bool isFinalized = CheckIfCondFinalized(institutionCode, programCode, semester);
            if (isFinalized)
            {
                lblError.Text = "Condonable entries are finalized Click below to view and download report";
                PnlViewReport.Visible = true;
                PnlEditCondonable.Visible = false;
                btnSubmitCondList.Enabled = !isFinalized;
                try
                {
                    var markeendflag = AttendanceDAO.MarkEndAcademicYearForEligibleStudents(102, "CH", 1);

                    if (result.StatusCode == 0)
                    {
                        // ✅ Success
                        lblResult.CssClass = "text-success";
                        lblResult.Text = $"[{result.StatusCode}] {result.StatusMessage}";
                    }
                    else
                    {
                        // ⚠️ Stored procedure returned error
                        lblResult.CssClass = "text-warning";
                        lblResult.Text = $"Error [{result.StatusCode}]: {result.StatusMessage}";
                    }
                }
                catch (SqlException sqlEx)
                {
                    // ❌ SQL server-related error
                    lblResult.CssClass = "text-danger";
                    lblResult.Text = $"SQL Error: {sqlEx.Message}";
                }
                catch (Exception ex)
                {
                    // ❌ Any unexpected error
                    lblResult.CssClass = "text-danger";
                    lblResult.Text = $"Unexpected error: {ex.Message}";
                }
            }

            }
        private bool CheckIfCondFinalized(int instCode, string programCode, int semester)
        {
            bool result = AttendanceDAO.IsCondonaleListFinalized(instCode, programCode, semester, 4); // Process_ID = 1 (Attendance)
            return result;
        }
    }
}
