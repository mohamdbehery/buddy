using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;


namespace ConfigEncryption
{
    public partial class AccountStatement : System.Web.UI.Page
    {
        private const string Provider = "RSAProtectedConfigurationProvider";
        private static readonly string[] SectionNames = { "appSettings", "connectionStrings" };


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                Div_EncryptionControl.Visible = false;

            lbl_Setting.Text = ConfigurationManager.AppSettings["qbo.EMCImportTempFolder"];
            lbl_connectionString.Text = ConfigurationManager.ConnectionStrings["qbo.Data.Connection.Default"].ConnectionString;
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            if (Txt_Code.Text == "ns6%WJ%L#U92hGSJ")
            {
                Div_EncryptionControl.Visible = true;
                Div_CodeControls.Visible = false;
            }
        }
        protected void DecryptConnections(object sender, EventArgs e)
        {
            Txt_Msg.Text = UnProtectSection();
        }
        protected void EncryptConnections(object sender, EventArgs e)
        {
            Txt_Msg.Text = ProtectSection();
        }


        public string ProtectSection()
        {
            HttpContext.Current.Response.Write("");
            

            var config =
                WebConfigurationManager.
                    OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);


            foreach (var sectionName in SectionNames)
            {
                var section = config.GetSection(sectionName);

                if (section != null && !section.SectionInformation.IsProtected)
                {
                    section.SectionInformation.ProtectSection(Provider);
                    config.Save();
                }
            }


            
            return "Configuration Section is automatically encrypted";
        }


        public string UnProtectSection()
        {
            HttpContext.Current.Response.Write("");
            var config =
                WebConfigurationManager.
                    OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);

            foreach (var sectionName in SectionNames)
            {
                var section = config.GetSection(sectionName);

                if (section != null && section.SectionInformation.IsProtected)
                {
                    section.SectionInformation.UnprotectSection();
                    config.Save();
                }
            }
            return "Configuration Section is automatically decrypted";

        }


 
    }
}