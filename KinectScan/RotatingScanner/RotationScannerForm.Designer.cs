namespace KinectScan
{
    partial class RotationScannerForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RotationScannerForm));
            this.MM = new System.Windows.Forms.MainMenu(this.components);
            this.MIFile = new System.Windows.Forms.MenuItem();
            this.MINewProject = new System.Windows.Forms.MenuItem();
            this.MIS1 = new System.Windows.Forms.MenuItem();
            this.MISave = new System.Windows.Forms.MenuItem();
            this.MISaveAs = new System.Windows.Forms.MenuItem();
            this.MIS2 = new System.Windows.Forms.MenuItem();
            this.MIClear = new System.Windows.Forms.MenuItem();
            this.MIS3 = new System.Windows.Forms.MenuItem();
            this.MIClose = new System.Windows.Forms.MenuItem();
            this.MIScan = new System.Windows.Forms.MenuItem();
            this.MICalibration = new System.Windows.Forms.MenuItem();
            this.MISettings = new System.Windows.Forms.MenuItem();
            this.SS = new System.Windows.Forms.StatusStrip();
            this.TSSL = new System.Windows.Forms.ToolStripStatusLabel();
            this.TSPB = new System.Windows.Forms.ToolStripProgressBar();
            this.SS.SuspendLayout();
            this.SuspendLayout();
            // 
            // MM
            // 
            this.MM.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.MIFile,
            this.MIScan,
            this.MICalibration,
            this.MISettings});
            // 
            // MIFile
            // 
            this.MIFile.Index = 0;
            this.MIFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.MINewProject,
            this.MIS1,
            this.MISave,
            this.MISaveAs,
            this.MIS2,
            this.MIClear,
            this.MIS3,
            this.MIClose});
            resources.ApplyResources(this.MIFile, "MIFile");
            // 
            // MINewProject
            // 
            this.MINewProject.Index = 0;
            resources.ApplyResources(this.MINewProject, "MINewProject");
            // 
            // MIS1
            // 
            this.MIS1.Index = 1;
            resources.ApplyResources(this.MIS1, "MIS1");
            // 
            // MISave
            // 
            this.MISave.Index = 2;
            resources.ApplyResources(this.MISave, "MISave");
            // 
            // MISaveAs
            // 
            this.MISaveAs.Index = 3;
            resources.ApplyResources(this.MISaveAs, "MISaveAs");
            // 
            // MIS2
            // 
            this.MIS2.Index = 4;
            resources.ApplyResources(this.MIS2, "MIS2");
            // 
            // MIClear
            // 
            this.MIClear.Index = 5;
            resources.ApplyResources(this.MIClear, "MIClear");
            // 
            // MIS3
            // 
            this.MIS3.Index = 6;
            resources.ApplyResources(this.MIS3, "MIS3");
            // 
            // MIClose
            // 
            this.MIClose.Index = 7;
            resources.ApplyResources(this.MIClose, "MIClose");
            // 
            // MIScan
            // 
            this.MIScan.Index = 1;
            resources.ApplyResources(this.MIScan, "MIScan");
            // 
            // MICalibration
            // 
            this.MICalibration.Index = 2;
            resources.ApplyResources(this.MICalibration, "MICalibration");
            this.MICalibration.Click += new System.EventHandler(this.MICalibration_Click);
            // 
            // MISettings
            // 
            this.MISettings.Index = 3;
            resources.ApplyResources(this.MISettings, "MISettings");
            this.MISettings.Click += new System.EventHandler(this.MISettings_Click);
            // 
            // SS
            // 
            this.SS.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSSL,
            this.TSPB});
            this.SS.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            resources.ApplyResources(this.SS, "SS");
            this.SS.Name = "SS";
            // 
            // TSSL
            // 
            this.TSSL.Name = "TSSL";
            resources.ApplyResources(this.TSSL, "TSSL");
            // 
            // TSPB
            // 
            this.TSPB.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.TSPB.Name = "TSPB";
            resources.ApplyResources(this.TSPB, "TSPB");
            // 
            // RotationScannerForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SS);
            this.Menu = this.MM;
            this.Name = "RotationScannerForm";
            this.SS.ResumeLayout(false);
            this.SS.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MainMenu MM;
        private System.Windows.Forms.MenuItem MIFile;
        private System.Windows.Forms.MenuItem MINewProject;
        private System.Windows.Forms.MenuItem MIS1;
        private System.Windows.Forms.MenuItem MISave;
        private System.Windows.Forms.MenuItem MISaveAs;
        private System.Windows.Forms.MenuItem MIS2;
        private System.Windows.Forms.MenuItem MIClear;
        private System.Windows.Forms.MenuItem MIS3;
        private System.Windows.Forms.MenuItem MIClose;
        private System.Windows.Forms.MenuItem MIScan;
        private System.Windows.Forms.MenuItem MICalibration;
        private System.Windows.Forms.MenuItem MISettings;
        private System.Windows.Forms.StatusStrip SS;
        private System.Windows.Forms.ToolStripStatusLabel TSSL;
        private System.Windows.Forms.ToolStripProgressBar TSPB;
    }
}