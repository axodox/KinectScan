using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KinectScan
{
    public partial class SettingsForm : Form
    {
        bool start = true;
        public SettingsForm()
        {
            InitializeComponent();
            FLPSettings_Resize(this, null);
            LLShowAdvancedSettings.Text = LocalizedStrings.ShowAdvancedSettings;
            LLShowTestSettings.Text = LocalizedStrings.ShowTestSettings;
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

        bool ShowAdvancedSettings = false;
        bool ShowTestSettings = false;
        private void LLShowTestSettings_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowTestSettings = !ShowTestSettings;
            foreach (Control C in FLPSettings.Controls)
            {
                if ((string)C.Tag == "Test") C.Visible = ShowTestSettings;
            }
            LLShowTestSettings.Text = ShowTestSettings ? LocalizedStrings.HideTestSettings : LocalizedStrings.ShowTestSettings;
        }

        private void LLShowAdvancedSettings_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowAdvancedSettings = !ShowAdvancedSettings;
            foreach (Control C in FLPSettings.Controls)
            {
                if ((string)C.Tag == "Advanced") C.Visible = ShowAdvancedSettings;
            }
            LLShowAdvancedSettings.Text = ShowAdvancedSettings ? LocalizedStrings.HideAdvancedSettings : LocalizedStrings.ShowAdvancedSettings;
        }
    }
}
