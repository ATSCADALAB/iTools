namespace ATSCADA.iWinTools.Alarm
{
    partial class iAlarmSettings
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAddUpdate = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.lstvAlarmLoggerSettings = new System.Windows.Forms.ListView();
            this.colTrackingTag = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAlias = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colHighLevel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLowLevel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colEmail = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.grbEmail = new System.Windows.Forms.GroupBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.grbAlarmTag = new System.Windows.Forms.GroupBox();
            this.cbxLowLevel = new ATSCADA.ToolExtensions.TagCollection.SmartTagComboBox();
            this.cbxTracking = new ATSCADA.ToolExtensions.TagCollection.SmartTagComboBox();
            this.cbxHighLevel = new ATSCADA.ToolExtensions.TagCollection.SmartTagComboBox();
            this.txtAlias = new System.Windows.Forms.TextBox();
            this.lblLowLevel = new System.Windows.Forms.Label();
            this.lblAlias = new System.Windows.Forms.Label();
            this.lblTracking = new System.Windows.Forms.Label();
            this.lblHighLevel = new System.Windows.Forms.Label();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.tstStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tstContent = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblExample = new System.Windows.Forms.Label();
            this.grbEmail.SuspendLayout();
            this.grbAlarmTag.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDown.Location = new System.Drawing.Point(308, 400);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(89, 28);
            this.btnDown.TabIndex = 35;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = true;
            // 
            // btnUp
            // 
            this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUp.Location = new System.Drawing.Point(211, 400);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(89, 28);
            this.btnUp.TabIndex = 34;
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemove.Location = new System.Drawing.Point(114, 400);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(89, 28);
            this.btnRemove.TabIndex = 33;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            // 
            // btnAddUpdate
            // 
            this.btnAddUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddUpdate.Location = new System.Drawing.Point(17, 400);
            this.btnAddUpdate.Name = "btnAddUpdate";
            this.btnAddUpdate.Size = new System.Drawing.Size(89, 28);
            this.btnAddUpdate.TabIndex = 32;
            this.btnAddUpdate.Text = "Add/Update";
            this.btnAddUpdate.UseVisualStyleBackColor = true;
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(622, 400);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(89, 28);
            this.btnApply.TabIndex = 36;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            // 
            // lstvAlarmLoggerSettings
            // 
            this.lstvAlarmLoggerSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstvAlarmLoggerSettings.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colTrackingTag,
            this.colAlias,
            this.colHighLevel,
            this.colLowLevel,
            this.colEmail});
            this.lstvAlarmLoggerSettings.FullRowSelect = true;
            this.lstvAlarmLoggerSettings.GridLines = true;
            this.lstvAlarmLoggerSettings.HideSelection = false;
            this.lstvAlarmLoggerSettings.Location = new System.Drawing.Point(17, 173);
            this.lstvAlarmLoggerSettings.MultiSelect = false;
            this.lstvAlarmLoggerSettings.Name = "lstvAlarmLoggerSettings";
            this.lstvAlarmLoggerSettings.Size = new System.Drawing.Size(694, 219);
            this.lstvAlarmLoggerSettings.TabIndex = 31;
            this.lstvAlarmLoggerSettings.UseCompatibleStateImageBehavior = false;
            this.lstvAlarmLoggerSettings.View = System.Windows.Forms.View.Details;
            // 
            // colTrackingTag
            // 
            this.colTrackingTag.Text = "Tracking Tag";
            this.colTrackingTag.Width = 150;
            // 
            // colAlias
            // 
            this.colAlias.Text = "Alias";
            this.colAlias.Width = 100;
            // 
            // colHighLevel
            // 
            this.colHighLevel.Text = "HighLevel";
            this.colHighLevel.Width = 150;
            // 
            // colLowLevel
            // 
            this.colLowLevel.Text = "LowLevel";
            this.colLowLevel.Width = 150;
            // 
            // colEmail
            // 
            this.colEmail.Text = "Email";
            this.colEmail.Width = 140;
            // 
            // grbEmail
            // 
            this.grbEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grbEmail.Controls.Add(this.lblExample);
            this.grbEmail.Controls.Add(this.txtEmail);
            this.grbEmail.Location = new System.Drawing.Point(361, 9);
            this.grbEmail.Name = "grbEmail";
            this.grbEmail.Size = new System.Drawing.Size(350, 156);
            this.grbEmail.TabIndex = 30;
            this.grbEmail.TabStop = false;
            this.grbEmail.Text = "Email To";
            // 
            // txtEmail
            // 
            this.txtEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEmail.Location = new System.Drawing.Point(11, 26);
            this.txtEmail.Multiline = true;
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(329, 95);
            this.txtEmail.TabIndex = 4;
            // 
            // grbAlarmTag
            // 
            this.grbAlarmTag.Controls.Add(this.cbxLowLevel);
            this.grbAlarmTag.Controls.Add(this.cbxTracking);
            this.grbAlarmTag.Controls.Add(this.cbxHighLevel);
            this.grbAlarmTag.Controls.Add(this.txtAlias);
            this.grbAlarmTag.Controls.Add(this.lblLowLevel);
            this.grbAlarmTag.Controls.Add(this.lblAlias);
            this.grbAlarmTag.Controls.Add(this.lblTracking);
            this.grbAlarmTag.Controls.Add(this.lblHighLevel);
            this.grbAlarmTag.Location = new System.Drawing.Point(17, 9);
            this.grbAlarmTag.Name = "grbAlarmTag";
            this.grbAlarmTag.Size = new System.Drawing.Size(334, 156);
            this.grbAlarmTag.TabIndex = 29;
            this.grbAlarmTag.TabStop = false;
            this.grbAlarmTag.Text = "Alarm Tag";
            // 
            // cbxLowLevel
            // 
            this.cbxLowLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxLowLevel.InRuntime = false;
            this.cbxLowLevel.Location = new System.Drawing.Point(89, 120);
            this.cbxLowLevel.Name = "cbxLowLevel";
            this.cbxLowLevel.Size = new System.Drawing.Size(234, 23);
            this.cbxLowLevel.TabIndex = 3;
            this.cbxLowLevel.TagName = "";
            // 
            // cbxTracking
            // 
            this.cbxTracking.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxTracking.InRuntime = false;
            this.cbxTracking.Location = new System.Drawing.Point(89, 58);
            this.cbxTracking.Name = "cbxTracking";
            this.cbxTracking.Size = new System.Drawing.Size(234, 23);
            this.cbxTracking.TabIndex = 1;
            this.cbxTracking.TagName = "";
            // 
            // cbxHighLevel
            // 
            this.cbxHighLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxHighLevel.InRuntime = false;
            this.cbxHighLevel.Location = new System.Drawing.Point(89, 27);
            this.cbxHighLevel.Name = "cbxHighLevel";
            this.cbxHighLevel.Size = new System.Drawing.Size(234, 23);
            this.cbxHighLevel.TabIndex = 0;
            this.cbxHighLevel.TagName = "";
            // 
            // txtAlias
            // 
            this.txtAlias.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAlias.BackColor = System.Drawing.Color.LightCyan;
            this.txtAlias.Location = new System.Drawing.Point(89, 89);
            this.txtAlias.Multiline = true;
            this.txtAlias.Name = "txtAlias";
            this.txtAlias.Size = new System.Drawing.Size(234, 23);
            this.txtAlias.TabIndex = 2;
            // 
            // lblLowLevel
            // 
            this.lblLowLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLowLevel.AutoSize = true;
            this.lblLowLevel.Location = new System.Drawing.Point(7, 124);
            this.lblLowLevel.Name = "lblLowLevel";
            this.lblLowLevel.Size = new System.Drawing.Size(62, 15);
            this.lblLowLevel.TabIndex = 3;
            this.lblLowLevel.Text = "Low Level";
            // 
            // lblAlias
            // 
            this.lblAlias.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAlias.AutoSize = true;
            this.lblAlias.Location = new System.Drawing.Point(7, 93);
            this.lblAlias.Name = "lblAlias";
            this.lblAlias.Size = new System.Drawing.Size(33, 15);
            this.lblAlias.TabIndex = 2;
            this.lblAlias.Text = "Alias";
            // 
            // lblTracking
            // 
            this.lblTracking.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTracking.AutoSize = true;
            this.lblTracking.Location = new System.Drawing.Point(7, 62);
            this.lblTracking.Name = "lblTracking";
            this.lblTracking.Size = new System.Drawing.Size(54, 15);
            this.lblTracking.TabIndex = 1;
            this.lblTracking.Text = "Tracking";
            // 
            // lblHighLevel
            // 
            this.lblHighLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHighLevel.AutoSize = true;
            this.lblHighLevel.Location = new System.Drawing.Point(7, 31);
            this.lblHighLevel.Name = "lblHighLevel";
            this.lblHighLevel.Size = new System.Drawing.Size(65, 15);
            this.lblHighLevel.TabIndex = 0;
            this.lblHighLevel.Text = "High Level";
            // 
            // statusBar
            // 
            this.statusBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tstStatus,
            this.tstContent});
            this.statusBar.Location = new System.Drawing.Point(0, 445);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(728, 22);
            this.statusBar.TabIndex = 37;
            this.statusBar.Text = "statusStrip1";
            // 
            // tstStatus
            // 
            this.tstStatus.Name = "tstStatus";
            this.tstStatus.Size = new System.Drawing.Size(47, 17);
            this.tstStatus.Text = "Status :";
            // 
            // tstContent
            // 
            this.tstContent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tstContent.ForeColor = System.Drawing.Color.Green;
            this.tstContent.Name = "tstContent";
            this.tstContent.Size = new System.Drawing.Size(635, 17);
            this.tstContent.Spring = true;
            // 
            // lblExample
            // 
            this.lblExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblExample.AutoSize = true;
            this.lblExample.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblExample.Location = new System.Drawing.Point(8, 128);
            this.lblExample.Name = "lblExample";
            this.lblExample.Size = new System.Drawing.Size(233, 15);
            this.lblExample.TabIndex = 6;
            this.lblExample.Text = "Pattern:  abc@gmail.com, def@xyz.com,..";
            // 
            // iAlarmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAddUpdate);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.lstvAlarmLoggerSettings);
            this.Controls.Add(this.grbEmail);
            this.Controls.Add(this.grbAlarmTag);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "iAlarmSettings";
            this.Size = new System.Drawing.Size(728, 467);
            this.grbEmail.ResumeLayout(false);
            this.grbEmail.PerformLayout();
            this.grbAlarmTag.ResumeLayout(false);
            this.grbAlarmTag.PerformLayout();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAddUpdate;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.ListView lstvAlarmLoggerSettings;
        private System.Windows.Forms.ColumnHeader colTrackingTag;
        private System.Windows.Forms.ColumnHeader colAlias;
        private System.Windows.Forms.ColumnHeader colHighLevel;
        private System.Windows.Forms.ColumnHeader colLowLevel;
        private System.Windows.Forms.ColumnHeader colEmail;
        private System.Windows.Forms.GroupBox grbEmail;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.GroupBox grbAlarmTag;
        private ToolExtensions.TagCollection.SmartTagComboBox cbxLowLevel;
        private ToolExtensions.TagCollection.SmartTagComboBox cbxTracking;
        private ToolExtensions.TagCollection.SmartTagComboBox cbxHighLevel;
        private System.Windows.Forms.TextBox txtAlias;
        private System.Windows.Forms.Label lblLowLevel;
        private System.Windows.Forms.Label lblAlias;
        private System.Windows.Forms.Label lblTracking;
        private System.Windows.Forms.Label lblHighLevel;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel tstStatus;
        private System.Windows.Forms.ToolStripStatusLabel tstContent;
        private System.Windows.Forms.Label lblExample;
    }
}
