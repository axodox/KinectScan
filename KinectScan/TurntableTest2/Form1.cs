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
using System.IO.Ports;

namespace TurntableTest2
{
    public partial class Form1 : Form
    {
        Turntable T=null;
        public Form1()
        {
            InitializeComponent();
            T = Turntable.DefaultDevice;
            if (T == null)
            {
                MessageBox.Show("No device found!");
                Close();
            }
            else
            {
                T.SendCommandAsync(Turntable.Commands.ClearCounter);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (T != null) T.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (T != null)
            {
                int lep=0;
                try
                {
                    lep=Convert.ToInt32(textBox1.Text);
                }
                catch
                {
                }
                if (lep > 0)
                    T.SendCommandAsync(Turntable.Commands.Step, lep);
                else
                    T.SendCommandAsync(Turntable.Commands.StepBack, -lep);

            }
        }

        private void Up_Click(object sender, EventArgs e)
        {
            if (T != null) T.SendCommandAsync(Turntable.Commands.StartUp);
/*
            UInt32 s =Convert.ToUInt32(textBox1.Text);
            T.LimHexaPos = s; 
*/
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            if (T != null) T.SendCommandAsync(Turntable.Commands.Stop);
        }

        private void Down_Click(object sender, EventArgs e)
        {
            if (T != null) T.SendCommandAsync(Turntable.Commands.StartDown);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (T != null)
                this.Text = Convert.ToString(T.PositionInDegrees);
        }

        private void ToAngle_Click(object sender, EventArgs e)
        {

            if (T != null)
            {
                double szog = 0;
                try
                {
                    szog = Convert.ToDouble(textBox1.Text);
                }
                catch
                {
                }
                if (szog != 0)
                    T.RotateTo(szog);
            }

        }

        private void BTurnOff_Click(object sender, EventArgs e)
        {
            T.TurnOff();
        }

        private void BZero_Click(object sender, EventArgs e)
        {
            T.SendCommandAsync(Turntable.Commands.ToOrigin);
        }


    }
}
