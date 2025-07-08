using ATSCADA.ToolExtensions.Data;
using ATSCADA.ToolExtensions.ExtensionMethods;
using ATSCADA.ToolExtensions.TagCollection;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace ATSCADA.iWinTools.Data
{
    [ToolboxBitmap(typeof(System.Windows.Forms.Button))]
    public partial class iButtonWrite : System.Windows.Forms.Button
    {
        #region FILEDS

        private Color backColorDefault;

        private ITag tagControl;

        private DataTool dataWrite;

        private iDriver driver;

        #endregion

        #region PROPERTIES

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
        [Description("Select tag for ATSCADA control.")]
        [Editor(typeof(SmartTagEditor), typeof(UITypeEditor))]
        public string TagName { get; set; }

        [Category("ATSCADA Settings")]
        [Description("Enter value to write tag, the value will be written between 2 quotation marks (\"\").\nIf select from SmartTag editor, the write value will by the value of tag is selected.")]
        [Editor(typeof(SmartTagEditor), typeof(UITypeEditor))]
        public string Value { get; set; }

        [Category("ATSCADA Settings")]
        [Description("Background color when tag writed successfully.")]
        [TypeConverter(typeof(ColorConverter))]
        public Color ColorWriteSuccess { get; set; } = Color.GreenYellow;

        [Category("ATSCADA Settings")]
        [Description("If (true), change color when tag writed successfully.")]
        public bool IsFeedBack { get; set; } = true;

        #endregion

        #region CONSTRUCTORS

        public iButtonWrite() => FlatStyle = System.Windows.Forms.FlatStyle.Flat;

        #endregion

        #region METHODS

        public void Driver_ConstructionCompleted()
        {
            if (string.IsNullOrEmpty(TagName)) return;

            this.backColorDefault = this.BackColor;
            this.tagControl = this.driver.GetTagByName(TagName);
            if (this.tagControl is null) return;

            ActionWrite(Value);
        }

        private void ActionWrite(string valueWrite)
        {
            if (this.tagControl is Tag externalTag && externalTag?.AccessRight == "ReadOnly") return;

            this.dataWrite = new DataTool(this.driver, valueWrite);
            this.Click += (sender, e) => this.tagControl.ASynWrite(this.dataWrite.Value);

            if (IsFeedBack)
                this.tagControl.WriteStatusFeedback += (sender, e) => UpdateWhenTagWriteStatusFeedback(e.NewStatus);
        }

        private void UpdateWhenTagWriteStatusFeedback(string newStatus)
        {
            if (newStatus != "Good") return;
            this.SynchronizedInvokeAction(async () => 
            { 
                this.BackColor = ColorWriteSuccess;
                await System.Threading.Tasks.Task.Delay(1000);
                this.BackColor = this.backColorDefault;
            });            
        }

        #endregion
    }
}

