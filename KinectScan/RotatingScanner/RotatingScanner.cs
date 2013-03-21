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

        public ProcessorForm PF { get; private set; }
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

            //Context.ProgramClosing += (object sender, EventArgs e) => { PF.Exit(); };

            //PF = new ProcessorForm(context);
            //PF.LoadDirectory += (object sender, EventArgs e) => { InitVirtualLoad(PF.WorkingDirectory); };
            //PF.Show();

            RSF = new RotationScannerForm(context);
            RSF.Show();

            //LoadTimer = new Timer();
            //LoadTimer.Interval = 50;
            //LoadTimer.Tick += (object sender, EventArgs e) => { LoadNext(); };

            //InitHardwareLoad();
        }
        double Rotation;
        //#region Test
        //string[] Files;
        //double[] Rotations;
        //int FileID = 0;
        //FileScanner FS;
        //Timer LoadTimer;
        //private void InitVirtualLoad(string root)
        //{
        //    string[] paths = Directory.GetFiles(root, "*.rwd");
        //    Rotations = new double[paths.Length];
        //    double rotationstep = Math.PI * 2d / paths.Length;
        //    SortedDictionary<string, string> flist = new SortedDictionary<string, string>();
        //    string fileName, path;
        //    string number;
        //    for (int i = 0; i < paths.Length; i++)
        //    {
        //        path = paths[i];
        //        fileName = Path.GetFileName(path);
        //        number = fileName;//int.Parse(fileName.Substring(0, fileName.IndexOf('_')));
        //        flist.Add(number, path);
        //        Rotations[i] = -i * rotationstep;
        //    }
        //    Files = flist.Values.ToArray();     
        //    Context.InitiateVirtualDevice();

        //    PF.OnProjectLoad();
        //    PF.FrameCount = flist.Count;
        //}

        //private void LoadNext()
        //{
        //    FileID++;
        //    if (FileID >= Files.Length) FileID = 0;
        //    int id = Files.Length - FileID - 1;
        //    if (FS != null)
        //    {
        //        string file = Files[id];
        //        FS.LoadImage(file.Substring(0, file.LastIndexOf('.')));
        //    }
        //    Rotation = Rotations[id];
        //}

        //void fs_RawFrameIn(object sender, Scanner.RawFrameEventArgs e)
        //{
        //    if (e.FrameType == Scanner.FrameTypes.Depth)
        //    {
        //        PF.LoadDepth(e.Data, (float)Table.PositionInRadians);
        //    }
        //}
        //#endregion

        //void OnRotationPadConnected()
        //{
        //    LoadTimer.Start();
        //}

        //void OnRotationPadDisconnected()
        //{
        //    LoadTimer.Stop();
        //}

        
        //ScanningForm SF;
        //private void InitHardwareLoad()
        //{
        //    Context.ProgramClosing += Context_ProgramClosing;
        //    string[] devices = Turntable.GetDevices();
        //    if (devices.Length == 0)
        //    {
        //        MessageBox.Show(LocalizedResources.TurntableNotFound, LocalizedResources.ErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    else
        //    {
        //        if (Table != null) Table.Dispose();
        //        Table = new Turntable(devices[0]);
        //        SF = new ScanningForm(this);
        //        SF.Show();
        //    }
        //    Context.ScannerCreated += Context_ScannerCreated;
        //}

        //void Context_ScannerCreated(object sender, EventArgs e)
        //{
        //    if (Context.Scanner is KinectScanner && Table != null)
        //    {
        //        Context.Scanner.RawFrameIn += Scanner_RawFrameIn;
        //    }
        //    if (Context.Scanner is FileScanner)
        //    {
        //        FS = Context.Scanner as FileScanner;
        //        FS.RawFrameIn += new Scanner.RawFrameEventHandler(fs_RawFrameIn);
        //        OnRotationPadConnected();
        //    }
        //    else
        //    {
        //        FS = null;
        //        OnRotationPadDisconnected();
        //    }
        //}

        //void Scanner_RawFrameIn(object sender, Scanner.RawFrameEventArgs e)
        //{
        //    if (e.FrameType == Scanner.FrameTypes.Depth && SF.ScanningState == ScanningForm.ScanningStates.Scanning)
        //        PF.LoadDepth(e.Data, -(float)Table.PositionInRadians);
        //}

        void Context_ProgramClosing(object sender, EventArgs e)
        {
            //if (Table != null) Table.Dispose();
            RSF.Close();
        }


    }
}
