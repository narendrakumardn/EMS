

using System;
using System.Web.UI;
using BTEDiploma.Helper;

namespace BTEDiploma.login
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["FirstLoginUser"] != null && Session["FirstLoginType"] != null)
                {
                    hdnUserType.Value = Session["FirstLoginType"].ToString();
                    txtUsername.Text = Session["FirstLoginUser"].ToString();
                    txtUsername.ReadOnly = true;
                }
                else
                {
                    // If session is missing, redirect to SessionExpired or Login
                    Response.Redirect("~/login/SessionExpired.aspx"); // or "~/Login.aspx"

                }
            }

            // Preserve password fields across postbacks
            txtNewPassword.Attributes["value"] = txtNewPassword.Text;
            txtConfirmPassword.Attributes["value"] = txtConfirmPassword.Text;
        }

        protected void txtNewPassword_TextChanged(object sender, EventArgs e)
        {
            ValidatePassword(txtNewPassword.Text.Trim());
        }

        private bool ValidatePassword(string password)
        {
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$";

            if (string.IsNullOrWhiteSpace(password))
            {
                lblPasswordValidation.Text = "Password cannot be empty.";
                lblPasswordValidation.ForeColor = System.Drawing.Color.Red;
                return false;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(password, pattern))
            {
                lblPasswordValidation.Text = "Password must be at least 8 characters long, include uppercase, lowercase, number, and special character.";
                lblPasswordValidation.ForeColor = System.Drawing.Color.Red;
                return false;
            }
            else
            {
                lblPasswordValidation.Text = "Strong password ✔";
                lblPasswordValidation.ForeColor = System.Drawing.Color.Green;
                return true;
            }
        }

        protected void btnSendOTP_Click(object sender, EventArgs e)
        {
            ResetMessage(); // Clear previous messages

            string username = Session["FirstLoginUser"] != null
                ? Session["FirstLoginUser"].ToString()
                : txtUsername.Text.Trim();

            string usertype = Session["FirstLoginType"]?.ToString() ?? hdnUserType.Value;

            if (ForgotPasswordDao.UserExists(usertype, username))
            {
                string email = ForgotPasswordDao.GetEmailByUsername(usertype, username);

                Random rnd = new Random();
                string generatedOtp = rnd.Next(100000, 999999).ToString();
                Session["GeneratedOTP"] = generatedOtp; // ✅ Store in session

                bool sent = EmailHelper.SendOtpEmail(email, generatedOtp);

                if (sent)
                {
                    ShowMessage("OTP sent to your registered email.", true);
                    pnlOTP.Visible = true;
                }
                else
                {
                    ShowMessage("Failed to send OTP email.", false);
                    pnlOTP.Visible = false;
                }
            }
            else
            {
                ShowMessage("Username not found.", false);
                pnlOTP.Visible = false;
            }
        }

        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            ResetMessage(); // Clear previous messages

            string username = Session["FirstLoginUser"] != null
                ? Session["FirstLoginUser"].ToString()
                : txtUsername.Text.Trim();

            string userType = Session["FirstLoginType"] != null
                ? Session["FirstLoginType"].ToString()
                : hdnUserType.Value;

            string enteredOtp = txtOTP.Text.Trim();
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (newPassword != confirmPassword)
            {
                ShowMessage("New Password and Confirm Password do not match.", false);
                return;
            }

            if (!ValidatePassword(newPassword))
            {
                return;
            }

            string storedOtp = Session["GeneratedOTP"]?.ToString();
            if (enteredOtp == storedOtp)
            {
                bool updated = ForgotPasswordDao.UpdatePassword(userType, username, newPassword);

                if (updated)
                {
                    ForgotPasswordDao.UpdateFirstLoginFlag(username, userType);
                    Session.Remove("FirstLoginUser");
                    Session.Remove("FirstLoginType");
                    Session.Remove("GeneratedOTP");

                    Session["SuccessMessage"] = "✅ Password reset successful. Please log in.";
                    Response.Redirect("~/login/Login.aspx");
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    ShowMessage("Error updating password.", false);
                }
            }
            else
            {
                ShowMessage("Invalid OTP.", false);
            }
        }

        // 🔧 Utility methods
        private void ResetMessage()
        {
            lblMessage.Text = "";
            lblMessage.Visible = false;
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            lblMessage.Text = message;
            lblMessage.ForeColor = isSuccess ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            lblMessage.Visible = true;
        }
    }
}