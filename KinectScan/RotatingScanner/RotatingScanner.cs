using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modules;
using System.IO;
using System.Windows.Forms;
using KinectScan;
using Turntables;

namespace KinectScan
{
    public class KinectScanModule : ModuleBase
    {
        RotationScannerForm RSF;

        public Turntable Table { get; private set; }
        
        KinectScanContext Context;
        public override string Name
        {
            get { return "Rotating Scanner"; }
        }

        public KinectScanModule(KinectScanContext context)
        {
            Context = context;

            Turntable.DeviceConnected += (object sender, EventArgs e) => { Context.ShowTrayMessage("KinectScan", LocalizedResources.TurntableConnected,ToolTipIcon.Info); };
            Turntable.DeviceDisconnected += (object sender, EventArgs e) => { Context.ShowTrayMessage("KinectScan", LocalizedResources.TurntableDisconnected, ToolTipIcon.Info); };
            
            Context.ProgramClosing+=Context_ProgramClosing;

            RSF = new RotationScannerForm(context);
            RSF.Show();
        }
        double Rotation;        

        void Context_ProgramClosing(object sender, EventArgs e)
        {
            //if (Table != null) Table.Dispose();
            RSF.Close();
        }


    }
}
