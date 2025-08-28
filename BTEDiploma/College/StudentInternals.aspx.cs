using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using BTEDiploma.Helper;
using BTEDiploma.sqlhelper;
using Microsoft.Reporting.WebForms;
namespace BTEDiploma.admin
{
    public partial class StudentInternals : System.Web.UI.Page
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
                BindPrograms();
                LoadSemester();
                BindAdmissionTypes();

            }
            if (ViewState["ReportLoaded"] != null && (bool)ViewState["ReportLoaded"])
            {
                string programCode = ddlProgramme.SelectedValue;
                int semester = Convert.ToInt32(ddlSemester.SelectedValue);
                LoadReport(programCode, semester);
                bool agreed = ViewState["IA_Agreed"] is bool a && a;
                bool isLocked = ViewState["IA_Locked"] is bool b && b;
                pnlAfterReportConfirm.Visible = !agreed && !isLocked;
                btnFinalSubmit.Visible = agreed && !isLocked;
            }
            else if (ViewState["MarksData"] is DataTable dt)
            {
                BuildEditableTable(dt); // Recreate dynamic controls on every postback
            }
        }

        // Dropdown bindings
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

        private void BindPrograms()
        {
            var result = ProgramDAO.GetProgramList(instituteCode);
            if (result.StatusCode == 200)
            {
                ddlProgramme.DataSource = result.Programs;
                ddlProgramme.DataTextField = "Program_Name";
                ddlProgramme.DataValueField = "Program_Code";
                ddlProgramme.DataBind();
                ddlProgramme.Items.Insert(0, new ListItem("-- Select Programme --", "0"));
            }
        }

        private void BindAdmissionTypes()
        {
            var result = IntrnalMarksDAO.GetAdmissionTypes();
            if (result.StatusCode == 200)
            {
                ddlAdmissionType.DataSource = result.AdmissionTypes;
                ddlAdmissionType.DataTextField = "Admission_Type_Description";
                ddlAdmissionType.DataValueField = "Admission_Type_ID";
                ddlAdmissionType.DataBind();
                ddlAdmissionType.Items.Insert(0, new ListItem("-- Select Admission Type --", "0"));
            }
        }

        private void TryBindCourses()
        {
            string programCode = ddlProgramme.SelectedValue;
            int semesterNo = int.TryParse(ddlSemester.SelectedValue, out int sem) ? sem : 0;
            int admissionTypeId = int.TryParse(ddlAdmissionType.SelectedValue, out int adm) ? adm : 0;

            if (programCode != "0" && semesterNo != 0 && admissionTypeId != 0)
            {
                var result = IntrnalMarksDAO.GetCoursesByProgramcurrentyear(programCode, semesterNo, admissionTypeId);
                rptCourses.DataSource = result.StatusCode == 200 ? result.Courses : null;
                rptCourses.DataBind();
            }
        }

        protected void ddlProgramme_SelectedIndexChanged(object sender, EventArgs e) => TryBindCourses();
        protected void ddlSemester_SelectedIndexChanged(object sender, EventArgs e) => TryBindCourses();
        protected void ddlAdmissionType_SelectedIndexChanged(object sender, EventArgs e) => TryBindCourses();

        // Course selection -> load table dynamically
        protected void rptCourses_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectCourse")
            {
                string courseCode = e.CommandArgument.ToString();
                string programCode = ddlProgramme.SelectedValue;
                int semester = Convert.ToInt32(ddlSemester.SelectedValue);
                int admissionType = Convert.ToInt32(ddlAdmissionType.SelectedValue);
                ViewState["SelectedCourseName"] = ((Button)e.CommandSource).Text;
                bool isFinalized = IntrnalMarksDAO.IsIAFinalized(instituteCode, programCode, semester);
                ViewState["IA_Locked"] = isFinalized;

               // ViewState["IA_Locked"] = isLocked;

                var result = IntrnalMarksDAO.GetStudentAssessmentMarks(instituteCode, programCode, semester, admissionType, courseCode);

                if (result.StatusCode == 200 && result.Data != null && result.Data.Rows.Count > 0)
                {
                    ViewState["MarksData"] = result.Data;
                    ViewState["SelectedCourse"] = courseCode;
                    TryBindCourses();
                    BuildEditableTable(result.Data);
                    pnlMarksEntry.Visible = true;
                    btnSaveDraft.Enabled = !isFinalized;
                    btnPreviewIA.Enabled = !isFinalized;
                    btnFinalSubmit.Visible = !isFinalized ;
                    if (isFinalized)
                    {
                        lblMessage.CssClass = "text-warning fw-semibold text-center d-block";
                        lblMessage.Text = "Editing is locked for this programme/semester as alreday finaalized IA marks";
                    }
                    else
                    {
                        lblMessage.Text = string.Empty;
                    }
                }
                else
                {
                    pnlMarksEntry.Visible = false;
                    ShowError("No students found for this course.");
                }

            }
        }

        // Build dynamic HTML table with TextBoxes
        private void BuildEditableTable(DataTable dt)
        {
            bool isLocked = ViewState["IA_Locked"] is bool b && b;
            tblMarks.Controls.Clear();

            // Table Header
            TableHeaderRow header = new TableHeaderRow();
            header.Cells.Add(new TableHeaderCell { Text = "Reg No" });
            header.Cells.Add(new TableHeaderCell { Text = "Student Name" });

            foreach (DataColumn col in dt.Columns)
            {
                if (col.ColumnName.Equals("Student_Semester_ID", StringComparison.OrdinalIgnoreCase) ||
                    col.ColumnName.Equals("Course_Code", StringComparison.OrdinalIgnoreCase) ||
                    col.ColumnName.Equals("Register_Number", StringComparison.OrdinalIgnoreCase) ||
                    col.ColumnName.Equals("Name", StringComparison.OrdinalIgnoreCase))
                    continue;

                header.Cells.Add(new TableHeaderCell { Text = col.ColumnName });
            }

            tblMarks.Rows.Add(header);

            // Get Max Marks for the course (from SP)
            string courseCode = ViewState["SelectedCourse"]?.ToString();
            double maxTotalMarks = 50;//IntrnalMarksDAO.GetCourseTotalMaxMarks(courseCode);

            // Table Rows
            foreach (DataRow dr in dt.Rows)
            {
                TableRow row = new TableRow();

                row.Cells.Add(new TableCell { Text = dr["Register_Number"].ToString() });
                row.Cells.Add(new TableCell { Text = dr["Name"].ToString() });

                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName.Equals("Student_Semester_ID", StringComparison.OrdinalIgnoreCase) ||
                        col.ColumnName.Equals("Course_Code", StringComparison.OrdinalIgnoreCase) ||
                        col.ColumnName.Equals("Register_Number", StringComparison.OrdinalIgnoreCase) ||
                        col.ColumnName.Equals("Name", StringComparison.OrdinalIgnoreCase))
                        continue;

                    TableCell cell = new TableCell();

                    // If this is "Total Marks", show as LABEL, apply RED if < 40% of Max Marks
                    if (col.ColumnName.Equals("Total Marks", StringComparison.OrdinalIgnoreCase))
                    {
                        double totalMarks = 0;
                        double.TryParse(dr[col] == DBNull.Value ? "0" : dr[col].ToString(), out totalMarks);

                        cell.Text = totalMarks.ToString();
                        cell.Font.Bold = true;

                        // Highlight RED if < 40% of Max Marks
                        if (maxTotalMarks > 0 && totalMarks < (0.4 * maxTotalMarks))
                            cell.ForeColor = System.Drawing.Color.Red;
                        else
                            cell.ForeColor = System.Drawing.Color.Blue;
                    }
                    else
                    {
                        TextBox tb = new TextBox
                        {
                            ID = $"txt_{dr["Student_Semester_ID"]}_{col.ColumnName}",
                            CssClass = "form-control form-control-sm",
                            Width = Unit.Pixel(70),
                            Text = dr[col] == DBNull.Value ? "" : dr[col].ToString()
                        };
                        if (isLocked)
                        {
                            tb.ReadOnly = true;         // prevents edits but keeps visual
                            tb.Enabled = false;         // fully disables interaction/tab-stop
                            tb.CssClass += " bg-light"; // subtle disabled look
                            tb.ToolTip = "Editing locked (Process ID = 1)";
                        }
                        cell.Controls.Add(tb);
                    }

                    row.Cells.Add(cell);
                }

                tblMarks.Rows.Add(row);
            }
            if (btnSaveDraft != null) btnSaveDraft.Enabled = !isLocked;
            if (btnPreviewIA != null) btnPreviewIA.Enabled = !isLocked;
        }



        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            bool isLocked = ViewState["IA_Locked"] is bool b && b;
            if (isLocked)
            {
                litModalBody.Text = "Editing is locked (finalized). You cannot save changes.";
                pnlServerModal.CssClass = "modal fade show d-block";
                pnlServerModal.Visible = true;
                return;
            }
            var dt = ViewState["MarksData"] as DataTable;
            if (dt == null) return;

            string courseCode = ViewState["SelectedCourse"]?.ToString();
            if (string.IsNullOrEmpty(courseCode)) return;

            bool hasErrors = false;
            string errorMessages = "";

            foreach (DataRow dr in dt.Rows)
            {
                int studentSemId = Convert.ToInt32(dr["Student_Semester_ID"]);
                string regNo = dr["Register_Number"].ToString();
                string studentName = dr["Name"].ToString();

                // 1️⃣ Insert / Update IA marks
                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName.Equals("Student_Semester_ID", StringComparison.OrdinalIgnoreCase) ||
                        col.ColumnName.Equals("Course_Code", StringComparison.OrdinalIgnoreCase) ||
                        col.ColumnName.Equals("Register_Number", StringComparison.OrdinalIgnoreCase) ||
                        col.ColumnName.Equals("Name", StringComparison.OrdinalIgnoreCase) ||
                        col.ColumnName.Equals("Total Marks", StringComparison.OrdinalIgnoreCase)) // Skip Total Marks
                        continue;

                    string tbId = $"txt_{studentSemId}_{col.ColumnName}";
                    TextBox tb = (TextBox)tblMarks.FindControl(tbId);

                    double marks = 0;
                    bool valid = true;

                    if (tb != null)
                    {
                        string input = tb.Text.Trim();

                        // Server-side validation: must be positive number
                        if (!double.TryParse(input, out marks) || marks < 0)
                        {
                            valid = false;
                            hasErrors = true;
                            tb.BorderColor = System.Drawing.Color.Red;
                            errorMessages += $"<div style='color:red'><strong>{regNo} - {studentName}</strong>, {col.ColumnName}: Invalid value (only positive numbers allowed)</div>";
                        }
                        else
                        {
                            tb.BorderColor = System.Drawing.Color.Empty; // reset border if valid
                        }

                        if (valid)
                        {
                            string msg = IntrnalMarksDAO.InsertStudentIAMarks(studentSemId, courseCode, col.ColumnName, marks);
                            if (msg.ToLower().Contains("exceed") || msg.ToLower().Contains("error"))
                            {
                                hasErrors = true;
                                tb.BorderColor = System.Drawing.Color.Red;
                                errorMessages += $"<div style='color:red'><strong>{regNo} - {studentName}</strong>, {col.ColumnName}: {msg}</div>";
                            }
                        }
                    }
                }

                // 2️⃣ Calculate Final IA Marks
                string finalMsg = IntrnalMarksDAO.CalculateFinalIAMarks(studentSemId, courseCode);
                if (finalMsg.ToLower().Contains("error"))
                {
                    hasErrors = true;
                    errorMessages += $"<div style='color:red'><strong>{regNo} - {studentName}</strong>, Final IA: {finalMsg}</div>";
                }
            }

            // 3️⃣ Reload updated data
            string programCode = ddlProgramme.SelectedValue;
            int semester = Convert.ToInt32(ddlSemester.SelectedValue);
            int admissionType = Convert.ToInt32(ddlAdmissionType.SelectedValue);
            string selectedCourse = ViewState["SelectedCourse"]?.ToString();

            if (!string.IsNullOrEmpty(selectedCourse))
            {
                var result = IntrnalMarksDAO.GetStudentAssessmentMarks(instituteCode, programCode, semester, admissionType, selectedCourse);

                if (result.StatusCode == 200 && result.Data != null && result.Data.Rows.Count > 0)
                {
                    ViewState["MarksData"] = result.Data;
                    BuildEditableTable(result.Data);
                    pnlMarksEntry.Visible = true;
                }
                else
                {
                    pnlMarksEntry.Visible = false;
                    ShowError("No students found for this course after saving.");
                }
            }

            // 4️⃣ Show modal (errors only OR single success)
            //string script;
            // 4️⃣ Show modal (errors only OR single success)
            if (hasErrors)
            {
                litModalBody.Text = errorMessages;
                pnlServerModal.CssClass = "modal fade show d-block"; // show modal
            }
            else
            {
                litModalBody.Text = "<div style='color:green; font-weight:bold;'>All records were inserted successfully.</div>";
                pnlServerModal.CssClass = "modal fade show d-block";
            }
            pnlServerModal.Visible = true;

        }

        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            pnlServerModal.Visible = false;
            pnlServerModal.CssClass = "modal fade"; // reset to hidden
        }

        // Step 1: Preview button clicked - show modal
        protected void btnPreviewIA_Click(object sender, EventArgs e)
        {
            pnlPreviewConfirm.CssClass = "modal fade show d-block";
            pnlPreviewConfirm.Visible = true;
        }
        protected void btnClosePreviewModal_Click(object sender, EventArgs e)
        {
            pnlPreviewConfirm.Visible = false;
            pnlPreviewConfirm.CssClass = "modal fade";
        }
        protected void btnPreviewIAMarks_Click(object sender, EventArgs e)
        {
            // Close the previous confirm modal correctly (keep your current logic)
            pnlPreviewConfirm.CssClass = "modal fade";
            pnlPreviewConfirm.Visible = true; // keep rendered to avoid layout jumps

            string programCode = ddlProgramme.SelectedValue;
            int semester = Convert.ToInt32(ddlSemester.SelectedValue);

            // Load the report
            LoadReport(programCode, semester);

            // Mark report loaded
            ViewState["ReportLoaded"] = true;

            // Show inline confirmation, hide final submit until "Yes"
            ViewState["IA_Agreed"] = false;
            pnlAfterReportConfirm.Visible = true;
            btnFinalSubmit.Visible = false;

            // Ensure the ReportViewer panel updates
            upReportViewer.Update();
        }

        private void LoadReport(string programCode, int semester)
        {
            DataTable dt = IntrnalMarksDAO.GetIAData(instituteCode, programCode, semester);

            if (dt == null || dt.Rows.Count == 0)
            {
                lblMessage.Text = "No data found for this report.";
                rvIA.Visible = false;
                return;
            }

            rvIA.Visible = true;
            rvIA.ProcessingMode = ProcessingMode.Local;
            rvIA.LocalReport.ReportPath = Server.MapPath("~/Reports/IA_AllCourses.rdlc");
            rvIA.LocalReport.DataSources.Clear();
            rvIA.LocalReport.DataSources.Add(new ReportDataSource("IADataSource", dt));
            // rvIA.LocalReport.SetParameters(new ReportParameter("PageCourseCount", "2"));
            rvIA.AsyncRendering = false;
            rvIA.SizeToReportContent = true;
            rvIA.LocalReport.Refresh();
        }
        // Step 2: User confirmed to continue
        protected void btnAfterReportYes_Click(object sender, EventArgs e)
        {
            ViewState["IA_Agreed"] = true;
            pnlAfterReportConfirm.Visible = false; // or keep true if you want the text to remain
            btnFinalSubmit.Visible = true;

            lblMessage.CssClass = "text-success fw-semibold";
            lblMessage.Text = "Verified. You can now proceed to Final Submit.";

            upReportViewer.Update();
        }

        protected void btnAfterReportNo_Click(object sender, EventArgs e)
        {
            ViewState["IA_Agreed"] = false;
            pnlAfterReportConfirm.Visible = true;
            btnFinalSubmit.Visible = false;

            lblMessage.CssClass = "text-danger fw-semibold";
            lblMessage.Text = "Please revisit and re-verify the IA marks before final submission.";

            upReportViewer.Update();
        }

        protected void btnFinalSubmit_Click(object sender, EventArgs e)
        {
            bool agreed = ViewState["IA_Agreed"] is bool b && b;
            if (!agreed)
            {
                litModalBody.Text = "Please confirm the verification below the report before final submission.";
                pnlServerModal.Visible = true;
                pnlServerModal.CssClass = "modal fade show d-block";
                return;
            }

            // Show your irreversible confirmation modal (existing)
            ShowModal(pnlIrreversible);
        }
        protected void btnIrrevCancel_Click(object sender, EventArgs e)
        {
            HideModal(pnlIrreversible);
        }

        protected void btnIrrevProceed_Click(object sender, EventArgs e)
        {
            HideModal(pnlIrreversible);

            // 1) Your existing finalization logic for IA (persist/lock marks etc.)
            // bool ok = IntrnalMarksDAO.SubmitFinalIA(...);

            // 2) Upsert into Tb_Process_State (SP will pull Academic Year itself)
            string programCode = ddlProgramme.SelectedValue;
            int semester = Convert.ToInt32(ddlSemester.SelectedValue);

            bool upsertOk = IntrnalMarksDAO.UpsertFinalizeIA(instituteCode, programCode, semester);
            // Immediately reflect finalized state in the UI
            ViewState["IA_Locked"] = true;
            btnSaveDraft.Enabled = false;
            btnPreviewIA.Enabled = false;
            btnFinalSubmit.Visible = false;

            // If you still have data on screen, rebuild the table to disable inputs visually
            if (ViewState["MarksData"] is DataTable currentDt)
            {
                BuildEditableTable(currentDt);
            }


            if (!upsertOk)
            {
                litModalBody.Text = "Finalization saved, but process state could not be updated. Please contact admin.";
                pnlServerModal.Visible = true;
                pnlServerModal.CssClass = "modal fade show d-block";
                return;
            }

            // 3) Lock UI
            btnFinalSubmit.Visible = false;
            lblMessage.CssClass = "text-success fw-semibold";
            lblMessage.Text = "Final IA marks submitted successfully and process state updated.";
        }

        private void ShowModal(Panel modalPanel)
        {
            modalPanel.Visible = true;
            modalPanel.CssClass = "modal fade show d-block";
        }

        private void HideModal(Panel modalPanel)
        {
            modalPanel.Visible = true;           // keep it rendered
            modalPanel.CssClass = "modal fade";  // hidden state
            RemoveBackdrop();
        }

        private void RemoveBackdrop()
        {
            // Clean body/backdrop (needed when modals are toggled via server)
            ScriptManager.RegisterStartupScript(
                this, GetType(), Guid.NewGuid().ToString(),
                "document.body.classList.remove('modal-open');" +
                "var b=document.querySelector('.modal-backdrop'); if(b) b.remove();", true);
        }
        private void ShowError(string msg)
        {
            lblMessage.CssClass = "text-danger fw-semibold";
            lblMessage.Text = msg;
        }
        protected void rptCourses_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var btnCourse = (Button)e.Item.FindControl("btnCourse");
                string courseCode = DataBinder.Eval(e.Item.DataItem, "Course_Code").ToString();
                if (ViewState["SelectedCourse"] != null && ViewState["SelectedCourse"].ToString() == courseCode)
                {
                    btnCourse.CssClass = "btn btn-primary m-1"; // highlighted
                }
                else
                {
                    btnCourse.CssClass = "btn btn-outline-primary m-1";
                }
            }
        }
    }
}