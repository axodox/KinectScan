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
    public partial class ProbeForm : Form
    {
        public ProbeForm()
        {
            InitializeComponent();
        }

        private void ProbeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
