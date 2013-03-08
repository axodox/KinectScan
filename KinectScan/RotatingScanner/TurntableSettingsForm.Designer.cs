namespace KinectScan
{
    partial class TurntableSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TurntableSettingsForm));
            this.FLPSettings = new System.Windows.Forms.FlowLayoutPanel();
            this.CBClip = new System.Windows.Forms.CheckBox();
            this.NBMinClipRadius = new CustomControls.NumericBar();
            this.NBMaxClipRadius = new CustomControls.NumericBar();
            this.NBZLimit = new CustomControls.NumericBar();
            this.NBReprojectionRotationX = new CustomControls.NumericBar();
            this.NBReprojectionRotationZ = new CustomControls.NumericBar();
            this.NBReprojectionTranslationX = new CustomControls.NumericBar();
            this.NBReprojectionTranslationY = new CustomControls.NumericBar();
            this.NBReprojectionTranslationZ = new CustomControls.NumericBar();
            this.NBProjectionWidth = new CustomControls.NumericBar();
            this.NBMaxY = new CustomControls.NumericBar();
            this.NBMinY = new CustomControls.NumericBar();
            this.NBSplitPlane = new CustomControls.NumericBar();
            this.NBAveragedFrames = new CustomControls.NumericBar();
            this.NBDepthAveragingLimit = new CustomControls.NumericBar();
            this.NBMinAvgCount = new CustomControls.NumericBar();
            this.NBFusionSpacing = new CustomControls.NumericBar();
            this.NBMinClipY = new CustomControls.NumericBar();
            this.SSSettings = new System.Windows.Forms.StatusStrip();
            this.TSDDBSaveFile = new System.Windows.Forms.ToolStripDropDownButton();
            this.TSMIImport = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMIExport = new System.Windows.Forms.ToolStripMenuItem();
            this.SFDExport = new System.Windows.Forms.SaveFileDialog();
            this.OFDImport = new System.Windows.Forms.OpenFileDialog();
            this.NBCoreX = new CustomControls.NumericBar();
            this.NBCoreY = new CustomControls.NumericBar();
            this.FLPSettings.SuspendLayout();
            this.SSSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // FLPSettings
            // 
            resources.ApplyResources(this.FLPSettings, "FLPSettings");
            this.FLPSettings.Controls.Add(this.CBClip);
            this.FLPSettings.Controls.Add(this.NBMinClipRadius);
            this.FLPSettings.Controls.Add(this.NBMaxClipRadius);
            this.FLPSettings.Controls.Add(this.NBZLimit);
            this.FLPSettings.Controls.Add(this.NBReprojectionRotationX);
            this.FLPSettings.Controls.Add(this.NBReprojectionRotationZ);
            this.FLPSettings.Controls.Add(this.NBReprojectionTranslationX);
            this.FLPSettings.Controls.Add(this.NBReprojectionTranslationY);
            this.FLPSettings.Controls.Add(this.NBReprojectionTranslationZ);
            this.FLPSettings.Controls.Add(this.NBProjectionWidth);
            this.FLPSettings.Controls.Add(this.NBMaxY);
            this.FLPSettings.Controls.Add(this.NBMinY);
            this.FLPSettings.Controls.Add(this.NBSplitPlane);
            this.FLPSettings.Controls.Add(this.NBAveragedFrames);
            this.FLPSettings.Controls.Add(this.NBDepthAveragingLimit);
            this.FLPSettings.Controls.Add(this.NBMinAvgCount);
            this.FLPSettings.Controls.Add(this.NBFusionSpacing);
            this.FLPSettings.Controls.Add(this.NBMinClipY);
            this.FLPSettings.Controls.Add(this.NBCoreX);
            this.FLPSettings.Controls.Add(this.NBCoreY);
            this.FLPSettings.Name = "FLPSettings";
            this.FLPSettings.Resize += new System.EventHandler(this.FLPSettings_Resize);
            // 
            // CBClip
            // 
            resources.ApplyResources(this.CBClip, "CBClip");
            this.CBClip.Name = "CBClip";
            this.CBClip.UseVisualStyleBackColor = true;
            // 
            // NBMinClipRadius
            // 
            this.NBMinClipRadius.ByteValue = ((byte)(0));
            this.NBMinClipRadius.DecimalPlaces = 2;
            this.NBMinClipRadius.DoubleValue = 0D;
            this.NBMinClipRadius.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NBMinClipRadius.Int32Value = 0;
            this.NBMinClipRadius.IntegerValue = false;
            resources.ApplyResources(this.NBMinClipRadius, "NBMinClipRadius");
            this.NBMinClipRadius.Maximum = 0.5D;
            this.NBMinClipRadius.Minimum = 0D;
            this.NBMinClipRadius.Name = "NBMinClipRadius";
            this.NBMinClipRadius.Value = 0F;
            this.NBMinClipRadius.ValueChangeEventEnabled = true;
            // 
            // NBMaxClipRadius
            // 
            this.NBMaxClipRadius.ByteValue = ((byte)(0));
            this.NBMaxClipRadius.DecimalPlaces = 2;
            this.NBMaxClipRadius.DoubleValue = 0D;
            this.NBMaxClipRadius.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NBMaxClipRadius.Int32Value = 0;
            this.NBMaxClipRadius.IntegerValue = false;
            resources.ApplyResources(this.NBMaxClipRadius, "NBMaxClipRadius");
            this.NBMaxClipRadius.Maximum = 0.5D;
            this.NBMaxClipRadius.Minimum = 0D;
            this.NBMaxClipRadius.Name = "NBMaxClipRadius";
            this.NBMaxClipRadius.Value = 0F;
            this.NBMaxClipRadius.ValueChangeEventEnabled = true;
            // 
            // NBZLimit
            // 
            this.NBZLimit.ByteValue = ((byte)(0));
            this.NBZLimit.DecimalPlaces = 2;
            this.NBZLimit.DoubleValue = 0D;
            this.NBZLimit.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NBZLimit.Int32Value = 0;
            this.NBZLimit.IntegerValue = false;
            resources.ApplyResources(this.NBZLimit, "NBZLimit");
            this.NBZLimit.Maximum = 10D;
            this.NBZLimit.Minimum = 0D;
            this.NBZLimit.Name = "NBZLimit";
            this.NBZLimit.Value = 0F;
            this.NBZLimit.ValueChangeEventEnabled = true;
            // 
            // NBReprojectionRotationX
            // 
            this.NBReprojectionRotationX.ByteValue = ((byte)(0));
            this.NBReprojectionRotationX.DecimalPlaces = 2;
            this.NBReprojectionRotationX.DoubleValue = 0D;
            this.NBReprojectionRotationX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NBReprojectionRotationX.Int32Value = 0;
            this.NBReprojectionRotationX.IntegerValue = false;
            resources.ApplyResources(this.NBReprojectionRotationX, "NBReprojectionRotationX");
            this.NBReprojectionRotationX.Maximum = 45D;
            this.NBReprojectionRotationX.Minimum = -45D;
            this.NBReprojectionRotationX.Name = "NBReprojectionRotationX";
            this.NBReprojectionRotationX.Value = 0F;
            this.NBReprojectionRotationX.ValueChangeEventEnabled = true;
            // 
            // NBReprojectionRotationZ
            // 
            this.NBReprojectionRotationZ.ByteValue = ((byte)(0));
            this.NBReprojectionRotationZ.DecimalPlaces = 2;
            this.NBReprojectionRotationZ.DoubleValue = 0D;
            this.NBReprojectionRotationZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NBReprojectionRotationZ.Int32Value = 0;
            this.NBReprojectionRotationZ.IntegerValue = false;
            resources.ApplyResources(this.NBReprojectionRotationZ, "NBReprojectionRotationZ");
            this.NBReprojectionRotationZ.Maximum = 45D;
            this.NBReprojectionRotationZ.Minimum = -45D;
            this.NBReprojectionRotationZ.Name = "NBReprojectionRotationZ";
            this.NBReprojectionRotationZ.Value = 0F;
            this.NBReprojectionRotationZ.ValueChangeEventEnabled = true;
            // 
            // NBReprojectionTranslationX
            // 
            this.NBReprojectionTranslationX.ByteValue = ((byte)(0));
            this.NBReprojectionTranslationX.DecimalPlaces = 3;
            this.NBReprojectionTranslationX.DoubleValue = 0D;
            this.NBReprojectionTranslationX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.NBReprojectionTranslationX.Int32Value = 0;
            this.NBReprojectionTranslationX.IntegerValue = false;
            resources.ApplyResources(this.NBReprojectionTranslationX, "NBReprojectionTranslationX");
            this.NBReprojectionTranslationX.Maximum = 1D;
            this.NBReprojectionTranslationX.Minimum = -1D;
            this.NBReprojectionTranslationX.Name = "NBReprojectionTranslationX";
            this.NBReprojectionTranslationX.Value = 0F;
            this.NBReprojectionTranslationX.ValueChangeEventEnabled = true;
            // 
            // NBReprojectionTranslationY
            // 
            this.NBReprojectionTranslationY.ByteValue = ((byte)(0));
            this.NBReprojectionTranslationY.DecimalPlaces = 3;
            this.NBReprojectionTranslationY.DoubleValue = 0D;
            this.NBReprojectionTranslationY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.NBReprojectionTranslationY.Int32Value = 0;
            this.NBReprojectionTranslationY.IntegerValue = false;
            resources.ApplyResources(this.NBReprojectionTranslationY, "NBReprojectionTranslationY");
            this.NBReprojectionTranslationY.Maximum = 1D;
            this.NBReprojectionTranslationY.Minimum = -1D;
            this.NBReprojectionTranslationY.Name = "NBReprojectionTranslationY";
            this.NBReprojectionTranslationY.Value = 0F;
            this.NBReprojectionTranslationY.ValueChangeEventEnabled = true;
            // 
            // NBReprojectionTranslationZ
            // 
            this.NBReprojectionTranslationZ.ByteValue = ((byte)(0));
            this.NBReprojectionTranslationZ.DecimalPlaces = 3;
            this.NBReprojectionTranslationZ.DoubleValue = 0D;
            this.NBReprojectionTranslationZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.NBReprojectionTranslationZ.Int32Value = 0;
            this.NBReprojectionTranslationZ.IntegerValue = false;
            resources.ApplyResources(this.NBReprojectionTranslationZ, "NBReprojectionTranslationZ");
            this.NBReprojectionTranslationZ.Maximum = 3D;
            this.NBReprojectionTranslationZ.Minimum = 0D;
            this.NBReprojectionTranslationZ.Name = "NBReprojectionTranslationZ";
            this.NBReprojectionTranslationZ.Value = 0F;
            this.NBReprojectionTranslationZ.ValueChangeEventEnabled = true;
            // 
            // NBProjectionWidth
            // 
            this.NBProjectionWidth.ByteValue = ((byte)(0));
            this.NBProjectionWidth.DecimalPlaces = 2;
            this.NBProjectionWidth.DoubleValue = 0D;
            this.NBProjectionWidth.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NBProjectionWidth.Int32Value = 0;
            this.NBProjectionWidth.IntegerValue = false;
            resources.ApplyResources(this.NBProjectionWidth, "NBProjectionWidth");
            this.NBProjectionWidth.Maximum = 4D;
            this.NBProjectionWidth.Minimum = 0D;
            this.NBProjectionWidth.Name = "NBProjectionWidth";
            this.NBProjectionWidth.Value = 0F;
            this.NBProjectionWidth.ValueChangeEventEnabled = true;
            // 
            // NBMaxY
            // 
            this.NBMaxY.ByteValue = ((byte)(0));
            this.NBMaxY.DecimalPlaces = 2;
            this.NBMaxY.DoubleValue = 0D;
            this.NBMaxY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NBMaxY.Int32Value = 0;
            this.NBMaxY.IntegerValue = false;
            resources.ApplyResources(this.NBMaxY, "NBMaxY");
            this.NBMaxY.Maximum = 2D;
            this.NBMaxY.Minimum = -2D;
            this.NBMaxY.Name = "NBMaxY";
            this.NBMaxY.Value = 0F;
            this.NBMaxY.ValueChangeEventEnabled = true;
            // 
            // NBMinY
            // 
            this.NBMinY.ByteValue = ((byte)(0));
            this.NBMinY.DecimalPlaces = 2;
            this.NBMinY.DoubleValue = 0D;
            this.NBMinY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NBMinY.Int32Value = 0;
            this.NBMinY.IntegerValue = false;
            resources.ApplyResources(this.NBMinY, "NBMinY");
            this.NBMinY.Maximum = 2D;
            this.NBMinY.Minimum = -2D;
            this.NBMinY.Name = "NBMinY";
            this.NBMinY.Value = 0F;
            this.NBMinY.ValueChangeEventEnabled = true;
            // 
            // NBSplitPlane
            // 
            this.NBSplitPlane.ByteValue = ((byte)(0));
            this.NBSplitPlane.DecimalPlaces = 2;
            this.NBSplitPlane.DoubleValue = 0D;
            this.NBSplitPlane.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NBSplitPlane.Int32Value = 0;
            this.NBSplitPlane.IntegerValue = false;
            resources.ApplyResources(this.NBSplitPlane, "NBSplitPlane");
            this.NBSplitPlane.Maximum = 180D;
            this.NBSplitPlane.Minimum = 0D;
            this.NBSplitPlane.Name = "NBSplitPlane";
            this.NBSplitPlane.Value = 0F;
            this.NBSplitPlane.ValueChangeEventEnabled = true;
            // 
            // NBAveragedFrames
            // 
            this.NBAveragedFrames.ByteValue = ((byte)(0));
            this.NBAveragedFrames.DecimalPlaces = 2;
            this.NBAveragedFrames.DoubleValue = 0D;
            this.NBAveragedFrames.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NBAveragedFrames.Int32Value = 0;
            this.NBAveragedFrames.IntegerValue = true;
            resources.ApplyResources(this.NBAveragedFrames, "NBAveragedFrames");
            this.NBAveragedFrames.Maximum = 60D;
            this.NBAveragedFrames.Minimum = 0D;
            this.NBAveragedFrames.Name = "NBAveragedFrames";
            this.NBAveragedFrames.Value = 0F;
            this.NBAveragedFrames.ValueChangeEventEnabled = true;
            // 
            // NBDepthAveragingLimit
            // 
            this.NBDepthAveragingLimit.ByteValue = ((byte)(0));
            this.NBDepthAveragingLimit.DecimalPlaces = 3;
            this.NBDepthAveragingLimit.DoubleValue = 0D;
            this.NBDepthAveragingLimit.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.NBDepthAveragingLimit.Int32Value = 0;
            this.NBDepthAveragingLimit.IntegerValue = false;
            resources.ApplyResources(this.NBDepthAveragingLimit, "NBDepthAveragingLimit");
            this.NBDepthAveragingLimit.Maximum = 0.1D;
            this.NBDepthAveragingLimit.Minimum = 0D;
            this.NBDepthAveragingLimit.Name = "NBDepthAveragingLimit";
            this.NBDepthAveragingLimit.Value = 0F;
            this.NBDepthAveragingLimit.ValueChangeEventEnabled = true;
            // 
            // NBMinAvgCount
            // 
            this.NBMinAvgCount.ByteValue = ((byte)(1));
            this.NBMinAvgCount.DecimalPlaces = 2;
            this.NBMinAvgCount.DoubleValue = 1D;
            this.NBMinAvgCount.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NBMinAvgCount.Int32Value = 1;
            this.NBMinAvgCount.IntegerValue = true;
            resources.ApplyResources(this.NBMinAvgCount, "NBMinAvgCount");
            this.NBMinAvgCount.Maximum = 60D;
            this.NBMinAvgCount.Minimum = 1D;
            this.NBMinAvgCount.Name = "NBMinAvgCount";
            this.NBMinAvgCount.Value = 1F;
            this.NBMinAvgCount.ValueChangeEventEnabled = true;
            // 
            // NBFusionSpacing
            // 
            this.NBFusionSpacing.ByteValue = ((byte)(1));
            this.NBFusionSpacing.DecimalPlaces = 2;
            this.NBFusionSpacing.DoubleValue = 1D;
            this.NBFusionSpacing.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NBFusionSpacing.Int32Value = 1;
            this.NBFusionSpacing.IntegerValue = true;
            resources.ApplyResources(this.NBFusionSpacing, "NBFusionSpacing");
            this.NBFusionSpacing.Maximum = 60D;
            this.NBFusionSpacing.Minimum = 1D;
            this.NBFusionSpacing.Name = "NBFusionSpacing";
            this.NBFusionSpacing.Value = 1F;
            this.NBFusionSpacing.ValueChangeEventEnabled = true;
            // 
            // NBMinClipY
            // 
            this.NBMinClipY.ByteValue = ((byte)(0));
            this.NBMinClipY.DecimalPlaces = 2;
            this.NBMinClipY.DoubleValue = 0D;
            this.NBMinClipY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NBMinClipY.Int32Value = 0;
            this.NBMinClipY.IntegerValue = false;
            resources.ApplyResources(this.NBMinClipY, "NBMinClipY");
            this.NBMinClipY.Maximum = 2D;
            this.NBMinClipY.Minimum = -2D;
            this.NBMinClipY.Name = "NBMinClipY";
            this.NBMinClipY.Value = 0F;
            this.NBMinClipY.ValueChangeEventEnabled = true;
            // 
            // SSSettings
            // 
            this.SSSettings.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSDDBSaveFile});
            resources.ApplyResources(this.SSSettings, "SSSettings");
            this.SSSettings.Name = "SSSettings";
            // 
            // TSDDBSaveFile
            // 
            this.TSDDBSaveFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TSDDBSaveFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMIImport,
            this.TSMIExport});
            this.TSDDBSaveFile.Image = global::KinectScan.Resources.settingsfile16;
            resources.ApplyResources(this.TSDDBSaveFile, "TSDDBSaveFile");
            this.TSDDBSaveFile.Name = "TSDDBSaveFile";
            // 
            // TSMIImport
            // 
            this.TSMIImport.Image = global::KinectScan.Resources.open16;
            this.TSMIImport.Name = "TSMIImport";
            resources.ApplyResources(this.TSMIImport, "TSMIImport");
            this.TSMIImport.Click += new System.EventHandler(this.TSMIImport_Click);
            // 
            // TSMIExport
            // 
            this.TSMIExport.Image = global::KinectScan.Resources.save16;
            this.TSMIExport.Name = "TSMIExport";
            resources.ApplyResources(this.TSMIExport, "TSMIExport");
            this.TSMIExport.Click += new System.EventHandler(this.TSMIExport_Click);
            // 
            // SFDExport
            // 
            this.SFDExport.DefaultExt = "xml";
            resources.ApplyResources(this.SFDExport, "SFDExport");
            // 
            // OFDImport
            // 
            this.OFDImport.DefaultExt = "xml";
            resources.ApplyResources(this.OFDImport, "OFDImport");
            // 
            // NBCoreX
            // 
            this.NBCoreX.ByteValue = ((byte)(0));
            this.NBCoreX.DecimalPlaces = 2;
            this.NBCoreX.DoubleValue = 0D;
            this.NBCoreX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NBCoreX.Int32Value = 0;
            this.NBCoreX.IntegerValue = false;
            resources.ApplyResources(this.NBCoreX, "NBCoreX");
            this.NBCoreX.Maximum = 0.3D;
            this.NBCoreX.Minimum = 0D;
            this.NBCoreX.Name = "NBCoreX";
            this.NBCoreX.Value = 0F;
            this.NBCoreX.ValueChangeEventEnabled = true;
            // 
            // NBCoreY
            // 
            this.NBCoreY.ByteValue = ((byte)(0));
            this.NBCoreY.DecimalPlaces = 2;
            this.NBCoreY.DoubleValue = 0D;
            this.NBCoreY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NBCoreY.Int32Value = 0;
            this.NBCoreY.IntegerValue = false;
            resources.ApplyResources(this.NBCoreY, "NBCoreY");
            this.NBCoreY.Maximum = 0.3D;
            this.NBCoreY.Minimum = -0.3D;
            this.NBCoreY.Name = "NBCoreY";
            this.NBCoreY.Value = 0F;
            this.NBCoreY.ValueChangeEventEnabled = true;
            // 
            // RSSettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SSSettings);
            this.Controls.Add(this.FLPSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RSSettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            this.FLPSettings.ResumeLayout(false);
            this.FLPSettings.PerformLayout();
            this.SSSettings.ResumeLayout(false);
            this.SSSettings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel FLPSettings;
        private System.Windows.Forms.StatusStrip SSSettings;
        private System.Windows.Forms.ToolStripDropDownButton TSDDBSaveFile;
        private System.Windows.Forms.ToolStripMenuItem TSMIImport;
        private System.Windows.Forms.ToolStripMenuItem TSMIExport;
        private System.Windows.Forms.SaveFileDialog SFDExport;
        private System.Windows.Forms.OpenFileDialog OFDImport;
        public CustomControls.NumericBar NBReprojectionTranslationZ;
        public CustomControls.NumericBar NBZLimit;
        public CustomControls.NumericBar NBReprojectionRotationX;
        public CustomControls.NumericBar NBReprojectionRotationZ;
        public CustomControls.NumericBar NBReprojectionTranslationX;
        public CustomControls.NumericBar NBReprojectionTranslationY;
        public CustomControls.NumericBar NBMaxY;
        public CustomControls.NumericBar NBSplitPlane;
        public CustomControls.NumericBar NBMinY;
        public CustomControls.NumericBar NBProjectionWidth;
        public CustomControls.NumericBar NBMaxClipRadius;
        public CustomControls.NumericBar NBMinClipRadius;
        public CustomControls.NumericBar NBMinAvgCount;
        public CustomControls.NumericBar NBDepthAveragingLimit;
        public System.Windows.Forms.CheckBox CBClip;
        public CustomControls.NumericBar NBAveragedFrames;
        public CustomControls.NumericBar NBFusionSpacing;
        public CustomControls.NumericBar NBMinClipY;
        public CustomControls.NumericBar NBCoreX;
        public CustomControls.NumericBar NBCoreY;

    }
}