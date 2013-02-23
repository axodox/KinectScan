namespace KinectScan
{
    partial class CalibrationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalibrationForm));
            this.TSCalibrate = new System.Windows.Forms.ToolStrip();
            this.TSBCapture = new System.Windows.Forms.ToolStripButton();
            this.TSBTable = new System.Windows.Forms.ToolStripButton();
            this.TSS1 = new System.Windows.Forms.ToolStripSeparator();
            this.TSBCylinder = new System.Windows.Forms.ToolStripButton();
            this.TSBCalibrate = new System.Windows.Forms.ToolStripButton();
            this.TSCalibrate.SuspendLayout();
            this.SuspendLayout();
            // 
            // TSCalibrate
            // 
            this.TSCalibrate.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSBCapture,
            this.TSS1,
            this.TSBTable,
            this.TSBCylinder,
            this.TSBCalibrate});
            this.TSCalibrate.Location = new System.Drawing.Point(0, 0);
            this.TSCalibrate.Name = "TSCalibrate";
            this.TSCalibrate.Size = new System.Drawing.Size(330, 25);
            this.TSCalibrate.TabIndex = 0;
            this.TSCalibrate.Text = "toolStrip1";
            // 
            // TSBCapture
            // 
            this.TSBCapture.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.TSBCapture.Enabled = false;
            this.TSBCapture.Image = ((System.Drawing.Image)(resources.GetObject("TSBCapture.Image")));
            this.TSBCapture.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSBCapture.Name = "TSBCapture";
            this.TSBCapture.Size = new System.Drawing.Size(53, 22);
            this.TSBCapture.Text = "Capture";
            this.TSBCapture.Click += new System.EventHandler(this.TSBCapture_Click);
            // 
            // TSBTable
            // 
            this.TSBTable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.TSBTable.Image = ((System.Drawing.Image)(resources.GetObject("TSBTable.Image")));
            this.TSBTable.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSBTable.Name = "TSBTable";
            this.TSBTable.Size = new System.Drawing.Size(71, 22);
            this.TSBTable.Text = "Select table";
            this.TSBTable.Click += new System.EventHandler(this.TSBTable_Click);
            // 
            // TSS1
            // 
            this.TSS1.Name = "TSS1";
            this.TSS1.Size = new System.Drawing.Size(6, 25);
            // 
            // TSBCylinder
            // 
            this.TSBCylinder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.TSBCylinder.Image = ((System.Drawing.Image)(resources.GetObject("TSBCylinder.Image")));
            this.TSBCylinder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSBCylinder.Name = "TSBCylinder";
            this.TSBCylinder.Size = new System.Drawing.Size(87, 22);
            this.TSBCylinder.Text = "Select cylinder";
            this.TSBCylinder.Click += new System.EventHandler(this.TSBSelectCylinder_Click);
            // 
            // TSBCalibrate
            // 
            this.TSBCalibrate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.TSBCalibrate.Image = ((System.Drawing.Image)(resources.GetObject("TSBCalibrate.Image")));
            this.TSBCalibrate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSBCalibrate.Name = "TSBCalibrate";
            this.TSBCalibrate.Size = new System.Drawing.Size(58, 22);
            this.TSBCalibrate.Text = "Calibrate";
            this.TSBCalibrate.Click += new System.EventHandler(this.TSBCalibrate_Click);
            // 
            // CalibrationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 261);
            this.Controls.Add(this.TSCalibrate);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CalibrationForm";
            this.Text = "CalibrationForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CalibrationForm_FormClosing);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CalibrationForm_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CalibrationForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CalibrationForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CalibrationForm_MouseUp);
            this.TSCalibrate.ResumeLayout(false);
            this.TSCalibrate.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip TSCalibrate;
        private System.Windows.Forms.ToolStripButton TSBCapture;
        private System.Windows.Forms.ToolStripSeparator TSS1;
        private System.Windows.Forms.ToolStripButton TSBTable;
        private System.Windows.Forms.ToolStripButton TSBCylinder;
        private System.Windows.Forms.ToolStripButton TSBCalibrate;
    }
}