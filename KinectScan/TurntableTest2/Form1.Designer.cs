namespace TurntableTest2
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.Up = new System.Windows.Forms.Button();
            this.Stop = new System.Windows.Forms.Button();
            this.Down = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.ToAngle = new System.Windows.Forms.Button();
            this.BTurnOff = new System.Windows.Forms.Button();
            this.BZero = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(14, 109);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(99, 27);
            this.button1.TabIndex = 0;
            this.button1.Text = "Step";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Up
            // 
            this.Up.Location = new System.Drawing.Point(16, 39);
            this.Up.Name = "Up";
            this.Up.Size = new System.Drawing.Size(97, 29);
            this.Up.TabIndex = 1;
            this.Up.Text = "Up";
            this.Up.UseVisualStyleBackColor = true;
            this.Up.Click += new System.EventHandler(this.Up_Click);
            // 
            // Stop
            // 
            this.Stop.Location = new System.Drawing.Point(152, 35);
            this.Stop.Name = "Stop";
            this.Stop.Size = new System.Drawing.Size(108, 32);
            this.Stop.TabIndex = 2;
            this.Stop.Text = "Stop";
            this.Stop.UseVisualStyleBackColor = true;
            this.Stop.Click += new System.EventHandler(this.Stop_Click);
            // 
            // Down
            // 
            this.Down.Location = new System.Drawing.Point(16, 74);
            this.Down.Name = "Down";
            this.Down.Size = new System.Drawing.Size(97, 29);
            this.Down.TabIndex = 3;
            this.Down.Text = "Down";
            this.Down.UseVisualStyleBackColor = true;
            this.Down.Click += new System.EventHandler(this.Down_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(152, 128);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 4;
            // 
            // ToAngle
            // 
            this.ToAngle.Location = new System.Drawing.Point(14, 142);
            this.ToAngle.Name = "ToAngle";
            this.ToAngle.Size = new System.Drawing.Size(99, 27);
            this.ToAngle.TabIndex = 5;
            this.ToAngle.Text = "ToAngle";
            this.ToAngle.UseVisualStyleBackColor = true;
            this.ToAngle.Click += new System.EventHandler(this.ToAngle_Click);
            // 
            // BTurnOff
            // 
            this.BTurnOff.Location = new System.Drawing.Point(16, 175);
            this.BTurnOff.Name = "BTurnOff";
            this.BTurnOff.Size = new System.Drawing.Size(97, 23);
            this.BTurnOff.TabIndex = 6;
            this.BTurnOff.Text = "Turn off";
            this.BTurnOff.UseVisualStyleBackColor = true;
            this.BTurnOff.Click += new System.EventHandler(this.BTurnOff_Click);
            // 
            // BZero
            // 
            this.BZero.Location = new System.Drawing.Point(16, 204);
            this.BZero.Name = "BZero";
            this.BZero.Size = new System.Drawing.Size(97, 23);
            this.BZero.TabIndex = 7;
            this.BZero.Text = "Zero";
            this.BZero.UseVisualStyleBackColor = true;
            this.BZero.Click += new System.EventHandler(this.BZero_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.BZero);
            this.Controls.Add(this.BTurnOff);
            this.Controls.Add(this.ToAngle);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.Down);
            this.Controls.Add(this.Stop);
            this.Controls.Add(this.Up);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button Up;
        private System.Windows.Forms.Button Stop;
        private System.Windows.Forms.Button Down;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button ToAngle;
        private System.Windows.Forms.Button BTurnOff;
        private System.Windows.Forms.Button BZero;
    }
}

