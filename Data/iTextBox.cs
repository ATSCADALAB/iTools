using ATSCADA.ToolExtensions.ExtensionMethods;
using ATSCADA.ToolExtensions.TagCollection;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Threading;
using System.Windows.Forms;

namespace ATSCADA.iWinTools.Data
{
    [ToolboxBitmap(typeof(System.Windows.Forms.TextBox))]
    public partial class iTextBox : System.Windows.Forms.TextBox
    {
        #region FILEDS

        private Color backColorDefault;

        private ITag tagControl;

        private iDriver driver;

        private CancellationTokenSource tokenSource;

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
        [Description("Background color when tag writed successfully.")]
        [TypeConverter(typeof(ColorConverter))]
        public Color ColorWriteSuccess { get; set; } = Color.FromKnownColor(KnownColor.GreenYellow);

        [Category("ATSCADA Settings")]
        [Description("Define the max length of the entered value.")]
        public new int MaxLength
        {
            get => base.MaxLength;
            set
            {
                if (value <= 0) return;
                base.MaxLength = value;
            }
        }

        [Category("ATSCADA Settings")]
        [Description("Max time(s) the control has input focus.")]
        public uint FocusedTime { get; set; } = 30;

        [Category("ATSCADA Settings")]
        [Description("If (true), change color when tag writed successfully.")]
        public bool IsFeedBack { get; set; } = true;

        #endregion

        #region CONSTRUCTORS

        public iTextBox()
        {
            base.MaxLength = 10;
            this.TabStop = false;
            this.TextAlign = HorizontalAlignment.Center;

        }

        #endregion

        #region METHODS

        private void Driver_ConstructionCompleted()
        {
            if (string.IsNullOrEmpty(TagName)) return;

            this.backColorDefault = this.BackColor;

            this.tagControl = this.driver.GetTagByName(TagName);
            if (this.tagControl == null) return;

            this.tagControl.TagValueChanged += (sender, e) => UpdateWhenTagValueChanged();
            this.tagControl.TagStatusChanged += (sender, e) => UpdateWhenTagValueChanged();
            ActionWrite();

            this.GotFocus += (sender, e) => ActionGotFocus();
            this.Leave += (sender, e) => ActionLeave();
            this.TextChanged += (sender, e) => ActionTextChanged();

            UpdateWhenTagValueChanged();
        }
        private void UpdateWhenTagValueChanged()
        {
            var newValue = this.tagControl.Value;
            this.SynchronizedInvokeAction(() =>
            {
                if (this.Focused) return;
                this.Text = newValue;
                this.BackColor = this.backColorDefault;
            });
        }

        private void ActionWrite()
        {
            if (this.tagControl is Tag externalTag && externalTag.AccessRight == "ReadOnly") return;

            if (IsFeedBack)
                this.tagControl.WriteStatusFeedback += (sender, e) => UpdateWhenTagWriteStatusFeedback(e.NewStatus);
            this.KeyPress += (sender, e) =>
            {
                if (e.KeyChar != (char)13) return;
                WriteValue();
            };
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

        private void WriteValue()
        {
            var text = this.Text.Trim();
            var length = text.Length;
            if (length == 0)
            {
                MessageBox.Show(
                    "Value is not null!",
                    "ATSCADA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }
            if (length > MaxLength)
            {
                MessageBox.Show(
                    "Input data length should not be longer than MaxLength!",
                    "ATSCADA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            this.tagControl.ASynWrite(text);
            this.SynchronizedInvokeAction(() =>
            {
                this.Enabled = false;
                this.Enabled = true;
            });
        }

        private void ActionGotFocus()
        {
            this.tokenSource = new CancellationTokenSource();
            var token = this.tokenSource.Token;
            System.Threading.Tasks.Task.Run(() =>
            {
                if (token.IsCancellationRequested) return;
                var count = 0;
                while (count < FocusedTime)
                {
                    if (token.IsCancellationRequested) return;
                    Thread.Sleep(1000);
                    count++;
                }
                this.SynchronizedInvokeAction(() =>
                {
                    this.Enabled = false;
                    this.Enabled = true;
                });

            }, token);
        }

        private void ActionLeave()
        {
            this.tokenSource?.Cancel();
        }

        private void ActionTextChanged()
        {
            if (!this.Text.Contains(Environment.NewLine)) return;
            this.SynchronizedInvokeAction(() =>
            {
                this.Text = this.Text.Replace(Environment.NewLine, "");
            });
        }

        #endregion
    }
}
