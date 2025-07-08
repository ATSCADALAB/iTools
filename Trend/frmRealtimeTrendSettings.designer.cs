namespace ATSCADA.iWinTools.Trend
{
    partial class frmRealtimeTrendSettings
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.colLineWidth = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLineColor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFillColor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAlias = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSelectedTag = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lstvReatimeTrendSettings = new System.Windows.Forms.ListView();
            this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtAlias = new System.Windows.Forms.TextBox();
            this.lblSelectedAlias = new System.Windows.Forms.Label();
            this.lblSelectedName = new System.Windows.Forms.Label();
            this.grbSettings = new System.Windows.Forms.GroupBox();
            this.cbxType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbxTagName = new ATSCADA.ToolExtensions.TagCollection.SmartTagComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbxLineWidth = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxFillColor = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAddUpdate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.cbxLineColor = new System.Windows.Forms.ComboBox();
            this.grbSettings.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // colLineWidth
            // 
            this.colLineWidth.Text = "Line Width";
            this.colLineWidth.Width = 80;
            // 
            // colLineColor
            // 
            this.colLineColor.Text = "Line Color";
            this.colLineColor.Width = 110;
            // 
            // colFillColor
            // 
            this.colFillColor.Text = "Fill Color";
            this.colFillColor.Width = 110;
            // 
            // colAlias
            // 
            this.colAlias.Text = "Alias";
            this.colAlias.Width = 120;
            // 
            // colSelectedTag
            // 
            this.colSelectedTag.Text = "Selected Tag";
            this.colSelectedTag.Width = 160;
            // 
            // lstvReatimeTrendSettings
            // 
            this.lstvReatimeTrendSettings.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSelectedTag,
            this.colAlias,
            this.colType,
            this.colFillColor,
            this.colLineColor,
            this.colLineWidth});
            this.lstvReatimeTrendSettings.FullRowSelect = true;
            this.lstvReatimeTrendSettings.GridLines = true;
            this.lstvReatimeTrendSettings.HideSelection = false;
            this.lstvReatimeTrendSettings.Location = new System.Drawing.Point(14, 145);
            this.lstvReatimeTrendSettings.MultiSelect = false;
            this.lstvReatimeTrendSettings.Name = "lstvReatimeTrendSettings";
            this.lstvReatimeTrendSettings.Size = new System.Drawing.Size(660, 257);
            this.lstvReatimeTrendSettings.TabIndex = 38;
            this.lstvReatimeTrendSettings.UseCompatibleStateImageBehavior = false;
            this.lstvReatimeTrendSettings.View = System.Windows.Forms.View.Details;
            // 
            // colType
            // 
            this.colType.Text = "Type";
            this.colType.Width = 78;
            // 
            // txtAlias
            // 
            this.txtAlias.Location = new System.Drawing.Point(79, 57);
            this.txtAlias.Name = "txtAlias";
            this.txtAlias.Size = new System.Drawing.Size(234, 21);
            this.txtAlias.TabIndex = 3;
            // 
            // lblSelectedAlias
            // 
            this.lblSelectedAlias.AutoSize = true;
            this.lblSelectedAlias.Location = new System.Drawing.Point(10, 60);
            this.lblSelectedAlias.Name = "lblSelectedAlias";
            this.lblSelectedAlias.Size = new System.Drawing.Size(33, 15);
            this.lblSelectedAlias.TabIndex = 2;
            this.lblSelectedAlias.Text = "Alias";
            // 
            // lblSelectedName
            // 
            this.lblSelectedName.AutoSize = true;
            this.lblSelectedName.Location = new System.Drawing.Point(10, 28);
            this.lblSelectedName.Name = "lblSelectedName";
            this.lblSelectedName.Size = new System.Drawing.Size(41, 15);
            this.lblSelectedName.TabIndex = 0;
            this.lblSelectedName.Text = "Name";
            // 
            // grbSettings
            // 
            this.grbSettings.Controls.Add(this.cbxType);
            this.grbSettings.Controls.Add(this.label2);
            this.grbSettings.Controls.Add(this.cbxTagName);
            this.grbSettings.Controls.Add(this.txtAlias);
            this.grbSettings.Controls.Add(this.lblSelectedAlias);
            this.grbSettings.Controls.Add(this.lblSelectedName);
            this.grbSettings.Location = new System.Drawing.Point(15, 9);
            this.grbSettings.Name = "grbSettings";
            this.grbSettings.Size = new System.Drawing.Size(325, 126);
            this.grbSettings.TabIndex = 37;
            this.grbSettings.TabStop = false;
            this.grbSettings.Text = "Trend Tag";
            // 
            // cbxType
            // 
            this.cbxType.FormattingEnabled = true;
            this.cbxType.Items.AddRange(new object[] {
            "Line",
            "Bar"});
            this.cbxType.Location = new System.Drawing.Point(79, 90);
            this.cbxType.Name = "cbxType";
            this.cbxType.Size = new System.Drawing.Size(234, 23);
            this.cbxType.TabIndex = 27;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 15);
            this.label2.TabIndex = 26;
            this.label2.Text = "Type";
            // 
            // cbxTagName
            // 
            this.cbxTagName.InRuntime = false;
            this.cbxTagName.Location = new System.Drawing.Point(79, 25);
            this.cbxTagName.Name = "cbxTagName";
            this.cbxTagName.Size = new System.Drawing.Size(234, 23);
            this.cbxTagName.TabIndex = 26;
            this.cbxTagName.TagName = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbxLineColor);
            this.groupBox1.Controls.Add(this.cbxLineWidth);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cbxFillColor);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(349, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(325, 126);
            this.groupBox1.TabIndex = 38;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // cbxLineWidth
            // 
            this.cbxLineWidth.FormattingEnabled = true;
            this.cbxLineWidth.Items.AddRange(new object[] {
            "0.1",
            "0.5",
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.cbxLineWidth.Location = new System.Drawing.Point(79, 89);
            this.cbxLineWidth.Name = "cbxLineWidth";
            this.cbxLineWidth.Size = new System.Drawing.Size(234, 23);
            this.cbxLineWidth.TabIndex = 24;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 15);
            this.label1.TabIndex = 23;
            this.label1.Text = "Line Color";
            // 
            // cbxFillColor
            // 
            this.cbxFillColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxFillColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxFillColor.FormattingEnabled = true;
            this.cbxFillColor.Location = new System.Drawing.Point(79, 25);
            this.cbxFillColor.Name = "cbxFillColor";
            this.cbxFillColor.Size = new System.Drawing.Size(234, 22);
            this.cbxFillColor.TabIndex = 22;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 15);
            this.label4.TabIndex = 21;
            this.label4.Text = "Line Width";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 15);
            this.label5.TabIndex = 20;
            this.label5.Text = "Fill Color";
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(304, 411);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(89, 28);
            this.btnDown.TabIndex = 42;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = true;
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(207, 411);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(89, 28);
            this.btnUp.TabIndex = 41;
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(110, 411);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(89, 28);
            this.btnRemove.TabIndex = 40;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            // 
            // btnAddUpdate
            // 
            this.btnAddUpdate.Location = new System.Drawing.Point(13, 411);
            this.btnAddUpdate.Name = "btnAddUpdate";
            this.btnAddUpdate.Size = new System.Drawing.Size(89, 28);
            this.btnAddUpdate.TabIndex = 39;
            this.btnAddUpdate.Text = "Add/Update";
            this.btnAddUpdate.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(585, 411);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 28);
            this.btnCancel.TabIndex = 44;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(488, 411);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 28);
            this.btnOK.TabIndex = 43;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // cbxLineColor
            // 
            this.cbxLineColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxLineColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxLineColor.FormattingEnabled = true;
            this.cbxLineColor.Location = new System.Drawing.Point(79, 57);
            this.cbxLineColor.Name = "cbxLineColor";
            this.cbxLineColor.Size = new System.Drawing.Size(234, 22);
            this.cbxLineColor.TabIndex = 25;
            // 
            // frmRealtimeTrendSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 454);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAddUpdate);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lstvReatimeTrendSettings);
            this.Controls.Add(this.grbSettings);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmRealtimeTrendSettings";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Realtime Trend Settings";
            this.grbSettings.ResumeLayout(false);
            this.grbSettings.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ColumnHeader colLineWidth;
        private System.Windows.Forms.ColumnHeader colLineColor;
        private System.Windows.Forms.ColumnHeader colFillColor;
        private System.Windows.Forms.ColumnHeader colAlias;
        private System.Windows.Forms.ColumnHeader colSelectedTag;
        private System.Windows.Forms.ListView lstvReatimeTrendSettings;
        private System.Windows.Forms.TextBox txtAlias;
        private System.Windows.Forms.Label lblSelectedAlias;
        private System.Windows.Forms.Label lblSelectedName;
        private System.Windows.Forms.GroupBox grbSettings;
        private ToolExtensions.TagCollection.SmartTagComboBox cbxTagName;
        private System.Windows.Forms.ComboBox cbxType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbxLineWidth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbxFillColor;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAddUpdate;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ComboBox cbxLineColor;
    }
}