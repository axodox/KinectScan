using Microsoft.Xna.Framework;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace KinectScan
{
    public partial class CalibrationForm : Form
    {
        #region UI
        MainForm.KinectScanContext Context;
        byte[] DepthData;
        SynchronizationContext SC;
        enum States { None, SelectTable, SelectCylinder };
        States State;
        Point TableA, TableB, CylinderA, CylinderB;
        public CalibrationData Params { get; private set; }

        public CalibrationForm(MainForm.KinectScanContext context)
        {
            InitializeComponent();
            Context = context;
            Context.ScannerCreated += (object o, EventArgs e) => { TSBCapture.Enabled = true; };
            Context.ScannerDisposing += (object o, EventArgs e) => { TSBCapture.Enabled = false; };
            ClientSize = new Size(Context.DepthWidth, Context.DepthHeight);
            SC = SynchronizationContext.Current;
            TableA = ModuleSettings.Default.CalibrationTableA;
            TableB = ModuleSettings.Default.CalibrationTableB;
            CylinderA = ModuleSettings.Default.CalibrationCylinderA;
            CylinderB = ModuleSettings.Default.CalibrationCylinderB;
            Params = new CalibrationData();
        }

        private void TSBCapture_Click(object sender, EventArgs e)
        {
            Context.Scanner.RawFrameIn += Scanner_RawFrameIn;
        }

        void Scanner_RawFrameIn(object sender, Scanner.RawFrameEventArgs e)
        {
            if (e.FrameType == Scanner.FrameTypes.Depth)
            {
                Context.Scanner.RawFrameIn -= Scanner_RawFrameIn;
                SC.Post(CalibrationFrameIn, e.Data);
            }
        }

        private void CalibrationFrameIn(object o)
        {            
            DepthData = (byte[])o;
            Params.LoadData(DepthData, Context);
            BackgroundImage = DepthToBitmap(DepthData, Context.DepthWidth, Context.DepthHeight);
        }

        private void CalibrationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            ModuleSettings.Default.CalibrationTableA = TableA;
            ModuleSettings.Default.CalibrationTableB = TableB;
            ModuleSettings.Default.CalibrationCylinderA = CylinderA;
            ModuleSettings.Default.CalibrationCylinderB = CylinderB;
            ModuleSettings.Default.Save();
            Hide();
        }

        private Bitmap DepthToBitmap(byte[] data, int width, int height)
        {
            Bitmap B = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData BD = B.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int strideJump = BD.Stride - BD.Width * 3;
            byte intensity;
            unsafe
            {
                byte* sBitmap = (byte*)BD.Scan0;
                byte* pBitmap = sBitmap;
                fixed (byte* sData = &data[0])
                {
                    short* pData = (short*)sData;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            intensity = (byte)(256 * *pData++ / 2047);
                            *pBitmap++ = intensity;
                            *pBitmap++ = intensity;
                            *pBitmap++ = intensity;
                        }
                        pBitmap += strideJump;
                    }
                }
            }
            B.UnlockBits(BD);
            return B;
        }

        void SetState(States state)
        {
            State = state;
            TSBTable.Checked = State == States.SelectTable;
            TSBCylinder.Checked = State == States.SelectCylinder;
            switch (State)
            {
                case States.SelectTable:
                    TableA = TableB = Point.Empty;
                    break;
                case States.SelectCylinder:
                    CylinderA = CylinderB = Point.Empty;
                    break;
            }
        }

        private void CalibrationForm_Paint(object sender, PaintEventArgs e)
        {
            if (!TableA.IsEmpty && !TableB.IsEmpty)
                e.Graphics.DrawEllipse(Pens.Red, TableA.X, TableA.Y, TableB.X - TableA.X, TableB.Y - TableA.Y);
            if (!CylinderA.IsEmpty && !CylinderB.IsEmpty)
                e.Graphics.DrawRectangle(Pens.Blue, CylinderA.X, CylinderA.Y, CylinderB.X - CylinderA.X, CylinderB.Y - CylinderA.Y);
        }


        private void CalibrationForm_MouseDown(object sender, MouseEventArgs e)
        {
            switch (State)
            {
                case States.SelectTable:
                    TableA = e.Location;
                    break;
                case States.SelectCylinder:
                    CylinderA = e.Location;
                    break;
            }
        }

        private void CalibrationForm_MouseMove(object sender, MouseEventArgs e)
        {
            switch (State)
            {
                case States.SelectTable:
                    TableB = e.Location;
                    break;
                case States.SelectCylinder:
                    CylinderB = e.Location;
                    break;
            }
            if (State != States.None) Refresh();
        }

        private void CalibrationForm_MouseUp(object sender, MouseEventArgs e)
        {
            switch (State)
            {
                case States.SelectTable:
                    TableB = e.Location;
                    SetState(States.None);
                    break;
                case States.SelectCylinder:
                    CylinderB = e.Location;
                    SetState(States.None);
                    break;
            }
        }

        private void TSBTable_Click(object sender, EventArgs e)
        {
            SetState(States.SelectTable);
        }

        private void TSBSelectCylinder_Click(object sender, EventArgs e)
        {
            SetState(States.SelectCylinder);
        }

        private void TSBCalibrate_Click(object sender, EventArgs e)
        {
            Params.CalibratePlane(TableA, TableB);
        }
        #endregion

        #region Calibration
        public class CalibrationData
        {
            Vector3 Origin;
            byte[] Data;
            MainForm.KinectScanContext Context;
            public CalibrationData()
            {
            }
            public void LoadData(byte[] data, MainForm.KinectScanContext context)
            {
                Data = data;
                Context = context;
            }
            public void CalibratePlane(Point tableA, Point tableB)
            {
                if (Data == null || Context == null) return;
                double ellipseX = (tableB.X + tableA.X) / 2d;
                double ellipseY = (tableB.Y + tableA.Y) / 2d;
                double ellipseA = Math.Pow((tableB.X - tableA.X) / 2d, 2d);
                double ellipseB = Math.Pow((tableB.Y - tableA.Y) / 2d, 2d);
                Vector4 pos = Vector4.Zero;
                unsafe
                {
                    fixed (byte* sData = &Data[0])
                    {
                        ushort* pData = (ushort*)sData;
                        for (int y = 0; y < Context.DepthHeight; y++)
                            for (int x = 0; x < Context.DepthWidth; x++)
                            {
                                pData++;
                                if (Math.Pow(x - ellipseX, 2) / ellipseA + Math.Pow(y - ellipseY, 2) / ellipseB < 1)
                                {
                                    if(*pData!=2047) pos += Context.GetWorldPosition(x, y, *pData);
                                }
                            }
                    }
                }
                pos /= pos.W;
                Origin = new Vector3(pos.X, pos.Y, pos.Z);
            }
        }
        #endregion


    }
}
