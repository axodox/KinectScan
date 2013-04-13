using KinectScan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KinectScanLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string path = null;
            if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments != null && AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData != null)
            {
                path = System.Web.HttpUtility.UrlDecode(AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData[0]).Substring(8);
            }
            //SecurityClient.ResetActivation();
            MainForm.Activate();
            if (MainForm.IsActivated) Application.Run(new MainForm(path));
        }
    }
}
