using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using BTEDiploma.Helper;


namespace BTEDiploma.Helper
{
    public class StudentUploadModel
    {
        public int Institution_Code { get; set; }
        public string Program_Code { get; set; }
        public string AdharNumber { get; set; }
        public string Application_Number { get; set; }
        public string SATS_Number { get; set; }
        public string Name { get; set; }
        public string Father_Name { get; set; }
        public string Mother_Name { get; set; }
        public string Address { get; set; }
        public string PINcode { get; set; }
        public int TalukID { get; set; }
        public string Cat_Code { get; set; }
        public string Gender { get; set; }
        public int Admission_Type { get; set; }
        public DateTime Date_Of_Birth { get; set; }
        public string Mobile_Number { get; set; }
        public string Email_ID { get; set; }
        public bool SNQ { get; set; }
    }

}
