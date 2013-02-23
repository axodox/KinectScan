namespace ExcaliburSecurity
{
    partial class ActivationForm
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
            this.LUsername = new System.Windows.Forms.Label();
            this.TBUsername = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.LActivation = new System.Windows.Forms.Label();
            this.LLicenseNumber = new System.Windows.Forms.Label();
            this.TBLicenseNumber = new System.Windows.Forms.TextBox();
            this.LStatusLabel = new System.Windows.Forms.Label();
            this.LStatus = new System.Windows.Forms.Label();
            this.BActivate = new System.Windows.Forms.Button();
            this.BExit = new System.Windows.Forms.Button();
            this.BTrial = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // LUsername
            // 
            this.LUsername.AutoSize = true;
            this.LUsername.Location = new System.Drawing.Point(146, 95);
            this.LUsername.Name = "LUsername";
            this.LUsername.Size = new System.Drawing.Size(58, 13);
            this.LUsername.TabIndex = 0;
            this.LUsername.Text = "Username:";
            // 
            // TBUsername
            // 
            this.TBUsername.Location = new System.Drawing.Point(237, 92);
            this.TBUsername.Name = "TBUsername";
            this.TBUsername.Size = new System.Drawing.Size(184, 20);
            this.TBUsername.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ExcaliburSecurity.Properties.Resources.windows_7_security_icon;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(128, 128);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // LActivation
            // 
            this.LActivation.AutoSize = true;
            this.LActivation.Location = new System.Drawing.Point(146, 12);
            this.LActivation.Name = "LActivation";
            this.LActivation.Size = new System.Drawing.Size(60, 13);
            this.LActivation.TabIndex = 3;
            this.LActivation.Text = "Description";
            // 
            // LLicenseNumber
            // 
            this.LLicenseNumber.AutoSize = true;
            this.LLicenseNumber.Location = new System.Drawing.Point(146, 126);
            this.LLicenseNumber.Name = "LLicenseNumber";
            this.LLicenseNumber.Size = new System.Drawing.Size(85, 13);
            this.LLicenseNumber.TabIndex = 4;
            this.LLicenseNumber.Text = "License number:";
            // 
            // TBLicenseNumber
            // 
            this.TBLicenseNumber.Location = new System.Drawing.Point(237, 123);
            this.TBLicenseNumber.Name = "TBLicenseNumber";
            this.TBLicenseNumber.Size = new System.Drawing.Size(184, 20);
            this.TBLicenseNumber.TabIndex = 5;
            // 
            // LStatusLabel
            // 
            this.LStatusLabel.AutoSize = true;
            this.LStatusLabel.Location = new System.Drawing.Point(146, 153);
            this.LStatusLabel.Name = "LStatusLabel";
            this.LStatusLabel.Size = new System.Drawing.Size(40, 13);
            this.LStatusLabel.TabIndex = 6;
            this.LStatusLabel.Text = "Status:";
            // 
            // LStatus
            // 
            this.LStatus.AutoSize = true;
            this.LStatus.Location = new System.Drawing.Point(234, 153);
            this.LStatus.Name = "LStatus";
            this.LStatus.Size = new System.Drawing.Size(10, 13);
            this.LStatus.TabIndex = 6;
            this.LStatus.Text = "-";
            // 
            // BActivate
            // 
            this.BActivate.Location = new System.Drawing.Point(346, 222);
            this.BActivate.Name = "BActivate";
            this.BActivate.Size = new System.Drawing.Size(75, 23);
            this.BActivate.TabIndex = 7;
            this.BActivate.Text = "Activate";
            this.BActivate.UseVisualStyleBackColor = true;
            // 
            // BExit
            // 
            this.BExit.Location = new System.Drawing.Point(156, 222);
            this.BExit.Name = "BExit";
            this.BExit.Size = new System.Drawing.Size(75, 23);
            this.BExit.TabIndex = 7;
            this.BExit.Text = "Exit";
            this.BExit.UseVisualStyleBackColor = true;
            // 
            // BTrial
            // 
            this.BTrial.Location = new System.Drawing.Point(237, 222);
            this.BTrial.Name = "BTrial";
            this.BTrial.Size = new System.Drawing.Size(103, 23);
            this.BTrial.TabIndex = 8;
            this.BTrial.Text = "Contiune trial";
            this.BTrial.UseVisualStyleBackColor = true;
            // 
            // ActivationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(433, 256);
            this.Controls.Add(this.BTrial);
            this.Controls.Add(this.BExit);
            this.Controls.Add(this.BActivate);
            this.Controls.Add(this.LStatus);
            this.Controls.Add(this.LStatusLabel);
            this.Controls.Add(this.TBLicenseNumber);
            this.Controls.Add(this.LLicenseNumber);
            this.Controls.Add(this.LActivation);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.TBUsername);
            this.Controls.Add(this.LUsername);
            this.Name = "ActivationForm";
            this.Text = "Software activation";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LUsername;
        private System.Windows.Forms.TextBox TBUsername;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label LActivation;
        private System.Windows.Forms.Label LLicenseNumber;
        private System.Windows.Forms.TextBox TBLicenseNumber;
        private System.Windows.Forms.Label LStatusLabel;
        private System.Windows.Forms.Label LStatus;
        private System.Windows.Forms.Button BActivate;
        private System.Windows.Forms.Button BExit;
        private System.Windows.Forms.Button BTrial;
    }
}