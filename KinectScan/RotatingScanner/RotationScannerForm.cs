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
    public partial class RotationScannerForm : Form
    {
        #region Init
        KinectScanContext Context;
        TurntableSettingsForm SF;
        Modeller Modeller;
        MenuItem MISelectedView;
        Timer TScan;

        public RotationScannerForm(KinectScanContext context)
        {
            InitializeComponent();
            Context = context;

            //Modeller
            Modeller = new ExCoreModeller(Context);
            Modeller.CreateDevice(XPanel);
            Modeller.DebugLoad();

            //GUI
            Array ViewModes = Enum.GetValues(typeof(Modeller.Views));
            string[] specialViewModes = Modeller.SpecialViewModes;

            MenuItem MI;
            foreach (Modeller.Views mode in ViewModes)
            {
                if (mode == KinectScan.Modeller.Views.Special && specialViewModes.Length > 0) continue;
                MI = new MenuItem(mode.ToString());
                MI.RadioCheck = true;
                if (Modeller.ViewMode == mode)
                {
                    MI.Checked = true;
                    MISelectedView = MI;
                }
                MI.Click += (object sender, EventArgs e) =>
                {
                    if (MISelectedView != null) MISelectedView.Checked = false;
                    MISelectedView = (MenuItem)sender;
                    MISelectedView.Checked = true;                    
                    Modeller.ViewMode = (Modeller.Views)MISelectedView.Tag;
                };
                MI.Tag = mode;
                MIView.MenuItems.Add(MI);
            }

            if (specialViewModes.Length > 0)
            {
                int i = 0;
                MIView.MenuItems.Add(new MenuItem("-"));
                foreach (string mode in specialViewModes)
                {
                    MI = new MenuItem(mode.ToString());
                    MI.RadioCheck = true;
                    MI.Checked = (Modeller.ViewMode == Modeller.Views.Special && Modeller.SpecialViewMode == i);
                    MI.Click += (object sender, EventArgs e) =>
                    {
                        if (MISelectedView != null) MISelectedView.Checked = false;
                        MISelectedView = (MenuItem)sender;
                        MISelectedView.Checked = true;
                        Modeller.ViewMode = KinectScan.Modeller.Views.Special;
                        Modeller.SpecialViewMode=(int)MISelectedView.Tag;
                    };
                    MI.Tag = i++;
                    MIView.MenuItems.Add(MI);
                }
            }

            MIViewLegs.MenuItems[Modeller.VisualizedLeg].Checked = true;

            XPanel.KeyDown += (object o, KeyEventArgs e) => {
                switch (e.KeyCode)
                {
                    case Keys.Space:
                        MIScan_Click(this, null);
                        break;
                    case Keys.Tab:
                        TScan.Start();
                        break;
                }
            };

            //Settings
            SF = new TurntableSettingsForm();
            SF.LoadSettings();
            SF.SetModeller(Modeller);

            //Scanning
            ScanUITimer = new Timer();
            ScanUITimer.Interval = 100;
            ScanUITimer.Tick += (object sender, EventArgs e) =>
                {
                    if (ScanningState == ScanningStates.Scanning)
                    {
                        int degrees = (int)Turntable.PositionInDegrees;
                        TSPB.Value = degrees;
                        TSSL.Text = string.Format(LocalizedResources.ScannerScanning, degrees);
                    }
                };
            SetState(ScanningStates.None);
            Context.ScannerCreated += Context_ScannerCreated;
            if (Context.Scanner != null) Context_ScannerCreated(this, null);
            Turntable.DeviceConnected += Turntable_DeviceConnected;
            Turntable.DeviceDisconnected += Turntable_DeviceDisconnected;
            TScan = new Timer();
            TScan.Interval = 20000;
            TScan.Tick += (object sender, EventArgs e) => { MIScan_Click(this, null); TScan.Stop(); };
        }

        void Context_ScannerCreated(object s, EventArgs e2)
        {
            Context.Scanner.RawFrameIn += (object sender, Scanner.RawFrameEventArgs e) =>
            {
                if (e.FrameType == Scanner.FrameTypes.Depth && ScanningState == ScanningStates.Scanning)
                {
                    Modeller.LoadDepth(e.Data, -(float)Turntable.PositionInRadians);
                }
            };
        }

        void Turntable_DeviceDisconnected(object sender, EventArgs e)
        {
            MIScan.Enabled = Turntable != null; 
        }

        void Turntable_DeviceConnected(object sender, EventArgs e)
        {
            if (Turntable == null) InitScanning(Turntable.DefaultDevice);
            MIScan.Enabled = Turntable != null; 
        }
        #endregion

        #region Scanning
        public enum ScanningStates {None, MoveToOrigin, Scanning, Done }
        public ScanningStates ScanningState { get; private set; }
        Timer ScanUITimer;
        private Turntable Turntable;
        public void InitScanning(Turntable turntable)
        {
            if (turntable == null) return;
            Turntable = turntable;
            Turntable.SendCommandAsync(Turntable.Commands.ToOrigin);
            Turntable.MotorStopped += (object sender, EventArgs e) => { SetState(ScanningStates.Done); };
            Turntable.TurnComplete += (object sender, EventArgs e) =>
            {
                switch (ScanningState)
                {
                    case ScanningStates.MoveToOrigin:
                        SetState(ScanningStates.Scanning);
                        break;
                    case ScanningStates.Scanning:
                        Modeller.DebugSave();
                        break;
                }
                
            };
        }

        private void MIScan_Click(object sender, EventArgs e)
        {
            //if (Context.Scanner == null)
                Context.InitiateKinectDevice(Scanner.Modes.Depth480);
            if (Context.Scanner == null)
                MessageBox.Show(LocalizedResources.DeviceNotFound, LocalizedResources.TurntableTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                SetState(ScanningStates.Scanning);
        }

        private void SetState(ScanningStates state)
        {
            ScanningState = state;
            switch (ScanningState)
            {
                case ScanningStates.None:
                    TSSL.Text = LocalizedResources.ScannerReady;
                    TSPB.Enabled = false;
                    TSPB.Value = 0;
                    TSPB.Style = ProgressBarStyle.Blocks;
                    MIScan.Enabled = Turntable != null; 
                    MIStop.Enabled = false;
                    break;
                case ScanningStates.MoveToOrigin:
                    MIScan.Enabled = false;
                    MIStop.Enabled = false;
                    Turntable.SendCommandAsync(Turntable.Commands.ToOrigin);
                    TSSL.Text = LocalizedResources.ScannerMovingToOrigin;
                    TSPB.Enabled = true;
                    TSPB.Style = ProgressBarStyle.Marquee;
                    break;
                case ScanningStates.Scanning:
                    MIScan.Enabled = false;
                    MIStop.Enabled = true;
                    TSPB.Style = ProgressBarStyle.Continuous;
                    Modeller.Clear();
                    Modeller.Start();
                    Turntable.TurnOnce();                    
                    ScanUITimer.Start();
                    break;
                case ScanningStates.Done:                    
                    ScanUITimer.Stop();
                    Modeller.Stop();
                    TSPB.Value = 360;
                    TSPB.Enabled = false;
                    TSSL.Text = LocalizedResources.ScannerDone;
                    MIScan.Enabled = Turntable != null; 
                    MIStop.Enabled = false;
                    break;
            }
        }

        private void MIStop_Click(object sender, EventArgs e)
        {
            SetState(ScanningStates.None);
            Turntable.SendCommandAsync(Turntable.Commands.Stop);
        }

        private void ScanningForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
        #endregion

        #region Calibration
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
            TurntableCalibrationForm TCF = sender as TurntableCalibrationForm;
            SF.Calibrate(TCF.Calibrator);
            Context.SetUIMode(true);
            Show();
        }
        #endregion

        #region Settings
        private void MISettings_Click(object sender, EventArgs e)
        {
            SF.Show();
        }
        #endregion

        #region Unload
        private void RotationScannerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            SetState(ScanningStates.None);
            Turntable.DeviceConnected -= Turntable_DeviceConnected;
            Turntable.DeviceDisconnected -= Turntable_DeviceDisconnected;
            if (Modeller != null)
            {
                Modeller.Dispose();
                Modeller = null;
            }
            if (Turntable != null)
            {
                Turntable.Dispose();
                Turntable = null;
            }
            SF.SaveSettings();
            SF.Dispose();
            SF = null;
        }
        #endregion

        private void MISaveAs_Click(object sender, EventArgs e)
        {
            if (SaveDialog.ShowDialog() == DialogResult.OK)
            {
                Modeller.Save(SaveDialog.FileName);
            }
        }

        private void MIViewLeg_Click(object sender, EventArgs e)
        {
            MenuItem MI = sender as MenuItem;
            MIViewLegs.MenuItems[Modeller.VisualizedLeg].Checked = false;
            Modeller.VisualizedLeg = (int)MI.Tag;
            MIViewLegs.MenuItems[Modeller.VisualizedLeg].Checked = true;
        }

        private void RotationScannerForm_KeyDown(object sender, KeyEventArgs e)
        {
            XPanel.Focus();
        }
    }
}
