using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ATSCADA.ToolExtensions.ExtensionMethods;
using ATSCADA.ToolExtensions.TagCollection;
using System.Drawing.Design;

namespace ATSCADA.iWinTools.Time
{
    [ToolboxBitmap(typeof(System.Windows.Forms.Timer))]
    public partial class iOverTimeStamp : UserControl
    {
        private ITag tagControl;

        private int timeRate = 10;

        private double interval = 10000;

        private System.Timers.Timer tmrOverTimeStamp;

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
        [Description("Select tag.")]
        [Editor(typeof(SmartTagEditor), typeof(UITypeEditor))]
        public string TagName { get; set; }

        [Category("ATSCADA Settings")]
        [Description("Overtime (second).")]
        public int OverTime
        {
            get => this.timeRate;
            set
            {
                if (value < 1) return;

                this.timeRate = value;
                this.interval = value * 1000;
            }
        }

        [Category("ATSCADA Settings")]
        [Description("Background color when timestamp in time.")]
        [TypeConverter(typeof(ColorConverter))]
        public Color ColorGood { get; set; } = Color.FromKnownColor(KnownColor.LimeGreen);

        [Category("ATSCADA Settings")]
        [Description("Background color when timestamp in over time.")]
        [TypeConverter(typeof(ColorConverter))]
        public Color ColorBad { get; set; } = Color.FromKnownColor(KnownColor.Red);

        public iOverTimeStamp()
        {
            InitializeComponent();
        }

        private void Driver_ConstructionCompleted()
        {
            this.tagControl = DriverExtensionMethod.GetTagByName(this.driver, TagName);
            if (this.tagControl == null) return;

            ActionStatus();
        }

        private void ActionStatus()
        {
            this.tmrOverTimeStamp = new System.Timers.Timer();
            this.tmrOverTimeStamp.Interval = this.interval;
            this.tmrOverTimeStamp.AutoReset = false;

            this.tmrOverTimeStamp.Elapsed += (sender, e) => UpdateStatus();
            this.tmrOverTimeStamp.Start();
        }

        private void UpdateStatus()
        {
            try
            {
                this.tmrOverTimeStamp.Stop();

                var timeStampParse = System.DateTime.ParseExact(this.tagControl.TimeStamp, "HH:mm:ss:fff",
                                       System.Globalization.CultureInfo.InvariantCulture);

                var subTimeStamp = System.DateTime.Now.Subtract(timeStampParse).Seconds;

                if (subTimeStamp > this.timeRate)
                    this.SynchronizedInvokeAction(() => this.BackColor = ColorBad);
                else
                    this.SynchronizedInvokeAction(() => this.BackColor = ColorGood);

                this.tmrOverTimeStamp.Start();
            }
            catch
            {
                this.SynchronizedInvokeAction(() => this.BackColor = ColorBad);
                this.tmrOverTimeStamp.Start();
            }
        }
    }
}
