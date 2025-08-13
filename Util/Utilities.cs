using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Util
{
    public static class Utilities
    {
        public static string getIpadress()
        {
            string IpAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (IpAddress == null)
            {
                IpAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            return IpAddress;
        }
        public static void ClearInputs(ControlCollection ctrls)
        {
            foreach (Control ctrl in ctrls)
            {
                if (ctrl is TextBox)
                    ((TextBox)ctrl).Text = string.Empty;
                else if (ctrl is Label)
                    ((Label)ctrl).Text = string.Empty;
                else if (ctrl is DropDownList)
                    ((DropDownList)ctrl).ClearSelection();
                else if (ctrl is ListBox)
                    ((ListBox)ctrl).ClearSelection();
                ClearInputs(ctrl.Controls);
            }
        }

        public static string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyz1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public static void DisableCache()
        {
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache); //Cache-Control : no-cache, Pragma : no-cache
            HttpContext.Current.Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1)); //Expires : date time
            HttpContext.Current.Response.Cache.SetNoStore(); //Cache-Control :  no-store
            HttpContext.Current.Response.Cache.SetProxyMaxAge(new TimeSpan(0, 0, 0)); //Cache-Control: s-maxage=0
            HttpContext.Current.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Current.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);//Cache-Control:  must-revalidate
        }

        public static String PasswordHashing(String password)
        {
            byte[] hash = SHA256_GetHash(password);
            string result = SHA256_GetBase64String(hash);
            String hashedPassword = result;
            return hashedPassword;
        }
        static byte[] SHA256_GetHash(string str)
        {
            byte[] byteArr = GetBytes(str);
            SHA256Managed sha256 = new SHA256Managed();
            byte[] hash = sha256.ComputeHash(byteArr);
            return hash;
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        static string SHA256_GetBase64String(byte[] hash)
        {
            string result = "";
            result = Convert.ToBase64String(hash);
            return result;
        }
        public static bool checkpasswordchars(string password)
        {

            bool spl = false, num = false, alpha = false;
            foreach (char ch in password)
            {
                if (Char.IsDigit(ch))
                {

                    num = true;

                }
                else if (Char.IsLetter(ch))
                {
                    alpha = true;

                }

                else if (!Char.IsLetterOrDigit(ch))
                {
                    spl = true;


                }


            }

            if (num && alpha && spl)
            {

                return true;

            }
            else
            {

                return false;

            }

        }


        public static bool checkdata(bool marital_status, bool spouse_nm, bool pgdegree, bool pg_degree_nm, bool pg_dip, bool pgdip_name, bool mbbs_dgr_kar, bool oth_dist, bool disttric, bool talukadd)
        {
            bool ret = false;
            if (marital_status == true && spouse_nm == true && pgdegree == true && pg_degree_nm == true && pg_dip == true && pgdip_name == true && mbbs_dgr_kar == true && oth_dist == true && disttric == true && talukadd == true)
            {

                ret = true;


            }
            return (ret);
        }



        //public static bool update_img(string nbeid, FileUpload uploadctrl, string ColName, string entrydate)
        //{
        //    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Cet2015OnlineApplicationConnectionString"].ConnectionString);
        //    try
        //    {

        //        int img_photo = uploadctrl.PostedFile.ContentLength;
        //        byte[] ImageName_p = new byte[img_photo];
        //        uploadctrl.PostedFile.InputStream.Read(ImageName_p, 0, img_photo);

        //        //           String Image_p = appno.ToString() + ".jpg";

        //        //            uploadctrl.SaveAs(Server.MapPath("Photos/" + Image_p));
        //        string Query = "update pget_application set " + ColName + " = @Photo ,upload_date  = @upload_date where testing_id =@testing_id";
        //        SqlCommand cmd = new SqlCommand(Query);
        //        cmd.Connection = con;
        //        cmd.Parameters.AddWithValue("@Photo", ImageName_p);
        //        cmd.Parameters.AddWithValue("@testing_id", nbeid);
        //        cmd.Parameters.AddWithValue("@upload_date", entrydate);
        //        // cmd.Parameters.AddWithValue("@ImagePath", "Photos/" + Image_p);
        //        cmd.CommandType = CommandType.Text;
        //        con.Open();
        //        int ret = cmd.ExecuteNonQuery();
        //        con.Close();

        //        if (ret > 0)
        //        {
        //            return (true);
        //        }
        //        else
        //        {
        //            return (false);
        //        }

        //    }
        //    catch (Exception)
        //    {

        //        return (false);
        //    }

        //}






        //public static DateTime Getdate(string date, string uid)
        //{

        //    try
        //    {

        //        string[] aDate = date.Split('/', '-', '.', ' ', ':');
        //        if (aDate.Length < 3)
        //            throw new Exception("Date value is not properly delimited." + date);

        //        /*
        //                    short day = Convert.ToInt16(date.Substring(0, 2));
        //                    short mon = Convert.ToInt16(date.Substring(3, 2));
        //                    short yr = Convert.ToInt16(date.Substring(6, 4));
        //        */

        //        short day = Convert.ToInt16(aDate[0]);
        //        short mon = Convert.ToInt16(aDate[1]);
        //        short yr = Convert.ToInt16(aDate[2]);


        //        short hh = 0, mm = 0, ss = 0, fff = 0;


        //        if (aDate.Length > 3 && (aDate.Length <= 6 || aDate.Length <= 7))
        //        {
        //            hh = Convert.ToInt16(aDate[3]);
        //            mm = Convert.ToInt16(aDate[4]);
        //            ss = Convert.ToInt16(aDate[5]);

        //        }

        //        if (aDate.Length >= 7)
        //            fff = Convert.ToInt16(aDate[6]);

        //        /*            if(date.Length>10)
        //                    {
        //                        hh = Convert.ToInt16(date.Substring(11, 2));
        //                        mm = Convert.ToInt16(date.Substring(14, 2));
        //                        ss = Convert.ToInt16(date.Substring(17, 2));

        //                    } */

        //        //            if(date.Length>19 && date.Length==23)
        //        //                fff = Convert.ToInt16(date.Substring(20, 3));

        //        DateTime dob = new DateTime(yr, mon, day, hh, mm, ss, fff);


        //        return dob;
        //    }
        //    catch (Exception ex)
        //    {

        //        Write_Error(uid, "", ex.Message, "Getdate," + date);
        //        return DateTime.MinValue;
        //    }
        //}


        public static string CheckForSQLInjectionName(string strInput)
        {
            string sql_injection_characters_present = string.Empty;
            string[] badChars = null;

            badChars = new string[] {"<script>alert(123)</script>","script","alert",
        "truncate",		"database",		"table",		"column",		"select",		"drop",	 "alter",	"update",		"grant",
        "insert",		"delete",		";",		":",		"SQL",		"Sql",		"sql",		"--",		"'",		"xp_","sp_",
        "=",		"<",		">",		"&",		"~",		"$",		"(",		")",		"%",				"+",
                "\\",		"[",		"]",		"{",		"}",		"`",		"!",		"^",		"\"",		"?",
        "*",		"|",		"#",  "\"" ,";", ":", "--", "'",  "%", "+"	};

        //    badChars = new string[] {"<script>alert(123)</script>","script","alert",
        //"truncate",		"database",		"table",		"column",		"select",		"drop",		"update",		"grant",
        //"insert",		"delete",		";",		":",		"SQL",		"Sql",		"sql",		"--",		"'",		"xp_","sp_",
        //"=",		"<",		">",		"&",		"~",		"$",		"%", "\\",		"[",		"]",		"{",		"}",		"`",		"!",		"^",		"\"",		"?",
        //"*",		"|",		"#",  "\"" ,";", ":", "--", "'",  "%"	};

            for (int i = 0; i <= badChars.Length - 1; i++)
            {
                if (strInput.Trim().Contains(badChars[i]))
                {
                    sql_injection_characters_present = badChars[i];
                    break; // TODO: might not be correct. Was : Exit For

                }
            }
            return sql_injection_characters_present;
        }

        //public static IEnumerable<T> GetChildControls<T>(this Control control) where T : Control
        //{
        //    var children = control.Controls.OfType<T>();
        //    return children.SelectMany(c => GetChildControls<T>(c)).Concat(children);
        //}

        public static IEnumerable<T> FindControls<T>(this Control control, bool recurse) where T : Control
        {
            List<T> found = new List<T>();
            Action<Control> search = null;
            search = ctrl =>
            {
                foreach (Control child in ctrl.Controls)
                {
                    if (typeof(T).IsAssignableFrom(child.GetType()))
                    {
                        found.Add((T)child);
                    }
                    if (recurse)
                    {
                        search(child);
                    }
                }
            };
            search(control);
            return found;
        }

        public static string CheckSQLInjection_Controls(IEnumerable<TextBox> allTextBoxes)
        {
            String sret = String.Empty;
            foreach (TextBox ctrl in allTextBoxes)
            {
                //CheckSQLInjection_Controls(ctrl);
                if (ctrl is TextBox)
                {
                    if (ctrl.ClientID.ToString().Contains("_ReportViewer1"))
                        continue;
                    sret = CheckForSQLInjectionName(((TextBox)ctrl).Text.ToLower());
                    if (sret != string.Empty)
                    {

                        ((TextBox)ctrl).Focus();

                        return (sret);
                    }
                }
            }
            return (sret);
        }
        public static string CheckSQLInjection_Controls(IEnumerable<DropDownList> allDDLS)
        {
            String sret = String.Empty;
            foreach (DropDownList ctrl in allDDLS)
            {
                //CheckSQLInjection_Controls(ctrl);
                if (ctrl is DropDownList)
                {
                    sret = CheckForSQLInjectionName(((DropDownList)ctrl).SelectedValue.ToLower());
                    if (sret != string.Empty)
                    {

                        ((DropDownList)ctrl).Focus();

                        return (sret);
                    }
                }
            }
            return (sret);
        }



        public static string GetApplication_SecurityCode(decimal applnno)
        {
            //string ret = "lkjasldkfjlkasd fjlkaj sdlkja sdlfk";     /// Test purpose
            string ret = "";                                                      /// 
            // ret = s_hex_md5(applnno+name+dob+category+entry_date+native_state);       /// native_state(address)
            //dob yyyy-MM-dd
            string password = "";
            ret = s_hex_md5(password);
            return (ret);
        }



        public static String s_hex_md5(String originalPassword)
        {

            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            Byte[] hashedbytes = md5.ComputeHash(encoder.GetBytes(originalPassword));

            StringBuilder ret = new StringBuilder();

            for (int i = 0; i < hashedbytes.Length; i++)
            {

                ret.Append(hashedbytes[i].ToString("x2"));

            }

            return ret.ToString();
            //System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            //System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            //Byte[] hashedbytes = md5.ComputeHash(encoder.GetBytes(originalPassword));


            //        return BitConverter.ToString(hashedbytes).Replace("-", "").ToLower();


        }

        public static bool SendEmail(string EmailID, string Subject, String Message)
        {
            //    //bool email = false;

            //    //emailWS.Service emailservice = new emailWS.Service();

            //    //email = emailservice.SendMail("app4", "App%4$01", EmailID, Subject, Message);
            return false;
        }
        public static bool SendEmailCC(string sendto, string ccto, string subject, String Message)
        {
            bool email = false;
            //    //emailWS.Service emailservice = new emailWS.Service();
            //    //email = emailservice.SendMailCCBCC("app4", "App%4$01", sendto, ccto, "", subject, Message);
            return email;
        }


        public static bool loginstatus(String login)
        {
            //  loginstatus = false;
            if (login == "")
            {
                return false;

            }
            else
            {
                return true;
            }
        }

        public static int ErrorMessage(String ErrMessage, String connString)
        {
            SqlConnection conn = new SqlConnection(connString);
            int ErrCd = 0;

            try
            {
                SqlCommand cmdMaxError = new SqlCommand("select max(ErrCd) as ErrCd from ErrorDesc", conn);
                conn.Open();
                object objErr = cmdMaxError.ExecuteScalar();
                if (System.Convert.IsDBNull(objErr))
                    ErrCd = 1;
                else
                    ErrCd = int.Parse(objErr.ToString()) + 1;

                SqlCommand cmdInsert = new SqlCommand("Insert into ErrorDesc (ErrCd, Description) values(@ErrCd,@Description)", conn);
                cmdInsert.Parameters.AddWithValue("@ErrCd", ErrCd);
                cmdInsert.Parameters.AddWithValue("@Description", ErrMessage.Trim());
                cmdInsert.ExecuteNonQuery();

                conn.Close();

                return (ErrCd);
            }
            catch (Exception exc)
            {
                return (0);
            }
        }

        public static bool IsNaturalNumber(String strNumber)
        {
            Regex objNotNaturalPattern = new Regex("[^0-9]");
            Regex objNaturalPattern = new Regex("0*[1-9][0-9]*");
            return !objNotNaturalPattern.IsMatch(strNumber) &&
                    objNaturalPattern.IsMatch(strNumber);
        }

        // Function to test for Positive Integers with zero inclusive
        public static bool IsWholeNumber(String strNumber)
        {
            Regex objNotWholePattern = new Regex("[^0-9]");
            return !objNotWholePattern.IsMatch(strNumber);
        }

        // Function to Test for Integers both Positive & Negative
        public static bool IsInteger(String strNumber)
        {
            Regex objNotIntPattern = new Regex("[^0-9-]");
            Regex objIntPattern = new Regex("^-[0-9]+$|^[0-9]+$");
            return !objNotIntPattern.IsMatch(strNumber) &&
                    objIntPattern.IsMatch(strNumber);
        }

        // Function to Test for Positive Number both Integer & Real
        public static bool IsPositiveNumber(String strNumber)
        {
            Regex objNotPositivePattern = new Regex("[^0-9.]");
            Regex objPositivePattern = new Regex("^[.][0-9]+$|[0-9]*[.]*[0-9]+$");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            return !objNotPositivePattern.IsMatch(strNumber) &&
                    objPositivePattern.IsMatch(strNumber) &&
                    !objTwoDotPattern.IsMatch(strNumber);
        }

        // Function to test whether the string is valid number or not
        public static bool IsNumber(String strNumber)
        {
            Regex objNotNumberPattern = new Regex("[^0-9.-]");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            String strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");

            return !objNotNumberPattern.IsMatch(strNumber) &&
                    !objTwoDotPattern.IsMatch(strNumber) &&
                    !objTwoMinusPattern.IsMatch(strNumber) &&
                    objNumberPattern.IsMatch(strNumber);
        }

        // Function To test for Alphabets.
        public static bool IsAlpha(String strToCheck)
        {
            Regex objAlphaPattern = new Regex("[^a-zA-Z]|[.]|[^a-zA-Z]");
            return !objAlphaPattern.IsMatch(strToCheck);
        }

        //public static bool IsAlphaDot(String strToCheck)
        //{
        //    Regex objAlphaPattern = new Regex("[^a-zA-Z]");            
        //    return !objAlphaPattern.IsMatch(strToCheck);
        //}

        // Function to Check for AlphaNumeric.
        public static bool IsAlphaNumeric(String strToCheck)
        {
            Regex objAlphaNumericPattern = new Regex("[^a-zA-Z0-9]");
            return !objAlphaNumericPattern.IsMatch(strToCheck);
        }

        //Function to Check for valid date (d/mm/yyyy) in the range
        //01/01/0001 to 31/12/99999
        //public static bool IsDate(String strToCheck)
        //{
        //    Regex objDatePattern = new Regex("^(?:(?:0?[1-9]|1\\d|2[0-9])(\\/|-)(?:0?[1-9]|1[0-2]))(\\/|-)(?:[1-9]\\d\\d\\d|\\d[1-9]\\d\\d|\\d\\d[1-9]\\d|\\d\\d\\d[1-9])$|^(?:(?:31(\\/|-)(?:0?[13578]|1[02]))|(?:(?:29|30)(\\/|-)(?:0?[1,3-9]|1[0-2])))(\\/|-)(?:[1-9]\\d\\d\\d|\\d[1-9]\\d\\d|\\d\\d[1-9]\\d|\\d\\d\\d[1-9])$|^(29(\\/|-)0?2)(\\/|-)(?:(?:0[48]00|[13579][26]00|[2468][048]00)|(?:\\d\\d)?(?:0[48]|[2468][048]|[13579][26]))$ ");
        //    return objDatePattern.IsMatch(strToCheck);
        //}

        public static bool IsDate(String strToCheck)
        {
            DateTime dt = new DateTime();
            bool result = false;
            try
            {
                dt = Convert.ToDateTime(strToCheck);
                result = true;
            }
            catch
            {
                result = false;
            }
            if (result)
            {
                if (dt.Month == 2 && dt.Day == 29)
                    result = dt.Year % 4 == 0 ? true : false;
                else
                    result = (new Regex("^(?:(?:0?[1-9]|1\\d|2[0-8])(\\/|-)(?:0?[1-9]|1[0-2]))(\\/|-)(?:[1-9]\\d\\d\\d|\\d[1-9]\\d\\d|\\d\\d[1-9]\\d|\\d\\d\\d[1-9])$|^(?:(?:31(\\/|-)(?:0?[13578]|1[02]))|(?:(?:29|30)(\\/|-)(?:0?[1,3-9]|1[0-2])))(\\/|-)(?:[1-9]\\d\\d\\d|\\d[1-9]\\d\\d|\\d\\d[1-9]\\d|\\d\\d\\d[1-9])$|^(29(\\/|-)0?2)(\\/|-)(?:(?:0[48]00|[13579][26]00|[2468][048]00)|(?:\\d\\d)?(?:0[48]|[2468][048]|[13579][26]))$ ")).IsMatch(strToCheck);
            }
            return result;
        }

        //Function to check valid filenames with doc extension
        //public static bool IsDocfile(String strToCheck)
        //{
        //Regex objDocfilePattern=new Regex("^([a-zA-Z]\\:|\\)\\([^\\]+\\)*[^\\/:*?"<;>;|]+\.DOC(l)?$");
        //return objDocfilePattern.IsMatch(strToCheck);
        //}
        public static String convt(String txt)
        {
            String newtext;

            StringBuilder sb = new StringBuilder(txt);

            Console.WriteLine();


            sb.Replace("'", "`");
            sb.Replace("--", "-");


            sb.Replace("<", "");
            sb.Replace('\\', '`');

            sb.Replace("/>", "");
            sb.Replace(")", "");
            sb.Replace("(", "");
            sb.Replace("#", "");
            sb.Replace("&", "");
            sb.Replace(";", "");
            sb.Replace('"', '`');




            newtext = txt;
            return sb.ToString();
        }


        public static bool isvaliddate(String txt)
        {
            bool val;
            val = false;
            //isvaliddate = False;
            String txt1;
            DateTime txtd;

            if (txt.Length > 0)
            {
                try
                {
                    CultureInfo culture = new CultureInfo("dd/MM/YYYY");

                    txt1 = txt.Substring(3, 2) + "/" + txt.Substring(0, 2) + "/" + txt.Substring(6, 4);
                    txtd = Convert.ToDateTime(txt1, culture);
                    txt1 = txtd.ToString();
                    val = true;

                    return val;
                }

                catch (Exception e)
                {
                }
            }
            else
            {

                return val;
            }

            return val;
        }

        public static bool ValidateDate(String date, String format)
        {
            try
            {
                System.Globalization.DateTimeFormatInfo dtfi = new
                System.Globalization.DateTimeFormatInfo();
                dtfi.ShortDatePattern = format;
                DateTime dt = DateTime.ParseExact(date, "d", dtfi);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }



    }
}
