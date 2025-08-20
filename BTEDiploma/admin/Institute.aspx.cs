using System;
using System.Data;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using BTEDiploma.sqlhelper; // << use InstDao & SQLHelper

namespace BTEDiploma.admin
{
    public partial class Institute : System.Web.UI.Page
    {
        #region Lifecycle
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindShifts();
                BindTaluks();
                BindExamZones();
                BindGender();
                BindInstitutionTypes();
                LoadGridData();
            }
        }
        #endregion

        #region Binding (via InstDao)
        private void BindShifts()
        {
            var dt = ManagesystemdataDAO.GetShifts();
            ddlShift.DataSource = dt;
            ddlShift.DataTextField = "Shift_Name";
            ddlShift.DataValueField = "Inst_Shift_ID";
            ddlShift.DataBind();
            ddlShift.Items.Insert(0, new ListItem("-- Select Shift --", ""));
        }

        private void BindTaluks()
        {
            var dt = ManagesystemdataDAO.GetTaluks();
            ddltaluk.DataSource = dt;
            ddltaluk.DataTextField = "Taluk_Name";
            ddltaluk.DataValueField = "Taluk_ID";
            ddltaluk.DataBind();
            ddltaluk.Items.Insert(0, new ListItem("-- Select Taluk --", ""));
        }

        private void BindExamZones()
        {
            var dt = ManagesystemdataDAO.GetExamZones();
            ddlExamzone.DataSource = dt;
            ddlExamzone.DataTextField = "Exam_Zone_Name";
            ddlExamzone.DataValueField = "Exam_Zone_ID";
            ddlExamzone.DataBind();
            ddlExamzone.Items.Insert(0, new ListItem("-- Select Exam Zone --", ""));
        }

        private void BindGender()
        {
            ddlgender.Items.Clear();
            ddlgender.Items.Add(new ListItem("-- Select Gender --", ""));
            ddlgender.Items.Add(new ListItem("CO-ED", "C"));
            ddlgender.Items.Add(new ListItem("Girls", "W"));
        }

        private void BindInstitutionTypes()
        {
            var dt = ManagesystemdataDAO.GetInstitutionTypes();
            ddlinsttype.DataSource = dt;
            ddlinsttype.DataTextField = "Institution_Type_Description";
            ddlinsttype.DataValueField = "Institution_Type_ID";
            ddlinsttype.DataBind();
            ddlinsttype.Items.Insert(0, new ListItem("-- Select Institution Type --", ""));
        }
        #endregion

        #region GridView Events
        protected void gvInstitute_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && gvInstitute.EditIndex == e.Row.RowIndex)
            {
                // Institution Type
                var ddlInstitutionType = (DropDownList)e.Row.FindControl("ddlInstitutionType");
                if (ddlInstitutionType != null)
                {
                    ddlInstitutionType.DataSource = ManagesystemdataDAO.GetInstitutionTypes();
                    ddlInstitutionType.DataTextField = "Institution_Type_Description";
                    ddlInstitutionType.DataValueField = "Institution_Type_ID";
                    ddlInstitutionType.DataBind();
                    ddlInstitutionType.Items.Insert(0, new ListItem("-- Select Institution Type --", ""));
                    var val = DataBinder.Eval(e.Row.DataItem, "Institution_Type_ID");
                    SelectIfFound(ddlInstitutionType, val);
                }

                // Taluk
                var ddlTaluk = (DropDownList)e.Row.FindControl("ddlTaluk");
                if (ddlTaluk != null)
                {
                    ddlTaluk.DataSource = ManagesystemdataDAO.GetTaluks();
                    ddlTaluk.DataTextField = "Taluk_Name";
                    ddlTaluk.DataValueField = "Taluk_ID";
                    ddlTaluk.DataBind();
                    ddlTaluk.Items.Insert(0, new ListItem("-- Select Taluk --", ""));
                    var val = DataBinder.Eval(e.Row.DataItem, "Taluk_ID");
                    SelectIfFound(ddlTaluk, val);
                }

                // Exam Zone
                var ddlExamZone = (DropDownList)e.Row.FindControl("ddlExamZone");
                if (ddlExamZone != null)
                {
                    ddlExamZone.DataSource = ManagesystemdataDAO.GetExamZones();
                    ddlExamZone.DataTextField = "Exam_Zone_Name";
                    ddlExamZone.DataValueField = "Exam_Zone_ID";
                    ddlExamZone.DataBind();
                    ddlExamZone.Items.Insert(0, new ListItem("-- Select Exam Zone --", ""));
                    var val = DataBinder.Eval(e.Row.DataItem, "Exam_Zone_ID");
                    SelectIfFound(ddlExamZone, val);
                }

                // Shift
                var ddlShift = (DropDownList)e.Row.FindControl("ddlShift");
                if (ddlShift != null)
                {
                    ddlShift.DataSource = ManagesystemdataDAO.GetShifts();
                    ddlShift.DataTextField = "Shift_Name";
                    ddlShift.DataValueField = "Inst_Shift_ID";
                    ddlShift.DataBind();
                    ddlShift.Items.Insert(0, new ListItem("-- Select Shift --", ""));
                    var val = DataBinder.Eval(e.Row.DataItem, "Inst_Shift_ID");
                    SelectIfFound(ddlShift, val);
                }

                // Gender
                var ddlGender = (DropDownList)e.Row.FindControl("ddlGender");
                if (ddlGender != null)
                {
                    ddlGender.Items.Clear();
                    ddlGender.Items.Add(new ListItem("-- Select Gender --", ""));
                    ddlGender.Items.Add(new ListItem("CO-ED", "C"));
                    ddlGender.Items.Add(new ListItem("Girls", "W"));
                    var val = DataBinder.Eval(e.Row.DataItem, "Gender");
                    SelectIfFound(ddlGender, val);
                }
            }
        }

        protected void gvInstitute_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvInstitute.EditIndex = e.NewEditIndex;
            LoadGridData();
        }

        protected void gvInstitute_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvInstitute.EditIndex = -1;
            LoadGridData();
        }

        protected void gvInstitute_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            Page.Validate("RowEdit");
            if (!Page.IsValid)
            {
                e.Cancel = true;
                return;
            }

            GridViewRow row = gvInstitute.Rows[e.RowIndex];

            int instCode = Convert.ToInt32(gvInstitute.DataKeys[e.RowIndex].Value);
            string instName = ((TextBox)row.FindControl("txtInstName")).Text.Trim();

            var ddlTaluk = (DropDownList)row.FindControl("ddlTaluk");
            var ddlInstitutionType = (DropDownList)row.FindControl("ddlInstitutionType");
            var ddlShift = (DropDownList)row.FindControl("ddlShift");
            var ddlExamZone = (DropDownList)row.FindControl("ddlExamZone");
            var ddlGender = (DropDownList)row.FindControl("ddlGender");

            int talukId = Convert.ToInt32(ddlTaluk.SelectedValue);
            int institutionTypeId = Convert.ToInt32(ddlInstitutionType.SelectedValue);
            int instShift = Convert.ToInt32(ddlShift.SelectedValue);
            int instExamZone = Convert.ToInt32(ddlExamZone.SelectedValue);
            string gender = ddlGender.SelectedValue;

            string address = ((TextBox)row.FindControl("txtAddress")).Text.Trim();
            string pincode = ((TextBox)row.FindControl("txtPincode")).Text.Trim();
            string aicteId = ((TextBox)row.FindControl("txtAICTEID")).Text.Trim();
            bool isExamCenter = ((CheckBox)row.FindControl("chkIsExamCenter")).Checked;

            int? principalId = null;
            var txtPrincipal = (TextBox)row.FindControl("txtPrincipalID");
            if (txtPrincipal != null && !string.IsNullOrWhiteSpace(txtPrincipal.Text))
                principalId = Convert.ToInt32(txtPrincipal.Text.Trim());

            string emailId = ((TextBox)row.FindControl("txtEmail")).Text.Trim();
            string phone = ((TextBox)row.FindControl("txtPhone")).Text.Trim();

            // Minimal server checks for row edit
            if (string.IsNullOrWhiteSpace(instName) ||
                string.IsNullOrEmpty(ddlTaluk.SelectedValue) ||
                string.IsNullOrEmpty(ddlInstitutionType.SelectedValue) ||
                string.IsNullOrEmpty(ddlShift.SelectedValue) ||
                string.IsNullOrEmpty(ddlExamZone.SelectedValue) ||
                string.IsNullOrWhiteSpace(address) ||
                !Regex.IsMatch(pincode, "^[1-9][0-9]{5}$") ||
                (!string.IsNullOrWhiteSpace(emailId) && !Regex.IsMatch(emailId, @"^[^\s@]+@[^\s@]+\.[^\s@]+$")) ||
                (!string.IsNullOrWhiteSpace(phone) && !Regex.IsMatch(phone, "^[6-9]\\d{9}$")))
            {
                lblMessage.Text = "Please correct highlighted values before saving.";
                lblMessage.CssClass = "text-danger";
                e.Cancel = true;
                return;
            }

            // Call DAO (SP: UPDATE)
            bool ok = ManagesystemdataDAO.UpdateInstitution(
                instCode, instName, talukId, address, pincode,
                institutionTypeId, instShift, instExamZone, aicteId, isExamCenter,
                gender, principalId, emailId, phone,
                out int statusCode, out string message);

            if (!ok) { statusCode = 500; if (string.IsNullOrEmpty(message)) message = "Unknown error"; }

            gvInstitute.EditIndex = -1;
            LoadGridData();
            ClearFields();

            lblMessage.Text = message;
            lblMessage.CssClass = (statusCode == 200) ? "text-success" : "text-danger";
        }

        protected void gvInstitute_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int instCode = Convert.ToInt32(gvInstitute.DataKeys[e.RowIndex].Value);

                bool ok = ManagesystemdataDAO.DeleteInstitution(instCode, out int statusCode, out string message);

                lblMessage.Text = message;
                lblMessage.CssClass = (ok && statusCode == 200) ? "text-success" : "text-danger";

                LoadGridData();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error deleting institute: " + ex.Message;
                lblMessage.CssClass = "text-danger";
            }
        }
        #endregion

        #region Commands
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Page.Validate("Institute");
                if (!Page.IsValid) return;

                if (!ValidateForm(out string err))
                {
                    lblMessage.Text = err;
                    lblMessage.CssClass = "text-danger";
                    return;
                }

                // Parse values
                int instCode = Convert.ToInt32(TxtInst.Text.Trim());
                int talukID = Convert.ToInt32(ddltaluk.SelectedValue);
                int instTypeID = Convert.ToInt32(ddlinsttype.SelectedValue);
                int instShift = Convert.ToInt32(ddlShift.SelectedValue);
                int examZone = Convert.ToInt32(ddlExamzone.SelectedValue);
                int? principalID = string.IsNullOrWhiteSpace(TxtPrincipal.Text) ? (int?)null : Convert.ToInt32(TxtPrincipal.Text.Trim());
                bool isExamCenter = ddlexamcenter.SelectedValue == "1";

                // Call DAO (SP: INSERT)
                bool ok = ManagesystemdataDAO.InsertInstitution(
                    instCode, TxtInstName.Text.Trim(), talukID, TxtAddress.Text.Trim(), Txtpincode.Text.Trim(),
                    instTypeID, instShift, examZone, TxtAicte.Text.Trim(), isExamCenter,
                    ddlgender.SelectedValue, principalID, Txtemail.Text.Trim(), Txtphone.Text.Trim(),
                    out int statusCode, out string message);

                lblMessage.Text = message;
                lblMessage.CssClass = (ok && statusCode == 200) ? "text-success" : "text-danger";

                if (ok && statusCode == 200)
                {
                    LoadGridData();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.CssClass = "text-danger";
            }
        }
        #endregion
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // exit edit mode if grid was editing
            gvInstitute.EditIndex = -1;

            // clear all inputs
            ClearFields();              // <- use your existing helper, but ensure it sets Principal = 100

            // reset validators so the summary disappears
            foreach (BaseValidator v in Page.Validators)
                v.IsValid = true;

            // clear any status text
            lblMessage.Text = string.Empty;

            // (optional) reload grid to ensure a clean state
            LoadGridData();
        }


        #region Data Load / Utilities
        private void LoadGridData()
        {
            gvInstitute.DataSource = ManagesystemdataDAO.GetAllInstitutions1();
            gvInstitute.DataBind();
        }

        private void ClearFields()
        {
            TxtInst.Text = "";
            TxtInstName.Text = "";
            ddlinsttype.SelectedIndex = 0;
            ddlShift.SelectedIndex = 0;
            ddlgender.SelectedIndex = 0;
            ddltaluk.SelectedIndex = 0;
            ddlexamcenter.SelectedIndex = 0;
            ddlExamzone.SelectedIndex = 0;
            TxtAddress.Text = "";
            TxtAicte.Text = "";
            TxtPrincipal.Text = "";
            Txtemail.Text = "";
            Txtphone.Text = "";
        }

        private static void SelectIfFound(DropDownList ddl, object value)
        {
            string v = (value == null || value == DBNull.Value) ? "" : value.ToString();
            if (!string.IsNullOrEmpty(v) && ddl.Items.FindByValue(v) != null)
                ddl.SelectedValue = v;
            else
                ddl.SelectedIndex = 0; // "-- Select --"
        }
        #endregion

        #region Validation Helpers
        private static bool TryGetInt(string input, out int value) =>
            int.TryParse((input ?? "").Trim(), out value);

        private bool ValidateForm(out string error)
        {
            error = null;

            // Required text fields
            if (string.IsNullOrWhiteSpace(TxtInst.Text)) { error = "Institute Code is required."; return false; }
            if (string.IsNullOrWhiteSpace(TxtInstName.Text)) { error = "Institute Name is required."; return false; }
            if (string.IsNullOrWhiteSpace(TxtAicte.Text)) { error = "AICTE ID is required."; return false; }
            if (string.IsNullOrWhiteSpace(TxtAddress.Text)) { error = "Address is required."; return false; }
            if (string.IsNullOrWhiteSpace(Txtpincode.Text)) { error = "Pincode is required."; return false; }
            if (string.IsNullOrWhiteSpace(Txtemail.Text)) { error = "Email is required."; return false; }
            if (string.IsNullOrWhiteSpace(Txtphone.Text)) { error = "Phone is required."; return false; }

            // Required dropdowns
            if (string.IsNullOrEmpty(ddlinsttype.SelectedValue)) { error = "Institute Type is required."; return false; }
            if (string.IsNullOrEmpty(ddlShift.SelectedValue)) { error = "Shift is required."; return false; }
            if (string.IsNullOrEmpty(ddlgender.SelectedValue)) { error = "Gender is required."; return false; }
            if (string.IsNullOrEmpty(ddltaluk.SelectedValue)) { error = "Taluk is required."; return false; }
            if (string.IsNullOrEmpty(ddlexamcenter.SelectedValue)) { error = "Is Exam Center selection is required."; return false; }
            if (string.IsNullOrEmpty(ddlExamzone.SelectedValue)) { error = "Exam Zone is required."; return false; }

            // Numeric fields
            if (!TryGetInt(TxtInst.Text, out _)) { error = "Institute Code must be numeric."; return false; }
            if (!TryGetInt(ddltaluk.SelectedValue, out _)) { error = "Taluk is invalid."; return false; }
            if (!TryGetInt(ddlinsttype.SelectedValue, out _)) { error = "Institution Type is invalid."; return false; }
            if (!TryGetInt(ddlShift.SelectedValue, out _)) { error = "Shift is invalid."; return false; }
            if (!TryGetInt(ddlExamzone.SelectedValue, out _)) { error = "Exam Zone is invalid."; return false; }
            if (!string.IsNullOrWhiteSpace(TxtPrincipal.Text) && !TryGetInt(TxtPrincipal.Text, out _))
            { error = "Principal ID must be numeric."; return false; }

            // Pincode (Indian 6-digit)
            if (!Regex.IsMatch(Txtpincode.Text.Trim(), "^[1-9][0-9]{5}$"))
            { error = "Enter a valid 6-digit Indian pincode."; return false; }

            // Email
            if (!Regex.IsMatch(Txtemail.Text.Trim(), @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
            { error = "Enter a valid email address."; return false; }

            // Phone (Indian 10-digit starting 6-9)
            if (!Regex.IsMatch(Txtphone.Text.Trim(), "^[6-9]\\d{9}$"))
            { error = "Enter a valid 10-digit Indian mobile number."; return false; }

            return true;
        }
        #endregion
    }
}