using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BTEDiploma.Helper
{
    public class RegisterDao
    {
        public static DataTable GetUserTypes()
        {
            return SQLHelper.ExecuteStoredProcedure("spGetUserTypes",null);
        }

    }
}