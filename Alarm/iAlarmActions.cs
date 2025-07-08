using ATSCADA.ToolExtensions.Data;
using ATSCADA.ToolExtensions.ExtensionMethods;
using ATSCADA.ToolExtensions.TagCollection;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace ATSCADA.iWinTools.Alarm
{
    [ToolboxBitmap(typeof(System.Diagnostics.EventLog))]
    public partial class iAlarmActions : Component
    {
        private ITag outputTag;

        private AlarmTag alarmTag;       
      
        private DataTool dataWriteOnAlarm;

        private DataTool dataWriteOffAlarm;

        private iDriver driver;

        [Category("ATSCADA Settings")]
        [Description("Select driver object.")]
        public iDriver Driver
        {
            get => driver;
            set
            {
                if (driver != null) driver.ConstructionCompleted -= Driver_ConstructionCompleted;
                driver = value;
                if (driver != null) driver.ConstructionCompleted += Driver_ConstructionCompleted;
            }
        }

        [Category("ATSCADA Settings")]
        [Description("Select tracking tag Alarm.")]
        [Editor(typeof(SmartTagEditor), typeof(UITypeEditor))]
        public string Tracking { get; set; }

        [Category("ATSCADA Settings")]
        [Description("Select output tag.")]
        [Editor(typeof(SmartTagEditor), typeof(UITypeEditor))]
        public string Output { get; set; }

        [Category("ATSCADA Settings")]
        [Description("Enter low level of Alarm, the value will be written between 2 quotation marks (\"\").\nIf select from SmartTag editor, the low level will by the value of tag is selected.")]
        [Editor(typeof(SmartTagEditor), typeof(UITypeEditor))]
        public string LowLevel { get; set; }

        [Category("ATSCADA Settings")]
        [Description("Enter high level of Alarm, the value will be written between 2 quotation marks (\"\").\nIf select from SmartTag editor, the high level will by the value of tag is selected.")]
        [Editor(typeof(SmartTagEditor), typeof(UITypeEditor))]
        public string HighLevel { get; set; }       

        [Category("ATSCADA Settings")]
        [Description("Enter value to write tag when alarm has turned on, the value will be written between 2 quotation marks (\"\").\nIf select from SmartTag editor, the write value will by the value of tag is selected.")]
        [Editor(typeof(SmartTagEditor), typeof(UITypeEditor))]
        public string ValueOnAlarm { get; set; }

        [Category("ATSCADA Settings")]
        [Description("Enter value to write tag when alarm has turned off, the value will be written between 2 quotation marks (\"\").\nIf select from SmartTag editor, the write value will by the value of tag is selected.")]
        [Editor(typeof(SmartTagEditor), typeof(UITypeEditor))]
        public string ValueOffAlarm { get; set; }

        public iAlarmActions()
        {
            InitializeComponent();
        }

        public iAlarmActions(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        public void Driver_ConstructionCompleted()
        {
            if (string.IsNullOrEmpty(Tracking) || string.IsNullOrEmpty(Output) ||
                string.IsNullOrEmpty(LowLevel) || string.IsNullOrEmpty(HighLevel) ||
                string.IsNullOrEmpty(ValueOnAlarm) || string.IsNullOrEmpty(ValueOffAlarm)) return;

            this.outputTag = this.driver.GetTagByName(Output);
            if (this.outputTag is null) return;

            this.alarmTag = new AlarmTag(this.driver, new AlarmParametter()
            {
                Tracking = this.Tracking,
                LowLevel = this.LowLevel,
                HighLevel = this.HighLevel
            });

            if (this.alarmTag.ActiveCondition == null) return;
                   
            this.dataWriteOffAlarm = new DataTool(this.driver, ValueOffAlarm);
            this.dataWriteOnAlarm = new DataTool(this.driver, ValueOnAlarm);           
           
            ActionAlarm();
        }

        private void ActionAlarm()
        {
            this.alarmTag.StatusChanged += (sender, e) =>
            {
                if (this.alarmTag.ActiveCondition.Status == AlarmStatus.Normal)
                    this.outputTag.ASynWrite(this.dataWriteOffAlarm.Value);
                else
                    this.outputTag.ASynWrite(this.dataWriteOnAlarm.Value);
            };

            if (this.alarmTag.ActiveCondition.Status == AlarmStatus.Normal)
                this.outputTag.ASynWrite(this.dataWriteOffAlarm.Value);
            else
                this.outputTag.ASynWrite(this.dataWriteOnAlarm.Value);
        }

    }
}
