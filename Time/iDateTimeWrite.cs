using ATSCADA.ToolExtensions.ExtensionMethods;
using ATSCADA.ToolExtensions.TagCollection;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace ATSCADA.iWinTools.Time
{
    [ToolboxBitmap(typeof(System.Windows.Forms.Timer))]
    public partial class iDateTimeWrite : Component
    {
        private ITag tagControl;

        private int timeRate = 1;

        private double interval = 1000;

        private System.Timers.Timer tmrDateTimeWrite;

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
        [Description("Time rate (second) of timer.")]
        public int TimeRate
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
        [Description("Format datetime.")]
        public string Format { get; set; } = "dd/MM/yyyy HH:mm:ss";
        public iDateTimeWrite()
        {
            InitializeComponent();
        }

        public iDateTimeWrite(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        private void Driver_ConstructionCompleted()
        {
            this.tagControl = DriverExtensionMethod.GetTagByName(this.driver, TagName);
            if (this.tagControl == null) return;

            ActionWrite();
        }

        private void ActionWrite()
        {
            this.tmrDateTimeWrite = new System.Timers.Timer();
            this.tmrDateTimeWrite.Interval = this.interval;
            this.tmrDateTimeWrite.AutoReset = false;

            this.tmrDateTimeWrite.Elapsed += (sender, e) => UpdateValue();
            this.tmrDateTimeWrite.Start();
        }

        private void UpdateValue()
        {
            try
            {
                this.tmrDateTimeWrite.Stop();

                var value = System.DateTime.Now.ToString(Format);
                this.tagControl.ASynWrite(value);

                this.tmrDateTimeWrite.Start();
            }
            catch
            {
                this.tmrDateTimeWrite.Start();
            }
        }
    }
}
