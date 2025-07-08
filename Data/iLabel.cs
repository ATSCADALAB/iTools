using ATSCADA.ToolExtensions.ExtensionMethods;
using ATSCADA.ToolExtensions.TagCollection;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace ATSCADA.iWinTools.Data
{
    [ToolboxBitmap(typeof(System.Windows.Forms.Label))]
    public partial class iLabel : System.Windows.Forms.Label
    {
        private ITag tag;

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
        [Description("Select tag for ATSCADA control.")]
        [Editor(typeof(SmartTagEditor), typeof(UITypeEditor))]
        public string TagName { get; set; }

        private void Driver_ConstructionCompleted()
        {
            if (string.IsNullOrEmpty(TagName)) return;

            this.tag = this.driver.GetTagByName(TagName);
            if (this.tag is null) return;

            this.tag.TagValueChanged += (sender, e) => UpdateDisplay();
            this.tag.TagStatusChanged += (sender, e) => UpdateDisplay();
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            this.SynchronizedInvokeAction(() => 
            { 
                this.Text = this.tag.Value; 
            });
        }
    }
}
