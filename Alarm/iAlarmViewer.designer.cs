namespace ATSCADA.iWinTools.Alarm
{
    partial class iAlarmViewer
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabActiveAlarm = new System.Windows.Forms.TabPage();
            this.dgrvActiveAlarm = new System.Windows.Forms.DataGridView();
            this.tabHistoryAlarm = new System.Windows.Forms.TabPage();
            this.dgrvHistoryAlarm = new System.Windows.Forms.DataGridView();
            this.tabControl.SuspendLayout();
            this.tabActiveAlarm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrvActiveAlarm)).BeginInit();
            this.tabHistoryAlarm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrvHistoryAlarm)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabActiveAlarm);
            this.tabControl.Controls.Add(this.tabHistoryAlarm);
            this.tabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.Location = new System.Drawing.Point(3, 3);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(757, 291);
            this.tabControl.TabIndex = 0;
            // 
            // tabActiveAlarm
            // 
            this.tabActiveAlarm.BackColor = System.Drawing.SystemColors.Control;
            this.tabActiveAlarm.Controls.Add(this.dgrvActiveAlarm);
            this.tabActiveAlarm.Location = new System.Drawing.Point(4, 24);
            this.tabActiveAlarm.Name = "tabActiveAlarm";
            this.tabActiveAlarm.Padding = new System.Windows.Forms.Padding(3);
            this.tabActiveAlarm.Size = new System.Drawing.Size(749, 263);
            this.tabActiveAlarm.TabIndex = 0;
            this.tabActiveAlarm.Text = "Active Alarms";
            // 
            // dgrvActiveAlarm
            // 
            this.dgrvActiveAlarm.AllowUserToAddRows = false;
            this.dgrvActiveAlarm.AllowUserToDeleteRows = false;
            this.dgrvActiveAlarm.AllowUserToResizeRows = false;
            this.dgrvActiveAlarm.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgrvActiveAlarm.BackgroundColor = System.Drawing.SystemColors.ActiveBorder;
            this.dgrvActiveAlarm.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgrvActiveAlarm.Location = new System.Drawing.Point(6, 6);
            this.dgrvActiveAlarm.MultiSelect = false;
            this.dgrvActiveAlarm.Name = "dgrvActiveAlarm";
            this.dgrvActiveAlarm.ReadOnly = true;
            this.dgrvActiveAlarm.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgrvActiveAlarm.Size = new System.Drawing.Size(737, 251);
            this.dgrvActiveAlarm.TabIndex = 0;
            // 
            // tabHistoryAlarm
            // 
            this.tabHistoryAlarm.BackColor = System.Drawing.SystemColors.Control;
            this.tabHistoryAlarm.Controls.Add(this.dgrvHistoryAlarm);
            this.tabHistoryAlarm.Location = new System.Drawing.Point(4, 24);
            this.tabHistoryAlarm.Name = "tabHistoryAlarm";
            this.tabHistoryAlarm.Padding = new System.Windows.Forms.Padding(3);
            this.tabHistoryAlarm.Size = new System.Drawing.Size(749, 263);
            this.tabHistoryAlarm.TabIndex = 1;
            this.tabHistoryAlarm.Text = "History Alarms";
            // 
            // dgrvHistoryAlarm
            // 
            this.dgrvHistoryAlarm.AllowUserToAddRows = false;
            this.dgrvHistoryAlarm.AllowUserToDeleteRows = false;
            this.dgrvHistoryAlarm.AllowUserToResizeRows = false;
            this.dgrvHistoryAlarm.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgrvHistoryAlarm.BackgroundColor = System.Drawing.SystemColors.ActiveBorder;
            this.dgrvHistoryAlarm.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgrvHistoryAlarm.Location = new System.Drawing.Point(6, 6);
            this.dgrvHistoryAlarm.MultiSelect = false;
            this.dgrvHistoryAlarm.Name = "dgrvHistoryAlarm";
            this.dgrvHistoryAlarm.ReadOnly = true;
            this.dgrvHistoryAlarm.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgrvHistoryAlarm.Size = new System.Drawing.Size(737, 251);
            this.dgrvHistoryAlarm.TabIndex = 0;
            // 
            // iAlarmViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "iAlarmViewer";
            this.Size = new System.Drawing.Size(763, 297);
            this.tabControl.ResumeLayout(false);
            this.tabActiveAlarm.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrvActiveAlarm)).EndInit();
            this.tabHistoryAlarm.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrvHistoryAlarm)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabActiveAlarm;
        private System.Windows.Forms.DataGridView dgrvActiveAlarm;
        private System.Windows.Forms.TabPage tabHistoryAlarm;
        private System.Windows.Forms.DataGridView dgrvHistoryAlarm;
    }
}