namespace KinectScan
{
    partial class ProbeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProbeForm));
            this.LVProbes = new System.Windows.Forms.ListView();
            this.CHProbeNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.CHProbeX = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.CHProbeY = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.CHProbeZ = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.CHProbeU = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.CHProbeV = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.CHProbeRawDepth = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SBStatus = new System.Windows.Forms.StatusBar();
            this.TSProbes = new System.Windows.Forms.ToolStrip();
            this.TSBAddProbe = new System.Windows.Forms.ToolStripButton();
            this.TSBMoveProbe = new System.Windows.Forms.ToolStripButton();
            this.TSBRemoveProbe = new System.Windows.Forms.ToolStripButton();
            this.TSProbes.SuspendLayout();
            this.SuspendLayout();
            // 
            // LVProbes
            // 
            resources.ApplyResources(this.LVProbes, "LVProbes");
            this.LVProbes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.CHProbeNumber,
            this.CHProbeX,
            this.CHProbeY,
            this.CHProbeZ,
            this.CHProbeU,
            this.CHProbeV,
            this.CHProbeRawDepth});
            this.LVProbes.FullRowSelect = true;
            this.LVProbes.Name = "LVProbes";
            this.LVProbes.UseCompatibleStateImageBehavior = false;
            this.LVProbes.View = System.Windows.Forms.View.Details;
            // 
            // CHProbeNumber
            // 
            resources.ApplyResources(this.CHProbeNumber, "CHProbeNumber");
            // 
            // CHProbeX
            // 
            resources.ApplyResources(this.CHProbeX, "CHProbeX");
            // 
            // CHProbeY
            // 
            resources.ApplyResources(this.CHProbeY, "CHProbeY");
            // 
            // CHProbeZ
            // 
            resources.ApplyResources(this.CHProbeZ, "CHProbeZ");
            // 
            // CHProbeU
            // 
            resources.ApplyResources(this.CHProbeU, "CHProbeU");
            // 
            // CHProbeV
            // 
            resources.ApplyResources(this.CHProbeV, "CHProbeV");
            // 
            // CHProbeRawDepth
            // 
            resources.ApplyResources(this.CHProbeRawDepth, "CHProbeRawDepth");
            // 
            // SBStatus
            // 
            resources.ApplyResources(this.SBStatus, "SBStatus");
            this.SBStatus.Name = "SBStatus";
            // 
            // TSProbes
            // 
            this.TSProbes.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.TSProbes.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSBAddProbe,
            this.TSBMoveProbe,
            this.TSBRemoveProbe});
            resources.ApplyResources(this.TSProbes, "TSProbes");
            this.TSProbes.Name = "TSProbes";
            this.TSProbes.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // TSBAddProbe
            // 
            resources.ApplyResources(this.TSBAddProbe, "TSBAddProbe");
            this.TSBAddProbe.Image = Properties.Resources.probe16add;
            this.TSBAddProbe.Name = "TSBAddProbe";
            // 
            // TSBMoveProbe
            // 
            resources.ApplyResources(this.TSBMoveProbe, "TSBMoveProbe");
            this.TSBMoveProbe.Image = Properties.Resources.probe16move;
            this.TSBMoveProbe.Name = "TSBMoveProbe";
            // 
            // TSBRemoveProbe
            // 
            resources.ApplyResources(this.TSBRemoveProbe, "TSBRemoveProbe");
            this.TSBRemoveProbe.Image = Properties.Resources.probe16remove;
            this.TSBRemoveProbe.Name = "TSBRemoveProbe";
            // 
            // ProbeForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TSProbes);
            this.Controls.Add(this.SBStatus);
            this.Controls.Add(this.LVProbes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProbeForm";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProbeForm_FormClosing);
            this.TSProbes.ResumeLayout(false);
            this.TSProbes.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColumnHeader CHProbeNumber;
        private System.Windows.Forms.ColumnHeader CHProbeX;
        private System.Windows.Forms.ColumnHeader CHProbeY;
        private System.Windows.Forms.ColumnHeader CHProbeZ;
        private System.Windows.Forms.ColumnHeader CHProbeU;
        private System.Windows.Forms.ColumnHeader CHProbeV;
        private System.Windows.Forms.ColumnHeader CHProbeRawDepth;
        private System.Windows.Forms.ToolStrip TSProbes;
        public System.Windows.Forms.ListView LVProbes;
        public System.Windows.Forms.ToolStripButton TSBAddProbe;
        public System.Windows.Forms.ToolStripButton TSBRemoveProbe;
        public System.Windows.Forms.ToolStripButton TSBMoveProbe;
        public System.Windows.Forms.StatusBar SBStatus;
    }
}