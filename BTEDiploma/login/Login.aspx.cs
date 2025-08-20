using System;
using System.Linq;
using System.Web.UI;
using BTEDiploma.Helper;  // For LoginDao
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace BTEDiploma.login
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {



            if (!IsPostBack)
            {
                // ✅ Step 1: Show success message if available
                if (Session["SuccessMessage"] != null)
                {
                    lblMessage.Text = Session["SuccessMessage"].ToString();
                    lblMessage.Visible = true;
                    Session.Remove("SuccessMessage");
                }
                // ✅ Step 2: Clear only login-related session keys, not everything
                Session.Remove("FirstLoginUser");
                Session.Remove("FirstLoginType");
                Session.Remove("OTP"); // if you stored OTP
                                       // don’t call Session.Clear() or Session.RemoveAll(), that kills SuccessMessage too

                // ✅ Step 3: Load captcha
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

        private bool CheckFirstTimeLogin(string userType, string username)
        {
            // This method will:
            // 1. Query the respective table for IsFirstLogin bit
            // 2. If true, store username & userType in session
            // 3. Redirect to ForgotPassword.aspx
            // 4. Return true if redirected (stop normal flow)

            bool isFirstLogin = false;

            if (userType == "S") // Student
            {

                isFirstLogin = LoginDao.CheckFirstLoginStudent("spCheckFirstLogin_Student", username);

            }
            else if (userType == "E") // Employee
            {
                isFirstLogin = LoginDao.CheckFirstLoginEmployee("spCheckFirstLogin_Employee", username);
            }
            else if (userType == "I") // Institution
            {
                isFirstLogin = LoginDao.CheckFirstLoginInstitution("spCheckFirstLogin_Institution", username);
            }

            if (isFirstLogin)
            {
                // Store username & type for ForgotPassword.aspx
                Session["FirstLoginUser"] = username;
                Session["FirstLoginType"] = userType;

                Response.Redirect("ForgotPassword.aspx");
                return true; // stop further login process
            }

            return false;
        }

        private string GetClientIp()
        {
            string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ip))
            {
                // X-Forwarded-For may contain multiple IPs, take the first one
                string[] addresses = ip.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0].Trim();
                }
            }

            ip = Request.ServerVariables["REMOTE_ADDR"];

            // Replace localhost IPv6 (::1) or IPv4 (127.0.0.1) with local machine IP
            if (ip == "::1" || ip == "127.0.0.1")
            {
                string hostName = System.Net.Dns.GetHostName();
                ip = System.Net.Dns.GetHostEntry(hostName)
                        .AddressList
                        .FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?
                        .ToString();
            }

            return ip;
        }
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string userType = hdnUserType.Value; // 'I', 'E', or 'S'
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string ip = GetClientIp();




            // STEP 1: CAPTCHA VALIDATION (Only for Institution/Employee)
            if (userType == "I" || userType == "E")
            {
                if (Session["Captcha"] == null || txtCaptcha.Text.Trim() != Session["Captcha"].ToString())
                {
                    lblCaptchaError.Text = "Invalid CAPTCHA. Please try again.";
                    LoadCaptcha();
                    return;
                }
            }

            // STEP 2: VALIDATE USERNAME AND PASSWORD INPUT
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Enter username and password.";
                return;
            }

            try
            {
                // STEP 3: FETCH USER ROW
                DataTable dt = LoginDao.GetUserLogin(userType, username, ip);

                if (dt == null || dt.Rows.Count == 0)
                {
                    lblError.Text = "Invalid Username or Password.";
                    if (userType != "S") LoadCaptcha();
                    return;
                }

                DataRow dr = dt.Rows[0];

                // STEP 4: ACCOUNT ACTIVE CHECK
                bool isActive = Convert.ToBoolean(dr["Is_Active"]);
                if (!isActive)
                {
                    lblError.Text = "Your account is not active.";
                    return;
                }

                // STEP 5: IP CHECK FOR COLLEGE LOGIN
                  if (userType == "I")
                      {
                          string registeredIp = dr["IP"]?.ToString();
                          string currentIp = ip; // hardcoded for testing; replace with Request.UserHostAddress in production

                          if (!string.Equals(currentIp, registeredIp))
                          {
                              lblError.Text = "Login denied. IP address is not whitelisted.";
                              return;
                          }
                      }

                // STEP 6: PASSWORD HASH VALIDATION
                byte[] storedHash = (byte[])dr["PasswordHash"];
                byte[] storedSalt = (byte[])dr["PasswordSalt"];
                byte[] enteredHash = ComputeHash(password, storedSalt);

                if (!storedHash.SequenceEqual(enteredHash))
                {
                    lblError.Text = "Invalid Username or Password.";
                    if (userType != "S") LoadCaptcha();
                    return;
                }
                // ✅ STEP 6a: FIRST LOGIN CHECK AFTER CREDENTIALS VERIFIED
                bool isFirstLogin = CheckFirstTimeLogin(userType, username);
                if (isFirstLogin)
                {
                    Session["FirstLoginUser"] = username;
                    Session["FirstLoginType"] = userType;
                    Response.Redirect("ForgotPassword.aspx");
                    return; // stop here
                }
                // STEP 7: SUCCESS — SET SESSION VALUES

                Session["UserType"] = userType;

                if (userType == "I")
                {
                    string institutionCode = LoginDao.GetInstitutionCodeByLoginName(username);

                    if (!string.IsNullOrEmpty(institutionCode))
                    {
                        Session["CollegeCode"] = institutionCode;

                        // Optional: Get institution name and principal name
                        DataTable instDetails = LoginDao.GetInstitutionDetails(institutionCode);
                        if (instDetails != null && instDetails.Rows.Count > 0)
                        {
                            Session["CollegeName"] = instDetails.Rows[0]["Inst_Name"].ToString();
                            Session["username"] = instDetails.Rows[0]["Principal_Name"].ToString();
                            Session["Institute_Type"] = Convert.ToInt32(instDetails.Rows[0]["Institute_Type"]);
                            // from DB

                        }
                        else
                        {
                            Session["CollegeName"] = dr["Login_Name"].ToString(); // fallback
                        }
                    }
                    else
                    {
                        lblError.Text = "Institution not found.";
                        return;
                    }


                }
                else if (userType == "E")
                {

                    string institutionCode = LoginDao.GetInstitutionCodeByEmployeeLogin(username);

                    if (!string.IsNullOrEmpty(institutionCode))
                    {
                        Session["CollegeCode"] = institutionCode;

                        // Optional: Get institution name and principal name
                        DataTable studDetails = LoginDao.GetEmployeeDetails(institutionCode, username);
                        if (studDetails != null && studDetails.Rows.Count > 0)
                        {
                            Session["CollegeName"] = studDetails.Rows[0]["Inst_Name"].ToString();
                            Session["username"] = studDetails.Rows[0]["Name"].ToString();
                            Session["Institute_Type"] = Convert.ToInt32(studDetails.Rows[0]["Designation_ID"]);
                            // from DB

                        }
                        else
                        {
                            Session["CollegeName"] = dr["Login_Name"].ToString(); // fallback
                        }
                    }
                    else
                    {
                        lblError.Text = "Institution not found.";
                        return;
                    }




                }
                else if (userType == "S")
                {
                    Session["StudentName"] = dr["Login_Name"].ToString();

                }

                // STEP 8: REDIRECT TO DASHBOARD
                Response.Redirect("Dashboard.aspx");
            }
            catch (Exception ex)
            {
                lblError.Text = "Error: " + ex.Message;
            }
        }





        protected void btnStudentLogin_Click(object sender, EventArgs e)
        {
            string userType = hdnUserType.Value; // 'I', 'E', or 'S'
            string username = txtStudentReg.Text.Trim();
            string password = txtStudentPassword.Text.Trim();

            string ip = Request.UserHostAddress;
            // Only run student login validation for Student tab
            if (userType != "S")
            {
                return; // Do nothing if it's not student login
            }

            // STEP 2: VALIDATE USERNAME AND PASSWORD INPUT
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Enter correct register number and password.";
                return;
            }
            try
            {
                // STEP 3: FETCH USER ROW
                DataTable dt = LoginDao.GetUserLogin(userType, username, ip);

                if (dt == null || dt.Rows.Count == 0)
                {
                    lblError.Text = "Invalid Username or Password.";
                    if (userType != "S") LoadCaptcha();
                    return;
                }

                DataRow dr = dt.Rows[0];

                // STEP 4: ACCOUNT ACTIVE CHECK
                bool isActive = Convert.ToBoolean(dr["Is_Active"]);
                if (!isActive)
                {
                    lblError.Text = "Your account is not active.";
                    return;
                }


                // STEP 5: PASSWORD HASH VALIDATION
                byte[] storedHash = (byte[])dr["PasswordHash"];
                byte[] storedSalt = (byte[])dr["PasswordSalt"];
                byte[] enteredHash = ComputeHash(password, storedSalt);

                if (!storedHash.SequenceEqual(enteredHash))
                {
                    lblError.Text = "Invalid Username or Password.";
                    if (userType != "S") LoadCaptcha();
                    return;
                }

                // ✅ STEP 6: FIRST LOGIN CHECK
                bool isFirstLogin = LoginDao.CheckFirstLoginStudent("spCheckFirstLogin_Student", username);
                if (isFirstLogin)
                {
                    // Session["FirstLoginType"] = hdnUserType.Value;
                    Session["FirstLoginUser"] = username;
                    Session["FirstLoginType"] = userType;
                    Response.Redirect("ForgotPassword.aspx");
                    return; // stop normal login
                }


                // STEP 7: SUCCESS — SET SESSION VALUES
                Session["UserType"] = userType;
                if (userType == "S")
                {
                    string institutionCode = LoginDao.GetInstitutionCodeByStudentEnrollment(username);

                    if (!string.IsNullOrEmpty(institutionCode))
                    {
                        Session["CollegeCode"] = institutionCode;

                        // Optional: Get institution name and principal name
                        DataTable studDetails = LoginDao.GetStudentDetails(institutionCode, username);
                        if (studDetails != null && studDetails.Rows.Count > 0)
                        {
                            Session["CollegeName"] = studDetails.Rows[0]["Inst_Name"].ToString();
                            Session["username"] = studDetails.Rows[0]["Name"].ToString();
                            Session["Institute_Type"] = Convert.ToInt32(studDetails.Rows[0]["Institute_Type"]);
                            // from DB

                        }
                        else
                        {
                            Session["CollegeName"] = dr["Login_Name"].ToString(); // fallback
                        }
                    }
                    else
                    {
                        lblError.Text = "STUDENT not found.";
                        return;
                    }


                }


                // STEP 8: REDIRECT TO DASHBOARD
                Response.Redirect("Dashboard.aspx");
            }
            catch (Exception ex)
            {
                lblError.Text = "Error: " + ex.Message;
            }


        }



    }



}

