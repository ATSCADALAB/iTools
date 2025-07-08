namespace ATSCADA.iWinTools.Logger
{
    partial class frmDataLoggerSettings
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
            this.grbSelectedTag = new System.Windows.Forms.GroupBox();
            this.cbxSelectedName = new ATSCADA.ToolExtensions.TagCollection.SmartTagComboBox();
            this.chkTrigger = new System.Windows.Forms.CheckBox();
            this.txtAlias = new System.Windows.Forms.TextBox();
            this.lblAlias = new System.Windows.Forms.Label();
            this.lblSelectedName = new System.Windows.Forms.Label();
            this.lstvDataLoggerSettings = new System.Windows.Forms.ListView();
            this.colSeletectedTag = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAlias = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTrigger = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAddUpdate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.grbSelectedTag.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbSelectedTag
            // 
            this.grbSelectedTag.Controls.Add(this.cbxSelectedName);
            this.grbSelectedTag.Controls.Add(this.chkTrigger);
            this.grbSelectedTag.Controls.Add(this.txtAlias);
            this.grbSelectedTag.Controls.Add(this.lblAlias);
            this.grbSelectedTag.Controls.Add(this.lblSelectedName);
            this.grbSelectedTag.Location = new System.Drawing.Point(15, 9);
            this.grbSelectedTag.Name = "grbSelectedTag";
            this.grbSelectedTag.Size = new System.Drawing.Size(608, 73);
            this.grbSelectedTag.TabIndex = 26;
            this.grbSelectedTag.TabStop = false;
            // 
            // cbxSelectedName
            // 
            this.cbxSelectedName.InRuntime = false;
            this.cbxSelectedName.Location = new System.Drawing.Point(10, 38);
            this.cbxSelectedName.Name = "cbxSelectedName";
            this.cbxSelectedName.Size = new System.Drawing.Size(273, 23);
            this.cbxSelectedName.TabIndex = 0;
            this.cbxSelectedName.TagName = "";
            // 
            // chkTrigger
            // 
            this.chkTrigger.AutoSize = true;
            this.chkTrigger.Location = new System.Drawing.Point(541, 40);
            this.chkTrigger.Name = "chkTrigger";
            this.chkTrigger.Size = new System.Drawing.Size(65, 19);
            this.chkTrigger.TabIndex = 2;
            this.chkTrigger.Text = "Trigger";
            this.chkTrigger.UseVisualStyleBackColor = true;
            // 
            // txtAlias
            // 
            this.txtAlias.Location = new System.Drawing.Point(292, 38);
            this.txtAlias.Name = "txtAlias";
            this.txtAlias.Size = new System.Drawing.Size(240, 21);
            this.txtAlias.TabIndex = 1;
            // 
            // lblAlias
            // 
            this.lblAlias.AutoSize = true;
            this.lblAlias.Location = new System.Drawing.Point(290, 13);
            this.lblAlias.Name = "lblAlias";
            this.lblAlias.Size = new System.Drawing.Size(158, 15);
            this.lblAlias.TabIndex = 2;
            this.lblAlias.Text = "Alias (without special chars)";
            // 
            // lblSelectedName
            // 
            this.lblSelectedName.AutoSize = true;
            this.lblSelectedName.Location = new System.Drawing.Point(7, 13);
            this.lblSelectedName.Name = "lblSelectedName";
            this.lblSelectedName.Size = new System.Drawing.Size(79, 15);
            this.lblSelectedName.TabIndex = 0;
            this.lblSelectedName.Text = "Selected Tag";
            // 
            // lstvDataLoggerSettings
            // 
            this.lstvDataLoggerSettings.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSeletectedTag,
            this.colAlias,
            this.colTrigger});
            this.lstvDataLoggerSettings.FullRowSelect = true;
            this.lstvDataLoggerSettings.GridLines = true;
            this.lstvDataLoggerSettings.HideSelection = false;
            this.lstvDataLoggerSettings.Location = new System.Drawing.Point(15, 90);
            this.lstvDataLoggerSettings.MultiSelect = false;
            this.lstvDataLoggerSettings.Name = "lstvDataLoggerSettings";
            this.lstvDataLoggerSettings.Size = new System.Drawing.Size(608, 204);
            this.lstvDataLoggerSettings.TabIndex = 27;
            this.lstvDataLoggerSettings.UseCompatibleStateImageBehavior = false;
            this.lstvDataLoggerSettings.View = System.Windows.Forms.View.Details;
            // 
            // colSeletectedTag
            // 
            this.colSeletectedTag.Text = "Selected Tag";
            this.colSeletectedTag.Width = 285;
            // 
            // colAlias
            // 
            this.colAlias.Text = "Alias";
            this.colAlias.Width = 248;
            // 
            // colTrigger
            // 
            this.colTrigger.Text = "Trigger";
            this.colTrigger.Width = 70;
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(306, 302);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(89, 28);
            this.btnDown.TabIndex = 6;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = true;
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(209, 302);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(89, 28);
            this.btnUp.TabIndex = 5;
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(112, 302);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(89, 28);
            this.btnRemove.TabIndex = 4;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            // 
            // btnAddUpdate
            // 
            this.btnAddUpdate.Location = new System.Drawing.Point(15, 302);
            this.btnAddUpdate.Name = "btnAddUpdate";
            this.btnAddUpdate.Size = new System.Drawing.Size(89, 28);
            this.btnAddUpdate.TabIndex = 3;
            this.btnAddUpdate.Text = "Add/Update";
            this.btnAddUpdate.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(534, 302);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 28);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(437, 302);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 28);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // frmDataLoggerSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 346);
            this.Controls.Add(this.grbSelectedTag);
            this.Controls.Add(this.lstvDataLoggerSettings);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAddUpdate);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDataLoggerSettings";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Data Logger Settings";
            this.grbSelectedTag.ResumeLayout(false);
            this.grbSelectedTag.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grbSelectedTag;
        private System.Windows.Forms.Label lblAlias;
        private System.Windows.Forms.Label lblSelectedName;
        private System.Windows.Forms.ListView lstvDataLoggerSettings;
        private System.Windows.Forms.ColumnHeader colSeletectedTag;
        private System.Windows.Forms.ColumnHeader colAlias;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAddUpdate;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox txtAlias;
        private System.Windows.Forms.ColumnHeader colTrigger;
        private System.Windows.Forms.CheckBox chkTrigger;
        private ToolExtensions.TagCollection.SmartTagComboBox cbxSelectedName;
    }
}