using ATSCADA.ToolExtensions.ExtensionMethods;
using ATSCADA.ToolExtensions.TagCollection;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace ATSCADA.iWinTools.Data
{
    [ToolboxBitmap(typeof(System.Windows.Forms.Button))]
    public partial class iButton : System.Windows.Forms.Button
    {
        #region FIELDS

        private string textDefault;

        private Color textColorDefault;

        private Color backColorDefault;

        private ITag tagControl;

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
        [Description("Value active.")]
        public string ValueActive { get; set; } = "1";

        [Category("ATSCADA Settings")]
        [Description("Value inactive.")]
        public string ValueInactive { get; set; } = "0";

        [Category("ATSCADA Settings")]
        [Description("Text active.")]
        public string TextActive { get; set; } = "ON";

        [Category("ATSCADA Settings")]
        [Description("Text inactive.")]
        public string TextInactive { get; set; } = "OFF";

        [Category("ATSCADA Settings")]
        [Description("Text color when value of tag is 'ValueActive'.")]
        [TypeConverter(typeof(ColorConverter))]
        public Color TextColorActive { get; set; } = Color.White;

        [Category("ATSCADA Settings")]
        [Description("Text color when value of tag is 'ValueInactive'.")]
        [TypeConverter(typeof(ColorConverter))]
        public Color TextColorInactive { get; set; } = Color.White;

        [Category("ATSCADA Settings")]
        [Description("Background color when value of tag is 'ValueActive'.")]
        [TypeConverter(typeof(ColorConverter))]
        public Color ColorActive { get; set; } = Color.Green;

        [Category("ATSCADA Settings")]
        [Description("Background color when value of tag is 'ValueInactive'.")]
        [TypeConverter(typeof(ColorConverter))]
        public Color ColorInactive { get; set; } = Color.Red;
        
        [Category("ATSCADA Settings")]
        [Description("Background color when tag writed successfully.")]
        [TypeConverter(typeof(ColorConverter))]
        public Color ColorWriteSuccess { get; set; } = Color.Gray;

        [Category("ATSCADA Settings")]
        [Description("If (true), change color when tag writed successfully.")]
        public bool IsFeedBack { get; set; } = true;

        #endregion

        #region CONSTRUCTORS

        public iButton() => FlatStyle = System.Windows.Forms.FlatStyle.Flat;

        #endregion

        #region

        public void Driver_ConstructionCompleted()
        {
            if (string.IsNullOrEmpty(TagName)) return;

            this.textDefault = this.Text;
            this.textColorDefault = this.ForeColor;
            this.backColorDefault = this.BackColor;

            this.tagControl = this.driver.GetTagByName(TagName);
            if (this.tagControl is null) return;

            this.tagControl.TagValueChanged += (sender, e) => UpdateWhenTagValueChanged();
            this.tagControl.TagStatusChanged += (sender, e) => UpdateWhenTagValueChanged();
            ActionWrite();
            UpdateWhenTagValueChanged();
        }

        private void ActionWrite()
        {
            if (this.tagControl is Tag externalTag && externalTag?.AccessRight == "ReadOnly") return;
            if (IsFeedBack)
                this.tagControl.WriteStatusFeedback += (sender, e) => 
                UpdateWhenTagWriteStatusFeedback(e.NewStatus);
            this.Click += (sender, e) =>
            {
                var currentValue = this.tagControl.Value.Trim();
                if (string.Equals(currentValue, ValueActive))
                    this.tagControl.ASynWrite(ValueInactive);
                else if (string.Equals(currentValue, ValueInactive))
                    this.tagControl.ASynWrite(ValueActive);
                else
                    this.tagControl.ASynWrite(ValueInactive);
            };
        }

        private void UpdateWhenTagWriteStatusFeedback(string newStatus)
        {
            if (newStatus != "Good") return;
            this.SynchronizedInvokeAction(async () => 
            {
                this.BackColor = ColorWriteSuccess;
                await System.Threading.Tasks.Task.Delay(1000);
                UpdateWhenTagValueChanged();                
            });           
        }

        private void UpdateWhenTagValueChanged()
        {
            var newValue = this.tagControl.Value.Trim();
            this.SynchronizedInvokeAction(() =>
            {
                if (string.Equals(newValue, ValueActive))
                {
                    this.Text = TextActive;
                    this.ForeColor = TextColorActive;
                    this.BackColor = ColorActive;
                    
                }                    
                else if (string.Equals(newValue, ValueInactive))
                {
                    this.Text = TextInactive;
                    this.ForeColor = TextColorActive;
                    this.BackColor = ColorInactive;                    
                }
                else
                {
                    this.Text = this.textDefault;
                    this.ForeColor = this.textColorDefault;
                    this.BackColor = this.backColorDefault;                    
                }                   
            });
        }

        #endregion
    }
}
