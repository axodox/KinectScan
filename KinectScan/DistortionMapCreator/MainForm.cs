using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DistortionMapCreator.Properties;
using AntiDistortion;

namespace DistortionMapCreator
{
    public partial class MainForm : Form
    {
        DistortionMap DM;
        public MainForm()
        {
            InitializeComponent();
            DM = new DistortionMap();
            DM.Deserialize(Settings.Default.SettingsString);
            CalculationProperyGrid.SelectedObject = DM;
        }

        private void BCalculate_Click(object sender, EventArgs e)
        {
            if (DM.Width <= 0)
            {
                MessageBox.Show("Map width must be greater than zero.");
                return;
            }
            if (DM.Height <= 0)
            {
                MessageBox.Show("Map height must be greater than zero.");
                return;
            }
            if (DM.OriginalWidth <= 0)
            {
                MessageBox.Show("Original width must be greater than zero.");
                return;
            }
            if (DM.OriginalHeight <= 0)
            {
                MessageBox.Show("Original height must be greater than zero.");
                return;
            }
            if (DM.FX <= 0 || DM.FY <= 0)
            {
                MessageBox.Show("Focal length must be greater than zero.");
                return;
            }
            float[] Map = DM.GenerateMap();
            if (SFD.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (FileStream FS = new FileStream(SFD.FileName, FileMode.Create, FileAccess.Write))
                    using (BinaryWriter BW = new BinaryWriter(FS))
                    {
                        BW.Write(DM.Width);
                        BW.Write(DM.Height);
                        for (int i = 0; i < Map.Length; i++)
                        {
                            BW.Write(Map[i]);
                        }
                        BW.Flush();
                        BW.Close();
                    }
                }
                catch
                {
                    MessageBox.Show("Save was unsuccesful!");
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.SettingsString = DM.Serialize();
            Settings.Default.Save();
        }

        private void BClipboardCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(DM.Serialize());
        }
    }
}
