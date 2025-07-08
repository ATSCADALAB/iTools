namespace ATSCADA.iWinTools.Trend
{
    partial class iRealtimeTrend
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
            this.components = new System.ComponentModel.Container();
            this.trendViewer = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // trendViewer
            // 
            this.trendViewer.BackColor = System.Drawing.SystemColors.Control;
            this.trendViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trendViewer.IsEnableSelection = true;
            this.trendViewer.IsShowPointValues = true;
            this.trendViewer.Location = new System.Drawing.Point(0, 0);
            this.trendViewer.Name = "trendViewer";
            this.trendViewer.ScrollGrace = 0D;
            this.trendViewer.ScrollMaxX = 0D;
            this.trendViewer.ScrollMaxY = 0D;
            this.trendViewer.ScrollMaxY2 = 0D;
            this.trendViewer.ScrollMinX = 0D;
            this.trendViewer.ScrollMinY = 0D;
            this.trendViewer.ScrollMinY2 = 0D;
            this.trendViewer.Size = new System.Drawing.Size(577, 264);
            this.trendViewer.TabIndex = 0;
            // 
            // iRealtimeTrend
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.trendViewer);
            this.Name = "iRealtimeTrend";
            this.Size = new System.Drawing.Size(577, 264);
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl trendViewer;
    }
}
