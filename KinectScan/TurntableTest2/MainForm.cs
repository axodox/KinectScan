using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Turntables;

namespace TurntableTest2
{
    public partial class MainForm : Form
    {
        Timer XT;
        public MainForm()
        {
            InitializeComponent();
            Turntables.Turntable.DeviceConnected += Turntable_DeviceConnected;
            Enabled = false;
            XT = new Timer();
            XT.Interval = 10;
            XT.Tick += T_Tick;
            XT.Start();
        }

        void T_Tick(object sender, EventArgs e)
        {
            if (T != null) Text = T.PositionInSteps.ToString();
        }

        Turntable T;
        void Turntable_DeviceConnected(object sender, EventArgs e)
        {
            //MessageBox.Show("Device connected!");
            T = Turntable.DefaultDevice;
            Enabled = T == null;
            T.MotorStopped+=T_MotorStopped;
            T.ToOrigin();
        }

        int i = 0;
        void T_MotorStopped(object sender, EventArgs e)
        {
            switch(i)
            {
                case 0:
                    T.ResetCounter();
                    T.RotateTo(1000);
                    break;
                case 1:
                    T.Rotate();
                    T.StopAtMagneticSwitch = true;
                    break;
                    
                    
            }
            
            
            i++;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            T.TurnOff();
        }

        private void MainForm_DoubleClick(object sender, EventArgs e)
        {
            
        }
    }
}
