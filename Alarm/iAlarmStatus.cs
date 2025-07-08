using ATSCADA.ToolExtensions.ExtensionMethods;
using ATSCADA.ToolExtensions.TagCollection;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace ATSCADA.iWinTools.Alarm
{
    public partial class iAlarmStatus : UserControl
    {
        private AlarmTag alarmTag;

        private Color colorNonActive;

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
        [Description("Enter low level of Alarm, the value will be written between 2 quotation marks (\"\").\nIf select from SmartTag editor, the low level will by the value of tag is selected.")]
        [Editor(typeof(SmartTagEditor), typeof(UITypeEditor))]
        public string LowLevel { get; set; }

        [Category("ATSCADA Settings")]
        [Description("Enter high level of Alarm, the value will be written between 2 quotation marks (\"\").\nIf select from SmartTag editor, the high level will by the value of tag is selected.")]
        [Editor(typeof(SmartTagEditor), typeof(UITypeEditor))]
        public string HighLevel { get; set; }

        [Category("ATSCADA Settings")]
        [Description("Background color when the alarm has actived.")]
        [TypeConverter(typeof(ColorConverter))]
        public Color ColorActive { get; set; } = Color.FromKnownColor(KnownColor.Red);

        public iAlarmStatus()
        {
            InitializeComponent();            
        }

        private void Driver_ConstructionCompleted()
        {
            if (string.IsNullOrEmpty(Tracking) ||
                string.IsNullOrEmpty(LowLevel) || 
                string.IsNullOrEmpty(HighLevel)) return;

            this.colorNonActive = this.BackColor;            
            this.alarmTag = new AlarmTag(this.driver, new AlarmParametter()
            {
                Tracking = this.Tracking,
                LowLevel = this.LowLevel,
                HighLevel = this.HighLevel
            });

            if (this.alarmTag.ActiveCondition == null) return;

            ActionAlarm();            
        }

        private void ActionAlarm()
        {
            this.alarmTag.StatusChanged += (sender, e) =>
            {
                if (e.Condition.Status == AlarmStatus.Normal)
                    this.SynchronizedInvokeAction(() => this.BackColor = this.colorNonActive);
                else
                    this.SynchronizedInvokeAction(() => this.BackColor = ColorActive);
            };

            if (alarmTag.ActiveCondition.Status == AlarmStatus.Normal)
                this.SynchronizedInvokeAction(() => this.BackColor = this.colorNonActive);
            else
                this.SynchronizedInvokeAction(() => this.BackColor = ColorActive);
        }
    }
}
