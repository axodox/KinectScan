using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace KinectScan
{
    public partial class CalibrationForm : Form
    {
        MainForm Master;
        public CalibrationForm(MainForm mainForm)
        {
            InitializeComponent();
            Master = mainForm;
            StepCheckBoxes = new CheckBox[] { CBDepth, CBColor, CBIR };
            Master.CalibrationSaveCompleted += new EventHandler(Master_CalibrationSaveCompleted);
        }

        void Master_CalibrationSaveCompleted(object sender, EventArgs e)
        {
            StepCheckBoxes[Step].CheckState = CheckState.Checked;
            
            Next();            
        }

        private void LLPath_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SelectPath();
        }

        string CalibrationPath;
        private void SelectPath()
        {
            using(FolderBrowserDialog FBD=new FolderBrowserDialog())
            {
                FBD.Description = LocalizedStrings.SelectCalibrationFolder;
                if (FBD.ShowDialog() == DialogResult.OK)
                {
                    CalibrationPath = FBD.SelectedPath;
                    LLPath.Text = CalibrationPath;
                    if (Directory.Exists(CalibrationPath + "\\Color") || Directory.Exists(CalibrationPath + "\\IR") || Directory.Exists(CalibrationPath + "\\Depth"))
                    {
                        LPathHelp.Visible = true;
                        BStart.Enabled = false;
                    }
                    else
                    {
                        LPathHelp.Visible = false;
                        BStart.Enabled = true;
                    }
                }
            }
        }

        private void BStart_Click(object sender, EventArgs e)
        {
            try
            {
                Directory.CreateDirectory(CalibrationPath + "\\Color");
                Directory.CreateDirectory(CalibrationPath + "\\IR");
                Directory.CreateDirectory(CalibrationPath + "\\Depth");
            }
            catch
            {
                MessageBox.Show(LocalizedStrings.DirectoryCreationError);
            }
            GBPath.Enabled = false;
            GBImage.Enabled = true;
            GBFinish.Enabled = true;

            Master.StartCalibration();
            ImageIndex = 1;
            Step = 0;

            PrepareToCapture();
            BExit.Enabled = true;
        }

        int ImageIndex;
        int Step;
        private void PrepareToCapture()
        {
            Master.SwitchToMode(ScannerModes[Step]);
            StepCheckBoxes[Step].CheckState = CheckState.Indeterminate;            
            BNext.Enabled = true;
            BReset.Enabled = true;            
        }

        CheckBox[] StepCheckBoxes;
        readonly Scanner.Modes[] ScannerModes = { Scanner.Modes.Depth480, Scanner.Modes.RGB1024, Scanner.Modes.IR1024 };
        readonly string[] ImageTypes = {"Depth", "Color", "IR"  };
        readonly string[] ImageExtensions = { ".float",".bmp", ".bmp" };
        private string GetImagePath(int index, int step, bool ext = true)
        {
            return CalibrationPath + "\\" + ImageTypes[step] + "\\" + ImageTypes[step] + index.ToString("D3") + (ext ? ImageExtensions[step] : "");            
        }

        private void Next()
        {

            Step++;
            if (Step == 3)
            {
                Step = 0;
                Master.SwitchToMode(ScannerModes[Step]);
                ImageIndex++;
                CBColor.Checked = File.Exists(GetImagePath(ImageIndex, 0));
                CBIR.Checked = File.Exists(GetImagePath(ImageIndex, 1));
                CBDepth.Checked = File.Exists(GetImagePath(ImageIndex, 2));
                LStatus.Text = (ImageIndex - 1).ToString() + LocalizedStrings.PositionCaptured;
                BExit.Enabled = true;
            }
            PrepareToCapture();
        }

        private void BNext_Click(object sender, EventArgs e)
        {
            BNext.Enabled = false;
            BReset.Enabled = false;
            BExit.Enabled = false;
            Master.CalibrationSave(GetImagePath(ImageIndex, Step, false));
            
        }

        private void CalibrationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Master.StopCalibration();
        }

        private void BReset_Click(object sender, EventArgs e)
        {
            BNext.Enabled = false;
            BReset.Enabled = false;
            for (int i = 0; i < 3; i++)
            {
                File.Delete(GetImagePath(ImageIndex, i));
                StepCheckBoxes[i].CheckState = CheckState.Unchecked;
            }
            Step = 0;
            PrepareToCapture();
        }

        private void BExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CalibrationForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    if (BNext.Enabled) BNext_Click(this, null);
                    break;
                case Keys.Up:
                    Master.TiltUp();
                    break;
                case Keys.Down:
                    Master.TiltDown();
                    break;
                case Keys.NumPad0:
                    Master.TiltHorizontal();
                    break;
            }
        }


    }
}
