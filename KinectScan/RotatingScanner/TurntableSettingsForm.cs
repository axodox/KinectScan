using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KinectScan
{
    public partial class TurntableSettingsForm : Form
    {
        bool start = true;
        public TurntableSettingsForm()
        {
            InitializeComponent();
            CBClippingEnabled.CheckedChanged += ClippingChanged;
            FLPSettings_Resize(this, null);

            //Clipping
            CBClippingEnabled.CheckedChanged += (object o, EventArgs e) => { if (Modeller != null) Modeller.ClippingEnabled = CBClippingEnabled.Checked; };
            NBClippingRadiusMin.ValueChanged += (object o, EventArgs e) => { if (Modeller != null) Modeller.ClippingRadiusMin = NBClippingRadiusMin.Value; };
            NBClippingRadiusMax.ValueChanged += (object o, EventArgs e) => { if (Modeller != null) Modeller.ClippingRadiusMax = NBClippingRadiusMax.Value; };
            NBClippingDepthMax.ValueChanged += (object o, EventArgs e) => { if (Modeller != null) Modeller.ClippingDepthMax = NBClippingDepthMax.Value; };
            NBClippingYMin.ValueChanged += (object o, EventArgs e) => { if (Modeller != null) Modeller.ClippingYMin = NBClippingYMin.Value; };

            //Transformations
            NBRotationX.ValueChanged += (object o, EventArgs e) => { if (Modeller != null) Modeller.RotationX = NBRotationX.Value.ToRadians(); };
            NBSplitPlaneAngle.ValueChanged += (object o, EventArgs e) => { if (Modeller != null) Modeller.SplitPlaneAngle = NBSplitPlaneAngle.Value.ToRadians(); };
            NBRotationZ.ValueChanged += (object o, EventArgs e) => { if (Modeller != null) Modeller.RotationZ = NBRotationZ.Value.ToRadians(); };
            NBTranslationX.ValueChanged += (object o, EventArgs e) => { if (Modeller != null) Modeller.TranslationX = NBTranslationX.Value; };
            NBTranslationY.ValueChanged += (object o, EventArgs e) => { if (Modeller != null) Modeller.TranslationY = NBTranslationY.Value; };
            NBTranslationZ.ValueChanged += (object o, EventArgs e) => { if (Modeller != null) Modeller.TranslationZ = NBTranslationZ.Value; };

            //Depth averaging
            NBDepthAveragingCacheSize.ValueChanged += (object o, EventArgs e) => { if (Modeller != null) Modeller.DepthAveragingCacheSize = NBDepthAveragingCacheSize.Int32Value; };
            NBDepthAveragingLimit.ValueChanged += (object o, EventArgs e) => { if (Modeller != null) Modeller.DepthAveragingLimit = NBDepthAveragingLimit.Value; };
            NBDepthAveragingMinCount.ValueChanged += (object o, EventArgs e) => { if (Modeller != null) Modeller.DepthAveragingMinCount = NBDepthAveragingMinCount.Int32Value; };
            NBFusionSpacing.ValueChanged += (object o, EventArgs e) => { if (Modeller != null) Modeller.FusionSpacing = NBFusionSpacing.Int32Value; };

            //Projection
            NBProjectionYMax.ValueChanged += (object o, EventArgs e) => { if (Modeller != null) Modeller.ProjectionYMax = NBProjectionYMax.Value; };
            NBProjectionYMin.ValueChanged += (object o, EventArgs e) => { if (Modeller != null) Modeller.ProjectionYMin = NBProjectionYMin.Value; };

            //ExCore modeller
            NBCoreX.ValueChanged += (object o, EventArgs e) => 
            {
                if (Modeller != null && Modeller is ExCoreModeller) (Modeller as ExCoreModeller).CoreX = NBCoreX.Value;
            };
            NBCoreY.ValueChanged += (object o, EventArgs e) =>
            {
                if (Modeller != null && Modeller is ExCoreModeller) (Modeller as ExCoreModeller).CoreY = NBCoreY.Value;
            };

            //Fusion modeller
            NBProjectionWidth.ValueChanged += (object o, EventArgs e) =>
            {
                if (Modeller != null && Modeller is FusionModeller)
                    (Modeller as FusionModeller).ProjectionWidth = NBProjectionWidth.Value;
            };
        }

        private void ClippingChanged(object sender, EventArgs e)
        {
            NBClippingRadiusMin.Enabled = NBClippingRadiusMax.Enabled = NBClippingDepthMax.Enabled = NBClippingYMin.Enabled = CBClippingEnabled.Checked;
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void FLPSettings_Resize(object sender, EventArgs e)
        {
            int scrollsize = start ? 24 : 6;
            FLPSettings.SuspendLayout();
            foreach(Control C in FLPSettings.Controls)
            {
                if (C is Label)
                {
                    C.BackColor = SystemColors.Highlight;
                    C.ForeColor = SystemColors.HighlightText;
                    (C as Label).AutoSize = false;
                }
                C.Width = FLPSettings.ClientSize.Width - scrollsize;
            }
            FLPSettings.ResumeLayout();
            start = false;
        }

        public void Calibrate(TurntableCalibrator calibrator)
        {
            if (calibrator.IsCalibrated)
            {
                NBTranslationX.Value = calibrator.TranslationX;
                NBTranslationY.Value = calibrator.TranslationY;
                NBTranslationZ.Value = calibrator.TranslationZ;
                NBRotationX.Value = calibrator.RotationX;
                NBRotationZ.Value = calibrator.RotationZ;
            }
        }

        Modeller Modeller;
        public void SetModeller(Modeller modeller)
        {
            Modeller = modeller;

            //Clipping
            Modeller.ClippingEnabled = CBClippingEnabled.Checked;
            Modeller.ClippingRadiusMin = NBClippingRadiusMin.Value;
            Modeller.ClippingRadiusMax = NBClippingRadiusMax.Value;
            Modeller.ClippingDepthMax = NBClippingDepthMax.Value;
            Modeller.ClippingYMin = NBClippingYMin.Value;

            //Transformations
            Modeller.RotationX = NBRotationX.Value.ToRadians();
            Modeller.SplitPlaneAngle = NBSplitPlaneAngle.Value.ToRadians();
            Modeller.RotationZ = NBRotationZ.Value.ToRadians();
            Modeller.TranslationX = NBTranslationX.Value;
            Modeller.TranslationY = NBTranslationY.Value;
            Modeller.TranslationZ = NBTranslationZ.Value;

            //Depth averaging
            Modeller.DepthAveragingCacheSize = NBDepthAveragingCacheSize.Int32Value;
            Modeller.DepthAveragingLimit = NBDepthAveragingLimit.Value;
            Modeller.DepthAveragingMinCount = NBDepthAveragingMinCount.Int32Value;
            Modeller.FusionSpacing = NBFusionSpacing.Int32Value;

            //Projection
            Modeller.ProjectionYMax = NBProjectionYMax.Value;
            Modeller.ProjectionYMin = NBProjectionYMin.Value;

            if (Modeller is ExCoreModeller)
            {
                ExCoreModeller ExCoreModeller = Modeller as ExCoreModeller;
                ExCoreModeller.CoreX = NBCoreX.Value;
                ExCoreModeller.CoreY = NBCoreY.Value;
                NBCoreX.Visible = NBCoreY.Visible = true;
            }
            else
            {
                NBCoreX.Visible = NBCoreY.Visible = false;
            }

            if (Modeller is FusionModeller)
            {
                FusionModeller FusionModeller = Modeller as FusionModeller;                
                FusionModeller.ProjectionWidth = NBProjectionWidth.Value;
                NBProjectionWidth.Visible= true;
            }
            else
            {
                NBProjectionWidth.Visible = false;
            }
        }

        public void LoadSettings()
        {
            NBProjectionYMin.Value = Settings.Default.MinY;
            NBProjectionYMax.Value = Settings.Default.MaxY;
            NBProjectionWidth.Value = Settings.Default.MaxWidth;
            NBRotationX.Value = Settings.Default.RotationX;
            NBRotationZ.Value = Settings.Default.RotationZ;
            NBTranslationX.Value = Settings.Default.TranslationX;
            NBTranslationY.Value = Settings.Default.TranslationY;
            NBTranslationZ.Value = Settings.Default.TranslationZ;
            NBClippingDepthMax.Value = Settings.Default.DepthLimit;
            NBSplitPlaneAngle.Value = Settings.Default.SplitPlaneRotation;
            NBClippingRadiusMax.Value = Settings.Default.MaxClipRadius;
            NBDepthAveragingLimit.Value = Settings.Default.DepthAvgLimit;
            NBDepthAveragingMinCount.Int32Value = Settings.Default.MinAvgCount;
            NBClippingRadiusMin.Value = Settings.Default.MinClipRadius;
            CBClippingEnabled.Checked = Settings.Default.Clip;
            NBDepthAveragingCacheSize.Int32Value = Settings.Default.AveragedFrames;
            NBFusionSpacing.Int32Value = Settings.Default.FusionSpacing;
            NBCoreX.Value = Settings.Default.CoreX;
            NBCoreY.Value = Settings.Default.CoreY;
            ClippingChanged(this, null);
            if (SettingsLoaded != null) SettingsLoaded(this, null);            
        }

        public void SaveSettings()
        {
            Settings.Default.MinY = NBProjectionYMin.Value;
            Settings.Default.MaxY = NBProjectionYMax.Value;
            Settings.Default.MaxWidth = NBProjectionWidth.Value;
            Settings.Default.RotationX = NBRotationX.Value;
            Settings.Default.RotationZ = NBRotationZ.Value;
            Settings.Default.TranslationX = NBTranslationX.Value;
            Settings.Default.TranslationY = NBTranslationY.Value;
            Settings.Default.TranslationZ = NBTranslationZ.Value;
            Settings.Default.DepthLimit = NBClippingDepthMax.Value;
            Settings.Default.SplitPlaneRotation = NBSplitPlaneAngle.Value;
            Settings.Default.MaxClipRadius = NBClippingRadiusMax.Value;
            Settings.Default.DepthAvgLimit = NBDepthAveragingLimit.Value;
            Settings.Default.MinAvgCount = NBDepthAveragingMinCount.Int32Value;
            Settings.Default.MinClipRadius = NBClippingRadiusMin.Value;
            Settings.Default.Clip = CBClippingEnabled.Checked;
            Settings.Default.AveragedFrames = NBDepthAveragingCacheSize.Int32Value;
            Settings.Default.FusionSpacing = NBFusionSpacing.Int32Value;
            Settings.Default.CoreX = NBCoreX.Value;
            Settings.Default.CoreY = NBCoreY.Value;
            Settings.Default.Save();            
        }

        public event EventHandler SettingsLoaded;

        private void TSMIImport_Click(object sender, EventArgs e)
        {
            if (OFDImport.ShowDialog() == DialogResult.OK)
            {
                if (Settings.Default.PropertyValues.Import(OFDImport.FileName, "RotatingScanner"))
                {
                    LoadSettings();
                    MessageBox.Show(LocalizedResources.ImportSuccesful, LocalizedResources.SettingsTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    LoadSettings();
                    MessageBox.Show(LocalizedResources.ImportUnsuccesful, LocalizedResources.SettingsTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (SettingsLoaded != null) SettingsLoaded(this, null);
            }
        }

        private void TSMIExport_Click(object sender, EventArgs e)
        {
            if (SFDExport.ShowDialog() == DialogResult.OK)
            {
                if (Settings.Default.PropertyValues.Export(SFDExport.FileName, "RotatingScanner"))
                {
                    MessageBox.Show(LocalizedResources.ExportSuccesful, LocalizedResources.SettingsTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(LocalizedResources.ExportUnsuccesful, LocalizedResources.SettingsTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
