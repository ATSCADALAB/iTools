using System;
using System.IO.Ports;
using System.Threading;

namespace ATSCADA.iWinTools.SMS
{
    public class SMSCore
    {
        private readonly SerialPort serialPort;

        private readonly AutoResetEvent autoResetEvent;

        public SMSCore()
        {
            this.serialPort = new SerialPort()
            {
                PortName = "COM1",
                BaudRate = 11520,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One,
                ReadTimeout = 5000,
                WriteTimeout = 5000,
                DtrEnable = true,
                RtsEnable = true,
            };

            this.autoResetEvent = new AutoResetEvent(false);
            this.serialPort.DataReceived += (sender, e) => DataReceived(e.EventType);
        }

        public SMSCore(SMSConfigParammetter configParametter)
        {
            this.serialPort = new SerialPort()
            {
                PortName = configParametter.PortName,
                BaudRate = configParametter.BaudRate,
                DataBits = configParametter.DataBits,
                Parity = configParametter.Parity,
                StopBits = configParametter.StopBits,
                ReadTimeout = configParametter.TimeOut,
                WriteTimeout = configParametter.TimeOut,
                DtrEnable = true,
                RtsEnable = true,
            };

            this.autoResetEvent = new AutoResetEvent(false);
            this.serialPort.DataReceived += (sender, e) => DataReceived(e.EventType);
        }

        private void DataReceived(SerialData serialData)
        {
            if (serialData == SerialData.Chars)
                this.autoResetEvent.Set();
        }

        public bool Open()
        {
            try
            {
                if (this.serialPort.IsOpen) return true;

                this.serialPort.Open();
                return true;
            }
            catch { return false; }
        }

        public bool Close()
        {
            try
            {
                if (!this.serialPort.IsOpen) return true;

                this.serialPort.Close();
                return true;
            }
            catch { return false; }
        }

        public bool SendMessage(SMSMessageParametter messageParametter)
        {
            try
            {
                if (string.IsNullOrEmpty(messageParametter.Message)) return false;
                if (messageParametter.Recipients == null)
                    if (messageParametter.Recipients.Count == 0)
                        return false;

                Close();
                Open();

                var message = messageParametter.Message;
                foreach(var recipient in messageParametter.Recipients)
                {
                    int sendTimes = 0;
                    while (!SendMessage(recipient, message) &&
                        sendTimes < 5)
                    {
                        Thread.Sleep(1000);
                        sendTimes++;
                    }
                }

                DeleteMessage();
                Close();

                return true;
            }
            catch { return false; }
        }

        private bool SendMessage(string recipient, string message)
        {
            bool isSend = false;
            try
            {
                string receivedData = string.Empty;
                string command = string.Empty;

                command = "AT";
                receivedData = ExecCommand(command, 3000, "No phone connected");

                command = "AT+CMGF=1";
                receivedData = ExecCommand("AT+CMGF=1", 3000, "Failed to set message format.");

                command = "AT+CMGS=\"" + recipient + "\"";
                receivedData = ExecCommand(command, 3000, "Failed to accept phoneNo");

                command = message + char.ConvertFromUtf32(26) + "\r";
                receivedData = ExecCommand(command, 10000, "Failed to send message");

                if (receivedData.EndsWith("\r\nOK\r\n"))
                {
                    receivedData = "Message sent successfully";
                    isSend = true;
                }
                else if (receivedData.Contains("ERROR"))
                {
                    string recievedError = receivedData;
                    recievedError = recievedError.Trim();
                    receivedData = "Following error occured while sending the message" + recievedError;
                    isSend = false;
                }
                return isSend;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void DeleteMessage()
        {
            try
            {
                string receivedData = string.Empty;
                string command = string.Empty;

                command = "AT";
                receivedData = ExecCommand(command, 3000, "No phone connected");

                command = "AT+CMGF=1";
                receivedData = ExecCommand(command, 3000, "Failed to set message format.");

                command = "AT+CMGD=1,3";
                receivedData = ExecCommand(command, 3000, "Failed to delete message");

                if (receivedData.EndsWith("\r\nOK\r\n"))
                    receivedData = "Message delete successfully";
                if (receivedData.Contains("ERROR"))
                {
                    string recievedError = receivedData;
                    recievedError = recievedError.Trim();
                    receivedData = "Following error occured while sending the message" + recievedError;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string ExecCommand(string command, int responseTimeout, string errorMessage)
        {
            try
            {
                this.serialPort.DiscardOutBuffer();
                this.serialPort.DiscardInBuffer();

                this.autoResetEvent.Reset();
                this.serialPort.Write(command + "\r");
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

        private string ReadResponse(int timeout)
        {
            string buffer = string.Empty;
            do
            {
                if (this.autoResetEvent.WaitOne(timeout, false))
                    buffer += this.serialPort.ReadExisting();
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
    }
}
