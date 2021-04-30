using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buddy.Utilities.Models
{
    public class MailMessage
    {
        Helper helper = new Helper();
        public MailMessage()
        {
            SMTPClientHost = helper.GetAppKey("SendMailSMTPClientHost");
            SMTPClientPort = Convert.ToInt32(helper.GetAppKey("SendMailSMTPClientPort"));
            IsSSLEnabled = Convert.ToBoolean(Convert.ToInt32(helper.GetAppKey("SendMailIsSSLEnabled")));
            SMTPClientTimeout = Convert.ToInt32(helper.GetAppKey("SendMailSMTPClientTimeout"));
        }
        public string SenderName { get; set; }
        public string SenderMail { get; set; }
        public string SenderPassword { get; set; }
        public string SMTPClientHost { get; private set; }
        public int SMTPClientPort { get; private set; }
        public bool IsSSLEnabled { get; private set; }
        public int SMTPClientTimeout { get; private set; }
        public string RecieverName { get; set; }
        public string[] RecieverMail { get; set; }
        public string[] CCs { get; set; }
        public string MailSubject { get; set; }
        public string[] Attachments { get; set; }
        public string MailBody { get; set; }
    }
}
