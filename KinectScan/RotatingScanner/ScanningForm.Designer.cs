namespace KinectScan
{
    partial class ScanningForm
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
            this.PBScanning = new System.Windows.Forms.ProgressBar();
            this.BScan = new System.Windows.Forms.Button();
            this.LProgress = new System.Windows.Forms.Label();
            this.BStop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // PBScanning
            // 
            this.PBScanning.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PBScanning.Enabled = false;
            this.PBScanning.Location = new System.Drawing.Point(12, 41);
            this.PBScanning.Maximum = 360;
            this.PBScanning.Name = "PBScanning";
            this.PBScanning.Size = new System.Drawing.Size(621, 23);
            this.PBScanning.TabIndex = 0;
            // 
            // BScan
            // 
            this.BScan.Location = new System.Drawing.Point(12, 12);
            this.BScan.Name = "BScan";
            this.BScan.Size = new System.Drawing.Size(75, 23);
            this.BScan.TabIndex = 1;
            this.BScan.Text = "Scan";
            this.BScan.UseVisualStyleBackColor = true;
            this.BScan.Click += new System.EventHandler(this.BScan_Click);
            // 
            // LProgress
            // 
            this.LProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LProgress.Location = new System.Drawing.Point(93, 17);
            this.LProgress.Name = "LProgress";
            this.LProgress.Size = new System.Drawing.Size(540, 13);
            this.LProgress.TabIndex = 2;
            this.LProgress.Text = "LProgress";
            this.LProgress.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // BStop
            // 
            this.BStop.Location = new System.Drawing.Point(96, 12);
            this.BStop.Name = "BStop";
            this.BStop.Size = new System.Drawing.Size(75, 23);
            this.BStop.TabIndex = 3;
            this.BStop.Text = "Stop";
            this.BStop.UseVisualStyleBackColor = true;
            this.BStop.Click += new System.EventHandler(this.BStop_Click);
            // 
            // ScanningForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(645, 75);
            this.Controls.Add(this.BStop);
            this.Controls.Add(this.LProgress);
            this.Controls.Add(this.BScan);
            this.Controls.Add(this.PBScanning);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ScanningForm";
            this.ShowInTaskbar = false;
            this.Text = "Scanning";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScanningForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar PBScanning;
        private System.Windows.Forms.Button BScan;
        private System.Windows.Forms.Label LProgress;
        private System.Windows.Forms.Button BStop;
    }
}