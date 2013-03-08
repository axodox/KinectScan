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
            FLPSettings_Resize(this, null);
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
            for (int i = 0; i < FLPSettings.Controls.Count; i++)
            {
                FLPSettings.Controls[i].Width = FLPSettings.ClientSize.Width - scrollsize;
            }
            FLPSettings.ResumeLayout();
            start = false;
        }

        public void LoadSettings()
        {
            NBMinY.Value = Settings.Default.MinY;
            NBMaxY.Value = Settings.Default.MaxY;
            NBProjectionWidth.Value = Settings.Default.MaxWidth;
            NBReprojectionRotationX.Value = Settings.Default.RotationX;
            NBReprojectionRotationZ.Value = Settings.Default.RotationZ;
            NBReprojectionTranslationX.Value = Settings.Default.TranslationX;
            NBReprojectionTranslationY.Value = Settings.Default.TranslationY;
            NBReprojectionTranslationZ.Value = Settings.Default.TranslationZ;
            NBZLimit.Value = Settings.Default.DepthLimit;
            NBSplitPlane.Value = Settings.Default.SplitPlaneRotation;
            NBMaxClipRadius.Value = Settings.Default.MaxClipRadius;
            NBDepthAveragingLimit.Value = Settings.Default.DepthAvgLimit;
            NBMinAvgCount.Int32Value = Settings.Default.MinAvgCount;
            NBMinClipRadius.Value = Settings.Default.MinClipRadius;
            CBClip.Checked = Settings.Default.Clip;
            NBAveragedFrames.Int32Value = Settings.Default.AveragedFrames;
            NBFusionSpacing.Int32Value = Settings.Default.FusionSpacing;
            NBCoreX.Value = Settings.Default.CoreX;
            NBCoreY.Value = Settings.Default.CoreY;
            if (SettingsLoaded != null) SettingsLoaded(this, null);            
        }

        public void SaveSettings()
        {
            Settings.Default.MinY = NBMinY.Value;
            Settings.Default.MaxY = NBMaxY.Value;
            Settings.Default.MaxWidth = NBProjectionWidth.Value;
            Settings.Default.RotationX = NBReprojectionRotationX.Value;
            Settings.Default.RotationZ = NBReprojectionRotationZ.Value;
            Settings.Default.TranslationX = NBReprojectionTranslationX.Value;
            Settings.Default.TranslationY = NBReprojectionTranslationY.Value;
            Settings.Default.TranslationZ = NBReprojectionTranslationZ.Value;
            Settings.Default.DepthLimit = NBZLimit.Value;
            Settings.Default.SplitPlaneRotation = NBSplitPlane.Value;
            Settings.Default.MaxClipRadius = NBMaxClipRadius.Value;
            Settings.Default.DepthAvgLimit = NBDepthAveragingLimit.Value;
            Settings.Default.MinAvgCount = NBMinAvgCount.Int32Value;
            Settings.Default.MinClipRadius = NBMinClipRadius.Value;
            Settings.Default.Clip = CBClip.Checked;
            Settings.Default.AveragedFrames = NBAveragedFrames.Int32Value;
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
