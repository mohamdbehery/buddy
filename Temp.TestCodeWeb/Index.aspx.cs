using Buddy.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TestCodeWeb
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Helper helper = new Helper();
            DataTable dt = new DataTable();
            helper.GetDTFromSQLDB_SP("Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=AdventureWorks2014;Data Source=EPESMALW006D", "spTest", null, out dt);

            Console.ReadLine();
        }
    }
}