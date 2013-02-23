namespace DistortionMapCreator
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
            this.CalculationProperyGrid = new System.Windows.Forms.PropertyGrid();
            this.BCalculate = new System.Windows.Forms.Button();
            this.SFD = new System.Windows.Forms.SaveFileDialog();
            this.BClipboardCopy = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CalculationProperyGrid
            // 
            this.CalculationProperyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CalculationProperyGrid.Location = new System.Drawing.Point(1, 2);
            this.CalculationProperyGrid.MinimumSize = new System.Drawing.Size(315, 400);
            this.CalculationProperyGrid.Name = "CalculationProperyGrid";
            this.CalculationProperyGrid.Size = new System.Drawing.Size(315, 400);
            this.CalculationProperyGrid.TabIndex = 3;
            // 
            // BCalculate
            // 
            this.BCalculate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BCalculate.Location = new System.Drawing.Point(241, 408);
            this.BCalculate.Name = "BCalculate";
            this.BCalculate.Size = new System.Drawing.Size(75, 23);
            this.BCalculate.TabIndex = 4;
            this.BCalculate.Text = "Calculate";
            this.BCalculate.UseVisualStyleBackColor = true;
            this.BCalculate.Click += new System.EventHandler(this.BCalculate_Click);
            // 
            // SFD
            // 
            this.SFD.DefaultExt = "dismap";
            this.SFD.Filter = "Distortion maps|*.dismap";
            this.SFD.Title = "Save distortion map";
            // 
            // BClipboardCopy
            // 
            this.BClipboardCopy.Location = new System.Drawing.Point(96, 408);
            this.BClipboardCopy.Name = "BClipboardCopy";
            this.BClipboardCopy.Size = new System.Drawing.Size(139, 23);
            this.BClipboardCopy.TabIndex = 5;
            this.BClipboardCopy.Text = "Copy settings to clipboard";
            this.BClipboardCopy.UseVisualStyleBackColor = true;
            this.BClipboardCopy.Click += new System.EventHandler(this.BClipboardCopy_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(319, 434);
            this.Controls.Add(this.BClipboardCopy);
            this.Controls.Add(this.BCalculate);
            this.Controls.Add(this.CalculationProperyGrid);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(100, 472);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "Distortion map generator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid CalculationProperyGrid;
        private System.Windows.Forms.Button BCalculate;
        private System.Windows.Forms.SaveFileDialog SFD;
        private System.Windows.Forms.Button BClipboardCopy;
    }
}

