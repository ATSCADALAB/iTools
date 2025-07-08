using ATSCADA.ToolExtensions.Converter;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ATSCADA.iWinTools.Email
{
    [TypeConverter(typeof(ClassConverter<EmailConfigParametter>))]
    [Serializable]
    public class EmailConfigParametter
    {
        [Description("Email host, default is for Gmail.")]
        public string Host { get; set; }

        [Description("Email port, default for Gmail port is 587.")]
        public int Port { get; set; }

        [Description("Email sending timeout (milisecond), default is 10000.")]
        public int TimeOut { get; set; }

        [Description("Enscrypted connection SSL, default is true.")]
        public bool EnableSSL { get; set; }

        [Description("Credential email, if not set, the default account will be used.")]
        public string CredentialEmail { get; set; }

        [Description("Credential password.")]
        public string CredentialPass { get; set; }

        public override string ToString()
        {
            return "Email Settings";
        }

    }

    public class MailMessageParametter
    {
        public string Sender { get; set; } = "atscada@gmail.com";

        public List<string> Recipients { get; private set; }

        public List<string> CopyRecipients { get; private set; }

        public string Subject { get; set; } = "ATSCADA";

        public string Body { get; set; } = "ATSCADA";

        public List<string> AttachFiles { get; private set; }

        public void AddRecipient(string address)
        {
            if (Recipients == null) Recipients = new List<string>();
            Recipients.Add(address);
        }

        public void AddCopyRecipient(string address)
        {
            if (CopyRecipients == null) CopyRecipients = new List<string>();
            CopyRecipients.Add(address);
        }

        public void AddAttachFile(string fileName)
        {
            if (AttachFiles == null) AttachFiles = new List<string>();
            AttachFiles.Add(fileName);
        }
    }
}
