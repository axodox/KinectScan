namespace KinectScan
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.FLPSettings = new System.Windows.Forms.FlowLayoutPanel();
            this.NBZLimit = new CustomControls.NumericBar();
            this.NBRainbowPeriod = new CustomControls.NumericBar();
            this.CBReproject = new System.Windows.Forms.CheckBox();
            this.NBReprojectionRotationX = new CustomControls.NumericBar();
            this.NBReprojectionRotationY = new CustomControls.NumericBar();
            this.NBReprojectionRotationZ = new CustomControls.NumericBar();
            this.NBReprojectionTranslationX = new CustomControls.NumericBar();
            this.NBReprojectionTranslationY = new CustomControls.NumericBar();
            this.NBReprojectionTranslationZ = new CustomControls.NumericBar();
            this.NBGaussFilterPasses = new CustomControls.NumericBar();
            this.LShadingMode = new System.Windows.Forms.Label();
            this.DDBShadingMode = new CustomControls.DropDownButton();
            this.NBAveragedFrames = new CustomControls.NumericBar();
            this.NBMinColoringDepth = new CustomControls.NumericBar();
            this.LLShowAdvancedSettings = new System.Windows.Forms.LinkLabel();
            this.NBGaussSigma = new CustomControls.NumericBar();
            this.NBTriangleRemove = new CustomControls.NumericBar();
            this.LLShowTestSettings = new System.Windows.Forms.LinkLabel();
            this.NBDepthDispX = new CustomControls.NumericBar();
            this.NBDepthDispY = new CustomControls.NumericBar();
            this.NBDepthScaleX = new CustomControls.NumericBar();
            this.NBDepthScaleY = new CustomControls.NumericBar();
            this.CBAntidistortTest = new System.Windows.Forms.CheckBox();
            this.FLPSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // FLPSettings
            // 
            resources.ApplyResources(this.FLPSettings, "FLPSettings");
            this.FLPSettings.Controls.Add(this.NBZLimit);
            this.FLPSettings.Controls.Add(this.NBRainbowPeriod);
            this.FLPSettings.Controls.Add(this.CBReproject);
            this.FLPSettings.Controls.Add(this.NBReprojectionRotationX);
            this.FLPSettings.Controls.Add(this.NBReprojectionRotationY);
            this.FLPSettings.Controls.Add(this.NBReprojectionRotationZ);
            this.FLPSettings.Controls.Add(this.NBReprojectionTranslationX);
            this.FLPSettings.Controls.Add(this.NBReprojectionTranslationY);
            this.FLPSettings.Controls.Add(this.NBReprojectionTranslationZ);
            this.FLPSettings.Controls.Add(this.NBGaussFilterPasses);
            this.FLPSettings.Controls.Add(this.LShadingMode);
            this.FLPSettings.Controls.Add(this.DDBShadingMode);
            this.FLPSettings.Controls.Add(this.NBAveragedFrames);
            this.FLPSettings.Controls.Add(this.NBMinColoringDepth);
            this.FLPSettings.Controls.Add(this.LLShowAdvancedSettings);
            this.FLPSettings.Controls.Add(this.NBGaussSigma);
            this.FLPSettings.Controls.Add(this.NBTriangleRemove);
            this.FLPSettings.Controls.Add(this.LLShowTestSettings);
            this.FLPSettings.Controls.Add(this.NBDepthDispX);
            this.FLPSettings.Controls.Add(this.NBDepthDispY);
            this.FLPSettings.Controls.Add(this.NBDepthScaleX);
            this.FLPSettings.Controls.Add(this.NBDepthScaleY);
            this.FLPSettings.Controls.Add(this.CBAntidistortTest);
            this.FLPSettings.Name = "FLPSettings";
            this.FLPSettings.Resize += new System.EventHandler(this.FLPSettings_Resize);
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
            this.NBZLimit.MaximumSize = new System.Drawing.Size(4096, 56);
            this.NBZLimit.Minimum = 0D;
            this.NBZLimit.MinimumSize = new System.Drawing.Size(128, 56);
            this.NBZLimit.Name = "NBZLimit";
            this.NBZLimit.Value = 0F;
            this.NBZLimit.ValueChangeEventEnabled = true;
            // 
            // NBRainbowPeriod
            // 
            this.NBRainbowPeriod.ByteValue = ((byte)(0));
            this.NBRainbowPeriod.DecimalPlaces = 2;
            this.NBRainbowPeriod.DoubleValue = 0D;
            this.NBRainbowPeriod.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NBRainbowPeriod.Int32Value = 0;
            this.NBRainbowPeriod.IntegerValue = false;
            resources.ApplyResources(this.NBRainbowPeriod, "NBRainbowPeriod");
            this.NBRainbowPeriod.Maximum = 2D;
            this.NBRainbowPeriod.MaximumSize = new System.Drawing.Size(4096, 56);
            this.NBRainbowPeriod.Minimum = 0D;
            this.NBRainbowPeriod.MinimumSize = new System.Drawing.Size(128, 56);
            this.NBRainbowPeriod.Name = "NBRainbowPeriod";
            this.NBRainbowPeriod.Value = 0F;
            this.NBRainbowPeriod.ValueChangeEventEnabled = true;
            // 
            // CBReproject
            // 
            resources.ApplyResources(this.CBReproject, "CBReproject");
            this.CBReproject.Name = "CBReproject";
            this.CBReproject.UseVisualStyleBackColor = true;
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
            0});
            this.NBReprojectionRotationX.Int32Value = 0;
            this.NBReprojectionRotationX.IntegerValue = false;
            resources.ApplyResources(this.NBReprojectionRotationX, "NBReprojectionRotationX");
            this.NBReprojectionRotationX.Maximum = 45D;
            this.NBReprojectionRotationX.MaximumSize = new System.Drawing.Size(4096, 56);
            this.NBReprojectionRotationX.Minimum = -45D;
            this.NBReprojectionRotationX.MinimumSize = new System.Drawing.Size(128, 56);
            this.NBReprojectionRotationX.Name = "NBReprojectionRotationX";
            this.NBReprojectionRotationX.Value = 0F;
            this.NBReprojectionRotationX.ValueChangeEventEnabled = true;
            // 
            // NBReprojectionRotationY
            // 
            this.NBReprojectionRotationY.ByteValue = ((byte)(0));
            this.NBReprojectionRotationY.DecimalPlaces = 2;
            this.NBReprojectionRotationY.DoubleValue = 0D;
            this.NBReprojectionRotationY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NBReprojectionRotationY.Int32Value = 0;
            this.NBReprojectionRotationY.IntegerValue = false;
            resources.ApplyResources(this.NBReprojectionRotationY, "NBReprojectionRotationY");
            this.NBReprojectionRotationY.Maximum = 45D;
            this.NBReprojectionRotationY.MaximumSize = new System.Drawing.Size(4096, 56);
            this.NBReprojectionRotationY.Minimum = -45D;
            this.NBReprojectionRotationY.MinimumSize = new System.Drawing.Size(128, 56);
            this.NBReprojectionRotationY.Name = "NBReprojectionRotationY";
            this.NBReprojectionRotationY.Value = 0F;
            this.NBReprojectionRotationY.ValueChangeEventEnabled = true;
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
            0});
            this.NBReprojectionRotationZ.Int32Value = 0;
            this.NBReprojectionRotationZ.IntegerValue = false;
            resources.ApplyResources(this.NBReprojectionRotationZ, "NBReprojectionRotationZ");
            this.NBReprojectionRotationZ.Maximum = 45D;
            this.NBReprojectionRotationZ.MaximumSize = new System.Drawing.Size(4096, 56);
            this.NBReprojectionRotationZ.Minimum = -45D;
            this.NBReprojectionRotationZ.MinimumSize = new System.Drawing.Size(128, 56);
            this.NBReprojectionRotationZ.Name = "NBReprojectionRotationZ";
            this.NBReprojectionRotationZ.Value = 0F;
            this.NBReprojectionRotationZ.ValueChangeEventEnabled = true;
            // 
            // NBReprojectionTranslationX
            // 
            this.NBReprojectionTranslationX.ByteValue = ((byte)(0));
            this.NBReprojectionTranslationX.DecimalPlaces = 2;
            this.NBReprojectionTranslationX.DoubleValue = 0D;
            this.NBReprojectionTranslationX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NBReprojectionTranslationX.Int32Value = 0;
            this.NBReprojectionTranslationX.IntegerValue = false;
            resources.ApplyResources(this.NBReprojectionTranslationX, "NBReprojectionTranslationX");
            this.NBReprojectionTranslationX.Maximum = 1D;
            this.NBReprojectionTranslationX.MaximumSize = new System.Drawing.Size(4096, 56);
            this.NBReprojectionTranslationX.Minimum = -1D;
            this.NBReprojectionTranslationX.MinimumSize = new System.Drawing.Size(128, 56);
            this.NBReprojectionTranslationX.Name = "NBReprojectionTranslationX";
            this.NBReprojectionTranslationX.Value = 0F;
            this.NBReprojectionTranslationX.ValueChangeEventEnabled = true;
            // 
            // NBReprojectionTranslationY
            // 
            this.NBReprojectionTranslationY.ByteValue = ((byte)(0));
            this.NBReprojectionTranslationY.DecimalPlaces = 2;
            this.NBReprojectionTranslationY.DoubleValue = 0D;
            this.NBReprojectionTranslationY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NBReprojectionTranslationY.Int32Value = 0;
            this.NBReprojectionTranslationY.IntegerValue = false;
            resources.ApplyResources(this.NBReprojectionTranslationY, "NBReprojectionTranslationY");
            this.NBReprojectionTranslationY.Maximum = 1D;
            this.NBReprojectionTranslationY.MaximumSize = new System.Drawing.Size(4096, 56);
            this.NBReprojectionTranslationY.Minimum = -1D;
            this.NBReprojectionTranslationY.MinimumSize = new System.Drawing.Size(128, 56);
            this.NBReprojectionTranslationY.Name = "NBReprojectionTranslationY";
            this.NBReprojectionTranslationY.Value = 0F;
            this.NBReprojectionTranslationY.ValueChangeEventEnabled = true;
            // 
            // NBReprojectionTranslationZ
            // 
            this.NBReprojectionTranslationZ.ByteValue = ((byte)(0));
            this.NBReprojectionTranslationZ.DecimalPlaces = 2;
            this.NBReprojectionTranslationZ.DoubleValue = 0D;
            this.NBReprojectionTranslationZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NBReprojectionTranslationZ.Int32Value = 0;
            this.NBReprojectionTranslationZ.IntegerValue = false;
            resources.ApplyResources(this.NBReprojectionTranslationZ, "NBReprojectionTranslationZ");
            this.NBReprojectionTranslationZ.Maximum = 3D;
            this.NBReprojectionTranslationZ.MaximumSize = new System.Drawing.Size(4096, 56);
            this.NBReprojectionTranslationZ.Minimum = 0D;
            this.NBReprojectionTranslationZ.MinimumSize = new System.Drawing.Size(128, 56);
            this.NBReprojectionTranslationZ.Name = "NBReprojectionTranslationZ";
            this.NBReprojectionTranslationZ.Value = 0F;
            this.NBReprojectionTranslationZ.ValueChangeEventEnabled = true;
            // 
            // NBGaussFilterPasses
            // 
            this.NBGaussFilterPasses.ByteValue = ((byte)(0));
            this.NBGaussFilterPasses.DecimalPlaces = 2;
            this.NBGaussFilterPasses.DoubleValue = 0D;
            this.NBGaussFilterPasses.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NBGaussFilterPasses.Int32Value = 0;
            this.NBGaussFilterPasses.IntegerValue = true;
            resources.ApplyResources(this.NBGaussFilterPasses, "NBGaussFilterPasses");
            this.NBGaussFilterPasses.Maximum = 32D;
            this.NBGaussFilterPasses.MaximumSize = new System.Drawing.Size(4096, 56);
            this.NBGaussFilterPasses.Minimum = 0D;
            this.NBGaussFilterPasses.MinimumSize = new System.Drawing.Size(128, 56);
            this.NBGaussFilterPasses.Name = "NBGaussFilterPasses";
            this.NBGaussFilterPasses.Value = 0F;
            this.NBGaussFilterPasses.ValueChangeEventEnabled = true;
            // 
            // LShadingMode
            // 
            resources.ApplyResources(this.LShadingMode, "LShadingMode");
            this.LShadingMode.Name = "LShadingMode";
            // 
            // DDBShadingMode
            // 
            resources.ApplyResources(this.DDBShadingMode, "DDBShadingMode");
            this.DDBShadingMode.Name = "DDBShadingMode";
            this.DDBShadingMode.SelectedIndex = 0;
            this.DDBShadingMode.UseVisualStyleBackColor = true;
            // 
            // NBAveragedFrames
            // 
            this.NBAveragedFrames.ByteValue = ((byte)(2));
            this.NBAveragedFrames.DecimalPlaces = 2;
            this.NBAveragedFrames.DoubleValue = 2D;
            this.NBAveragedFrames.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NBAveragedFrames.Int32Value = 2;
            this.NBAveragedFrames.IntegerValue = true;
            resources.ApplyResources(this.NBAveragedFrames, "NBAveragedFrames");
            this.NBAveragedFrames.Maximum = 32D;
            this.NBAveragedFrames.MaximumSize = new System.Drawing.Size(4096, 56);
            this.NBAveragedFrames.Minimum = 2D;
            this.NBAveragedFrames.MinimumSize = new System.Drawing.Size(128, 56);
            this.NBAveragedFrames.Name = "NBAveragedFrames";
            this.NBAveragedFrames.Value = 2F;
            this.NBAveragedFrames.ValueChangeEventEnabled = true;
            // 
            // NBMinColoringDepth
            // 
            this.NBMinColoringDepth.ByteValue = ((byte)(0));
            this.NBMinColoringDepth.DecimalPlaces = 2;
            this.NBMinColoringDepth.DoubleValue = 0D;
            this.NBMinColoringDepth.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NBMinColoringDepth.Int32Value = 0;
            this.NBMinColoringDepth.IntegerValue = false;
            resources.ApplyResources(this.NBMinColoringDepth, "NBMinColoringDepth");
            this.NBMinColoringDepth.Maximum = 1D;
            this.NBMinColoringDepth.MaximumSize = new System.Drawing.Size(4096, 56);
            this.NBMinColoringDepth.Minimum = 0D;
            this.NBMinColoringDepth.MinimumSize = new System.Drawing.Size(128, 56);
            this.NBMinColoringDepth.Name = "NBMinColoringDepth";
            this.NBMinColoringDepth.Value = 0F;
            this.NBMinColoringDepth.ValueChangeEventEnabled = true;
            // 
            // LLShowAdvancedSettings
            // 
            resources.ApplyResources(this.LLShowAdvancedSettings, "LLShowAdvancedSettings");
            this.LLShowAdvancedSettings.Name = "LLShowAdvancedSettings";
            this.LLShowAdvancedSettings.TabStop = true;
            this.LLShowAdvancedSettings.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LLShowAdvancedSettings_LinkClicked);
            // 
            // NBGaussSigma
            // 
            this.NBGaussSigma.ByteValue = ((byte)(0));
            this.NBGaussSigma.DecimalPlaces = 2;
            this.NBGaussSigma.DoubleValue = 0D;
            this.NBGaussSigma.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NBGaussSigma.Int32Value = 0;
            this.NBGaussSigma.IntegerValue = false;
            resources.ApplyResources(this.NBGaussSigma, "NBGaussSigma");
            this.NBGaussSigma.Maximum = 8D;
            this.NBGaussSigma.MaximumSize = new System.Drawing.Size(4096, 56);
            this.NBGaussSigma.Minimum = 0D;
            this.NBGaussSigma.MinimumSize = new System.Drawing.Size(128, 56);
            this.NBGaussSigma.Name = "NBGaussSigma";
            this.NBGaussSigma.Tag = "Advanced";
            this.NBGaussSigma.Value = 0F;
            this.NBGaussSigma.ValueChangeEventEnabled = true;
            // 
            // NBTriangleRemove
            // 
            this.NBTriangleRemove.ByteValue = ((byte)(0));
            this.NBTriangleRemove.DecimalPlaces = 4;
            this.NBTriangleRemove.DoubleValue = 0D;
            this.NBTriangleRemove.Increment = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.NBTriangleRemove.Int32Value = 0;
            this.NBTriangleRemove.IntegerValue = false;
            resources.ApplyResources(this.NBTriangleRemove, "NBTriangleRemove");
            this.NBTriangleRemove.Maximum = 0.07D;
            this.NBTriangleRemove.MaximumSize = new System.Drawing.Size(4096, 56);
            this.NBTriangleRemove.Minimum = 0D;
            this.NBTriangleRemove.MinimumSize = new System.Drawing.Size(128, 56);
            this.NBTriangleRemove.Name = "NBTriangleRemove";
            this.NBTriangleRemove.Tag = "Advanced";
            this.NBTriangleRemove.Value = 0F;
            this.NBTriangleRemove.ValueChangeEventEnabled = true;
            // 
            // LLShowTestSettings
            // 
            resources.ApplyResources(this.LLShowTestSettings, "LLShowTestSettings");
            this.LLShowTestSettings.Name = "LLShowTestSettings";
            this.LLShowTestSettings.TabStop = true;
            this.LLShowTestSettings.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LLShowTestSettings_LinkClicked);
            // 
            // NBDepthDispX
            // 
            this.NBDepthDispX.ByteValue = ((byte)(0));
            this.NBDepthDispX.DecimalPlaces = 2;
            this.NBDepthDispX.DoubleValue = 0D;
            this.NBDepthDispX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NBDepthDispX.Int32Value = 0;
            this.NBDepthDispX.IntegerValue = true;
            resources.ApplyResources(this.NBDepthDispX, "NBDepthDispX");
            this.NBDepthDispX.Maximum = 32D;
            this.NBDepthDispX.MaximumSize = new System.Drawing.Size(4096, 56);
            this.NBDepthDispX.Minimum = -32D;
            this.NBDepthDispX.MinimumSize = new System.Drawing.Size(128, 56);
            this.NBDepthDispX.Name = "NBDepthDispX";
            this.NBDepthDispX.Tag = "Test";
            this.NBDepthDispX.Value = 0F;
            this.NBDepthDispX.ValueChangeEventEnabled = true;
            // 
            // NBDepthDispY
            // 
            this.NBDepthDispY.ByteValue = ((byte)(0));
            this.NBDepthDispY.DecimalPlaces = 2;
            this.NBDepthDispY.DoubleValue = 0D;
            this.NBDepthDispY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NBDepthDispY.Int32Value = 0;
            this.NBDepthDispY.IntegerValue = true;
            resources.ApplyResources(this.NBDepthDispY, "NBDepthDispY");
            this.NBDepthDispY.Maximum = 32D;
            this.NBDepthDispY.MaximumSize = new System.Drawing.Size(4096, 56);
            this.NBDepthDispY.Minimum = -32D;
            this.NBDepthDispY.MinimumSize = new System.Drawing.Size(128, 56);
            this.NBDepthDispY.Name = "NBDepthDispY";
            this.NBDepthDispY.Tag = "Test";
            this.NBDepthDispY.Value = 0F;
            this.NBDepthDispY.ValueChangeEventEnabled = true;
            // 
            // NBDepthScaleX
            // 
            this.NBDepthScaleX.ByteValue = ((byte)(1));
            this.NBDepthScaleX.DecimalPlaces = 2;
            this.NBDepthScaleX.DoubleValue = 1D;
            this.NBDepthScaleX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NBDepthScaleX.Int32Value = 1;
            this.NBDepthScaleX.IntegerValue = false;
            resources.ApplyResources(this.NBDepthScaleX, "NBDepthScaleX");
            this.NBDepthScaleX.Maximum = 1.2D;
            this.NBDepthScaleX.MaximumSize = new System.Drawing.Size(4096, 56);
            this.NBDepthScaleX.Minimum = 0.8D;
            this.NBDepthScaleX.MinimumSize = new System.Drawing.Size(128, 56);
            this.NBDepthScaleX.Name = "NBDepthScaleX";
            this.NBDepthScaleX.Tag = "Test";
            this.NBDepthScaleX.Value = 1F;
            this.NBDepthScaleX.ValueChangeEventEnabled = true;
            // 
            // NBDepthScaleY
            // 
            this.NBDepthScaleY.ByteValue = ((byte)(1));
            this.NBDepthScaleY.DecimalPlaces = 2;
            this.NBDepthScaleY.DoubleValue = 1D;
            this.NBDepthScaleY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NBDepthScaleY.Int32Value = 1;
            this.NBDepthScaleY.IntegerValue = false;
            resources.ApplyResources(this.NBDepthScaleY, "NBDepthScaleY");
            this.NBDepthScaleY.Maximum = 1.2D;
            this.NBDepthScaleY.MaximumSize = new System.Drawing.Size(4096, 56);
            this.NBDepthScaleY.Minimum = 0.8D;
            this.NBDepthScaleY.MinimumSize = new System.Drawing.Size(128, 56);
            this.NBDepthScaleY.Name = "NBDepthScaleY";
            this.NBDepthScaleY.Tag = "Test";
            this.NBDepthScaleY.Value = 1F;
            this.NBDepthScaleY.ValueChangeEventEnabled = true;
            // 
            // CBAntidistortTest
            // 
            resources.ApplyResources(this.CBAntidistortTest, "CBAntidistortTest");
            this.CBAntidistortTest.Name = "CBAntidistortTest";
            this.CBAntidistortTest.Tag = "Test";
            this.CBAntidistortTest.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.FLPSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            this.FLPSettings.ResumeLayout(false);
            this.FLPSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel FLPSettings;
        public CustomControls.NumericBar NBTriangleRemove;
        public CustomControls.NumericBar NBReprojectionRotationY;
        public CustomControls.NumericBar NBReprojectionTranslationZ;
        public CustomControls.NumericBar NBZLimit;
        public CustomControls.NumericBar NBRainbowPeriod;
        public System.Windows.Forms.CheckBox CBReproject;
        public CustomControls.NumericBar NBGaussFilterPasses;
        public CustomControls.NumericBar NBGaussSigma;
        private System.Windows.Forms.Label LShadingMode;
        public CustomControls.DropDownButton DDBShadingMode;
        public CustomControls.NumericBar NBReprojectionRotationX;
        public CustomControls.NumericBar NBReprojectionRotationZ;
        public CustomControls.NumericBar NBAveragedFrames;
        public CustomControls.NumericBar NBMinColoringDepth;
        public CustomControls.NumericBar NBReprojectionTranslationX;
        public CustomControls.NumericBar NBReprojectionTranslationY;
        public CustomControls.NumericBar NBDepthDispX;
        public CustomControls.NumericBar NBDepthDispY;
        public CustomControls.NumericBar NBDepthScaleX;
        public CustomControls.NumericBar NBDepthScaleY;
        private System.Windows.Forms.LinkLabel LLShowTestSettings;
        private System.Windows.Forms.LinkLabel LLShowAdvancedSettings;
        public System.Windows.Forms.CheckBox CBAntidistortTest;

    }
}