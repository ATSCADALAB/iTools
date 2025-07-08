using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ATSCADA;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Text.RegularExpressions;

namespace ATSCADA.iWinTools.SMS
{
    public partial class iSMS : UserControl
    {
        public bool Error = false;
        public List<string> Recipient = new List<string>();
        public string Message = "";

        protected static SerialPort sp = new SerialPort();
        protected string _Port = "COM1.115200.8.None.One";
        protected int _TimeOut = 5000;

        [Description("COM port for GPRS Modem")]
        [Browsable(true), Category("ATSCADA Settings")]
        public string COMPort
        {
            get { return _Port; }
            set { _Port = value; }
        }

        public iSMS()
        {
            InitializeComponent();
            receiveNow = new AutoResetEvent(false);
            btnSend.Click += BtnSend_Click;

        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            try
            {
                btnSend.Enabled = false;
                //Add recipients to list
                Recipient.Clear();

                Recipient.Add(txtTo.Text);

                Message = txtContent.Text;

                Close();
                Open();

                var recipientSplit = Recipient[0].Split(',');
                for (int j = 0; j < recipientSplit.Length; j++)
                {
                    var recipient = recipientSplit[j];
                    sendMsg(recipient);

                    if (!sendMsg(recipient))                       
                    Thread.Sleep(200);
                }

                DeleteMsg();

                Close();
                btnSend.Enabled = true;
            }
            catch (Exception ex) { btnSend.Enabled = true; MessageBox.Show(ex.ToString()); }
        }

        #region Port
        public bool Open()
        {
            try
            {
                System.IO.Ports.Parity _P = new System.IO.Ports.Parity();
                if (_Port.Split('.')[3] == "None")
                    _P = Parity.None;
                else if (_Port.Split('.')[3] == "Even")
                    _P = Parity.Even;
                else if (_Port.Split('.')[3] == "Odd")
                    _P = Parity.Odd;

                //stop bit
                System.IO.Ports.StopBits _S = new StopBits();
                if (_Port.Split('.')[4] == "One")
                    _S = StopBits.One;
                else if (_Port.Split('.')[4] == "Two")
                    _S = StopBits.Two;

                //Assign desired settings to the serial port:
                sp.PortName = _Port.Split('.')[0];
                sp.BaudRate = Convert.ToInt32(_Port.Split('.')[1]);
                sp.DataBits = Convert.ToInt16(_Port.Split('.')[2]);
                sp.Parity = _P;
                sp.StopBits = _S;
                sp.ReadTimeout = _TimeOut;
                sp.WriteTimeout = _TimeOut;
                sp.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
                sp.DtrEnable = true;
                sp.RtsEnable = true;

                if (!sp.IsOpen)
                {
                    sp.Open();

                    return true;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Close()
        {
            try
            {
                //Ensure port is opened before attempting to close:
                if (sp.IsOpen)
                {
                    try
                    {
                        sp.Close();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch { return false; }
        }
        #endregion

        //Execute AT Command
        public string ExecCommand(string command, int responseTimeout, string errorMessage)
        {
            try
            {
                sp.DiscardOutBuffer();
                sp.DiscardInBuffer();

                receiveNow.Reset();
                sp.Write(command + "\r");
                string input = ReadResponse(responseTimeout);
                if ((input.Length == 0) || ((!input.EndsWith("\r\n> ")) && (!input.EndsWith("\r\nOK\r\n"))))
                    throw new ApplicationException("No success message was received.");
                return input;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(errorMessage, ex);
            }
        }

        //Receive data from port
        public void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType == SerialData.Chars)
                receiveNow.Set();
        }
        public string ReadResponse(int timeout)
        {
            string buffer = string.Empty;
            do
            {
                if (receiveNow.WaitOne(timeout, false))
                {
                    string t = sp.ReadExisting();
                    buffer += t;
                }
                else
                {
                    if (buffer.Length > 0)
                        throw new ApplicationException("Response received is incomplete.");
                    else
                        throw new ApplicationException("No data received from phone.");
                }
            }
            while (!buffer.EndsWith("\r\nOK\r\n") && !buffer.EndsWith("\r\n> ") && !buffer.EndsWith("\r\nERROR\r\n"));
            return buffer;
        }

        #region Read SMS

        public AutoResetEvent receiveNow;
        public ShortMessageCollection ReadSMS()
        {
            // Set up the phone and read the messages
            ShortMessageCollection messages = null;
            try
            {
                #region Execute Command
                // Check connection
                ExecCommand("AT", 300, "No phone connected");
                // Use message format "Text mode"
                ExecCommand("AT+CMGF=1", 300, "Failed to set message format.");
                // Use character set "ISO 8859-1"
                //ExecCommand("AT+CSCS=\"8859-1\"", 300, "Failed to set character set."); //error
                ExecCommand("AT+CSCS=\"PCCP437\"", 300, "Failed to set character set.");
                // Select SIM storage
                ExecCommand("AT+CPMS=\"SM\"", 300, "Failed to select message storage.");
                // Read the messages
                string input = ExecCommand("AT+CMGL=\"ALL\"", 5000, "Failed to read the messages.");
                #endregion

                #region Parse messages
                messages = ParseMessages(input);
                #endregion

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            if (messages != null)
                return messages;
            else
                return null;
        }
        public ShortMessageCollection ParseMessages(string input)
        {
            ShortMessageCollection messages = new ShortMessageCollection();
            Regex r = new Regex(@"\+CMGL: (\d+),""(.+)"",""(.+)"",(.*),""(.+)""\r\n(.+)\r\n");
            Match m = r.Match(input);
            while (m.Success)
            {
                ShortMessage msg = new ShortMessage();
                msg.Index = m.Groups[1].Value.ToString();
                msg.Status = m.Groups[2].Value;
                msg.Sender = m.Groups[3].Value;
                msg.Alphabet = m.Groups[4].Value;
                msg.Sent = m.Groups[5].Value;
                msg.Message = m.Groups[6].Value;
                messages.Add(msg);

                m = m.NextMatch();
            }

            return messages;
        }

        #endregion

        #region Send SMS
        public bool sendMsg(string PhoneNo)
        {
            bool isSend = true;
            try
            {
                string recievedData = ExecCommand("AT", 3000, "No phone connected");
                recievedData = ExecCommand("AT+CMGF=1", 3000, "Failed to set message format.");
                String command = "AT+CMGS=\"" + PhoneNo + "\"";
                recievedData = ExecCommand(command, 3000, "Failed to accept phoneNo");
                command = Message + char.ConvertFromUtf32(26) + "\r";
                recievedData = ExecCommand(command, 10000, "Failed to send message"); //3 seconds
                if (recievedData.Contains("ERROR"))
                {
                    string recievedError = recievedData;
                    recievedError = recievedError.Trim();
                    recievedData = "Following error occured while sending the message" + recievedError;
                    isSend = false;
                }
                return isSend;
            }
            catch (Exception ex)
            {
                //return false;
                throw new Exception(ex.Message);
            }
        }
        public bool sendMsgMulti()
        {
            try
            {
                Close();
                Open();

                for (int j = 0; j < Recipient[0].Split(',').Length; j++)
                {
                    //sendMsg(Recipient[0].Split(',')[j].Trim());
                    //Thread.Sleep(200);  
                    if (!sendMsg(Recipient[0].Split(',')[j].Trim()))
                    {
                        Thread.Sleep(2000);
                        j--;
                    }
                }
                DeleteMsg();
                Close();

                return true;
            }
            catch { Close(); return false; }

        }
        #endregion

        #region Delete SMS
        public void DeleteMsg()
        {
            try
            {

                #region Execute Command
                string recievedData = ExecCommand("AT", 3000, "No phone connected");
                recievedData = ExecCommand("AT+CMGF=1", 3000, "Failed to set message format.");
                String command = "AT+CMGD=1,3";
                recievedData = ExecCommand(command, 3000, "Failed to delete message");
                #endregion

                if (recievedData.EndsWith("\r\nOK\r\n"))
                    recievedData = "Message delete successfully";
                if (recievedData.Contains("ERROR"))
                {
                    string recievedError = recievedData;
                    recievedError = recievedError.Trim();
                    recievedData = "Following error occured while sending the message" + recievedError;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        //Send SMS button
        private void btnSend_Click(object sender, EventArgs e)
        {

        }

    }

    public class ShortMessage
    {

        #region Private Variables
        private string index;
        private string status;
        private string sender;
        private string alphabet;
        private string sent;
        private string message;
        #endregion

        #region Public Properties
        public string Index
        {
            get { return index; }
            set { index = value; }
        }
        public string Status
        {
            get { return status; }
            set { status = value; }
        }
        public string Sender
        {
            get { return sender; }
            set { sender = value; }
        }
        public string Alphabet
        {
            get { return alphabet; }
            set { alphabet = value; }
        }
        public string Sent
        {
            get { return sent; }
            set { sent = value; }
        }
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        #endregion

    }
    public class ShortMessageCollection : List<ShortMessage>
    {
    }
}
