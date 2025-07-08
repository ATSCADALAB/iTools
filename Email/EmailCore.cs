using System;
using System.Net;
using System.Net.Mail;

namespace ATSCADA.iWinTools.Email
{
    public class EmailCore
    {
        private const string DEFAULT_HOST = "smtp.gmail.com";

        private const string DEFAULT_EMAIL = "alarm@atpro.com.vn";

        private const string DEFAULT_EMAIL_PASSWORD = "ATSCADA12345";

        private const string DEFAULT_CONTENT = "ATSCADA";

        private readonly SmtpClient smtpClient;

        public EmailCore()
        {
            this.smtpClient = new SmtpClient();

            this.smtpClient = new SmtpClient
            {
                Host = DEFAULT_HOST,
                Port = 587,
                EnableSsl = true,
                Timeout = 10000,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    DEFAULT_EMAIL,
                    DEFAULT_EMAIL_PASSWORD)
            };
        }

        public EmailCore(EmailConfigParametter configParametter)
        {
            ICredentialsByHost credentials;
            if ((configParametter.CredentialEmail == "" ||
                configParametter.CredentialEmail == null) &&
                configParametter.Host == DEFAULT_HOST)
                credentials = new NetworkCredential(
                    DEFAULT_EMAIL,
                    DEFAULT_EMAIL_PASSWORD);
            else
                credentials = new NetworkCredential(
                    configParametter.CredentialEmail,
                    configParametter.CredentialPass);

            this.smtpClient = new SmtpClient
            {
                Host = configParametter.Host,
                Port = configParametter.Port,
                EnableSsl = configParametter.EnableSSL,
                Timeout = configParametter.TimeOut,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = credentials
            };
        }

        public void SendEmail(MailMessageParametter messageParametter)
        {
            try
            {
                var message = CreateMailMessage(messageParametter);
                if (message == null) return;

                this.smtpClient.Send(message);
            }
            catch (Exception ex)
            { }
        }

        public async System.Threading.Tasks.Task SendEmailAsyn(MailMessageParametter messageParametter)
        {
            try
            {
                var message = CreateMailMessage(messageParametter);
                if (message == null) return;

                await this.smtpClient.SendMailAsync(message);
            }
            catch { }
        }

        private MailMessage CreateMailMessage(MailMessageParametter messageParametter)
        {
            try
            {
                if (messageParametter == null) return null;

                var mailMessage = new MailMessage();

                if (messageParametter.Sender == "" ||
                    messageParametter.Sender == null)
                    mailMessage.From = new MailAddress(DEFAULT_EMAIL);
                else
                    mailMessage.From = new MailAddress(messageParametter.Sender);

                if (messageParametter.Recipients != null)
                    foreach (var recipient in messageParametter.Recipients)
                        mailMessage.To.Add(recipient);

                if (messageParametter.CopyRecipients != null)
                    foreach (var copyRecipient in messageParametter.CopyRecipients)
                        mailMessage.CC.Add(copyRecipient);

                if (string.IsNullOrEmpty(messageParametter.Subject))
                    mailMessage.Subject = DEFAULT_CONTENT;
                else
                    mailMessage.Subject = messageParametter.Subject;

                if (string.IsNullOrEmpty(messageParametter.Body))
                    mailMessage.Body = DEFAULT_CONTENT;
                else
                    mailMessage.Body = messageParametter.Body;

                if (messageParametter.AttachFiles != null)
                    foreach (var attachFile in messageParametter.AttachFiles)
                        mailMessage.Attachments.Add(new Attachment(attachFile));

                return mailMessage;
            }
            catch { return null; }
        }
    }
}
