using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;

namespace ActiveDirectory
{
    public partial class index : System.Web.UI.Page
    {
        string MyDomainPath = "DC=local,DC=ebseg,DC=com";
        string ServerName = "";
        string RES = "";
        List<ADUserInfo> UserInfo = new List<ADUserInfo>();

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            lblStatus.Text = GetUserData(txtUN.Text.Trim(), txtPass.Text.Trim());
        }

        private string ValidateUser(string UserName, string Password)
        {
            try
            {
                using (DirectoryEntry DE = CreateDE(MyDomainPath, UserName, Password))
                {
                    using (DirectorySearcher DS = new DirectorySearcher(DE))
                    {
                        DS.Filter = "(samaccountname=" + UserName + ")";
                        DS.PropertiesToLoad.Add("displayname");
                        SearchResult SR = DS.FindOne();
                        if (SR != null)
                        {
                            if (SR.Properties["displayname"].Count == 1)
                            {
                                RES = (string)SR.Properties["displayname"][0];
                            }
                            return RES + " is OK!";
                        }
                    }
                }
                return "Not valid!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
                
        private string GetUserData(string UserName, string Password)
        {
            try
            {
                DirectoryEntry DE = CreateDE(MyDomainPath, UserName, Password);
                DirectorySearcher DS = new DirectorySearcher(DE);
                DS.Filter = "(samaccountname=" + UserName + ")";
                SearchResult result = DS.FindOne();
                if (result != null)
                {
                    ResultPropertyCollection Props = result.Properties;
                    foreach (string Prop in Props.PropertyNames)
                    {
                        foreach (object PropVal in Props[Prop])
                        {
                            RES += Prop + " : " + PropVal.ToString() + "<br />";
                        }
                    }
                    return RES;
                }
                else
                {
                    return "User not found!";
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public string GetGroupUsers(string GroupName)
        {
            PrincipalContext PC = new PrincipalContext(ContextType.Domain);
            GroupPrincipal GP = GroupPrincipal.FindByIdentity(PC, IdentityType.SamAccountName, GroupName);
            foreach (var Member in GP.Members)
            {
                RES += Member.Name + "<br />";
            }
            return RES;
        }
        
        public string ChangePassword(string UserName, string Password, string NewPassword)
        {
            try
            {
                DirectoryEntry DE = CreateDE(MyDomainPath, UserName, Password);
                DirectorySearcher DS = new DirectorySearcher(DE);
                DS.Filter = "(SAMAccountName=" + UserName + ")";
                SearchResult result = DS.FindOne();
                DirectoryEntry user = result.GetDirectoryEntry();
                if (user != null)
                {
                    user.Invoke("ChangePassword", new object[] { Password, NewPassword });
                    user.CommitChanges();
                    user.Close();
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "Done!";
        }

        private DirectoryEntry CreateDE(string DomainPath, string UserName, string Password)
        {
            DirectoryEntry DE = new DirectoryEntry(DomainPath);
            if (ServerName != string.Empty)
                DE.Path = "LDAP://" + ServerName + "/" + DomainPath;
            else
                DE.Path = "LDAP://" + DomainPath;

            if (UserName != string.Empty)
                DE.Username = UserName;

            if (Password != string.Empty)
                DE.Password = Password;

            DE.AuthenticationType = AuthenticationTypes.Secure;
            return DE;
        }

        private string GetADDomain()
        {
            using (DirectoryEntry deRoot = new DirectoryEntry("LDAP://RootDSE"))
            {
                if (deRoot.Properties["defaultNamingContext"] != null)
                {
                    return deRoot.Properties["defaultNamingContext"].Value.ToString();
                }
                else
                    return "";
            }
        }

        private string GetADCurrentUser()
        {
            using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain))
            {
                UserPrincipal currentUser = UserPrincipal.Current;
                return currentUser.DistinguishedName;
            }
        }
    }
}