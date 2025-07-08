using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Windows.Forms;

namespace ATSCADA.iWinTools
{

    public partial class iWebPort : Component, IDisposable
    {
        //Remote Driver
        protected ServiceHost ATWebPortHost = null;
        protected string urlMeta = "", urlService = "";
        protected string _ipAddress = "";

        //Client for updating data from local to remote
        protected string endPointAddr = "";
        protected InterfaceWebPort ATWebPortClient;

        protected System.Timers.Timer _RestartTimer = new System.Timers.Timer();
        protected double _RestartIISAfter = 120;// minute

        protected iDriver _Driver;

        protected bool _OnCloud = false;

        protected string _Name;
        protected string _Value;
        protected string _Status;
        protected string _TimeStamp;
        protected string _VtW;
        protected string _WebPort = "8010";
        public string applicationPath;

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

        [Description("If this App is on Cloud Server, select true")]
        [Browsable(true), Category("ATSCADA Settings")]
        public bool OnCloud
        {
            get { return _OnCloud; }
            set { _OnCloud = value; }
        }

        [Description("Restart IIS Web Server Every x MINUTES. Zero value: never restart")]
        [Browsable(true), Category("ATSCADA Settings")]
        public double RestartIISAfter
        {
            get { return _RestartIISAfter; }
            set { _RestartIISAfter = value; }
        }

        [Description("IP to invoke server")]
        [Browsable(true), Category("ATSCADA Settings")]
        public string ServerIP
        {
            get { return _ipAddress; }
            set { _ipAddress = value; }
        }

        [Description("Enter port of web.")]
        [Browsable(true), Category("ATSCADA Settings")]
        public string WebPort
        {
            get { return _WebPort; }
            set { _WebPort = value; }
        }

        public iWebPort()
        {
            InitializeComponent();
            //Get path file debug


            if (_ipAddress == "")
            {
                // Returns a list of ipaddress configuration
                IPHostEntry ips = Dns.GetHostEntry(Dns.GetHostName());
                // Get server IP Address
                foreach (IPAddress ip in ips.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        _ipAddress = ip.ToString();

                        break;
                    }
                }
            }
        }

        void _Driver_ConstructionCompleted()
        {
            //Start Service for remote service
            StartService();

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

            if (_RestartIISAfter == 0)
            {
                _RestartTimer.Enabled = false;
            }
            else
            {
                _RestartTimer.Interval = _RestartIISAfter * 60000;
                _RestartTimer.Elapsed += _RestartTimer_Elapsed;

                _RestartTimer.Enabled = true;
            }

            //Restart IIs for the first time
            System.Threading.Thread.Sleep(5000);

            try
            {
                Process cmd = new Process();
                cmd.StartInfo.FileName = @"C:\Windows\System32\iisreset.exe";
                cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                cmd.Start();
                //Process.Start(@"C:\Windows\System32\iisreset.exe");
            }
            catch { }
        }

        private void T_TaskActed(object o, TaskActEventArgs e)
        {
            try
            {
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
                                    var name = T.Name + "." + t.Name;
                                    var nameEncrypt = name.EncryptAddress();

                                    var webTag = ATWebPortClient.GetTag(nameEncrypt);
                                    if (webTag != null)
                                    {
                                        //Update value                                        
                                        _Value = t.Value;
                                        _Status = t.Status;
                                        _TimeStamp = t.TimeStamp;

                                        ATWebPortClient.UpdateTag(nameEncrypt, _Value.EncryptValue(), _Status, _TimeStamp);

                                        //Write value
                                        var valueToWriteDecrypt = webTag.ValuetoWrite?.DecryptValue();
                                        if (valueToWriteDecrypt != "" && valueToWriteDecrypt != null)
                                        {
                                            t.Value = valueToWriteDecrypt;
                                            ATWebPortClient.WriteTagValue(nameEncrypt, "".EncryptValue());
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
                            catch (Exception ex) { MessageBox.Show(ex.ToString()); }

                            //Start Event
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
                                foreach (InternalTag t in T.TagCollection)
                                {
                                    var name = T.Name + "." + t.Name;
                                    var nameEncrypt = name.EncryptAddress();

                                    var webTag = ATWebPortClient.GetTag(nameEncrypt);
                                    if (webTag != null)
                                    {
                                        if (!_OnCloud)
                                        {
                                            ATWebPortClient.UpdateTag(nameEncrypt, t.Value.EncryptValue(), t.Status, t.TimeStamp);

                                            //Write value
                                            var valueToWriteDecrypt = webTag.ValuetoWrite?.DecryptValue();
                                            if (valueToWriteDecrypt != "" && valueToWriteDecrypt != null)
                                            {
                                                t.Value = valueToWriteDecrypt;
                                                ATWebPortClient.WriteTagValue(nameEncrypt, "".EncryptValue());
                                            }
                                        }
                                        else
                                        {
                                            // This is for ON CLOUD APP -> update from service to idriver to display on ONCLOUD tools
                                            t.Value = webTag.Value.DecryptValue();
                                            t.Status = webTag.Status;
                                            t.TimeStamp = webTag.TimeStamp;
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
                            catch (Exception ex) { MessageBox.Show(ex.ToString()); }

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

        //Restart IIS
        void _RestartTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                //Process.Start(@"C:\Windows\System32\iisreset.exe");
                Process cmd = new Process();
                cmd.StartInfo.FileName = @"C:\Windows\System32\iisreset.exe";
                cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                cmd.Start();
            }
            catch { }
        }

        public string ClientCounter()
        {
            return ATWebPortClient.GetClientCounter().ToString();
        }

        private void StartService()
        {
            try
            {

                // Create the url that is needed to specify where the service should be started
                urlService = "net.tcp://" + _ipAddress.ToString() + ":" + _WebPort + "/ATWebPort";
                // Instruct the ServiceHost that the type that is used is a ServiceLibrary.service1
                ATWebPortHost = new ServiceHost(typeof(iWebPortServer));

                // The binding is where we can choose what transport layer we want to use. HTTP, TCP ect.
                var binding = new CustomNetTcpBinding();
                binding.OpenTimeout = TimeSpan.FromMinutes(2);
                binding.SendTimeout = TimeSpan.FromMinutes(2);
                binding.ReceiveTimeout = TimeSpan.FromMinutes(10);

                ATWebPortHost.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new CustomValidator();
                ATWebPortHost.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
                ATWebPortHost.AddServiceEndpoint(typeof(InterfaceWebPort), binding, urlService);

                ServiceMetadataBehavior metadataBehavior;
                metadataBehavior = ATWebPortHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (metadataBehavior == null)
                {
                    // This is how I create the proxy object that is generated via the svcutil.exe tool
                    metadataBehavior = new ServiceMetadataBehavior();
                    metadataBehavior.HttpGetUrl = new Uri("http://" + _ipAddress.ToString() + ":" + (Convert.ToInt32(_WebPort) + 1).ToString() + "/ATWebPort");
                    metadataBehavior.HttpGetEnabled = true;
                    metadataBehavior.ToString();

                    ATWebPortHost.Description.Behaviors.Add(metadataBehavior);
                    urlMeta = metadataBehavior.HttpGetUrl.ToString();
                }
                ATWebPortHost.Open();
            }
            catch (Exception ex) { MessageBox.Show("Message form iWebPort" + ": " + ex.ToString()); }// MessageBox.Show("Could not start the Service. Please check the firewall settings", "ATSCADA"); }
        }

        //Create a client of remote server
        private void ClientCreate()
        {
            try
            {
                endPointAddr = "net.tcp://" + _ipAddress.ToString() + ":" + _WebPort + "/ATWebPort";

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

            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
            //catch { MessageBox.Show("Could not create the native client. Please check the firewall settings", "ATSCADA"); } 
        }

    }

}

