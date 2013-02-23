namespace CustomControls
{
    partial class NumericBar
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NumericBar));
            this.LTitle = new System.Windows.Forms.Label();
            this.TB = new NumericBarTrackBar();
            this.NUD = new NumericBarNumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.TB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD)).BeginInit();
            this.SuspendLayout();
            // 
            // LTitle
            // 
            resources.ApplyResources(this.LTitle, "LTitle");
            this.LTitle.AutoEllipsis = true;
            this.LTitle.Name = "LTitle";
            // 
            // TB
            // 
            resources.ApplyResources(this.TB, "TB");
            this.TB.Maximum = 100;
            this.TB.Name = "TB";
            this.TB.TickFrequency = 10;
            this.TB.ValueChanged += new System.EventHandler(this.TB_ValueChanged);
            // 
            // NUD
            // 
            resources.ApplyResources(this.NUD, "NUD");
            this.NUD.DecimalPlaces = 2;
            this.NUD.Name = "NUD";
            this.NUD.ValueChanged += new System.EventHandler(this.NUD_ValueChanged);
            // 
            // NumericBar
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.NUD);
            this.Controls.Add(this.TB);
            this.Controls.Add(this.LTitle);
            this.MaximumSize = new System.Drawing.Size(4096, 56);
            this.MinimumSize = new System.Drawing.Size(128, 56);
            this.Name = "NumericBar";
            ((System.ComponentModel.ISupportInitialize)(this.TB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label LTitle;
        private NumericBarTrackBar TB;
        private NumericBarNumericUpDown NUD;
    }
}
