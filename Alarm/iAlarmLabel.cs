using ATSCADA.iWinTools.Alarm;
using ATSCADA.ToolExtensions.ExtensionMethods;
using ATSCADA.ToolExtensions.TagCollection;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace ATSCADA.iWinTools
{
    [ToolboxBitmap(typeof(System.Windows.Forms.Label))]
    public class iAlarmLabel : System.Windows.Forms.Label
    {
        private AlarmTag alarmTag;

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
        
        private void Driver_ConstructionCompleted()
        {
            if (string.IsNullOrEmpty(Tracking) ||
                string.IsNullOrEmpty(LowLevel) || 
                string.IsNullOrEmpty(HighLevel)) return;

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
                this.SynchronizedInvokeAction(() => this.Text = e.Condition.Message);

            this.SynchronizedInvokeAction(() => this.Text = alarmTag.ActiveCondition.Message);
        }
    }
}
