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
    public partial class iInput : UserControl
    {
        #region FIELDS

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
        [Description("The font used to display text of button.")]
        [TypeConverter(typeof(FontConverter))]
        public Font ButtonFont
        {
            get => this.iButton.Font;
            set => this.iButton.Font = value;
        }

        [Category("ATSCADA Settings")]
        [Description("The background color of button.")]
        [TypeConverter(typeof(ColorConverter))]
        public Color ButtonBackColor
        {
            get => this.iButton.BackColor;
            set => this.iButton.BackColor = value;
        }

        [Category("ATSCADA Settings")]
        [Description("The foreground color of button, which is used to display text.")]
        [TypeConverter(typeof(ColorConverter))]
        public Color ButtonForeColor
        {
            get => this.iButton.ForeColor;
            set => this.iButton.ForeColor = value;
        }

        [Category("ATSCADA Settings")]
        [Description("The font used to display text of textbox.")]
        [TypeConverter(typeof(FontConverter))]
        public Font TextBoxFont
        {
            get => this.iTextBox.Font;
            set => this.iTextBox.Font = value;
        }

        [Category("ATSCADA Settings")]
        [Description("The background color of textbox.")]
        [TypeConverter(typeof(ColorConverter))]
        public Color TextBoxBackColor
        {
            get => this.iTextBox.BackColor;
            set => this.iTextBox.BackColor = value;
        }

        [Category("ATSCADA Settings")]
        [Description("The foreground color of textbox, which is used to display text.")]
        [TypeConverter(typeof(ColorConverter))]
        public Color TextBoxForeColor
        {
            get => this.iTextBox.ForeColor;
            set => this.iTextBox.ForeColor = value;
        }

        [Category("ATSCADA Settings")]
        [Description("Background color when tag writed successfully.")]
        [TypeConverter(typeof(ColorConverter))]
        public Color ColorWriteSuccess { get; set; } = Color.FromKnownColor(KnownColor.GreenYellow);

        [Category("ATSCADA Settings")]
        [Description("Define the max length of the entered value.")]
        public int MaxLength
        {
            get => this.iTextBox.MaxLength;
            set
            {
                if (value <= 0) return;
                this.iTextBox.MaxLength = value;
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

        public iInput()
        {
            InitializeComponent();

            iTextBox.Multiline = true;
            iTextBox.TabStop = false;
            iTextBox.TextAlign = HorizontalAlignment.Center;
            iTextBox.KeyDown += ITextBox_KeyDown;
            iTextBox.Text = "0000";
        }

        #endregion

        #region METHODS

        private void ITextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                e.SuppressKeyPress = true;
        }

        private void Driver_ConstructionCompleted()
        {
            if (string.IsNullOrEmpty(TagName)) return;

            this.backColorDefault = this.iTextBox.BackColor;

            this.tagControl = this.driver.GetTagByName(TagName);
            if (this.tagControl == null) return;

            this.tagControl.TagValueChanged += (sender, e) => UpdateWhenTagValueChanged();
            this.tagControl.TagStatusChanged += (sender, e) => UpdateWhenTagValueChanged();
            ActionWrite();

            this.iTextBox.GotFocus += (sender, e) => ActionGotFocus();
            this.iTextBox.Leave += (sender, e) => ActionLeave();
            this.iTextBox.TextChanged += (sender, e) => ActionTextChanged();

            UpdateWhenTagValueChanged();
        }

        private void UpdateWhenTagValueChanged()
        {
            var newValue = this.tagControl.Value;
            this.iTextBox.SynchronizedInvokeAction(() =>
            {
                if (this.iTextBox.Focused) return;
                this.iTextBox.Text = newValue;
                this.iTextBox.BackColor = this.backColorDefault;
            });
        }

        private void ActionWrite()
        {
            if (this.tagControl is Tag externalTag && externalTag.AccessRight == "ReadOnly") return;

            if (IsFeedBack)
                this.tagControl.WriteStatusFeedback += (sender, e) => UpdateWhenTagWriteStatusFeedback(e.NewStatus);
            this.iButton.Click += (sender, e) => WriteValue();
        }

        private void UpdateWhenTagWriteStatusFeedback(string newStatus)
        {
            if (newStatus != "Good") return;
            this.SynchronizedInvokeAction(async () =>
            {
                this.iTextBox.BackColor = ColorWriteSuccess;
                await System.Threading.Tasks.Task.Delay(1000);
                this.iTextBox.BackColor = this.backColorDefault;
            });
        }

        private void WriteValue()
        {
            var text = this.iTextBox.Text.Trim();
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
            this.iTextBox.SynchronizedInvokeAction(() =>
            {
                this.iTextBox.Enabled = false;
                this.iTextBox.Enabled = true;
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
                this.iTextBox.SynchronizedInvokeAction(() =>
                {
                    this.iTextBox.Enabled = false;
                    this.iTextBox.Enabled = true;
                });

            }, token);
        }

        private void ActionLeave()
        {
            this.tokenSource?.Cancel();
        }

        private void ActionTextChanged()
        {
            if (!this.iTextBox.Text.Contains(Environment.NewLine)) return;
            this.iTextBox.SynchronizedInvokeAction(() =>
            {
                this.iTextBox.Text = this.Text.Replace(Environment.NewLine, "");
            });
        }

        #endregion;
    }
}
