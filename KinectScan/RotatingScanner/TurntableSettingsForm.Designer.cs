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
            this.CBClippingEnabled = new System.Windows.Forms.CheckBox();
            this.NBClippingRadiusMin = new CustomControls.NumericBar();
            this.NBClippingRadiusMax = new CustomControls.NumericBar();
            this.NBClippingDepthMax = new CustomControls.NumericBar();
            this.NBClippingYMin = new CustomControls.NumericBar();
            this.LTransformations = new System.Windows.Forms.Label();
            this.NBRotationX = new CustomControls.NumericBar();
            this.NBSplitPlaneAngle = new CustomControls.NumericBar();
            this.NBRotationZ = new CustomControls.NumericBar();
            this.NBTranslationX = new CustomControls.NumericBar();
            this.NBTranslationY = new CustomControls.NumericBar();
            this.NBTranslationZ = new CustomControls.NumericBar();
            this.NBProjectionYMax = new CustomControls.NumericBar();
            this.NBProjectionYMin = new CustomControls.NumericBar();
            this.LDepthAveraging = new System.Windows.Forms.Label();
            this.NBDepthAveragingCacheSize = new CustomControls.NumericBar();
            this.NBDepthAveragingLimit = new CustomControls.NumericBar();
            this.NBDepthAveragingMinCount = new CustomControls.NumericBar();
            this.NBFusionSpacing = new CustomControls.NumericBar();
            this.NBProjectionWidth = new CustomControls.NumericBar();
            this.NBCoreX = new CustomControls.NumericBar();
            this.NBCoreY = new CustomControls.NumericBar();
            this.LProjection = new System.Windows.Forms.Label();
            this.SSSettings = new System.Windows.Forms.StatusStrip();
            this.TSDDBSaveFile = new System.Windows.Forms.ToolStripDropDownButton();
            this.TSMIImport = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMIExport = new System.Windows.Forms.ToolStripMenuItem();
            this.SFDExport = new System.Windows.Forms.SaveFileDialog();
            this.OFDImport = new System.Windows.Forms.OpenFileDialog();
            this.FLPSettings.SuspendLayout();
            this.SSSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // FLPSettings
            // 
            resources.ApplyResources(this.FLPSettings, "FLPSettings");
            this.FLPSettings.Controls.Add(this.CBClippingEnabled);
            this.FLPSettings.Controls.Add(this.NBClippingRadiusMin);
            this.FLPSettings.Controls.Add(this.NBClippingRadiusMax);
            this.FLPSettings.Controls.Add(this.NBClippingDepthMax);
            this.FLPSettings.Controls.Add(this.NBClippingYMin);
            this.FLPSettings.Controls.Add(this.LTransformations);
            this.FLPSettings.Controls.Add(this.NBRotationX);
            this.FLPSettings.Controls.Add(this.NBSplitPlaneAngle);
            this.FLPSettings.Controls.Add(this.NBRotationZ);
            this.FLPSettings.Controls.Add(this.NBTranslationX);
            this.FLPSettings.Controls.Add(this.NBTranslationY);
            this.FLPSettings.Controls.Add(this.NBTranslationZ);
            this.FLPSettings.Controls.Add(this.LProjection);
            this.FLPSettings.Controls.Add(this.NBProjectionYMax);
            this.FLPSettings.Controls.Add(this.NBProjectionYMin);
            this.FLPSettings.Controls.Add(this.NBProjectionWidth);
            this.FLPSettings.Controls.Add(this.NBCoreX);
            this.FLPSettings.Controls.Add(this.NBCoreY);
            this.FLPSettings.Controls.Add(this.LDepthAveraging);
            this.FLPSettings.Controls.Add(this.NBDepthAveragingCacheSize);
            this.FLPSettings.Controls.Add(this.NBDepthAveragingLimit);
            this.FLPSettings.Controls.Add(this.NBDepthAveragingMinCount);
            this.FLPSettings.Controls.Add(this.NBFusionSpacing);   
            this.FLPSettings.Name = "FLPSettings";
            this.FLPSettings.Resize += new System.EventHandler(this.FLPSettings_Resize);
            // 
            // CBClippingEnabled
            // 
            resources.ApplyResources(this.CBClippingEnabled, "CBClippingEnabled");
            this.CBClippingEnabled.Name = "CBClippingEnabled";
            this.CBClippingEnabled.UseVisualStyleBackColor = true;
            // 
            // NBClippingRadiusMin
            // 
            this.NBClippingRadiusMin.ByteValue = ((byte)(0));
            this.NBClippingRadiusMin.DecimalPlaces = 2;
            this.NBClippingRadiusMin.DoubleValue = 0D;
            this.NBClippingRadiusMin.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NBClippingRadiusMin.Int32Value = 0;
            this.NBClippingRadiusMin.IntegerValue = false;
            resources.ApplyResources(this.NBClippingRadiusMin, "NBClippingRadiusMin");
            this.NBClippingRadiusMin.Maximum = 0.5D;
            this.NBClippingRadiusMin.Minimum = 0D;
            this.NBClippingRadiusMin.Name = "NBClippingRadiusMin";
            this.NBClippingRadiusMin.Value = 0F;
            this.NBClippingRadiusMin.ValueChangeEventEnabled = true;
            // 
            // NBClippingRadiusMax
            // 
            this.NBClippingRadiusMax.ByteValue = ((byte)(0));
            this.NBClippingRadiusMax.DecimalPlaces = 2;
            this.NBClippingRadiusMax.DoubleValue = 0D;
            this.NBClippingRadiusMax.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NBClippingRadiusMax.Int32Value = 0;
            this.NBClippingRadiusMax.IntegerValue = false;
            resources.ApplyResources(this.NBClippingRadiusMax, "NBClippingRadiusMax");
            this.NBClippingRadiusMax.Maximum = 0.5D;
            this.NBClippingRadiusMax.Minimum = 0D;
            this.NBClippingRadiusMax.Name = "NBClippingRadiusMax";
            this.NBClippingRadiusMax.Value = 0F;
            this.NBClippingRadiusMax.ValueChangeEventEnabled = true;
            // 
            // NBClippingDepthMax
            // 
            this.NBClippingDepthMax.ByteValue = ((byte)(0));
            this.NBClippingDepthMax.DecimalPlaces = 2;
            this.NBClippingDepthMax.DoubleValue = 0D;
            this.NBClippingDepthMax.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NBClippingDepthMax.Int32Value = 0;
            this.NBClippingDepthMax.IntegerValue = false;
            resources.ApplyResources(this.NBClippingDepthMax, "NBClippingDepthMax");
            this.NBClippingDepthMax.Maximum = 10D;
            this.NBClippingDepthMax.Minimum = 0D;
            this.NBClippingDepthMax.Name = "NBClippingDepthMax";
            this.NBClippingDepthMax.Value = 0F;
            this.NBClippingDepthMax.ValueChangeEventEnabled = true;
            // 
            // NBClippingYMin
            // 
            this.NBClippingYMin.ByteValue = ((byte)(0));
            this.NBClippingYMin.DecimalPlaces = 2;
            this.NBClippingYMin.DoubleValue = 0D;
            this.NBClippingYMin.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NBClippingYMin.Int32Value = 0;
            this.NBClippingYMin.IntegerValue = false;
            resources.ApplyResources(this.NBClippingYMin, "NBClippingYMin");
            this.NBClippingYMin.Maximum = 2D;
            this.NBClippingYMin.Minimum = -2D;
            this.NBClippingYMin.Name = "NBClippingYMin";
            this.NBClippingYMin.Value = 0F;
            this.NBClippingYMin.ValueChangeEventEnabled = true;
            // 
            // LTransformations
            // 
            resources.ApplyResources(this.LTransformations, "LTransformations");
            this.LTransformations.Name = "LTransformations";
            // 
            // NBRotationX
            // 
            this.NBRotationX.ByteValue = ((byte)(0));
            this.NBRotationX.DecimalPlaces = 2;
            this.NBRotationX.DoubleValue = 0D;
            this.NBRotationX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NBRotationX.Int32Value = 0;
            this.NBRotationX.IntegerValue = false;
            resources.ApplyResources(this.NBRotationX, "NBRotationX");
            this.NBRotationX.Maximum = 45D;
            this.NBRotationX.Minimum = -45D;
            this.NBRotationX.Name = "NBRotationX";
            this.NBRotationX.Value = 0F;
            this.NBRotationX.ValueChangeEventEnabled = true;
            // 
            // NBSplitPlaneAngle
            // 
            this.NBSplitPlaneAngle.ByteValue = ((byte)(0));
            this.NBSplitPlaneAngle.DecimalPlaces = 2;
            this.NBSplitPlaneAngle.DoubleValue = 0D;
            this.NBSplitPlaneAngle.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NBSplitPlaneAngle.Int32Value = 0;
            this.NBSplitPlaneAngle.IntegerValue = false;
            resources.ApplyResources(this.NBSplitPlaneAngle, "NBSplitPlaneAngle");
            this.NBSplitPlaneAngle.Maximum = 180D;
            this.NBSplitPlaneAngle.Minimum = 0D;
            this.NBSplitPlaneAngle.Name = "NBSplitPlaneAngle";
            this.NBSplitPlaneAngle.Value = 0F;
            this.NBSplitPlaneAngle.ValueChangeEventEnabled = true;
            // 
            // NBRotationZ
            // 
            this.NBRotationZ.ByteValue = ((byte)(0));
            this.NBRotationZ.DecimalPlaces = 2;
            this.NBRotationZ.DoubleValue = 0D;
            this.NBRotationZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NBRotationZ.Int32Value = 0;
            this.NBRotationZ.IntegerValue = false;
            resources.ApplyResources(this.NBRotationZ, "NBRotationZ");
            this.NBRotationZ.Maximum = 45D;
            this.NBRotationZ.Minimum = -45D;
            this.NBRotationZ.Name = "NBRotationZ";
            this.NBRotationZ.Value = 0F;
            this.NBRotationZ.ValueChangeEventEnabled = true;
            // 
            // NBTranslationX
            // 
            this.NBTranslationX.ByteValue = ((byte)(0));
            this.NBTranslationX.DecimalPlaces = 3;
            this.NBTranslationX.DoubleValue = 0D;
            this.NBTranslationX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.NBTranslationX.Int32Value = 0;
            this.NBTranslationX.IntegerValue = false;
            resources.ApplyResources(this.NBTranslationX, "NBTranslationX");
            this.NBTranslationX.Maximum = 1D;
            this.NBTranslationX.Minimum = -1D;
            this.NBTranslationX.Name = "NBTranslationX";
            this.NBTranslationX.Value = 0F;
            this.NBTranslationX.ValueChangeEventEnabled = true;
            // 
            // NBTranslationY
            // 
            this.NBTranslationY.ByteValue = ((byte)(0));
            this.NBTranslationY.DecimalPlaces = 3;
            this.NBTranslationY.DoubleValue = 0D;
            this.NBTranslationY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.NBTranslationY.Int32Value = 0;
            this.NBTranslationY.IntegerValue = false;
            resources.ApplyResources(this.NBTranslationY, "NBTranslationY");
            this.NBTranslationY.Maximum = 1D;
            this.NBTranslationY.Minimum = -1D;
            this.NBTranslationY.Name = "NBTranslationY";
            this.NBTranslationY.Value = 0F;
            this.NBTranslationY.ValueChangeEventEnabled = true;
            // 
            // NBTranslationZ
            // 
            this.NBTranslationZ.ByteValue = ((byte)(0));
            this.NBTranslationZ.DecimalPlaces = 3;
            this.NBTranslationZ.DoubleValue = 0D;
            this.NBTranslationZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.NBTranslationZ.Int32Value = 0;
            this.NBTranslationZ.IntegerValue = false;
            resources.ApplyResources(this.NBTranslationZ, "NBTranslationZ");
            this.NBTranslationZ.Maximum = 3D;
            this.NBTranslationZ.Minimum = 0D;
            this.NBTranslationZ.Name = "NBTranslationZ";
            this.NBTranslationZ.Value = 0F;
            this.NBTranslationZ.ValueChangeEventEnabled = true;
            // 
            // NBProjectionYMax
            // 
            this.NBProjectionYMax.ByteValue = ((byte)(0));
            this.NBProjectionYMax.DecimalPlaces = 2;
            this.NBProjectionYMax.DoubleValue = 0D;
            this.NBProjectionYMax.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NBProjectionYMax.Int32Value = 0;
            this.NBProjectionYMax.IntegerValue = false;
            resources.ApplyResources(this.NBProjectionYMax, "NBProjectionYMax");
            this.NBProjectionYMax.Maximum = 2D;
            this.NBProjectionYMax.Minimum = -2D;
            this.NBProjectionYMax.Name = "NBProjectionYMax";
            this.NBProjectionYMax.Value = 0F;
            this.NBProjectionYMax.ValueChangeEventEnabled = true;
            // 
            // NBProjectionYMin
            // 
            this.NBProjectionYMin.ByteValue = ((byte)(0));
            this.NBProjectionYMin.DecimalPlaces = 2;
            this.NBProjectionYMin.DoubleValue = 0D;
            this.NBProjectionYMin.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NBProjectionYMin.Int32Value = 0;
            this.NBProjectionYMin.IntegerValue = false;
            resources.ApplyResources(this.NBProjectionYMin, "NBProjectionYMin");
            this.NBProjectionYMin.Maximum = 2D;
            this.NBProjectionYMin.Minimum = -2D;
            this.NBProjectionYMin.Name = "NBProjectionYMin";
            this.NBProjectionYMin.Value = 0F;
            this.NBProjectionYMin.ValueChangeEventEnabled = true;
            // 
            // LDepthAveraging
            // 
            resources.ApplyResources(this.LDepthAveraging, "LDepthAveraging");
            this.LDepthAveraging.Name = "LDepthAveraging";
            // 
            // NBDepthAveragingCacheSize
            // 
            this.NBDepthAveragingCacheSize.ByteValue = ((byte)(0));
            this.NBDepthAveragingCacheSize.DecimalPlaces = 2;
            this.NBDepthAveragingCacheSize.DoubleValue = 0D;
            this.NBDepthAveragingCacheSize.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NBDepthAveragingCacheSize.Int32Value = 0;
            this.NBDepthAveragingCacheSize.IntegerValue = true;
            resources.ApplyResources(this.NBDepthAveragingCacheSize, "NBDepthAveragingCacheSize");
            this.NBDepthAveragingCacheSize.Maximum = 60D;
            this.NBDepthAveragingCacheSize.Minimum = 0D;
            this.NBDepthAveragingCacheSize.Name = "NBDepthAveragingCacheSize";
            this.NBDepthAveragingCacheSize.Value = 0F;
            this.NBDepthAveragingCacheSize.ValueChangeEventEnabled = true;
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
            // NBDepthAveragingMinCount
            // 
            this.NBDepthAveragingMinCount.ByteValue = ((byte)(1));
            this.NBDepthAveragingMinCount.DecimalPlaces = 2;
            this.NBDepthAveragingMinCount.DoubleValue = 1D;
            this.NBDepthAveragingMinCount.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NBDepthAveragingMinCount.Int32Value = 1;
            this.NBDepthAveragingMinCount.IntegerValue = true;
            resources.ApplyResources(this.NBDepthAveragingMinCount, "NBDepthAveragingMinCount");
            this.NBDepthAveragingMinCount.Maximum = 60D;
            this.NBDepthAveragingMinCount.Minimum = 1D;
            this.NBDepthAveragingMinCount.Name = "NBDepthAveragingMinCount";
            this.NBDepthAveragingMinCount.Value = 1F;
            this.NBDepthAveragingMinCount.ValueChangeEventEnabled = true;
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
            // LProjection
            // 
            resources.ApplyResources(this.LProjection, "LProjection");
            this.LProjection.Name = "LProjection";
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
            // TurntableSettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SSSettings);
            this.Controls.Add(this.FLPSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TurntableSettingsForm";
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
        public CustomControls.NumericBar NBTranslationZ;
        public CustomControls.NumericBar NBClippingDepthMax;
        public CustomControls.NumericBar NBRotationX;
        public CustomControls.NumericBar NBRotationZ;
        public CustomControls.NumericBar NBTranslationX;
        public CustomControls.NumericBar NBTranslationY;
        public CustomControls.NumericBar NBProjectionYMax;
        public CustomControls.NumericBar NBSplitPlaneAngle;
        public CustomControls.NumericBar NBProjectionYMin;
        public CustomControls.NumericBar NBProjectionWidth;
        public CustomControls.NumericBar NBClippingRadiusMax;
        public CustomControls.NumericBar NBClippingRadiusMin;
        public CustomControls.NumericBar NBDepthAveragingMinCount;
        public CustomControls.NumericBar NBDepthAveragingLimit;
        public System.Windows.Forms.CheckBox CBClippingEnabled;
        public CustomControls.NumericBar NBDepthAveragingCacheSize;
        public CustomControls.NumericBar NBFusionSpacing;
        public CustomControls.NumericBar NBClippingYMin;
        public CustomControls.NumericBar NBCoreX;
        public CustomControls.NumericBar NBCoreY;
        private System.Windows.Forms.Label LTransformations;
        private System.Windows.Forms.Label LDepthAveraging;
        private System.Windows.Forms.Label LProjection;

    }
}