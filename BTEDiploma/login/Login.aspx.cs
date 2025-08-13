using System;
using System.Linq;
using System.Web.UI;
using BTEDiploma.Helper;  // For LoginDao
using System.Security.Cryptography;
using System.Text;
using System.Data;

namespace BTEDiploma.login
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session.Clear();
                LoadCaptcha();
            }
        }

        // ----------------- CAPTCHA METHODS -----------------
        private string GenerateCaptchaText()
        {
            string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Avoid confusing chars
            Random rand = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                              .Select(s => s[rand.Next(s.Length)]).ToArray());
        }

        private void LoadCaptcha()
        {
            string captchaText = GenerateCaptchaText();
            Session["Captcha"] = captchaText;
            lblCaptcha.Text = captchaText;

            string[] colors = {
                "linear-gradient(45deg, #ff9a9e, #fad0c4)",
                "linear-gradient(45deg, #a18cd1, #fbc2eb)",
                "linear-gradient(45deg, #f6d365, #fda085)",
                "linear-gradient(45deg, #84fab0, #8fd3f4)",
                "linear-gradient(45deg, #cfd9df, #e2ebf0)"
            };

            Random rnd = new Random();
            lblCaptcha.Attributes["style"] = $"background:{colors[rnd.Next(colors.Length)]}; color:white; padding:8px; border-radius:4px;";
            lblCaptchaError.Text = "";
        }

        protected void btnRefreshCaptcha_Click(object sender, EventArgs e)
        {
            LoadCaptcha();
        }

        // ----------------- LOGIN METHOD -----------------
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // Captcha validation
            if (Session["Captcha"] == null || txtCaptcha.Text.Trim() != Session["Captcha"].ToString())
            {
                lblCaptchaError.Text = "Invalid CAPTCHA. Please try again.";
                LoadCaptcha();
                return;
            }

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Enter username and password.";
                return;
            }

            try
            {
                // STEP 1: Fetch user row (Username, PasswordHash, PasswordSalt, etc.)
                DataTable dt = LoginDao.GetUserByUsername(username);

                if (dt == null || dt.Rows.Count == 0)
                {
                    lblError.Text = "Invalid Username or Password.";
                    LoadCaptcha();
                    return;
                }

                byte[] storedHash = (byte[])dt.Rows[0]["PasswordHash"];
                byte[] storedSalt = (byte[])dt.Rows[0]["PasswordSalt"];

                // STEP 2: Compute hash for entered password with stored salt
                byte[] enteredHash = ComputeHash(password, storedSalt);

                // STEP 3: Compare
                if (storedHash.SequenceEqual(enteredHash))
                {
                    // Authentication success
                    Session["Username"] = dt.Rows[0]["Username"].ToString();
                    Session["UserType"] = dt.Rows[0]["UserTypeCode"].ToString();
                    Session["CollegeCode"] = dt.Rows[0]["CollegeCode"].ToString();
                    Session["CollegeName"] = dt.Rows[0]["CollegeName"].ToString();

                    Response.Redirect("Dashboard.aspx");
                }
                else
                {
                    lblError.Text = "Invalid Username or Password.";
                    LoadCaptcha();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error: " + ex.Message;
            }
        }

        // ----------------- HELPER FOR HASH -----------------
        private byte[] ComputeHash(string password, byte[] salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] pwdBytes = Encoding.Unicode.GetBytes(password);
                byte[] combined = new byte[salt.Length + pwdBytes.Length];
                System.Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
                System.Buffer.BlockCopy(pwdBytes, 0, combined, salt.Length, pwdBytes.Length);
                return sha256.ComputeHash(combined);
            }
        }
    }
}
