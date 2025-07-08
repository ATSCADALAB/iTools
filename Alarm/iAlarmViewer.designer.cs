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
            this.dgrvAlarmViewer = new System.Windows.Forms.DataGridView();
            this.btnACK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgrvAlarmViewer)).BeginInit();
            this.SuspendLayout();
            // 
            // dgrvAlarmViewer
            // 
            this.dgrvAlarmViewer.AllowUserToAddRows = false;
            this.dgrvAlarmViewer.AllowUserToDeleteRows = false;
            this.dgrvAlarmViewer.AllowUserToResizeRows = false;
            this.dgrvAlarmViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgrvAlarmViewer.BackgroundColor = System.Drawing.SystemColors.ActiveBorder;
            this.dgrvAlarmViewer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgrvAlarmViewer.Location = new System.Drawing.Point(17, 17);
            this.dgrvAlarmViewer.MultiSelect = false;
            this.dgrvAlarmViewer.Name = "dgrvAlarmViewer";
            this.dgrvAlarmViewer.ReadOnly = true;
            this.dgrvAlarmViewer.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgrvAlarmViewer.Size = new System.Drawing.Size(729, 227);
            this.dgrvAlarmViewer.TabIndex = 0;
            // 
            // btnACK
            // 
            this.btnACK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnACK.Location = new System.Drawing.Point(657, 252);
            this.btnACK.Name = "btnACK";
            this.btnACK.Size = new System.Drawing.Size(89, 28);
            this.btnACK.TabIndex = 37;
            this.btnACK.Text = "ACK";
            this.btnACK.UseVisualStyleBackColor = true;
            // 
            // iAlarmViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.btnACK);
            this.Controls.Add(this.dgrvAlarmViewer);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "iAlarmViewer";
            this.Size = new System.Drawing.Size(763, 297);
            ((System.ComponentModel.ISupportInitialize)(this.dgrvAlarmViewer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgrvAlarmViewer;
        private System.Windows.Forms.Button btnACK;
    }
}
