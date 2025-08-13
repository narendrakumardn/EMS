using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using BTEDiploma.Helper;
using System.Web.UI.WebControls;
using System.Linq;

namespace BTEDiploma.login
{
    public partial class Register : System.Web.UI.Page
    {
        private const int SaltSize = 16; // 128-bit
        private const int HashSize = 32; // 256-bit

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadColleges();
                LoadUserTypes();
            }
        }

        private void LoadUserTypes()
        {
            DataTable dt = RegisterDao.GetUserTypes();

            ddlUserType.DataSource = dt;
            ddlUserType.DataTextField = "Name";
            ddlUserType.DataValueField = "Code";
            ddlUserType.DataBind();

            ddlUserType.Items.Insert(0, new ListItem("-- Select User Type --", ""));
        }

        private void LoadColleges()
        {
            DataTable dt = SQLHelper.ExecuteStoredProcedure("spGetColleges", null);
            ddlCollegeCode.DataSource = dt;
            ddlCollegeCode.DataTextField = "CollegeName";
            ddlCollegeCode.DataValueField = "CollegeCode";
            ddlCollegeCode.DataBind();
            ddlCollegeCode.Items.Insert(0, new ListItem("-- Select College --", ""));
        }

        /*protected void BtnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string name = txtName.Text.Trim();
            string collegeCode = ddlCollegeCode.SelectedValue;
            string userTypeCode = ddlUserType.SelectedValue;
            string mobile = txtMobile.Text.Trim();
            // Validate Mobile Number or  Server side validation
            if (string.IsNullOrEmpty(mobile) || !System.Text.RegularExpressions.Regex.IsMatch(mobile, @"^[6-9]\d{9}$"))
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Enter a valid mobile number.";
                return;
            }

            // Validate input
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(name) || string.IsNullOrEmpty(collegeCode) ||
                string.IsNullOrEmpty(userTypeCode))
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "All fields are required.";
                return;
            }

            // Generate Salt and Hash
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash;
            using (var sha256 = SHA256.Create())
            {
                // Use Unicode to match SQL NVARCHAR hashing
                byte[] pwdBytes = Encoding.Unicode.GetBytes(password);

                byte[] combined = new byte[salt.Length + pwdBytes.Length];
                // byte[] pwdBytes = Encoding.UTF8.GetBytes(password);
                //byte[] combined = new byte[salt.Length + pwdBytes.Length];
                System.Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
                System.Buffer.BlockCopy(pwdBytes, 0, combined, salt.Length, pwdBytes.Length);
                hash = sha256.ComputeHash(combined);
            }

            // Prepare parameters for stored procedure
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", username),
                new SqlParameter("@Name", name),
                new SqlParameter("@CollegeCode", collegeCode),
                new SqlParameter("@UserType", Convert.ToByte(userTypeCode)), // TINYINT
                new SqlParameter("@PasswordHash", SqlDbType.VarBinary, HashSize) { Value = hash },
                new SqlParameter("@PasswordSalt", SqlDbType.VarBinary, SaltSize) { Value = salt },
                new SqlParameter("@Mobile", string.IsNullOrEmpty(mobile) ? (object)DBNull.Value : mobile),
                new SqlParameter("@IsActive", true),
                new SqlParameter("@OverwriteIfExists", false)
            };

            // Execute stored procedure
            DataTable dt = SQLHelper.ExecuteStoredProcedure("spRegisterUser", parameters);

            if (dt != null && dt.Rows.Count > 0)
            {
                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "User registered successfully.";
                ClearFields();
            }
            else
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Registration failed. Try again.";
            }

        }
        */

        protected void BtnRegister_Click(object sender, EventArgs e)
        {
            // ----- Gather inputs -----
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string name = txtName.Text.Trim();
            string collegeCode = ddlCollegeCode.SelectedValue;
            string userTypeCode = ddlUserType.SelectedValue;
            string mobile = txtMobile.Text.Trim();

            // ----- Client-side server validation -----
            if (string.IsNullOrEmpty(mobile) ||
                !System.Text.RegularExpressions.Regex.IsMatch(mobile, @"^[6-9]\d{9}$"))
            {
                ShowError("Enter a valid mobile number (10 digits starting 6-9).");
                return;
            }

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(name) || string.IsNullOrEmpty(collegeCode) ||
                string.IsNullOrEmpty(userTypeCode))
            {
                ShowError("All fields are required.");
                return;
            }

            // ----- Salt + hash -----
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash;
            using (var sha256 = SHA256.Create())
            {
                // IMPORTANT: match encoding used at login verification time.
                // You commented both Unicode & UTF8; pick ONE and be consistent across register + login.
                // If your login code hashes with Unicode, keep Unicode here:
                byte[] pwdBytes = Encoding.Unicode.GetBytes(password);

                byte[] combined = new byte[salt.Length + pwdBytes.Length];
                System.Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
                System.Buffer.BlockCopy(pwdBytes, 0, combined, salt.Length, pwdBytes.Length);
                hash = sha256.ComputeHash(combined);
            }

            // ----- Parameters -----
            var parameters = new SqlParameter[]
            {
        new SqlParameter("@Username", username),
        new SqlParameter("@Name", name),
        new SqlParameter("@CollegeCode", collegeCode),
        new SqlParameter("@UserType", Convert.ToByte(userTypeCode)),
        new SqlParameter("@PasswordHash", SqlDbType.VarBinary, HashSize) { Value = hash },
        new SqlParameter("@PasswordSalt", SqlDbType.VarBinary, SaltSize) { Value = salt },
        new SqlParameter("@Mobile", string.IsNullOrEmpty(mobile) ? (object)DBNull.Value : mobile),
        new SqlParameter("@IsActive", SqlDbType.Bit) { Value = true },
        new SqlParameter("@OverwriteIfExists", SqlDbType.Bit) { Value = false }
            };

            DataTable dt = null;

            try
            {
                // Always schema-qualify if your helper doesn’t: "dbo.spRegisterUser"
                dt = SQLHelper.ExecuteStoredProcedure("dbo.spRegisterUser", parameters);
            }
            catch (SqlException ex)
            {
                // Check for username duplicate
                if (ex.Number == 50001 || ex.Message.IndexOf("Username already exists", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    ShowError("Username already exists.");
                    return;
                }

                // Check for invalid mobile number
                if (ex.Message.IndexOf("Invalid mobile number", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    ShowError("Enter a valid mobile number.");
                    return;
                }

                // SQL unique constraint violation (username or mobile)
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    ShowError("Duplicate entry (username or mobile).");
                    return;
                }

                // Generic fallback
                ShowError("Database error: " + ex.Message);
                return;
            }

            catch (Exception ex)
            {
                ShowError("Unexpected error: " + ex.Message);
                return;
            }

            // ----- Interpret result -----
            if (dt != null && dt.Rows.Count > 0)
            {
                // If you adopted the revised proc, first row contains StatusCode/StatusMessage.
                DataRow row = dt.Rows[0];
                if (dt.Columns.Contains("StatusCode"))
                {
                    int status = Convert.ToInt32(row["StatusCode"]);
                    string statusMsg = Convert.ToString(row["StatusMessage"]);

                    switch (status)
                    {
                        case 0:
                            ShowSuccess("User registered successfully.");
                            ClearFields();
                            break;
                        case 5:
                            ShowError("Username already exists.");
                            break;
                        case 6:
                            ShowError("Mobile number already exists.");
                            break;
                        case 4:
                            ShowError("Invalid mobile number.");
                            break;
                        case 1:
                            ShowError("Username is required.");
                            break;
                        case 2:
                            ShowError("PasswordHash is required (internal error).");
                            break;
                        case 3:
                            ShowError("PasswordSalt is required (internal error).");
                            break;
                        default:
                            ShowError(statusMsg ?? "Registration failed.");
                            break;
                    }
                    return;
                }
                else
                {
                    // Old proc path (no StatusCode). If we reached here we *assume* success.
                    ShowSuccess("User registered successfully.");
                    ClearFields();
                    return;
                }
            }
            else
            {
                // No rows returned at all. Likely validation failure w/ RAISERROR that your helper swallowed,
                // or SET NOCOUNT OFF confusion caused helper to look at wrong result.
                ShowError("Registration failed. No data returned.");
            }
        }

        private void ShowError(string msg)
        {
            lblMessage.ForeColor = System.Drawing.Color.Red;
            lblMessage.Text = msg;
        }

        private void ShowSuccess(string msg)
        {
            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Text = msg;
        }

        private void ClearFields()
        {
            txtUsername.Text = "";
            txtPassword.Text = "";
            txtName.Text = "";
            txtMobile.Text = "";
            ddlCollegeCode.SelectedIndex = 0;
            ddlUserType.SelectedIndex = 0;
        }
    }
}
