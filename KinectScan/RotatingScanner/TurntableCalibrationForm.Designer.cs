namespace KinectScan
{
    partial class TurntableCalibrationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TurntableCalibrationForm));
            this.TLP = new System.Windows.Forms.TableLayoutPanel();
            this.PImage = new System.Windows.Forms.Panel();
            this.PHelp = new System.Windows.Forms.Panel();
            this.PBCalibration = new System.Windows.Forms.ProgressBar();
            this.LStepText = new System.Windows.Forms.Label();
            this.BPrevious = new System.Windows.Forms.Button();
            this.BNext = new System.Windows.Forms.Button();
            this.LStepNumber = new System.Windows.Forms.Label();
            this.TLP.SuspendLayout();
            this.PHelp.SuspendLayout();
            this.SuspendLayout();
            // 
            // TLP
            // 
            resources.ApplyResources(this.TLP, "TLP");
            this.TLP.Controls.Add(this.PImage, 0, 0);
            this.TLP.Controls.Add(this.PHelp, 0, 1);
            this.TLP.Name = "TLP";
            // 
            // PImage
            // 
            this.PImage.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.PImage, "PImage");
            this.PImage.Name = "PImage";
            this.PImage.Paint += new System.Windows.Forms.PaintEventHandler(this.CalibrationForm_Paint);
            this.PImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CalibrationForm_MouseDown);
            this.PImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CalibrationForm_MouseMove);
            this.PImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CalibrationForm_MouseUp);
            // 
            // PHelp
            // 
            this.PHelp.Controls.Add(this.PBCalibration);
            this.PHelp.Controls.Add(this.LStepText);
            this.PHelp.Controls.Add(this.BPrevious);
            this.PHelp.Controls.Add(this.BNext);
            this.PHelp.Controls.Add(this.LStepNumber);
            resources.ApplyResources(this.PHelp, "PHelp");
            this.PHelp.Name = "PHelp";
            // 
            // PBCalibration
            // 
            resources.ApplyResources(this.PBCalibration, "PBCalibration");
            this.PBCalibration.Name = "PBCalibration";
            // 
            // LStepText
            // 
            resources.ApplyResources(this.LStepText, "LStepText");
            this.LStepText.Name = "LStepText";
            // 
            // BPrevious
            // 
            resources.ApplyResources(this.BPrevious, "BPrevious");
            this.BPrevious.Name = "BPrevious";
            this.BPrevious.UseVisualStyleBackColor = true;
            this.BPrevious.Click += new System.EventHandler(this.BPrevious_Click);
            // 
            // BNext
            // 
            resources.ApplyResources(this.BNext, "BNext");
            this.BNext.Name = "BNext";
            this.BNext.UseVisualStyleBackColor = true;
            this.BNext.Click += new System.EventHandler(this.BNext_Click);
            // 
            // LStepNumber
            // 
            resources.ApplyResources(this.LStepNumber, "LStepNumber");
            this.LStepNumber.Name = "LStepNumber";
            // 
            // TurntableCalibrationForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TLP);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TurntableCalibrationForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CalibrationForm_FormClosing);
            this.TLP.ResumeLayout(false);
            this.PHelp.ResumeLayout(false);
            this.PHelp.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TLP;
        private System.Windows.Forms.Panel PImage;
        private System.Windows.Forms.Panel PHelp;
        private System.Windows.Forms.Label LStepText;
        private System.Windows.Forms.Button BPrevious;
        private System.Windows.Forms.Button BNext;
        private System.Windows.Forms.Label LStepNumber;
        private System.Windows.Forms.ProgressBar PBCalibration;

    }
}