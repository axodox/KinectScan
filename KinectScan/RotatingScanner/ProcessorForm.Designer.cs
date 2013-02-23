namespace KinectScan
{
    partial class ProcessorForm
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
            this.TSMain = new System.Windows.Forms.ToolStrip();
            this.TSBCalibration = new System.Windows.Forms.ToolStripButton();
            this.TSBCalibrationSetA = new System.Windows.Forms.ToolStripButton();
            this.TSBSettings = new System.Windows.Forms.ToolStripButton();
            this.TSBCalibrationSetB = new System.Windows.Forms.ToolStripButton();
            this.TSBExitMode = new System.Windows.Forms.ToolStripButton();
            this.TSBCalibrationClear = new System.Windows.Forms.ToolStripButton();
            this.TSBCalibrationResetView = new System.Windows.Forms.ToolStripButton();
            this.TSDDBView = new System.Windows.Forms.ToolStripDropDownButton();
            this.TSS0 = new System.Windows.Forms.ToolStripSeparator();
            this.TSBBuildModel = new System.Windows.Forms.ToolStripButton();
            this.TSLLabel = new System.Windows.Forms.ToolStripLabel();
            this.TSTBLabel = new System.Windows.Forms.ToolStripTextBox();
            this.TSBSaveModel = new System.Windows.Forms.ToolStripButton();
            this.MMRotatingScanner = new System.Windows.Forms.MainMenu(this.components);
            this.MIProject = new System.Windows.Forms.MenuItem();
            this.MISelectWorkingDirectory = new System.Windows.Forms.MenuItem();
            this.MIModellingMode = new System.Windows.Forms.MenuItem();
            this.MICreateModel = new System.Windows.Forms.MenuItem();
            this.MIClearModel = new System.Windows.Forms.MenuItem();
            this.SSStatus = new System.Windows.Forms.StatusStrip();
            this.TSMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // TSMain
            // 
            this.TSMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.TSMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSBCalibration,
            this.TSBCalibrationSetA,
            this.TSBSettings,
            this.TSBCalibrationSetB,
            this.TSBExitMode,
            this.TSBCalibrationClear,
            this.TSBCalibrationResetView,
            this.TSDDBView,
            this.TSS0,
            this.TSBBuildModel,
            this.TSLLabel,
            this.TSTBLabel,
            this.TSBSaveModel});
            this.TSMain.Location = new System.Drawing.Point(0, 0);
            this.TSMain.Name = "TSMain";
            this.TSMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.TSMain.Size = new System.Drawing.Size(547, 25);
            this.TSMain.TabIndex = 0;
            this.TSMain.Text = "toolStrip1";
            // 
            // TSBCalibration
            // 
            this.TSBCalibration.Image = global::KinectScan.Resources.calibration16;
            this.TSBCalibration.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSBCalibration.Name = "TSBCalibration";
            this.TSBCalibration.Size = new System.Drawing.Size(85, 22);
            this.TSBCalibration.Text = "Calibration";
            this.TSBCalibration.Click += new System.EventHandler(this.TSBCalibration_Click);
            // 
            // TSBCalibrationSetA
            // 
            this.TSBCalibrationSetA.Image = global::KinectScan.Resources.seta16;
            this.TSBCalibrationSetA.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSBCalibrationSetA.Name = "TSBCalibrationSetA";
            this.TSBCalibrationSetA.Size = new System.Drawing.Size(90, 22);
            this.TSBCalibrationSetA.Text = "Set image A";
            this.TSBCalibrationSetA.Visible = false;
            this.TSBCalibrationSetA.Click += new System.EventHandler(this.TSBCalibrationSetA_Click);
            // 
            // TSBSettings
            // 
            this.TSBSettings.Image = global::KinectScan.Resources.settings16;
            this.TSBSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSBSettings.Name = "TSBSettings";
            this.TSBSettings.Size = new System.Drawing.Size(69, 22);
            this.TSBSettings.Text = "Settings";
            this.TSBSettings.Click += new System.EventHandler(this.TSBSettings_Click);
            // 
            // TSBCalibrationSetB
            // 
            this.TSBCalibrationSetB.Image = global::KinectScan.Resources.setb16;
            this.TSBCalibrationSetB.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSBCalibrationSetB.Name = "TSBCalibrationSetB";
            this.TSBCalibrationSetB.Size = new System.Drawing.Size(89, 22);
            this.TSBCalibrationSetB.Text = "Set image B";
            this.TSBCalibrationSetB.Visible = false;
            this.TSBCalibrationSetB.Click += new System.EventHandler(this.TSBCalibrationSetB_Click);
            // 
            // TSBExitMode
            // 
            this.TSBExitMode.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.TSBExitMode.Image = global::KinectScan.Resources.back16;
            this.TSBExitMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSBExitMode.Name = "TSBExitMode";
            this.TSBExitMode.Size = new System.Drawing.Size(45, 22);
            this.TSBExitMode.Text = "Exit";
            this.TSBExitMode.Visible = false;
            this.TSBExitMode.Click += new System.EventHandler(this.TSBExitMode_Click);
            // 
            // TSBCalibrationClear
            // 
            this.TSBCalibrationClear.Image = global::KinectScan.Resources.clear16;
            this.TSBCalibrationClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSBCalibrationClear.Name = "TSBCalibrationClear";
            this.TSBCalibrationClear.Size = new System.Drawing.Size(54, 22);
            this.TSBCalibrationClear.Text = "Clear";
            this.TSBCalibrationClear.Visible = false;
            this.TSBCalibrationClear.Click += new System.EventHandler(this.TSBCalibrationClear_Click);
            // 
            // TSBCalibrationResetView
            // 
            this.TSBCalibrationResetView.Image = global::KinectScan.Resources.resetview16;
            this.TSBCalibrationResetView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSBCalibrationResetView.Name = "TSBCalibrationResetView";
            this.TSBCalibrationResetView.Size = new System.Drawing.Size(82, 22);
            this.TSBCalibrationResetView.Text = "Reset view";
            this.TSBCalibrationResetView.Visible = false;
            this.TSBCalibrationResetView.Click += new System.EventHandler(this.TSBCalibrationResetView_Click);
            // 
            // TSDDBView
            // 
            this.TSDDBView.Image = global::KinectScan.Resources.view16;
            this.TSDDBView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSDDBView.Name = "TSDDBView";
            this.TSDDBView.Size = new System.Drawing.Size(61, 20);
            this.TSDDBView.Text = "View";
            // 
            // TSS0
            // 
            this.TSS0.Name = "TSS0";
            this.TSS0.Size = new System.Drawing.Size(6, 25);
            // 
            // TSBBuildModel
            // 
            this.TSBBuildModel.Image = global::KinectScan.Resources.buildmodel16;
            this.TSBBuildModel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSBBuildModel.Name = "TSBBuildModel";
            this.TSBBuildModel.Size = new System.Drawing.Size(91, 20);
            this.TSBBuildModel.Text = "Build model";
            this.TSBBuildModel.Click += new System.EventHandler(this.TSBBuildModel_Click);
            // 
            // TSLLabel
            // 
            this.TSLLabel.Name = "TSLLabel";
            this.TSLLabel.Size = new System.Drawing.Size(35, 15);
            this.TSLLabel.Text = "Label";
            // 
            // TSTBLabel
            // 
            this.TSTBLabel.Name = "TSTBLabel";
            this.TSTBLabel.Size = new System.Drawing.Size(100, 23);
            // 
            // TSBSaveModel
            // 
            this.TSBSaveModel.Image = global::KinectScan.Resources.save16;
            this.TSBSaveModel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSBSaveModel.Name = "TSBSaveModel";
            this.TSBSaveModel.Size = new System.Drawing.Size(88, 20);
            this.TSBSaveModel.Text = "Save model";
            this.TSBSaveModel.Click += new System.EventHandler(this.TSBSaveModel_Click);
            // 
            // MMRotatingScanner
            // 
            this.MMRotatingScanner.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.MIProject,
            this.MIModellingMode,
            this.MICreateModel,
            this.MIClearModel});
            // 
            // MIProject
            // 
            this.MIProject.Index = 0;
            this.MIProject.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.MISelectWorkingDirectory});
            this.MIProject.Text = "Project";
            // 
            // MISelectWorkingDirectory
            // 
            this.MISelectWorkingDirectory.Index = 0;
            this.MISelectWorkingDirectory.Text = "Select directory...";
            this.MISelectWorkingDirectory.Click += new System.EventHandler(this.MISelectWorkingDirectory_Click);
            // 
            // MIModellingMode
            // 
            this.MIModellingMode.Index = 1;
            this.MIModellingMode.Text = "Modelling mode";
            // 
            // MICreateModel
            // 
            this.MICreateModel.Index = 2;
            this.MICreateModel.Text = "Create model";
            // 
            // MIClearModel
            // 
            this.MIClearModel.Index = 3;
            this.MIClearModel.Text = "Clear model";
            this.MIClearModel.Click += new System.EventHandler(this.MIClearModel_Click);
            // 
            // SSStatus
            // 
            this.SSStatus.Location = new System.Drawing.Point(0, 300);
            this.SSStatus.Name = "SSStatus";
            this.SSStatus.Size = new System.Drawing.Size(547, 22);
            this.SSStatus.TabIndex = 1;
            this.SSStatus.Text = "statusStrip1";
            // 
            // ProcessorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 322);
            this.Controls.Add(this.SSStatus);
            this.Controls.Add(this.TSMain);
            this.Menu = this.MMRotatingScanner;
            this.Name = "ProcessorForm";
            this.Text = "Rotating scanner";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DebugForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DebugForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DebugForm_MouseUp);
            this.TSMain.ResumeLayout(false);
            this.TSMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip TSMain;
        private System.Windows.Forms.ToolStripButton TSBCalibration;
        private System.Windows.Forms.ToolStripButton TSBCalibrationSetA;
        private System.Windows.Forms.ToolStripButton TSBCalibrationSetB;
        private System.Windows.Forms.ToolStripButton TSBExitMode;
        private System.Windows.Forms.ToolStripButton TSBCalibrationClear;
        private System.Windows.Forms.ToolStripButton TSBCalibrationResetView;
        private System.Windows.Forms.ToolStripDropDownButton TSDDBView;
        private System.Windows.Forms.ToolStripButton TSBBuildModel;
        private System.Windows.Forms.ToolStripButton TSBSaveModel;
        private System.Windows.Forms.ToolStripButton TSBSettings;
        private System.Windows.Forms.ToolStripSeparator TSS0;
        private System.Windows.Forms.ToolStripLabel TSLLabel;
        private System.Windows.Forms.ToolStripTextBox TSTBLabel;
        private System.Windows.Forms.MainMenu MMRotatingScanner;
        private System.Windows.Forms.MenuItem MIProject;
        private System.Windows.Forms.MenuItem MISelectWorkingDirectory;
        private System.Windows.Forms.MenuItem MIModellingMode;
        private System.Windows.Forms.MenuItem MICreateModel;
        private System.Windows.Forms.StatusStrip SSStatus;
        private System.Windows.Forms.MenuItem MIClearModel;
    }
}