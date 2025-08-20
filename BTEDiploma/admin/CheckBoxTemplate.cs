using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using BTEDiploma.Helper;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace BTEDiploma.Helper
{
    public class CheckBoxTemplate : ITemplate
    {
        string _columnName;
        public CheckBoxTemplate(string columnName) { _columnName = columnName; }

        public void InstantiateIn(Control container)
        {
            CheckBox chk = new CheckBox();
            chk.ID = "chkApproved";
            chk.DataBinding += (sender, e) =>
            {
                CheckBox cb = (CheckBox)sender;
                GridViewRow row = (GridViewRow)cb.NamingContainer;
                object dataValue = DataBinder.Eval(row.DataItem, _columnName);
                if (dataValue != DBNull.Value)
                    cb.Checked = Convert.ToBoolean(dataValue);
            };
            container.Controls.Add(chk);
        }
    }

}
