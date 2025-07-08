using ATSCADA.ToolExtensions.Converter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;

namespace ATSCADA.iWinTools.SMS
{
    [TypeConverter(typeof(ClassConverter<SMSConfigParammetter>))]
    [Serializable]
    public class SMSConfigParammetter
    {
        [Description("Port name")]
        public string PortName { get; set; }

        [Description("Baud rate")]
        public int BaudRate { get; set; }

        [Description("Data bits")]
        public int DataBits { get; set; }

        [Description("Parity")]
        public Parity Parity { get; set; }

        [Description("Stop bits")]
        public StopBits StopBits { get; set; }

        [Description("Time out")]
        public int TimeOut { get; set; }

        public override string ToString()
        {
            return "SMS Settings";
        }

    }

    public class SMSMessageParametter
    {
        public List<string> Recipients { get; private set; }

        public string Message { get; set; }

        public void AddRecipient(string phoneNumber)
        {
            if (Recipients == null) Recipients = new List<string>();
            Recipients.Add(phoneNumber);
        }
    }
}
