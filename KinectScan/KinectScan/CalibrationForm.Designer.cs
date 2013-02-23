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
            this.LLPath = new System.Windows.Forms.LinkLabel();
            this.BStart = new System.Windows.Forms.Button();
            this.BNext = new System.Windows.Forms.Button();
            this.CBColor = new System.Windows.Forms.CheckBox();
            this.CBIR = new System.Windows.Forms.CheckBox();
            this.CBDepth = new System.Windows.Forms.CheckBox();
            this.LPathHelp = new System.Windows.Forms.Label();
            this.GBImage = new System.Windows.Forms.GroupBox();
            this.BReset = new System.Windows.Forms.Button();
            this.LImageHelp = new System.Windows.Forms.Label();
            this.BExit = new System.Windows.Forms.Button();
            this.GBPath = new System.Windows.Forms.GroupBox();
            this.GBFinish = new System.Windows.Forms.GroupBox();
            this.LStatus = new System.Windows.Forms.Label();
            this.GBImage.SuspendLayout();
            this.GBPath.SuspendLayout();
            this.GBFinish.SuspendLayout();
            this.SuspendLayout();
            // 
            // LLPath
            // 
            resources.ApplyResources(this.LLPath, "LLPath");
            this.LLPath.Name = "LLPath";
            this.LLPath.TabStop = true;
            this.LLPath.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LLPath_LinkClicked);
            // 
            // BStart
            // 
            resources.ApplyResources(this.BStart, "BStart");
            this.BStart.Name = "BStart";
            this.BStart.UseVisualStyleBackColor = true;
            this.BStart.Click += new System.EventHandler(this.BStart_Click);
            this.BStart.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CalibrationForm_KeyDown);
            // 
            // BNext
            // 
            resources.ApplyResources(this.BNext, "BNext");
            this.BNext.Name = "BNext";
            this.BNext.UseVisualStyleBackColor = true;
            this.BNext.Click += new System.EventHandler(this.BNext_Click);
            this.BNext.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CalibrationForm_KeyDown);
            // 
            // CBColor
            // 
            resources.ApplyResources(this.CBColor, "CBColor");
            this.CBColor.AutoCheck = false;
            this.CBColor.Name = "CBColor";
            this.CBColor.UseVisualStyleBackColor = true;
            this.CBColor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CalibrationForm_KeyDown);
            // 
            // CBIR
            // 
            resources.ApplyResources(this.CBIR, "CBIR");
            this.CBIR.AutoCheck = false;
            this.CBIR.Name = "CBIR";
            this.CBIR.UseVisualStyleBackColor = true;
            this.CBIR.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CalibrationForm_KeyDown);
            // 
            // CBDepth
            // 
            resources.ApplyResources(this.CBDepth, "CBDepth");
            this.CBDepth.AutoCheck = false;
            this.CBDepth.Name = "CBDepth";
            this.CBDepth.UseVisualStyleBackColor = true;
            this.CBDepth.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CalibrationForm_KeyDown);
            // 
            // LPathHelp
            // 
            resources.ApplyResources(this.LPathHelp, "LPathHelp");
            this.LPathHelp.Name = "LPathHelp";
            // 
            // GBImage
            // 
            resources.ApplyResources(this.GBImage, "GBImage");
            this.GBImage.Controls.Add(this.BReset);
            this.GBImage.Controls.Add(this.LImageHelp);
            this.GBImage.Controls.Add(this.CBColor);
            this.GBImage.Controls.Add(this.CBIR);
            this.GBImage.Controls.Add(this.CBDepth);
            this.GBImage.Controls.Add(this.BNext);
            this.GBImage.Name = "GBImage";
            this.GBImage.TabStop = false;
            // 
            // BReset
            // 
            resources.ApplyResources(this.BReset, "BReset");
            this.BReset.Name = "BReset";
            this.BReset.UseVisualStyleBackColor = true;
            this.BReset.Click += new System.EventHandler(this.BReset_Click);
            this.BReset.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CalibrationForm_KeyDown);
            // 
            // LImageHelp
            // 
            resources.ApplyResources(this.LImageHelp, "LImageHelp");
            this.LImageHelp.Name = "LImageHelp";
            // 
            // BExit
            // 
            resources.ApplyResources(this.BExit, "BExit");
            this.BExit.Name = "BExit";
            this.BExit.UseVisualStyleBackColor = true;
            this.BExit.Click += new System.EventHandler(this.BExit_Click);
            this.BExit.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CalibrationForm_KeyDown);
            // 
            // GBPath
            // 
            resources.ApplyResources(this.GBPath, "GBPath");
            this.GBPath.Controls.Add(this.LLPath);
            this.GBPath.Controls.Add(this.LPathHelp);
            this.GBPath.Controls.Add(this.BStart);
            this.GBPath.Name = "GBPath";
            this.GBPath.TabStop = false;
            // 
            // GBFinish
            // 
            resources.ApplyResources(this.GBFinish, "GBFinish");
            this.GBFinish.Controls.Add(this.LStatus);
            this.GBFinish.Controls.Add(this.BExit);
            this.GBFinish.Name = "GBFinish";
            this.GBFinish.TabStop = false;
            // 
            // LStatus
            // 
            resources.ApplyResources(this.LStatus, "LStatus");
            this.LStatus.Name = "LStatus";
            // 
            // CalibrationForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.GBFinish);
            this.Controls.Add(this.GBPath);
            this.Controls.Add(this.GBImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CalibrationForm";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CalibrationForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CalibrationForm_KeyDown);
            this.GBImage.ResumeLayout(false);
            this.GBImage.PerformLayout();
            this.GBPath.ResumeLayout(false);
            this.GBPath.PerformLayout();
            this.GBFinish.ResumeLayout(false);
            this.GBFinish.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.LinkLabel LLPath;
        private System.Windows.Forms.Button BStart;
        private System.Windows.Forms.Button BNext;
        private System.Windows.Forms.CheckBox CBColor;
        private System.Windows.Forms.CheckBox CBIR;
        private System.Windows.Forms.CheckBox CBDepth;
        private System.Windows.Forms.Label LPathHelp;
        private System.Windows.Forms.GroupBox GBImage;
        private System.Windows.Forms.Button BReset;
        private System.Windows.Forms.Label LImageHelp;
        private System.Windows.Forms.Button BExit;
        private System.Windows.Forms.GroupBox GBPath;
        private System.Windows.Forms.GroupBox GBFinish;
        private System.Windows.Forms.Label LStatus;
    }
}