using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveDirectory
{
    public class ADUserInfo
    {

        #region private ctor

        private ADUserInfo(DirectoryEntry directoryUser)
        {
            String domainAddress = string.Empty;
            String domainName = string.Empty;
            FirstName = GetPropertyValue(directoryUser, ADProperties.FIRSTNAME);
            MiddleName = GetPropertyValue(directoryUser, ADProperties.MIDDLENAME);
            LastName = GetPropertyValue(directoryUser, ADProperties.LASTNAME);
            LoginName = GetPropertyValue(directoryUser, ADProperties.LOGINNAME);
            PwdLastSet = GetPropertyValue(directoryUser, ADProperties.PWDLASTSET);
            String userPrincipalName = GetPropertyValue(directoryUser, ADProperties.USERPRINCIPALNAME);
            if (!string.IsNullOrEmpty(userPrincipalName))
                domainAddress = userPrincipalName.Split('@')[1];
            if (!string.IsNullOrEmpty(domainAddress))
                domainName = domainAddress.Split('.').First();
            LoginNameWithDomain = String.Format(@"{0}\{1}", domainName, LoginName);
            StreetAddress = GetPropertyValue(directoryUser, ADProperties.STREETADDRESS);
            City = GetPropertyValue(directoryUser, ADProperties.CITY);
            State = GetPropertyValue(directoryUser, ADProperties.STATE);
            PostalCode = GetPropertyValue(directoryUser, ADProperties.POSTALCODE);
            Country = GetPropertyValue(directoryUser, ADProperties.COUNTRY);
            Company = GetPropertyValue(directoryUser, ADProperties.COMPANY);
            Department = GetPropertyValue(directoryUser, ADProperties.DEPARTMENT);
            HomePhone = GetPropertyValue(directoryUser, ADProperties.HOMEPHONE);
            Extension = GetPropertyValue(directoryUser, ADProperties.EXTENSION);
            Mobile = GetPropertyValue(directoryUser, ADProperties.MOBILE);
            Fax = GetPropertyValue(directoryUser, ADProperties.FAX);
            EmailAddress = GetPropertyValue(directoryUser, ADProperties.EMAILADDRESS);
            Title = GetPropertyValue(directoryUser, ADProperties.TITLE);
            ManagerName = GetPropertyValue(directoryUser, ADProperties.MANAGER);
            if (!String.IsNullOrEmpty(ManagerName))
            {
                //String[] managerArray = Manager.Split(',');
                //ManagerName = managerArray[0].Replace("CN=", "");
            }
        }

        #endregion

        #region User Properties

        public string Department{get;private set;}
        public string FirstName{get;private set;}
        public string MiddleName{get;private set;}
        public string LastName{get;private set;}
        public string LoginName{get;private set;}
        public string ManagerName { get; private set; }
        public string LoginNameWithDomain{get;private set;}
        public string StreetAddress{get;private set;}
        public string City{get;private set;}
        public string State{get;private set;}
        public string PostalCode{get;private set;}
        public string Country{get;private set;}
        public string HomePhone{get;private set;}
        public string Extension{get;private set;}
        public string Mobile{get;private set;}
        public string Fax{get;private set;}
        public string EmailAddress{get;private set;}
        public string Title{get;private set;}
        public string Company{get;private set;}
        public string PwdLastSet { get; private set; }
        public ADUserInfo Manager
        {
            get
            {
                if (!String.IsNullOrEmpty(ManagerName))
                {
                    //ActiveDirectoryHelper ad = new ActiveDirectoryHelper();
                    //return ad.GetUserByFullName(ManagerName);
                }
                return null;
            }
        }

        #endregion

        private static string GetPropertyValue(DirectoryEntry userDetail, String propertyName)
        {
            if (userDetail.Properties.Contains(propertyName))
                return userDetail.Properties[propertyName][0].ToString();
            return string.Empty;
        }
 
        public static ADUserInfo GetUser(DirectoryEntry directoryUser)
        {
            return new ADUserInfo(directoryUser);
        }
    }
}
