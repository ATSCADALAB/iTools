using ATSCADA.ToolExtensions.ExtensionMethods;
using ATSCADA.ToolExtensions.TagCollection;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace ATSCADA.iWinTools.Data
{
    public partial class iStatus : UserControl
    {
        #region FIELDS

        private ITag tag;

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
        [Description("Background color when status of tag is Good.")]
        [TypeConverter(typeof(ColorConverter))]
        public Color ColorGood { get; set; } = Color.LimeGreen;

        [Category("ATSCADA Settings")]
        [Description("Background color when status of tag is Bad.")]
        [TypeConverter(typeof(ColorConverter))]
        public Color ColorBad { get; set; } = Color.Red;

        #endregion

        #region CONSTRUCTORS

        public iStatus() => InitializeComponent();

        #endregion

        #region METHODS

        private void Driver_ConstructionCompleted()
        {
            this.tag = DriverExtensionMethod.GetTagByName(this.driver, TagName);
            if (this.tag == null) return;

            this.tag.TagStatusChanged += (sender, e) => UpdateDisplay();

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            this.SynchronizedInvokeAction(() =>
            {
                if (this.tag.Status == "Good") this.BackColor = ColorGood;
                else this.BackColor = ColorBad;
            });
        }

        #endregion
    }
}
