using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Turntables;

namespace KinectScan
{
    public partial class ScanningForm : Form
    {
        public enum ScanningStates {None, MoveToOrigin, Scanning, Done }
        public ScanningStates ScanningState { get; private set; }
        KinectScanModule Context;
        Timer UITimer;
        public ScanningForm(KinectScanModule module)
        {
            InitializeComponent();
            ScanningState = ScanningStates.Done;
            Context = module;
            Context.Table.MotorStopped += Table_MotorStopped;
            Context.Table.TurnComplete += Table_TurnComplete;
            UITimer = new Timer();
            UITimer.Interval = 100;
            UITimer.Tick += T_Tick;
            SetState(ScanningStates.None);
        }

        void Table_TurnComplete(object sender, EventArgs e)
        {
            SetState(ScanningStates.Done);
        }

        void Table_MotorStopped(object sender, EventArgs e)
        {
            switch (ScanningState)
            {
                case ScanningStates.MoveToOrigin:
                    SetState(ScanningStates.Scanning);
                    break;
            }
        }

        void T_Tick(object sender, EventArgs e)
        {
            if (ScanningState == ScanningStates.Scanning)
            {
                int degrees = (int)Context.Table.PositionInDegrees;
                PBScanning.Value = degrees;
                LProgress.Text = string.Format(LocalizedResources.ScannerScanning, degrees);
            }
        }

        private void BScan_Click(object sender, EventArgs e)
        {
            SetState(ScanningStates.Scanning);
        }

        private void SetState(ScanningStates state)
        {
            ScanningState = state;
            switch (ScanningState)
            {
                case ScanningStates.None:
                    LProgress.Text = LocalizedResources.ScannerReady;
                    PBScanning.Enabled = false;
                    PBScanning.Value = 0;
                    PBScanning.Style = ProgressBarStyle.Blocks;
                    BScan.Enabled = true;
                    BStop.Enabled = false;
                    break;
                case ScanningStates.MoveToOrigin:
                    BScan.Enabled = false;
                    BStop.Enabled = false;
                    Context.Table.SendCommandAsync(Turntable.Commands.ToOrigin);
                    LProgress.Text = LocalizedResources.ScannerMovingToOrigin;
                    PBScanning.Enabled = true;
                    PBScanning.Style = ProgressBarStyle.Marquee;
                    break;
                case ScanningStates.Scanning:
                    BScan.Enabled = false;
                    BStop.Enabled = true;
                    PBScanning.Style = ProgressBarStyle.Continuous;
                    Context.PF.Clear();
                    Context.Table.TurnOnce();                    
                    UITimer.Start();
                    break;
                case ScanningStates.Done:
                    UITimer.Stop();
                    PBScanning.Value = 360;
                    PBScanning.Enabled = false;                    
                    LProgress.Text = LocalizedResources.ScannerDone;
                    BScan.Enabled = true;
                    BStop.Enabled = false;
                    break;
            }
        }

        private void BStop_Click(object sender, EventArgs e)
        {
            SetState(ScanningStates.None);
            Context.Table.SendCommandAsync(Turntable.Commands.Stop);
        }

        private void ScanningForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
