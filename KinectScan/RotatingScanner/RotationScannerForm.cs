using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KinectScan
{
    public partial class RotationScannerForm : Form
    {
        KinectScanContext Context;
        TurntableSettingsForm SF;
        public RotationScannerForm(KinectScanContext context)
        {
            InitializeComponent();
            Context = context;
            SF = new TurntableSettingsForm();
        }

        private void MICalibration_Click(object sender, EventArgs e)
        {
            while (KinectScanner.DeviceCount == 0)
            {
                if (MessageBox.Show(LocalizedResources.DeviceNotFound, LocalizedResources.CalibrationTitle, MessageBoxButtons.RetryCancel, MessageBoxIcon.Information) == DialogResult.Cancel) return;
            }

            Hide();
            Context.SetUIMode(false);
            TurntableCalibrationForm CF = new TurntableCalibrationForm(Context);
            CF.Show();
            CF.FormClosing += CF_FormClosing;
        }

        void CF_FormClosing(object sender, FormClosingEventArgs e)
        {
            Context.SetUIMode(true);
            Show();
        }

        private void MISettings_Click(object sender, EventArgs e)
        {
            SF.Show();
        }
    }
}
