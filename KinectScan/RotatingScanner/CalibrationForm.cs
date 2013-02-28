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
        public Plane Params { get; private set; }

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
            Params = new Plane();
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
            //Params.LoadData(DepthData, Context);
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
            
        }
        #endregion

        #region Calibration
        public class Calibrator
        {
            MainForm.KinectScanContext Context;
            Plane ground;

            public void LoadData(MainForm.KinectScanContext context)
            {
                Context=context;
            }
            public void Calibrate(byte[] data, Point tableA, Point tableB)
            {
                ground = Plane.FromDepthData(data, Context.DepthWidth, Context.DepthHeight, new EllipseRegion(tableA, tableB), Context.GetWorldPosition);
            }
            public void Calibrate(byte[] data, Point bookA, Point bookB)
            {
                int mX=(bookA.X + bookB.X) / 2;
                Point bookA1 = new Point(mX - 10, bookA.Y);
                Point bookA2 = new Point(mX + 10, bookA.Y);
                Plane a = Plane.FromDepthData(data, Context.DepthWidth, Context.DepthHeight, new EllipseRegion(bookA1, bookB), Context.GetWorldPosition);
                Plane b = Plane.FromDepthData(data, Context.DepthWidth, Context.DepthHeight, new EllipseRegion(bookA2, bookB), Context.GetWorldPosition);
                Line axis = Plane.Intersect(a, b);
                Vector3 origin = axis.Intersect(ground);
                new Plane(origin, axis.Direction);
            }
        }
        public struct Line
        {
            public Vector3 Origin, Direction;
            public Line(Vector3 origin, Vector3 direction)
            {
                Origin = origin;
                Direction = direction;
            }
            public static Vector3 DistanceBetweenLines(Line a, Line b)
            {
                Vector3 d = Vector3.Cross(a.Direction, b.Direction);
                d.Normalize();
                return d * Vector3.Dot(b.Origin - a.Origin, d);
            }

            public static Vector3 Midpoint(Line a, Line b)
            {
                Vector3 d = Line.DistanceBetweenLines(a, b);
                float t = ((a.Origin.Y + d.Y - b.Origin.Y) * b.Direction.X - (a.Origin.X + d.X - b.Origin.X) * b.Direction.Y) / (a.Direction.X * b.Direction.Y - a.Direction.Y * b.Direction.X);
                return a.Origin + a.Direction * t + d / 2;
            }

            public Vector3 Intersect(Plane p)
            {
                float t = (Vector3.Dot(p.Origin, p.Normal) - Vector3.Dot(Origin, p.Normal)) / (Vector3.Dot(Direction, p.Normal));
                return Origin + t * Direction;
            }
        }
        public struct Plane
        {
            public Vector3 Origin, Normal;
            public Plane(Vector3 origin, Vector3 normal)
            {
                Origin = origin;
                Normal = normal;
            }
            public delegate Vector4 WorldPositionFunction(int x, int y, ushort depth);
            public static Line Intersect(Plane a, Plane b)
            {
                Line L = new Line();
                L.Direction = Vector3.Cross(a.Normal, b.Normal);
                Line lA=new Line(a.Origin,Vector3.Cross(L.Direction, a.Normal));
                Line lB=new Line(b.Origin,Vector3.Cross(L.Direction, b.Normal));
                L.Origin = Line.Midpoint(lA, lB);
                return L;
            }
            public static Plane FromDepthData(byte[] Data,int width, int height, RegionSelector region, WorldPositionFunction worldFunc)
            {
                Plane P = new Plane();
                //Area selection
                int minX = (region.MinX < 0 ? 0 : region.MinX);
                int maxX = (region.MaxX > width ? width : region.MaxX);
                int minY = (region.MinY < 0 ? 0 : region.MinY);
                int maxY = (region.MaxY > height ? height : region.MaxY);
                int jumpX = width - (maxX - minX);
                int startX = minY * width + minX;
                Vector4 pos = Vector4.Zero;
                unsafe
                {
                    fixed (byte* sData = &Data[0])
                    {
                        //Calculate origin
                        ushort* pData = (ushort*)sData + startX;
                        for (int y = minY; y < maxY; y++)
                        {
                            for (int x = minX; x < maxX; x++)
                            {
                                pData++;
                                if (region.Covered(x,y) && *pData != 2047)
                                {
                                    pos += worldFunc(x, y, *pData);
                                }
                            }
                            pData += jumpX;
                        }
                        pos /= pos.W;
                        P.Origin = new Vector3(pos.X, pos.Y, pos.Z);

                        //Calculate normal
                        float a, b, c, d, e, f;
                        a = b = c = d = e = f=0;
                        float rx, ry, rz;
                        pData = (ushort*)sData + startX;
                        for (int y = minY; y < maxY; y++)
                        {
                            for (int x = minX; x < maxX; x++)
                            {
                                pData++;
                                if (region.Covered(x, y) && *pData != 2047)
                                {
                                    pos = worldFunc(x, y, *pData);
                                    rx = pos.X - P.Origin.X;
                                    ry = pos.Y - P.Origin.Y;
                                    rz = pos.Z - P.Origin.Z;
                                    a += ry * ry;
                                    b += rz * ry;
                                    c -= rx * ry;
                                    d += ry * rz;
                                    e += rz * rz;
                                    f -= rx * rz;
                                }
                            }
                            pData += jumpX;
                        }                        
                        float nz = (c * d - a * f) / (b * d - a * e);
                        float ny = (f - e * nz) / d;
                        P.Normal = new Vector3(1f, ny, nz);
                        P.Normal.Normalize();
                    }
                }
                return P;
            }

            
        }

        public abstract class RegionSelector
        {
            public int MinX { get; protected set; }
            public int MinY { get; protected set; }
            public int MaxX { get; protected set; }
            public int MaxY { get; protected set; }
            public abstract bool Covered(int x, int y);
        }

        public class EllipseRegion : RegionSelector
        {
            double ellipseX, ellipseY, ellipseA, ellipseB;
            public EllipseRegion(Point a, Point b)
            {
                MinX = Math.Min(a.X, b.X);
                MaxX = Math.Max(a.X, b.X);
                MinY = Math.Min(a.Y, b.Y);
                MaxY = Math.Max(a.Y, b.Y);

                ellipseX = (b.X + a.X) / 2d;
                ellipseY = (b.Y + a.Y) / 2d;
                ellipseA = Math.Pow((b.X - a.X) / 2d, 2d);
                ellipseB = Math.Pow((b.Y - a.Y) / 2d, 2d);
            }
            public override bool Covered(int x, int y)
            {
                return Math.Pow(x - ellipseX, 2) / ellipseA + Math.Pow(y - ellipseY, 2) / ellipseB < 1;
            }
        }

        public class RectangleRegion : RegionSelector
        {
            public RectangleRegion(Point a, Point b)
            {
                MinX = Math.Min(a.X, b.X);
                MaxX = Math.Max(a.X, b.X);
                MinY = Math.Min(a.Y, b.Y);
                MaxY = Math.Max(a.Y, b.Y);
            }

            public override bool Covered(int x, int y)
            {
                return x >= MinX && x <= MaxX && y >= MinY && y <= MaxY;
            }
        }
        #endregion


    }
}
