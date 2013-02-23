using KinectScan.Properties;
namespace KinectScan
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.MIFile = new System.Windows.Forms.MenuItem();
            this.MISaveTo = new System.Windows.Forms.MenuItem();
            this.MIWorkingDirectory = new System.Windows.Forms.MenuItem();
            this.MIFileSeparator = new System.Windows.Forms.MenuItem();
            this.MIOpenSavedModel = new System.Windows.Forms.MenuItem();
            this.MIOpenRawData = new System.Windows.Forms.MenuItem();
            this.MIDevices = new System.Windows.Forms.MenuItem();
            this.MIStart = new System.Windows.Forms.MenuItem();
            this.MIStop = new System.Windows.Forms.MenuItem();
            this.MIDisconnect = new System.Windows.Forms.MenuItem();
            this.MICalibration = new System.Windows.Forms.MenuItem();
            this.MIDevicesRefresh = new System.Windows.Forms.MenuItem();
            this.MIDevicesSeparator = new System.Windows.Forms.MenuItem();
            this.MIModes = new System.Windows.Forms.MenuItem();
            this.MITilt = new System.Windows.Forms.MenuItem();
            this.MITiltHorizontal = new System.Windows.Forms.MenuItem();
            this.MITiltUp = new System.Windows.Forms.MenuItem();
            this.MITiltDown = new System.Windows.Forms.MenuItem();
            this.MIDevicesSeparator2 = new System.Windows.Forms.MenuItem();
            this.MIVirtualDevice = new System.Windows.Forms.MenuItem();
            this.MIView = new System.Windows.Forms.MenuItem();
            this.MIRotateRight = new System.Windows.Forms.MenuItem();
            this.MIRotateLeft = new System.Windows.Forms.MenuItem();
            this.MIResetView = new System.Windows.Forms.MenuItem();
            this.MISettings = new System.Windows.Forms.MenuItem();
            this.SBStatus = new System.Windows.Forms.StatusBar();
            this.TSMain = new System.Windows.Forms.ToolStrip();
            this.TSBStartStop = new System.Windows.Forms.ToolStripButton();
            this.TSSMainSeparatorFile = new System.Windows.Forms.ToolStripSeparator();
            this.TSBFilePrevious = new System.Windows.Forms.ToolStripButton();
            this.TSLFileCount = new System.Windows.Forms.ToolStripLabel();
            this.TSBFileNext = new System.Windows.Forms.ToolStripButton();
            this.TSSMainSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.TSBProbes = new System.Windows.Forms.ToolStripButton();
            this.TSBRotateLeft = new System.Windows.Forms.ToolStripButton();
            this.TSBRotateRight = new System.Windows.Forms.ToolStripButton();
            this.TSBResetView = new System.Windows.Forms.ToolStripButton();
            this.TSDDBShading = new System.Windows.Forms.ToolStripDropDownButton();
            this.TSSMainSeparatorSave = new System.Windows.Forms.ToolStripSeparator();
            this.TSLLabel = new System.Windows.Forms.ToolStripLabel();
            this.TSTBLabel = new System.Windows.Forms.ToolStripTextBox();
            this.TSBSave = new System.Windows.Forms.ToolStripSplitButton();
            this.TSBSequenceSave = new System.Windows.Forms.ToolStripButton();
            this.XPanel = new System.Windows.Forms.Panel();
            this.TSSequence = new System.Windows.Forms.ToolStrip();
            this.TSLSequenceLabel = new System.Windows.Forms.ToolStripLabel();
            this.TSSSequence0 = new System.Windows.Forms.ToolStripSeparator();
            this.TSBSequenceRecord = new System.Windows.Forms.ToolStripButton();
            this.TSBSequencePause = new System.Windows.Forms.ToolStripButton();
            this.TSBSequenceStop = new System.Windows.Forms.ToolStripButton();
            this.TSSSequence1 = new System.Windows.Forms.ToolStripSeparator();
            this.TSLSequenceIntervalLabel = new System.Windows.Forms.ToolStripLabel();
            this.TSBSequenceDecreaseInterval = new System.Windows.Forms.ToolStripButton();
            this.TSLSequenceInterval = new System.Windows.Forms.ToolStripLabel();
            this.TSBSequenceIncreaseInterval = new System.Windows.Forms.ToolStripButton();
            this.TSSSequnce2 = new System.Windows.Forms.ToolStripSeparator();
            this.TSLSequencePerformance = new System.Windows.Forms.ToolStripProgressBar();
            this.TSLSequenceCounterLabel = new System.Windows.Forms.ToolStripLabel();
            this.TSLSequenceCounter = new System.Windows.Forms.ToolStripLabel();
            this.TSMain.SuspendLayout();
            this.TSSequence.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.MIFile,
            this.MIDevices,
            this.MIView,
            this.MISettings});
            resources.ApplyResources(this.MainMenu, "MainMenu");
            // 
            // MIFile
            // 
            resources.ApplyResources(this.MIFile, "MIFile");
            this.MIFile.Index = 0;
            this.MIFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.MISaveTo,
            this.MIWorkingDirectory,
            this.MIFileSeparator,
            this.MIOpenSavedModel,
            this.MIOpenRawData});
            // 
            // MISaveTo
            // 
            resources.ApplyResources(this.MISaveTo, "MISaveTo");
            this.MISaveTo.Index = 0;
            // 
            // MIWorkingDirectory
            // 
            resources.ApplyResources(this.MIWorkingDirectory, "MIWorkingDirectory");
            this.MIWorkingDirectory.Index = 1;
            this.MIWorkingDirectory.Click += new System.EventHandler(this.MIWorkingDirectory_Click);
            // 
            // MIFileSeparator
            // 
            resources.ApplyResources(this.MIFileSeparator, "MIFileSeparator");
            this.MIFileSeparator.Index = 2;
            // 
            // MIOpenSavedModel
            // 
            resources.ApplyResources(this.MIOpenSavedModel, "MIOpenSavedModel");
            this.MIOpenSavedModel.Index = 3;
            this.MIOpenSavedModel.Click += new System.EventHandler(this.MIOpenSavedModel_Click);
            // 
            // MIOpenRawData
            // 
            resources.ApplyResources(this.MIOpenRawData, "MIOpenRawData");
            this.MIOpenRawData.Index = 4;
            this.MIOpenRawData.Click += new System.EventHandler(this.MIOpenRawData_Click);
            // 
            // MIDevices
            // 
            resources.ApplyResources(this.MIDevices, "MIDevices");
            this.MIDevices.Index = 1;
            this.MIDevices.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.MIStart,
            this.MIStop,
            this.MIDisconnect,
            this.MICalibration,
            this.MIDevicesRefresh,
            this.MIDevicesSeparator,
            this.MIModes,
            this.MITilt,
            this.MIDevicesSeparator2,
            this.MIVirtualDevice});
            // 
            // MIStart
            // 
            resources.ApplyResources(this.MIStart, "MIStart");
            this.MIStart.Index = 0;
            this.MIStart.Click += new System.EventHandler(this.MIStart_Click);
            // 
            // MIStop
            // 
            resources.ApplyResources(this.MIStop, "MIStop");
            this.MIStop.Index = 1;
            this.MIStop.Click += new System.EventHandler(this.MIStop_Click);
            // 
            // MIDisconnect
            // 
            resources.ApplyResources(this.MIDisconnect, "MIDisconnect");
            this.MIDisconnect.Index = 2;
            this.MIDisconnect.Click += new System.EventHandler(this.MIDisconnect_Click);
            // 
            // MICalibration
            // 
            resources.ApplyResources(this.MICalibration, "MICalibration");
            this.MICalibration.Index = 3;
            this.MICalibration.Click += new System.EventHandler(this.MICalibration_Click);
            // 
            // MIDevicesRefresh
            // 
            resources.ApplyResources(this.MIDevicesRefresh, "MIDevicesRefresh");
            this.MIDevicesRefresh.Index = 4;
            this.MIDevicesRefresh.Click += new System.EventHandler(this.MIDevicesRefresh_Click);
            // 
            // MIDevicesSeparator
            // 
            resources.ApplyResources(this.MIDevicesSeparator, "MIDevicesSeparator");
            this.MIDevicesSeparator.Index = 5;
            // 
            // MIModes
            // 
            resources.ApplyResources(this.MIModes, "MIModes");
            this.MIModes.Index = 6;
            // 
            // MITilt
            // 
            resources.ApplyResources(this.MITilt, "MITilt");
            this.MITilt.Index = 7;
            this.MITilt.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.MITiltHorizontal,
            this.MITiltUp,
            this.MITiltDown});
            // 
            // MITiltHorizontal
            // 
            resources.ApplyResources(this.MITiltHorizontal, "MITiltHorizontal");
            this.MITiltHorizontal.Index = 0;
            this.MITiltHorizontal.Click += new System.EventHandler(this.MITiltHorizontal_Click);
            // 
            // MITiltUp
            // 
            resources.ApplyResources(this.MITiltUp, "MITiltUp");
            this.MITiltUp.Index = 1;
            this.MITiltUp.Click += new System.EventHandler(this.MITiltUp_Click);
            // 
            // MITiltDown
            // 
            resources.ApplyResources(this.MITiltDown, "MITiltDown");
            this.MITiltDown.Index = 2;
            this.MITiltDown.Click += new System.EventHandler(this.MITiltDown_Click);
            // 
            // MIDevicesSeparator2
            // 
            resources.ApplyResources(this.MIDevicesSeparator2, "MIDevicesSeparator2");
            this.MIDevicesSeparator2.Index = 8;
            // 
            // MIVirtualDevice
            // 
            resources.ApplyResources(this.MIVirtualDevice, "MIVirtualDevice");
            this.MIVirtualDevice.Index = 9;
            this.MIVirtualDevice.RadioCheck = true;
            this.MIVirtualDevice.Tag = -1;
            this.MIVirtualDevice.Click += new System.EventHandler(this.TSMIDevice_Click);
            // 
            // MIView
            // 
            resources.ApplyResources(this.MIView, "MIView");
            this.MIView.Index = 2;
            this.MIView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.MIRotateRight,
            this.MIRotateLeft,
            this.MIResetView});
            // 
            // MIRotateRight
            // 
            resources.ApplyResources(this.MIRotateRight, "MIRotateRight");
            this.MIRotateRight.Index = 0;
            this.MIRotateRight.Click += new System.EventHandler(this.MIRotateRight_Click);
            // 
            // MIRotateLeft
            // 
            resources.ApplyResources(this.MIRotateLeft, "MIRotateLeft");
            this.MIRotateLeft.Index = 1;
            this.MIRotateLeft.Click += new System.EventHandler(this.MIRotateLeft_Click);
            // 
            // MIResetView
            // 
            resources.ApplyResources(this.MIResetView, "MIResetView");
            this.MIResetView.Index = 2;
            this.MIResetView.Click += new System.EventHandler(this.MIResetView_Click);
            // 
            // MISettings
            // 
            resources.ApplyResources(this.MISettings, "MISettings");
            this.MISettings.Index = 3;
            this.MISettings.Click += new System.EventHandler(this.MISettings_Click);
            // 
            // SBStatus
            // 
            resources.ApplyResources(this.SBStatus, "SBStatus");
            this.SBStatus.Name = "SBStatus";
            // 
            // TSMain
            // 
            resources.ApplyResources(this.TSMain, "TSMain");
            this.TSMain.BackColor = System.Drawing.SystemColors.Control;
            this.TSMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.TSMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSBStartStop,
            this.TSSMainSeparatorFile,
            this.TSBFilePrevious,
            this.TSLFileCount,
            this.TSBFileNext,
            this.TSSMainSeparator,
            this.TSBProbes,
            this.TSBRotateLeft,
            this.TSBRotateRight,
            this.TSBResetView,
            this.TSDDBShading,
            this.TSSMainSeparatorSave,
            this.TSLLabel,
            this.TSTBLabel,
            this.TSBSave,
            this.TSBSequenceSave});
            this.TSMain.Name = "TSMain";
            this.TSMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // TSBStartStop
            // 
            resources.ApplyResources(this.TSBStartStop, "TSBStartStop");
            this.TSBStartStop.Image = global::KinectScan.Properties.Resources.start16;
            this.TSBStartStop.Name = "TSBStartStop";
            this.TSBStartStop.Click += new System.EventHandler(this.TSBStart_Click);
            // 
            // TSSMainSeparatorFile
            // 
            resources.ApplyResources(this.TSSMainSeparatorFile, "TSSMainSeparatorFile");
            this.TSSMainSeparatorFile.Name = "TSSMainSeparatorFile";
            // 
            // TSBFilePrevious
            // 
            resources.ApplyResources(this.TSBFilePrevious, "TSBFilePrevious");
            this.TSBFilePrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSBFilePrevious.Image = global::KinectScan.Properties.Resources.previous16;
            this.TSBFilePrevious.Name = "TSBFilePrevious";
            this.TSBFilePrevious.Click += new System.EventHandler(this.TSBFilePrevious_Click);
            // 
            // TSLFileCount
            // 
            resources.ApplyResources(this.TSLFileCount, "TSLFileCount");
            this.TSLFileCount.Name = "TSLFileCount";
            // 
            // TSBFileNext
            // 
            resources.ApplyResources(this.TSBFileNext, "TSBFileNext");
            this.TSBFileNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSBFileNext.Image = global::KinectScan.Properties.Resources.next16;
            this.TSBFileNext.Name = "TSBFileNext";
            this.TSBFileNext.Click += new System.EventHandler(this.TSBFileNext_Click);
            // 
            // TSSMainSeparator
            // 
            resources.ApplyResources(this.TSSMainSeparator, "TSSMainSeparator");
            this.TSSMainSeparator.Name = "TSSMainSeparator";
            // 
            // TSBProbes
            // 
            resources.ApplyResources(this.TSBProbes, "TSBProbes");
            this.TSBProbes.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSBProbes.Name = "TSBProbes";
            this.TSBProbes.Click += new System.EventHandler(this.TSBProbes_Click);
            // 
            // TSBRotateLeft
            // 
            resources.ApplyResources(this.TSBRotateLeft, "TSBRotateLeft");
            this.TSBRotateLeft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSBRotateLeft.Image = global::KinectScan.Properties.Resources.rotateLeft16;
            this.TSBRotateLeft.Name = "TSBRotateLeft";
            this.TSBRotateLeft.Click += new System.EventHandler(this.TSBRotateLeft_Click);
            // 
            // TSBRotateRight
            // 
            resources.ApplyResources(this.TSBRotateRight, "TSBRotateRight");
            this.TSBRotateRight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSBRotateRight.Image = global::KinectScan.Properties.Resources.rotateRight16;
            this.TSBRotateRight.Name = "TSBRotateRight";
            this.TSBRotateRight.Click += new System.EventHandler(this.TSBRotateRight_Click);
            // 
            // TSBResetView
            // 
            resources.ApplyResources(this.TSBResetView, "TSBResetView");
            this.TSBResetView.Image = global::KinectScan.Properties.Resources.resetView16;
            this.TSBResetView.Name = "TSBResetView";
            this.TSBResetView.Click += new System.EventHandler(this.TSBResetView_Click);
            // 
            // TSDDBShading
            // 
            resources.ApplyResources(this.TSDDBShading, "TSDDBShading");
            this.TSDDBShading.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSDDBShading.Image = global::KinectScan.Properties.Resources.shading16;
            this.TSDDBShading.Name = "TSDDBShading";
            // 
            // TSSMainSeparatorSave
            // 
            resources.ApplyResources(this.TSSMainSeparatorSave, "TSSMainSeparatorSave");
            this.TSSMainSeparatorSave.Name = "TSSMainSeparatorSave";
            // 
            // TSLLabel
            // 
            resources.ApplyResources(this.TSLLabel, "TSLLabel");
            this.TSLLabel.Name = "TSLLabel";
            // 
            // TSTBLabel
            // 
            resources.ApplyResources(this.TSTBLabel, "TSTBLabel");
            this.TSTBLabel.Name = "TSTBLabel";
            // 
            // TSBSave
            // 
            resources.ApplyResources(this.TSBSave, "TSBSave");
            this.TSBSave.Image = global::KinectScan.Properties.Resources.save16;
            this.TSBSave.Name = "TSBSave";
            this.TSBSave.ButtonClick += new System.EventHandler(this.TSMISave_Click);
            // 
            // TSBSequenceSave
            // 
            resources.ApplyResources(this.TSBSequenceSave, "TSBSequenceSave");
            this.TSBSequenceSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSBSequenceSave.Image = global::KinectScan.Properties.Resources.sequence16;
            this.TSBSequenceSave.Name = "TSBSequenceSave";
            this.TSBSequenceSave.Click += new System.EventHandler(this.TSBSequenceSave_Click);
            // 
            // XPanel
            // 
            resources.ApplyResources(this.XPanel, "XPanel");
            this.XPanel.BackColor = System.Drawing.Color.White;
            this.XPanel.Name = "XPanel";
            this.XPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.XPanel_MouseDown);
            this.XPanel.MouseLeave += new System.EventHandler(this.XPanel_MouseLeave);
            this.XPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.XPanel_MouseUp);
            // 
            // TSSequence
            // 
            resources.ApplyResources(this.TSSequence, "TSSequence");
            this.TSSequence.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSLSequenceLabel,
            this.TSSSequence0,
            this.TSBSequenceRecord,
            this.TSBSequencePause,
            this.TSBSequenceStop,
            this.TSSSequence1,
            this.TSLSequenceIntervalLabel,
            this.TSBSequenceDecreaseInterval,
            this.TSLSequenceInterval,
            this.TSBSequenceIncreaseInterval,
            this.TSSSequnce2,
            this.TSLSequencePerformance,
            this.TSLSequenceCounterLabel,
            this.TSLSequenceCounter});
            this.TSSequence.Name = "TSSequence";
            this.TSSequence.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // TSLSequenceLabel
            // 
            resources.ApplyResources(this.TSLSequenceLabel, "TSLSequenceLabel");
            this.TSLSequenceLabel.Name = "TSLSequenceLabel";
            // 
            // TSSSequence0
            // 
            resources.ApplyResources(this.TSSSequence0, "TSSSequence0");
            this.TSSSequence0.Name = "TSSSequence0";
            // 
            // TSBSequenceRecord
            // 
            resources.ApplyResources(this.TSBSequenceRecord, "TSBSequenceRecord");
            this.TSBSequenceRecord.Image = global::KinectScan.Properties.Resources.record16;
            this.TSBSequenceRecord.Name = "TSBSequenceRecord";
            this.TSBSequenceRecord.Click += new System.EventHandler(this.TSBSequenceRecord_Click);
            // 
            // TSBSequencePause
            // 
            resources.ApplyResources(this.TSBSequencePause, "TSBSequencePause");
            this.TSBSequencePause.Image = global::KinectScan.Properties.Resources.pause16;
            this.TSBSequencePause.Name = "TSBSequencePause";
            this.TSBSequencePause.Click += new System.EventHandler(this.TSBSequencePause_Click);
            // 
            // TSBSequenceStop
            // 
            resources.ApplyResources(this.TSBSequenceStop, "TSBSequenceStop");
            this.TSBSequenceStop.Image = global::KinectScan.Properties.Resources.stop16;
            this.TSBSequenceStop.Name = "TSBSequenceStop";
            this.TSBSequenceStop.Click += new System.EventHandler(this.TSBSequenceStop_Click);
            // 
            // TSSSequence1
            // 
            resources.ApplyResources(this.TSSSequence1, "TSSSequence1");
            this.TSSSequence1.Name = "TSSSequence1";
            // 
            // TSLSequenceIntervalLabel
            // 
            resources.ApplyResources(this.TSLSequenceIntervalLabel, "TSLSequenceIntervalLabel");
            this.TSLSequenceIntervalLabel.Name = "TSLSequenceIntervalLabel";
            // 
            // TSBSequenceDecreaseInterval
            // 
            resources.ApplyResources(this.TSBSequenceDecreaseInterval, "TSBSequenceDecreaseInterval");
            this.TSBSequenceDecreaseInterval.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSBSequenceDecreaseInterval.Image = global::KinectScan.Properties.Resources.previous16;
            this.TSBSequenceDecreaseInterval.Name = "TSBSequenceDecreaseInterval";
            this.TSBSequenceDecreaseInterval.Click += new System.EventHandler(this.TSBSequenceDecreaseInterval_Click);
            // 
            // TSLSequenceInterval
            // 
            resources.ApplyResources(this.TSLSequenceInterval, "TSLSequenceInterval");
            this.TSLSequenceInterval.Name = "TSLSequenceInterval";
            // 
            // TSBSequenceIncreaseInterval
            // 
            resources.ApplyResources(this.TSBSequenceIncreaseInterval, "TSBSequenceIncreaseInterval");
            this.TSBSequenceIncreaseInterval.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSBSequenceIncreaseInterval.Image = global::KinectScan.Properties.Resources.next16;
            this.TSBSequenceIncreaseInterval.Name = "TSBSequenceIncreaseInterval";
            this.TSBSequenceIncreaseInterval.Click += new System.EventHandler(this.TSBSequenceIncreaseInterval_Click);
            // 
            // TSSSequnce2
            // 
            resources.ApplyResources(this.TSSSequnce2, "TSSSequnce2");
            this.TSSSequnce2.Name = "TSSSequnce2";
            // 
            // TSLSequencePerformance
            // 
            resources.ApplyResources(this.TSLSequencePerformance, "TSLSequencePerformance");
            this.TSLSequencePerformance.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.TSLSequencePerformance.Name = "TSLSequencePerformance";
            // 
            // TSLSequenceCounterLabel
            // 
            resources.ApplyResources(this.TSLSequenceCounterLabel, "TSLSequenceCounterLabel");
            this.TSLSequenceCounterLabel.Name = "TSLSequenceCounterLabel";
            // 
            // TSLSequenceCounter
            // 
            resources.ApplyResources(this.TSLSequenceCounter, "TSLSequenceCounter");
            this.TSLSequenceCounter.Name = "TSLSequenceCounter";
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TSSequence);
            this.Controls.Add(this.XPanel);
            this.Controls.Add(this.SBStatus);
            this.Controls.Add(this.TSMain);
            this.Menu = this.MainMenu;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.TSMain.ResumeLayout(false);
            this.TSMain.PerformLayout();
            this.TSSequence.ResumeLayout(false);
            this.TSSequence.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MainMenu MainMenu;
        private System.Windows.Forms.MenuItem MIModes;
        private System.Windows.Forms.StatusBar SBStatus;
        private System.Windows.Forms.MenuItem MITilt;
        private System.Windows.Forms.MenuItem MITiltHorizontal;
        private System.Windows.Forms.MenuItem MITiltUp;
        private System.Windows.Forms.MenuItem MITiltDown;
        private System.Windows.Forms.MenuItem MISettings;
        private System.Windows.Forms.MenuItem MIRotateRight;
        private System.Windows.Forms.ToolStrip TSMain;
        private System.Windows.Forms.MenuItem MIDevices;
        private System.Windows.Forms.MenuItem MIStart;
        private System.Windows.Forms.MenuItem MIStop;
        private System.Windows.Forms.MenuItem MIDevicesSeparator;
        private System.Windows.Forms.Panel XPanel;
        private System.Windows.Forms.ToolStripButton TSBStartStop;
        private System.Windows.Forms.MenuItem MIDisconnect;
        private System.Windows.Forms.MenuItem MIDevicesRefresh;
        private System.Windows.Forms.MenuItem MIFile;
        private System.Windows.Forms.MenuItem MIDevicesSeparator2;
        private System.Windows.Forms.MenuItem MIView;
        private System.Windows.Forms.MenuItem MIRotateLeft;
        private System.Windows.Forms.ToolStripButton TSBRotateLeft;
        private System.Windows.Forms.ToolStripButton TSBRotateRight;
        private System.Windows.Forms.MenuItem MIWorkingDirectory;
        private System.Windows.Forms.ToolStripButton TSBResetView;
        private System.Windows.Forms.MenuItem MIResetView;
        private System.Windows.Forms.MenuItem MIFileSeparator;
        private System.Windows.Forms.MenuItem MIOpenSavedModel;
        private System.Windows.Forms.ToolStripSeparator TSSMainSeparator;
        private System.Windows.Forms.ToolStripLabel TSLLabel;
        private System.Windows.Forms.ToolStripTextBox TSTBLabel;
        private System.Windows.Forms.ToolStripDropDownButton TSDDBShading;
        private System.Windows.Forms.ToolStripSeparator TSSMainSeparatorSave;
        private System.Windows.Forms.ToolStripButton TSBProbes;
        private System.Windows.Forms.ToolStripSplitButton TSBSave;
        private System.Windows.Forms.MenuItem MIVirtualDevice;
        private System.Windows.Forms.MenuItem MIOpenRawData;
        private System.Windows.Forms.MenuItem MISaveTo;
        private System.Windows.Forms.ToolStrip TSSequence;
        private System.Windows.Forms.ToolStripButton TSBSequenceRecord;
        private System.Windows.Forms.ToolStripButton TSBSequencePause;
        private System.Windows.Forms.ToolStripButton TSBSequenceStop;
        private System.Windows.Forms.ToolStripSeparator TSSSequence1;
        private System.Windows.Forms.ToolStripLabel TSLSequenceInterval;
        private System.Windows.Forms.ToolStripButton TSBSequenceDecreaseInterval;
        private System.Windows.Forms.ToolStripButton TSBSequenceIncreaseInterval;
        private System.Windows.Forms.ToolStripSeparator TSSSequnce2;
        private System.Windows.Forms.ToolStripProgressBar TSLSequencePerformance;
        private System.Windows.Forms.ToolStripLabel TSLSequenceCounter;
        private System.Windows.Forms.ToolStripLabel TSLSequenceIntervalLabel;
        private System.Windows.Forms.ToolStripLabel TSLSequenceCounterLabel;
        private System.Windows.Forms.ToolStripLabel TSLSequenceLabel;
        private System.Windows.Forms.ToolStripButton TSBSequenceSave;
        private System.Windows.Forms.ToolStripSeparator TSSSequence0;
        private System.Windows.Forms.ToolStripSeparator TSSMainSeparatorFile;
        private System.Windows.Forms.ToolStripButton TSBFilePrevious;
        private System.Windows.Forms.ToolStripLabel TSLFileCount;
        private System.Windows.Forms.ToolStripButton TSBFileNext;
        private System.Windows.Forms.MenuItem MICalibration;
    }
}

