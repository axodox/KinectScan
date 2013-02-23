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
            SecurityClient SC = new SecurityClient();
            if (SC.PermissionToRun)
            {
                string path = null;
                if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments != null && AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData != null)
                {
                    path = System.Web.HttpUtility.UrlDecode(AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData[0]).Substring(8);
                }
                Application.Run(new MainForm(path));
            }
        }
    }
}
