using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;
using ATSCADA;
using System.IO;

namespace ATSCADA.iWinTools.Email
{
    public partial class iEmail : UserControl
    {
        public MailMessage Message;
        public SmtpClient Smtp;
        public bool Error = false;
        protected string _Host = "smtp.gmail.com";
        protected int _Port = 587;
        protected int _TimeOut = 10000;
        protected string _CredentialsUser = "";
        protected string _CredentialsPass = "";
        protected List<string> _FileLocs = new List<string>();
        protected bool _EnableSSL = true;

        [Description("Email Host, Default is for Gmail")]
        [Browsable(true), Category("ATSCADA Settings")]
        public string Host
        {
            get { return _Host; }
            set { _Host = value; }
        }

        [Description("Email Port, Default for Gmail Port is 587")]
        [Browsable(true), Category("ATSCADA Settings")]
        public int Port
        {
            get { return _Port; }
            set { _Port = value; }
        }

        [Description("Email Sending Timeout, Default is 10000")]
        [Browsable(true), Category("ATSCADA Settings")]
        public int TimeOut
        {
            get { return _TimeOut; }
            set { _TimeOut = value; }
        }

        [Description("Enscrypted connection SSL, Default is true")]
        [Browsable(true), Category("ATSCADA Settings")]
        public bool EnableSSL
        {
            get { return _EnableSSL; }
            set { _EnableSSL = value; }
        }
        [Description("Attached Files Locations")]
        [Browsable(false), Category("ATSCADA Settings")]
        public List<string> AttachFiles
        {
            get { return _FileLocs; }
            set { _FileLocs = value; }
        }

        [Description("Credential Email, if not set, the Default Account will be used")]
        [Browsable(true), Category("ATSCADA Settings")]
        public string CredentialEmail
        {
            get { return _CredentialsUser; }
            set { _CredentialsUser = value; }
        }

        [Description("Credential Password")]
        [Browsable(true), Category("ATSCADA Settings")]
        public string CredentialPass
        {
            get { return _CredentialsPass; }
            set { _CredentialsPass = value; }
        }

        public iEmail()
        {
            InitializeComponent();
            Message = new MailMessage();
            Smtp = new SmtpClient();
            btnSend.Click += BtnSend_Click;
            btnAttach.Click += BtnAttach_Click;
        }
        //Attach message from file
        private void BtnAttach_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                // setup a dialog;
                dlg.Title = "Select Files for Attach - ATSCADA";
                dlg.Multiselect = true;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    label4.Text = "";

                    _FileLocs.Clear();

                    //Generate news                    
                    _FileLocs.AddRange(dlg.FileNames);

                    foreach (string s in _FileLocs)
                    {
                        Attachment _a = new Attachment(s);

                        Message.Attachments.Add(_a);

                        if (label4.Text != "")
                            label4.Text = label4.Text + " ; " + s;
                        else
                            label4.Text = s;
                    }
                }
            }
            catch { MessageBox.Show("Something is wrong with attached files", "ATSCADA"); }
        }
        //Send Message
        private void BtnSend_Click(object sender, EventArgs e)
        {
            try
            {
                btnSend.Enabled = false;

                if (_CredentialsUser == null || _CredentialsUser == "")
                    Message.From = new MailAddress("alarm@atpro.com.vn");
                else
                    Message.From = new MailAddress(_CredentialsUser);

                Message.To.Clear();

                Message.To.Add(txtTo.Text);

                try
                {
                    foreach (string s in txtCC.Text.Split(','))
                    {
                        Message.CC.Add(s);
                    }
                }
                catch { }

                Message.Subject = txtSubject.Text;
                Message.Body = txtContent.Text;
                SendEmail();

                btnSend.Enabled = true;
            }
            catch (Exception ex)
            { MessageBox.Show("Something is wrong with email sending, please try again", "ATSCADA"); btnSend.Enabled = true; }

        }

        public void SendEmail()
        {
            try
            {
                Smtp.Host = _Host;
                Smtp.Port = _Port;

                Smtp.EnableSsl = _EnableSSL;

                Smtp.Timeout = _TimeOut;

                Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                Smtp.UseDefaultCredentials = false;
               

                if (_CredentialsUser == "" && _Host == "smtp.gmail.com")
                    Smtp.Credentials = new System.Net.NetworkCredential("alarm@atpro.com.vn", "ATSCADA12345");
                else
                    Smtp.Credentials = new System.Net.NetworkCredential(_CredentialsUser, _CredentialsPass);

                Smtp.Send(Message);
                Error = false;
            }
            catch (SmtpException ex) { Error = true; throw ex; }
        }       
    }
}
