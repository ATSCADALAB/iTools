using ATSCADA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATSCADA.iWinTools.Loading
{
    public partial class iLoadingProcess : Component
    {
        //protected System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
        protected System.Windows.Forms.Timer _OpenLoadingScreenTimer = new System.Windows.Forms.Timer();
        protected iDriver _Driver;
        protected System.Timers.Timer _LoadingTimer = new System.Timers.Timer();
        protected int _LoadTimerRate = 2000;

        //Loading form properties
        protected int _Form_Size_Width = 500;
        protected int _Form_Size_Height = 400;
        protected Image _Loading_Image;
        protected string _Loading_Text = "LOADING...";
        protected string _Note_Text = "Take a rest with Coffee";
        public Loading_Procees_Screen loadingScreen;
        protected Form myForm;
        protected bool _Lock_Last_Form = false;
        //protected int _Time_to_Open_Loading_Screen = 200;

        public Form _Action_Form;

        //Add Tag

        [Description("Select Driver for SCADA control")]
        [Browsable(true), Category("ATSCADA Loading Form Settings")]
        public iDriver Driver
        {
            get
            {
                return _Driver;
            }
            set
            {

                _Driver = value;
                if (_Driver != null)
                {
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
        }

        [Description("Select Parent Form to hide while loading")]
        [Browsable(true), Category("ATSCADA Loading Form Settings")]
        public Form Parent_Form
        {
            get { return myForm; }
            set
            {
                myForm = value;
            }
        }


        [Description("Time Delay after finishing the loading process")]
        [Browsable(true), Category("ATSCADA Loading Form Settings")]
        public int Delay_Time
        {
            get { return _LoadTimerRate; }
            set { _LoadTimerRate = value; }
        }

        [Description("Width of the Loading Form")]
        [Browsable(true), Category("ATSCADA Loading Form Settings")]
        public int Size_Width
        {
            get { return _Form_Size_Width; }
            set
            {
                _Form_Size_Width = value;
            }
        }


        [Description("Height of the Loading Form")]
        [Browsable(true), Category("ATSCADA Loading Form Settings")]
        public int Size_Height
        {
            get { return _Form_Size_Height; }
            set
            {
                _Form_Size_Height = value;
            }
        }

        [Description("Add Loading Style Image of the Loading Form")]
        [Browsable(true), Category("ATSCADA Loading Form Settings")]
        public Image Loading_Image
        {
            get { return _Loading_Image; }
            set
            {
                _Loading_Image = value;
            }
        }


        [Description("Title Text for Loading Process")]
        [Browsable(true), Category("ATSCADA Loading Form Settings")]
        public string Loading_Text
        {
            get { return _Loading_Text; }
            set
            {
                _Loading_Text = value;
            }
        }

        [Description("Writing a note while application loading")]
        [Browsable(true), Category("ATSCADA Loading Form Settings")]
        public string Note_Text
        {
            get { return _Note_Text; }
            set
            {
                _Note_Text = value;
            }
        }

        [Description("Select to Lock or Unlock Previous Form")]
        [Browsable(true), Category("ATSCADA Loading Form Settings")]
        public bool Lock_LastForm
        {
            get { return _Lock_Last_Form; }
            set
            {
                _Lock_Last_Form = value;
            }
        }

        void _Driver_ConstructionCompleted()
        {
            //In runtime        
            // Timer Delay sau khi Driver_ConstructionCompleted
            if (_LoadTimerRate <= 100)
            {
                _LoadingTimer.Interval = 100;
            }
            else
            {
                _LoadingTimer.Interval = _LoadTimerRate;
            }
            _LoadingTimer.Elapsed += _LoadingTimer_Elapsed;
            _LoadingTimer.Start();
            // return;
        }

        //Close form loading and show Parent_Form
        private void _LoadingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _LoadingTimer.Stop();
            if (myForm != null)
            {
                loadingScreen.Invoke((MethodInvoker)delegate ()
            {
                _LoadingTimer.Dispose();
                loadingScreen.Close();
                loadingScreen.Dispose();
                //_Action_Form.WindowState = FormWindowState.Normal;
                if (_Lock_Last_Form == true)
                {
                    _Action_Form.ShowDialog();
                    //_Action_Form.WindowState = FormWindowState.Normal;                    
                }
                   
                else
                {
                    _Action_Form.Show();
                    //_Action_Form.WindowState = FormWindowState.Normal;
                }                    
            }
            );
            }
                
        }

        public iLoadingProcess()
        {
            InitializeComponent();           
        }

        //Khoi tao timer 10ms tai thoi diem Component duoc khoi tao khi chay chuong trinh 
        public iLoadingProcess(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            this._OpenLoadingScreenTimer.Interval = 10;
            this._OpenLoadingScreenTimer.Tick += _OpenLoadingScreenTimer_Tick;
            this._OpenLoadingScreenTimer.Enabled = true;
        }

        //Bật form loading và hide Parent Form
        private void _OpenLoadingScreenTimer_Tick(object sender, EventArgs e)
        {
            if (myForm != null)
            {
                {
                    _Action_Form = myForm.FindForm();
                    _OpenLoadingScreenTimer.Enabled = false;
                    _OpenLoadingScreenTimer.Dispose();

                    loadingScreen = new Loading_Procees_Screen();
                    loadingScreen.height = _Form_Size_Height;
                    loadingScreen.width = _Form_Size_Width;
                    loadingScreen.img = _Loading_Image;
                    loadingScreen.loadingTxt = _Loading_Text;
                    loadingScreen.noteTxt = _Note_Text;
                    loadingScreen.Show();

                    foreach (Form f in Application.OpenForms)
                    {
                        if (f == _Action_Form)
                        {
                            f.Hide();
                        }
                        if (f == loadingScreen)
                        {
                            f.TopMost = true;
                        }
                    }
                }
            }
        }
    }
}
