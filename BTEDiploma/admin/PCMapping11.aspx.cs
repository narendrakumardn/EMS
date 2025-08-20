using BTEDiploma.Helper;
using BTEDiploma.sqlhelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BTEDiploma.admin
{
    public partial class ProgramCourseMapping : System.Web.UI.Page
    {
        private readonly ManagesystemdataDAO _dao = new ManagesystemdataDAO();


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSchemes();

                // Set min/max for semester and sno fields
                txtSemester.Attributes["min"] = "1";
                txtSemester.Attributes["max"] = "6";

                txtSno.Attributes["min"] = "1";
                txtSno.Attributes["max"] = "6";
            }
        }


        #region Schemes & Dropdowns

        private void LoadSchemes()
        {
            DataTable dt = _dao.GetCurriculumSchemeDetails();
            ddlScheme.DataSource = dt;
            ddlScheme.DataTextField = "Scheme_Name";
            ddlScheme.DataValueField = "Scheme_Code";
            ddlScheme.DataBind();
            ddlScheme.Items.Insert(0, new ListItem("-- Select Scheme --", ""));
        }

        protected void ddlScheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadProgramCourseDropdowns();
            gvSemesterMapping.DataSource = null;
            gvSemesterMapping.DataBind();
        }

        private void LoadProgramCourseDropdowns()
        {
            ddlProgram.Items.Clear();
            ddlCourse.Items.Clear();
            ddlAdmissionType.Items.Clear();

            DataSet ds = _dao.GetProgramCourseDropdowns();
            if (ds == null) return;

            // programs = table 0
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    ddlProgram.Items.Add(new ListItem(r["Program_Display"].ToString(), r["Program_Code"].ToString()));
                }
            }

            // courses = table 1
            if (ds.Tables.Count > 1)
            {
                foreach (DataRow r in ds.Tables[1].Rows)
                {
                    ddlCourse.Items.Add(new ListItem(r["Course_Display"].ToString(), r["Course_Code"].ToString()));
                }
            }

            // admission types = table 2
            if (ds.Tables.Count > 2)
            {
                foreach (DataRow r in ds.Tables[2].Rows)
                {
                    ddlAdmissionType.Items.Add(new ListItem(r["Admission_Display"].ToString(), r["Admission_Type_ID"].ToString()));
                }
            }

            ddlProgram.Items.Insert(0, new ListItem("-- Select Program --", ""));
            ddlCourse.Items.Insert(0, new ListItem("-- Select Course --", ""));
            ddlAdmissionType.Items.Insert(0, new ListItem("-- Select Admission Type --", ""));
        }

        protected void ddlProgram_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadGrid();
        }

        protected void ddlAdmissionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadGrid();
        }

        #endregion

        #region Insert

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // 1. Prevent adding when editing an existing record
            if (gvSemesterMapping.EditIndex != -1)
            {
                lblMessage.Text = "Please finish editing before adding a new mapping.";
                lblMessage.CssClass = "alert alert-warning";
                return;
            }

            // 2. Validate required fields
            if (ddlProgram.SelectedValue == "" || ddlCourse.SelectedValue == "" || ddlAdmissionType.SelectedValue == "" ||
                string.IsNullOrWhiteSpace(txtSemester.Text) || string.IsNullOrWhiteSpace(txtSno.Text))
            {
                lblMessage.Text = "Please fill in all required fields.";
                lblMessage.CssClass = "alert alert-warning";
                return;
            }

            // 3. Use DAO to insert (SP returns StatusCode & StatusMessage)
            int statusCode = 0;
            string statusMessage = string.Empty;

            int rows = _dao.InsertProgramCourse(
                ddlProgram.SelectedValue,
                Convert.ToInt32(txtSemester.Text),
                ddlCourse.SelectedValue,
                Convert.ToInt32(ddlAdmissionType.SelectedValue),
                Convert.ToInt32(txtSno.Text),
                out statusCode,
                out statusMessage);

            lblMessage.Text = statusMessage;
            lblMessage.CssClass = (statusCode == 200) ? "alert alert-success" : "alert alert-danger";

            LoadGrid();
        }

        #endregion

        #region Grid Loading & Bridge handling

        private void LoadGrid()
        {
            if (string.IsNullOrWhiteSpace(ddlProgram.SelectedValue) ||
                string.IsNullOrWhiteSpace(ddlAdmissionType.SelectedValue))
                return;

            string programCode = ddlProgram.SelectedValue;
            int admissionTypeId = int.Parse(ddlAdmissionType.SelectedValue);

            DataTable dt = _dao.GetProgramCoursePivoted(programCode, admissionTypeId);

            // If admission types require Bridge rows (2..4) then inject Bridge rows
            if (admissionTypeId >= 2 && admissionTypeId <= 4)
            {
                DataTable bridgeRows = dt.Clone();

                foreach (DataRow normalRow in dt.Rows)
                {
                    int semNo = Convert.ToInt32(normalRow["SemesterNo"]);
                    if (semNo == 3 || semNo == 4)
                    {
                        DataTable bridgeCourses = _dao.GetBridgeCourses(programCode, semNo);

                        DataRow bridgeRow = bridgeRows.NewRow();
                        bridgeRow["SemesterNo"] = semNo;
                        bridgeRow["Type"] = "Bridge";

                        if (dt.Columns.Contains("Program_Code"))
                            bridgeRow["Program_Code"] = normalRow["Program_Code"];

                        if (dt.Columns.Contains("Admission_Type_ID"))
                            bridgeRow["Admission_Type_ID"] = normalRow["Admission_Type_ID"];

                        // Copy first 2 bridge course slots, pad with DBNull for others
                        for (int i = 0; i < 5; i++)
                        {
                            if (i < 2 && i < bridgeCourses.Rows.Count)
                            {
                                bridgeRow[$"Course{i + 1}_Code"] = bridgeCourses.Rows[i]["Course_Code"];
                                bridgeRow[$"Course{i + 1}_Display"] = bridgeCourses.Rows[i]["Course_Display"];
                            }
                            else
                            {
                                bridgeRow[$"Course{i + 1}_Code"] = DBNull.Value;
                                bridgeRow[$"Course{i + 1}_Display"] = DBNull.Value;
                            }
                        }

                        bridgeRows.Rows.Add(bridgeRow);
                    }
                }

                // Merge back
                foreach (DataRow r in bridgeRows.Rows)
                    dt.ImportRow(r);

                // Add sort helper so Normal come before Bridge
                if (!dt.Columns.Contains("SortType"))
                    dt.Columns.Add("SortType", typeof(int));

                foreach (DataRow row in dt.Rows)
                    row["SortType"] = (row["Type"].ToString() == "Normal") ? 1 : 2;

                dt.DefaultView.Sort = "SemesterNo ASC, SortType ASC";
                dt = dt.DefaultView.ToTable();
            }

            gvSemesterMapping.DataSource = dt;
            gvSemesterMapping.DataBind();
        }

        // Replaced helper now delegates to DAO
        private DataTable GetBridgeCourses(string programCode, int semester)
        {
            return _dao.GetBridgeCourses(programCode, semester);
        }

        #endregion

        #region Grid Edit / Update / Cancel

        protected void gvSemesterMapping_RowEditing(object sender, GridViewEditEventArgs e)
        {
            // Disable submit while editing
            btnSubmit.Enabled = false;

            gvSemesterMapping.EditIndex = e.NewEditIndex;
            LoadGrid();

            // Read DataKeys and rowType (Normal / Bridge)
            string programCode = gvSemesterMapping.DataKeys[e.NewEditIndex]["Program_Code"].ToString();
            int semester = Convert.ToInt32(gvSemesterMapping.DataKeys[e.NewEditIndex]["SemesterNo"]);
            int admissionTypeID = Convert.ToInt32(gvSemesterMapping.DataKeys[e.NewEditIndex]["Admission_Type_ID"]);

            string rowType = ((Label)gvSemesterMapping.Rows[e.NewEditIndex].FindControl("lblType"))?.Text ?? "Normal";

            // Remap bridge semester (3->1, 4->2) while fetching
            if (rowType.Equals("Bridge", StringComparison.OrdinalIgnoreCase))
            {
                if (semester == 3) semester = 1;
                else if (semester == 4) semester = 2;
            }

            GridViewRow row = gvSemesterMapping.Rows[e.NewEditIndex];

            for (int sno = 1; sno <= 5; sno++)
            {
                DropDownList ddlCourse = (DropDownList)row.FindControl("ddlCourse" + sno);
                if (ddlCourse == null) continue;

                // Disable unused dropdowns for Bridge rows
                if (rowType.Equals("Bridge", StringComparison.OrdinalIgnoreCase) && sno > 2)
                {
                    ddlCourse.Enabled = false;
                    continue;
                }

                string currentCourseCode = gvSemesterMapping.DataKeys[e.NewEditIndex][$"Course{sno}_Code"]?.ToString();

                DataTable dt = null;
                if (rowType.Equals("Bridge", StringComparison.OrdinalIgnoreCase))
                {
                    // Convert logical semester to physical bridge semester
                    if (semester == 3) semester = 1;
                    else if (semester == 4) semester = 2;
                    dt = _dao.GetBridgeCourses(programCode, semester);
                }
                else
                {
                    // Use DAO that returns course list per sno (Program, AdmissionType, Semester, Sno)
                    //  dt = _dao.GetCoursesByProgramSemester(programCode, admissionTypeID, semester, sno);
                    // Correct parameter order
                    dt = _dao.GetCoursesByProgramSemester(programCode, semester, sno, admissionTypeID);

                }

                // Ensure current value exists in list
                if (!string.IsNullOrEmpty(currentCourseCode) &&
                    dt.Select($"Course_Code = '{currentCourseCode}'").Length == 0)
                {
                    DataRow newRow = dt.NewRow();
                    newRow["Course_Code"] = currentCourseCode;
                    newRow["Course_Display"] = currentCourseCode + " (Current)";
                    dt.Rows.InsertAt(newRow, 0);
                }

                ddlCourse.DataSource = dt;
                ddlCourse.DataTextField = "Course_Display";
                ddlCourse.DataValueField = "Course_Code";
                ddlCourse.DataBind();

                if (!string.IsNullOrEmpty(currentCourseCode))
                {
                    ListItem item = ddlCourse.Items.FindByValue(currentCourseCode);
                    if (item != null)
                    {
                        ddlCourse.ClearSelection();
                        item.Selected = true;
                    }
                }
            }
        }

        protected void gvSemesterMapping_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string programCode = gvSemesterMapping.DataKeys[e.RowIndex]["Program_Code"].ToString();
            int semester = Convert.ToInt32(gvSemesterMapping.DataKeys[e.RowIndex]["SemesterNo"]);
            int admissionTypeID = Convert.ToInt32(gvSemesterMapping.DataKeys[e.RowIndex]["Admission_Type_ID"]);
            string rowType = gvSemesterMapping.DataKeys[e.RowIndex]["Type"].ToString().Trim();

            string lastMessage = "Mapping updated successfully.";
            string cssClass = "alert alert-success";

            if (rowType.Equals("Bridge", StringComparison.OrdinalIgnoreCase))
            {
                string course1 = ((DropDownList)gvSemesterMapping.Rows[e.RowIndex].FindControl("ddlCourse1")).SelectedValue?.Trim();
                string course2 = ((DropDownList)gvSemesterMapping.Rows[e.RowIndex].FindControl("ddlCourse2")).SelectedValue?.Trim();

                // Delegate to DAO - include admissionTypeID
                _dao.UpdateBridgeCourse(programCode, semester, admissionTypeID,
                    string.IsNullOrEmpty(course1) ? null : course1,
                    string.IsNullOrEmpty(course2) ? null : course2);
            }
            else
            {
                // Build TVP table
                DataTable courseTable = new DataTable();
                courseTable.Columns.Add("Sno", typeof(int));
                courseTable.Columns.Add("Course_Code", typeof(string));

                for (int sno = 1; sno <= 5; sno++)
                {
                    DropDownList ddl = (DropDownList)gvSemesterMapping.Rows[e.RowIndex].FindControl("ddlCourse" + sno);
                    if (ddl != null)
                    {
                        string selectedCourse = ddl.SelectedValue?.Trim();
                        if (!string.IsNullOrEmpty(selectedCourse))
                            courseTable.Rows.Add(sno, selectedCourse);
                    }
                }

                // Call DAO TVP update
                _dao.UpdateProgramCourse_TVP(programCode, admissionTypeID, semester, courseTable);
            }

            lblMessage.Text = lastMessage;
            lblMessage.CssClass = cssClass;

            gvSemesterMapping.EditIndex = -1;
            LoadGrid();
            btnSubmit.Enabled = true;
        }

        protected void gvSemesterMapping_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvSemesterMapping.EditIndex = -1;
            LoadGrid();
            btnSubmit.Enabled = true;
        }

        #endregion

        #region RowDataBound & helpers

        protected void gvSemesterMapping_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow &&
                (e.Row.RowState & DataControlRowState.Edit) > 0)
            {
                string rowType = gvSemesterMapping.DataKeys[e.Row.RowIndex]["Type"]?.ToString();

                int semester = Convert.ToInt32(gvSemesterMapping.DataKeys[e.Row.RowIndex]["SemesterNo"]);
                System.Diagnostics.Debug.WriteLine($"Type={rowType}, SemesterNo(raw)={gvSemesterMapping.DataKeys[e.Row.RowIndex]["SemesterNo"]}");
                if (!string.IsNullOrEmpty(rowType) && rowType.Equals("Bridge", StringComparison.OrdinalIgnoreCase))
                {
                    if (semester == 3) semester = 1;
                    else if (semester == 4) semester = 2;
                }

                if (rowType == "Bridge")
                {
                    // Hide ddl3..5
                    e.Row.FindControl("ddlCourse3").Visible = false;
                    e.Row.FindControl("ddlCourse4").Visible = false;
                    e.Row.FindControl("ddlCourse5").Visible = false;

                    // Use DAO for bridge courses
                    string programCode = gvSemesterMapping.DataKeys[e.Row.RowIndex]["Program_Code"].ToString();
                    DataTable dtBridgeCourses = _dao.GetBridgeCourses(programCode, semester);

                    BindCourseDropdown(e.Row, "ddlCourse1", "Course1_Code", dtBridgeCourses);
                    BindCourseDropdown(e.Row, "ddlCourse2", "Course2_Code", dtBridgeCourses);
                }
                else
                {
                    // Use DAO to get dropdowns dataset; second table contains courses
                    DataSet ds = _dao.GetProgramCourseDropdowns();
                    DataTable dtCourses = ds != null && ds.Tables.Count > 1 ? ds.Tables[1] : null;

                    if (dtCourses != null && dtCourses.Rows.Count > 0)
                    {
                        BindCourseDropdown(e.Row, "ddlCourse1", "Course1_Code", dtCourses);
                        BindCourseDropdown(e.Row, "ddlCourse2", "Course2_Code", dtCourses);
                        BindCourseDropdown(e.Row, "ddlCourse3", "Course3_Code", dtCourses);
                        BindCourseDropdown(e.Row, "ddlCourse4", "Course4_Code", dtCourses);
                        BindCourseDropdown(e.Row, "ddlCourse5", "Course5_Code", dtCourses);
                    }
                }
            }
        }

        private void BindCourseDropdown(GridViewRow row, string ddlId, string selectedValueField, DataTable dtCourses)
        {
            DropDownList ddl = (DropDownList)row.FindControl(ddlId);
            if (ddl == null || dtCourses == null) return;

            ddl.DataSource = dtCourses;
            ddl.DataTextField = "Course_Display";
            ddl.DataValueField = "Course_Code";
            ddl.DataBind();

            string selectedValue = DataBinder.Eval(row.DataItem, selectedValueField)?.ToString();
            if (!string.IsNullOrEmpty(selectedValue))
            {
                ListItem item = ddl.Items.FindByValue(selectedValue);
                if (item != null)
                    ddl.SelectedValue = selectedValue;
            }
        }

        #endregion

        #region Optional helpers (delegated to DAO)

        private DataTable GetAllCourses1()
        {
            return _dao.GetAllCourses1(); // DAO should implement this if used
        }

        private void SaveBridgeCourses(string programCode, int semester, int admissionTypeID, string course1, string course2)
        {
            _dao.UpdateBridgeCourse(programCode, semester, admissionTypeID, course1, course2);
        }

        #endregion
    }

}

