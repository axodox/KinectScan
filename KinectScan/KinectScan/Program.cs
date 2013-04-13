using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Deployment.Application;
using System.Web;
using ExcaliburSecurity;
namespace KinectScan
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm.Activate();
            if (MainForm.IsActivated) Application.Run(new MainForm());
        }
    }
}
