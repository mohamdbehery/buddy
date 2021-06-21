using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using App.Contracts.Core;
using Buddy.Utilities.Enums;
using Buddy.Utilities.Models;
using MailMessage = Buddy.Utilities.Models.MailMessage;

namespace Buddy.Utilities
{
    public class Notifier
    {
        readonly Helper helper = Helper.CreateInstance();
        readonly ILogger logger = Logger.GetInstance();

        public void OnMessengerStarted(object source, EventArgs args)
        {            
            SendSMS();
        }

        public ExecResult SendMail(MailMessage mailMessage)
        {
            ExecResult execResult = new ExecResult();
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            try
            {
                msg.From = new MailAddress(mailMessage.SenderMail, mailMessage.SenderName);
                foreach (var receiver in mailMessage.RecieverMail)
                {
                    msg.To.Add(receiver);
                }
                foreach (var cc in mailMessage.CCs)
                {
                    msg.CC.Add(cc);
                }
                foreach (var attachmentFile in mailMessage.Attachments)
                {
                    Attachment attachment = new Attachment(attachmentFile, MediaTypeNames.Application.Octet);
                    ContentDisposition disposition = attachment.ContentDisposition;
                    disposition.CreationDate = File.GetCreationTime(attachmentFile);
                    disposition.ModificationDate = File.GetLastWriteTime(attachmentFile);
                    disposition.ReadDate = File.GetLastAccessTime(attachmentFile);
                    disposition.FileName = Path.GetFileName(attachmentFile);
                    disposition.Size = new FileInfo(attachmentFile).Length;
                    disposition.DispositionType = DispositionTypeNames.Attachment;
                    msg.Attachments.Add(attachment);
                }
                msg.Subject = mailMessage.MailSubject;
                msg.IsBodyHtml = true;
                msg.Body = HttpUtility.UrlDecode(mailMessage.MailBody);
                SmtpClient smtpClient = new SmtpClient
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = mailMessage.SMTPClientTimeout,
                    Host = mailMessage.SMTPClientHost,
                    Port = mailMessage.SMTPClientPort,
                    EnableSsl = mailMessage.IsSSLEnabled,
                    UseDefaultCredentials = true,
                };
                if (!string.IsNullOrEmpty(mailMessage.SenderPassword))
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(msg.From.User, mailMessage.SenderPassword);
                }

                smtpClient.Send(msg);
                return execResult;
            }
            catch (Exception ex)
            {
                execResult.ErrorCode = HelperEnums.ErrorCode.Exception;
                execResult.ErrorException = ex.ToString();
                return execResult;
            }
            finally
            {
                msg.Dispose();
            }
        }
        public void SendSMS()
        {
            logger.Log("Sending SMS...");
        }
    }
}
