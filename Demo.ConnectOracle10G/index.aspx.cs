using Oracle.ManagedDataAccess.Client;
using System;
using System.Configuration;
using System.Data;

namespace ConnectOracle10G
{
    public partial class index : System.Web.UI.Page
    {
        DataTable DT;
        int Data = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            GetClients();
        }

        private void GetClients()
        {
            CallOracle(out Data, GetConfigKey("AllClients"));
            if (Data == 1)
            {
                lblData.Text = DT.Rows.Count.ToString() + "<br />";
                foreach (DataRow row in DT.Rows)
                {
                    lblData.Text += row["CLIENT_NM"].ToString() + "<br/>";
                }
            }
            else
                lblData.Text = "No Data!";
        }

        private void CallOracle(out int Data, string Query)
        {
            string ConStr = ConfigurationManager.ConnectionStrings["OrConStr"].ToString();
            OracleConnection OrCon = new OracleConnection();
            OrCon.ConnectionString = ConStr;
            OrCon.Open();
            OracleCommand OrCommand = new OracleCommand(Query, OrCon);
            OrCommand.CommandType = CommandType.Text;
            OracleDataReader OrDR = OrCommand.ExecuteReader();
            if (OrDR.HasRows)
            {
                Data = 1;
                DT = new DataTable();
                DT.Load(OrDR);                
            }
            else
                Data = 0;
            OrCon.Close();
            OrCon.Dispose();
        }

        private string GetConfigKey(string Key)
        {
            return ConfigurationManager.AppSettings[Key].ToString();
        }
    }
}