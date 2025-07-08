using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows.Forms;

namespace ATSCADA.iWinTools
{

    [ToolboxBitmap(typeof(Control))]
    public partial class iCloudTie : Component, IDisposable
    {
        //Remote Driver
        protected ServiceHost ATWebPortHost = null;
        protected string urlMeta = "", urlService = "";
        protected string _ServeripAddress = "";

        //Client for updating data from local to remote
        protected string endPointAddr = "";
        protected InterfaceWebPort ATWebPortClient;

        //protected System.Timers.Timer _Timer = new System.Timers.Timer ();

        protected System.Timers.Timer _FailureTryTimer = new System.Timers.Timer();

        //protected double _TimerInterval = 1000;
        protected double _FailerTryInterval = 10000;

        protected int _MaxRemoteWriteTime = 10;

        protected iDriver _Driver;

        protected bool _DevicetoCloud = true;

        protected List<Event> EventLog = new List<Event>();

        protected string _WebPort = "8010";

        private int delayTimeWrite = 1000;


        [Description("ATSCADA Driver Object")]
        [Browsable(true), Category("ATSCADA Settings")]
        public iDriver Driver
        {
            get
            {
                return _Driver;
            }
            set
            {
                _Driver = value;
                try
                {
                    _Driver.ConstructionCompleted -= _Driver_ConstructionCompleted;
                }
                catch { }

                try
                {
                    _Driver.ConstructionCompleted += _Driver_ConstructionCompleted;
                }
                catch { }
            }
        }

        [Description("Try to reconnect Interval when on Failure")]
        [Browsable(true), Category("ATSCADA Settings")]
        public double FailureTryInterval
        {
            get { return _FailerTryInterval; }
            set { _FailerTryInterval = value; }
        }
        [Description("Max Times for each remote writing")]
        [Browsable(true), Category("ATSCADA Settings")]
        public int MaxRemoteWriteTime
        {
            get { return _MaxRemoteWriteTime; }
            set { _MaxRemoteWriteTime = value; }
        }

        [Description("Delay time for each remote writing")]
        [Browsable(true), Category("ATSCADA Settings")]
        public int DelayTimeWrite
        {
            get => this.delayTimeWrite;
            set
            {
                if (value < 1000) return;
                this.delayTimeWrite = value;
            }
        }

        [Description("if Device to Cloud, select true; elseif Cloud to Remote Location, select false")]
        [Browsable(true), Category("ATSCADA Settings")]
        public bool DevicetoCloud
        {
            get { return _DevicetoCloud; }
            set { _DevicetoCloud = value; }
        }

        [Description("Cloud Server IP Address")]
        [Browsable(true), Category("ATSCADA Settings")]
        public string CloudIP

        {
            get { return _ServeripAddress; }
            set { _ServeripAddress = value; }
        }


        [Description("WebService Port")]
        [Browsable(true), Category("ATSCADA Settings")]
        public string WebPort
        {
            get { return _WebPort; }
            set { _WebPort = value; }
        }

        [Description("Delay time for each remote writing")]
        [Browsable(true), Category("ATSCADA Settings")]
        public bool ShowMessageBadWriting { get; set; }

        public iCloudTie()
        {
            InitializeComponent();

            _FailureTryTimer.Elapsed += _FailureTryTimer_Elapsed;
            _FailureTryTimer.Interval = _FailerTryInterval;
            _FailureTryTimer.Enabled = false;
        }

        void _Driver_ConstructionCompleted()
        {
            //Create remote client for updating data for server
            ClientCreate();

            //Act when Task Act
            foreach (Task T in _Driver.TaskCollection)
            {
                T.TaskActed += T_TaskActed;
            }
            foreach (InternalTask T in _Driver.InternalTaskCollection)
            {
                T.TaskActed += T_TaskActed;
            }

            //for removing above event
            try
            {
                ((ICommunicationObject)ATWebPortClient).Faulted -= iCloudTie_Faulted;
            }
            catch { }

            ((ICommunicationObject)ATWebPortClient).Faulted += iCloudTie_Faulted;


        }

        private void T_TaskActed(object o, TaskActEventArgs e)
        {
            try
            {
                if (ATWebPortClient is null) return;
                if (_Driver != null)
                {
                    //Find the Task
                    if (!e.TaskName.Contains("InternalTask"))
                    {
                        Task T = _Driver.TaskCollection.Find(delegate (Task _T) { return _T.Name == e.TaskName; });

                        if (T != null)
                        {
                            //Stop Event
                            try
                            {
                                T.TaskActed -= T_TaskActed;
                            }
                            catch { }

                            try
                            {
                                foreach (Tag t in T.TagCollection)
                                {
                                    var name = $"InternalTask{T.Name}.{t.Name}";
                                    var nameEncrypt = name.EncryptAddress();

                                    var webTag = ATWebPortClient.GetTag(nameEncrypt);

                                    if (webTag != null)
                                    {
                                        if (_DevicetoCloud)
                                        {
                                            //Write value
                                            var valueToWriteDecrypt = webTag.ValuetoWrite?.DecryptValue();
                                            if (valueToWriteDecrypt != "" && valueToWriteDecrypt != null)
                                            {
                                                t.Value = valueToWriteDecrypt;
                                                ATWebPortClient.WriteTagValue(nameEncrypt, "".EncryptValue());
                                            }
                                            ATWebPortClient.UpdateTag(nameEncrypt, t.Value.EncryptValue(), t.Status, t.TimeStamp);
                                        }
                                    }
                                    else
                                    {
                                        WebTag wt = new WebTag();
                                        wt.Name = nameEncrypt;
                                        wt.Value = t.Value.EncryptValue();
                                        wt.Status = t.Status;
                                        wt.TimeStamp = t.TimeStamp;
                                        wt.ValuetoWrite = "";
                                        ATWebPortClient.AddTag(wt);
                                    }
                                }
                            }
                            catch { }

                            //Stop Event
                            try
                            {
                                T.TaskActed += T_TaskActed;
                            }
                            catch { }

                        }
                    }
                    else
                    {
                        InternalTask T = _Driver.InternalTaskCollection.Find(delegate (InternalTask _T) { return _T.Name == e.TaskName; });

                        if (T != null)
                        {

                            //Stop Event
                            try
                            {
                                T.TaskActed -= T_TaskActed;
                            }
                            catch { }

                            try
                            {
                                var hasTagNeedWrite = false;
                                foreach (InternalTag t in T.TagCollection)
                                {
                                    var name = $"{T.Name}.{t.Name}";
                                    var nameEncrypt = name.EncryptAddress();

                                    var webTag = ATWebPortClient.GetTag(nameEncrypt);
                                    if (webTag != null)
                                    {
                                        if (_DevicetoCloud)
                                        {
                                            //Write value
                                            var valueToWriteDecrypt = webTag.ValuetoWrite?.DecryptValue();
                                            if (valueToWriteDecrypt != "" && valueToWriteDecrypt != null)
                                            {
                                                t.Value = valueToWriteDecrypt;
                                                ATWebPortClient.WriteTagValue(nameEncrypt, "".EncryptValue());
                                            }
                                            //else
                                            ATWebPortClient.UpdateTag(nameEncrypt, t.Value.EncryptValue(), t.Status, t.TimeStamp);

                                        }
                                        else// remote location
                                        {
                                            //Write value

                                            var valueDecrypt = webTag.Value.DecryptValue();
                                            if (t.ValuetoWrite != "" && t.ValuetoWrite != null)
                                            {
                                                if
                                                    ((t.ValuetoWrite == valueDecrypt)
                                                    && (t.MaxCloudWriteTime > 0))
                                                {
                                                    t.ValuetoWrite = "";
                                                    t.MaxCloudWriteTime = _MaxRemoteWriteTime;
                                                }
                                                else if
                                                 ((t.ValuetoWrite != valueDecrypt)
                                                && (t.MaxCloudWriteTime <= 0))
                                                {
                                                    t.ValuetoWrite = "";
                                                    t.MaxCloudWriteTime = _MaxRemoteWriteTime;
                                                    if (ShowMessageBadWriting)
                                                        MessageBox.Show($"Tag {name} is in bad writing", "iCloudTie");
                                                }
                                                else
                                                {
                                                    ATWebPortClient.WriteTagValue(nameEncrypt, t.ValuetoWrite.EncryptValue());
                                                    if (t.MaxCloudWriteTime > 0)
                                                        t.MaxCloudWriteTime--;
                                                    hasTagNeedWrite = true;
                                                }
                                            }
                                            else
                                            {
                                                t.Value = webTag.Value.DecryptValue();
                                                t.Status = webTag.Status;
                                                t.TimeStamp = webTag.TimeStamp;
                                                t.MaxCloudWriteTime = _MaxRemoteWriteTime;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        WebTag wt = new WebTag();
                                        wt.Name = nameEncrypt;
                                        wt.Value = t.Value.EncryptValue();
                                        wt.Status = t.Status;
                                        wt.TimeStamp = t.TimeStamp;
                                        wt.ValuetoWrite = "";
                                        ATWebPortClient.AddTag(wt);
                                    }
                                }
                                if (hasTagNeedWrite)
                                    System.Threading.Thread.Sleep(this.delayTimeWrite);
                            }
                            catch { }

                            //Start Event
                            try
                            {
                                T.TaskActed += T_TaskActed;
                            }
                            catch { }

                        }
                    }
                }

            }
            catch (Exception ex) { }
        }

        ~iCloudTie()
        {
            string content = "";
            foreach (Event ev in EventLog)
            {
                if (content != "")
                {
                    content = content + Environment.NewLine + ev.Datetime + "   " + ev.EventString;
                }
                else
                {
                    content = ev.Datetime + "   " + ev.EventString;
                }
            }

            string _LogFile = AppDomain.CurrentDomain.BaseDirectory + "EventsLog.txt";

            if (File.Exists(_LogFile))
            {
                File.Delete(_LogFile);
            }

            File.WriteAllText(_LogFile, content);

        }
        void iCloudTie_Faulted(object sender, EventArgs e)
        {
            try
            {
                IChannel channel = sender as IChannel;
                if (channel != null)
                {
                    channel.Abort();
                    channel.Close();
                }

                //The proxy channel should no longer be used 
                ((IClientChannel)ATWebPortClient).Abort();
                ((IClientChannel)ATWebPortClient).Close();
                ATWebPortClient = null;

                //_Timer.Enabled = false; 

                _FailureTryTimer.Enabled = true;

                //log into event log                
                Event ev = new Event(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), "Cloud Server Connection broken!");
                EventLog.Add(ev);
            }
            catch { _FailureTryTimer.Enabled = true; }
        }

        void _FailureTryTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            //log into event log                
            Event ev = new Event(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), "Doing reconnect...");
            EventLog.Add(ev);

            if (ClientCreate())
            {
                //for stop aboved timer
                try
                {
                    ((ICommunicationObject)ATWebPortClient).Faulted -= iCloudTie_Faulted;
                }
                catch { }

                ((ICommunicationObject)ATWebPortClient).Faulted += iCloudTie_Faulted;


                //If all things are ok -> disable this failuretimer              
                _FailureTryTimer.Enabled = false;
            }
        }

        //string TotalTimeCost = "";
        //void _Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    try
        //    {
        //        //start time
        //        DateTime dt1 = DateTime.Now;

        //        if (_Driver != null)
        //        {
        //            _Timer.Enabled = false;

        //            foreach (Task T in _Driver.TaskCollection)
        //            {
        //                foreach (Tag t in T.TagCollection)
        //                {
        //                    if (ATWebPortClient.GetTag("InternalTask" + T.Name + "." + t.Name) != null)
        //                    {
        //                        if (_DevicetoCloud)
        //                        {
        //                            //Write value
        //                            if (ATWebPortClient.GetTag("InternalTask" + T.Name + "." + t.Name).ValuetoWrite != ""
        //                                && ATWebPortClient.GetTag("InternalTask" + T.Name + "." + t.Name).ValuetoWrite != null)
        //                            {
        //                                t.Value = ATWebPortClient.GetTag("InternalTask" + T.Name + "." + t.Name).ValuetoWrite;

        //                                ATWebPortClient.WriteTagValue("InternalTask" + T.Name + "." + t.Name, "");
        //                            }
        //                            //else 
        //                            ATWebPortClient.UpdateTag("InternalTask" + T.Name + "." + t.Name, t.Value, t.Status, t.TimeStamp);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        WebTag wt = new WebTag();
        //                        wt.Name = T.Name + "." + t.Name;
        //                        wt.Value = t.Value;
        //                        wt.Status = t.Status;
        //                        wt.TimeStamp = t.TimeStamp;
        //                        wt.ValuetoWrite = "";
        //                        ATWebPortClient.AddTag(wt);
        //                    }
        //                }
        //            }

        //            foreach (InternalTask T in _Driver.InternalTaskCollection)
        //            {
        //                foreach (InternalTag t in T.TagCollection)
        //                {
        //                    if (ATWebPortClient.GetTag(T.Name + "." + t.Name) != null)
        //                    {
        //                        if (_DevicetoCloud)
        //                        {
        //                            //Write value
        //                            if (ATWebPortClient.GetTag(T.Name + "." + t.Name).ValuetoWrite != ""
        //                                && ATWebPortClient.GetTag(T.Name + "." + t.Name).ValuetoWrite != null)
        //                            {
        //                                t.Value = ATWebPortClient.GetTag(T.Name + "." + t.Name).ValuetoWrite;
        //                                ATWebPortClient.WriteTagValue(T.Name + "." + t.Name, "");
        //                            }
        //                            //else
        //                            ATWebPortClient.UpdateTag(T.Name + "." + t.Name, t.Value, t.Status, t.TimeStamp);

        //                        }
        //                        else// remote location
        //                        {
        //                            //Write value
        //                            if (t.ValuetoWrite != "" && t.ValuetoWrite != null)
        //                            {
        //                                if 
        //                                    ((t.ValuetoWrite == ATWebPortClient.GetTag(T.Name + "." + t.Name).Value)
        //                                    && (t.MaxCloudWriteTime >0))
        //                                {
        //                                    t.ValuetoWrite = "";
        //                                    t.MaxCloudWriteTime = _MaxRemoteWriteTime;                                            
        //                                }  else if
        //                                    ((t.ValuetoWrite != ATWebPortClient.GetTag(T.Name + "." + t.Name).Value)
        //                                   && (t.MaxCloudWriteTime <= 0))                                           
        //                                {
        //                                    t.ValuetoWrite = "";
        //                                    t.MaxCloudWriteTime = _MaxRemoteWriteTime;
        //                                    MessageBox.Show("Tag " + T.Name + "."+ t.Name + " is in bad writing", "iCloudTie"); 
        //                                }
        //                                else
        //                                {
        //                                    ATWebPortClient.WriteTagValue(T.Name + "." + t.Name, t.ValuetoWrite);                                            
        //                                    if(t.MaxCloudWriteTime > 0)
        //                                        t.MaxCloudWriteTime--;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                t.Value = ATWebPortClient.GetTag(T.Name + "." + t.Name).Value;
        //                                t.Status = ATWebPortClient.GetTag(T.Name + "." + t.Name).Status;
        //                                t.TimeStamp = ATWebPortClient.GetTag(T.Name + "." + t.Name).TimeStamp;
        //                                t.MaxCloudWriteTime = _MaxRemoteWriteTime; 
        //                            }
        //                        }

        //                    }
        //                    else
        //                    {
        //                        WebTag wt = new WebTag();
        //                        wt.Name = T.Name + "." + t.Name;
        //                        wt.Value = t.Value;
        //                        wt.Status = t.Status;
        //                        wt.TimeStamp = t.TimeStamp;
        //                        wt.ValuetoWrite = "";
        //                        ATWebPortClient.AddTag(wt);
        //                    }
        //                }
        //            }
        //            _Timer.Enabled = true;
        //        }

        //        //end time
        //        DateTime dt2 = DateTime.Now;

        //        TotalTimeCost = TotalTimeCost + (dt2 - dt1).TotalSeconds.ToString() + "\r\n";

        //    }
        //    catch (Exception ex) { _Timer.Enabled = true;}
        //}

        public string ClientCounter()
        {
            return ATWebPortClient.GetClientCounter().ToString();
        }

        //Create a client of remote server
        private bool ClientCreate()
        {
            try
            {
                endPointAddr = "net.tcp://" + _ServeripAddress.ToString() + ":" + _WebPort + "/ATWebPort";

                var binding = new CustomNetTcpBinding();
                binding.OpenTimeout = TimeSpan.FromMinutes(2);
                binding.SendTimeout = TimeSpan.FromMinutes(2);
                binding.ReceiveTimeout = TimeSpan.FromMinutes(10);

                EndpointAddress endpointAddress = new EndpointAddress(endPointAddr);
                ChannelFactory<InterfaceWebPort> myChannelFactory =
                    new ChannelFactory<InterfaceWebPort>(binding, endpointAddress);

                myChannelFactory.Credentials.UserName.UserName = Account.UserName;
                myChannelFactory.Credentials.UserName.Password = Account.Password;

                ATWebPortClient = myChannelFactory.CreateChannel();

                return true;
            }
            catch (Exception ex) { return false; };
        }

    }
    public class Event : IDisposable
    {
        public string Datetime;
        public string EventString;

        public Event(string myDateTime, string myEvent)
        {
            Datetime = myDateTime;
            EventString = myEvent;
        }

        public void Dispose() { }

    }

}

