using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using BTEDiploma.Helper;


namespace BTEDiploma.Helper
{
    public class StudentEnrollmentViewModel
    {
        public int Student_ID { get; set; }
        public int Student_Enrollment_ID { get; set; }
        public string Student_Name { get; set; }
        public string Father_Name { get; set; }
        public string Gender { get; set; }
        public string CatCode { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Register_Number { get; set; }
        public string Program_Code { get; set; }
        public string Program_Name { get; set; }
        public int Institution_Code { get; set; }
        public int Admission_Type { get; set; }
        public string Admission_Type_Description { get; set; }
        public string Institution_Name { get; set; }
        public string Admission_Year { get; set; }
        public bool Is_Approved_By_ACM { get; set; }
        public String ACM_Approval_Status_Text { get; set; }
    }

}
