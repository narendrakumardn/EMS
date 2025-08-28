using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace BTEDiploma.Helper
{
    public class EmailHelper
    {
        public static bool SendOtpEmail(string toEmail, string otp)
        {
            try
            {

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("narendra.ait@gmail.com"); // use your Gmail

                mail.To.Add(toEmail);
                mail.Subject = "OTP for Password Reset";
                mail.Body = $"Your OTP to reset password is: {otp}";
                mail.IsBodyHtml = false;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("reshmaattavar@gmail.com", "tqmv pjfg ovku zcvh"); // App Password
                smtp.EnableSsl = true;
                smtp.Send(mail);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}