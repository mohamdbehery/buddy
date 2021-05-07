using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Buddy.Utilities
{
    public class EmailNotifier
    {
        private SmtpClient _client;
        private bool? _useSsl;
        private string _smtpServer;
        private int _smtpPort;
        private string SmtpServer
        {
            get
            {
                if (_smtpServer == null) _smtpServer = ConfigurationManager.AppSettings["qbo.SmtpServer"];
                return (_smtpServer);
            }
        }
        public int SmtpPort
        {
            get
            {

                if (_smtpPort == 0)
                {

                    _smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["qbo.SmtpPort"]);
                }
                return _smtpPort;
            }
        }

        public MailMessage Message { get; set; }
        public EmailNotifier()
        {
            Message = new MailMessage();
            _client = new SmtpClient();
            _client.Host = SmtpServer;
            _client.Port = SmtpPort;
        }

        public static EmailNotifier FromDefault()
        {
            var email = new EmailNotifier
            {
                Message = new MailMessage()
            };
            return email;
        }

        public static EmailNotifier From(string emailAddress, string name = "")
        {
            var email = new EmailNotifier
            {
                Message = { From = new MailAddress(emailAddress, name) }
            };
            return email;
        }

        public EmailNotifier To(string emailAddress, string name)
        {
            if (emailAddress.Contains(";"))
            {
                //email address has semi-colon, try split
                var nameSplit = name.Split(';');
                var addressSplit = emailAddress.Split(';');
                for (int i = 0; i < addressSplit.Length; i++)
                {
                    var currentName = string.Empty;
                    if ((nameSplit.Length - 1) >= i)
                    {
                        currentName = nameSplit[i];
                    }
                    Message.To.Add(new MailAddress(addressSplit[i], currentName));
                }
            }
            else
            {
                Message.To.Add(new MailAddress(emailAddress, name));
            }
            return this;
        }

        public EmailNotifier To(string emailAddress)
        {
            if (emailAddress.Contains(";"))
            {
                foreach (string address in emailAddress.Split(';'))
                {
                    Message.To.Add(new MailAddress(address));
                }
            }
            else
            {
                Message.To.Add(new MailAddress(emailAddress));
            }

            return this;
        }

        public EmailNotifier To(IList<MailAddress> mailAddresses)
        {
            foreach (var address in mailAddresses)
            {
                Message.To.Add(address);
            }
            return this;
        }

        public EmailNotifier CC(string emailAddress, string name = "")
        {
            if (!string.IsNullOrEmpty(emailAddress))
            {
                if (emailAddress.Contains(";"))
                {
                    foreach (string address in emailAddress.Split(';'))
                    {
                        Message.CC.Add(new MailAddress(address, name));
                    }
                }
                else
                {
                    Message.CC.Add(new MailAddress(emailAddress, name));
                }
            }

            return this;
        }

        public EmailNotifier CC(IList<MailAddress> mailAddresses)
        {
            foreach (var address in mailAddresses)
            {
                Message.CC.Add(address);
            }
            return this;
        }

        public EmailNotifier BCC(string emailAddress, string name = "")
        {

            if (emailAddress.Contains(";"))
            {
                foreach (string address in emailAddress.Split(';'))
                {
                    Message.Bcc.Add(new MailAddress(address, name));
                }
            }
            else
            {
                Message.Bcc.Add(new MailAddress(emailAddress, name));
            }
            return this;
        }

        public EmailNotifier BCC(IList<MailAddress> mailAddresses)
        {
            foreach (var address in mailAddresses)
            {
                Message.Bcc.Add(address);
            }
            return this;
        }

        public EmailNotifier ReplyTo(string address)
        {
            Message.ReplyToList.Add(new MailAddress(address));

            return this;
        }

        public EmailNotifier ReplyTo(string address, string name)
        {
            Message.ReplyToList.Add(new MailAddress(address, name));

            return this;
        }

        public EmailNotifier Subject(string subject)
        {
            Message.Subject = subject;
            return this;
        }

        public EmailNotifier Body(string body, bool isHtml = true)
        {
            Message.Body = body;
            Message.IsBodyHtml = isHtml;
            return this;
        }

        public EmailNotifier HighPriority()
        {
            Message.Priority = MailPriority.High;
            return this;
        }

        public EmailNotifier LowPriority()
        {
            Message.Priority = MailPriority.Low;
            return this;
        }

        public EmailNotifier Attach(Attachment attachment)
        {
            if (!Message.Attachments.Contains(attachment))
                Message.Attachments.Add(attachment);

            return this;
        }

        public EmailNotifier Attach(IList<Attachment> attachments)
        {
            foreach (var attachment in attachments.Where(attachment => !Message.Attachments.Contains(attachment)))
            {
                Message.Attachments.Add(attachment);
            }
            return this;
        }

        public EmailNotifier UsingClient(SmtpClient client)
        {
            _client = client;
            return this;
        }

        public EmailNotifier UseSSL()
        {
            _useSsl = false;
            return this;
        }


        public EmailNotifier Send()
        {
            try
            {
                _client.EnableSsl = false;
                _client.Send(Message);
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                throw new Exception(err);
            }
            return this;
        }

        public EmailNotifier SendAsync(SendCompletedEventHandler callback = null, object token = null)
        {
            try
            {

                if (_useSsl.HasValue)
                    _client.EnableSsl = _useSsl.Value;

                _client.SendCompleted += callback;
                _client.SendAsync(Message, token);
            }
            catch (Exception ex)
            {
                string err = ex.Message;

            }
            return this;
        }

        public EmailNotifier Cancel()
        {
            _client.SendAsyncCancel();
            return this;
        }

        public void Dispose()
        {
            if (_client != null)
                _client.Dispose();

            if (Message != null)
                Message.Dispose();
        }

        public EmailNotifier UsingBody<T>(string body, bool isHtml = true)
        {
            try
            {
                Message.Body = body;
                Message.IsBodyHtml = isHtml;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return this;
        }
    }
}
