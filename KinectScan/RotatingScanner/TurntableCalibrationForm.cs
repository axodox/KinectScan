using Microsoft.Xna.Framework;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Color = System.Drawing.Color;

namespace KinectScan
{
    public partial class TurntableCalibrationForm : Form
    {
        #region Control
        const int CalibrationSteps = 6;
        ResourceManager RM;
        KinectScanContext Context;
        byte[] StandardDepthData, TableDepthData;
        SynchronizationContext SC;
        enum States { None, SelectTable, SelectStandard };
        States State, ClickState;
        Point TableA, TableB, StandardA, StandardB;
        Calibrator C;
        Graphics3D G3D;
        public TurntableCalibrationForm(KinectScanContext context)
        {
            Context = context;
            InitializeComponent();
            Context.DeviceDisposing += Context_DeviceDisposing;
            RM = new ResourceManager(typeof(LocalizedResources));
            SetStep(1);            
            ClientSize = new System.Drawing.Size(Context.DepthWidth, Context.DepthHeight + 128);
            PHelp.Width = PImage.Width = Context.DepthHeight;
            TLP.RowStyles[0].Height = Context.DepthHeight;
            PHelp.PerformLayout();

            SC = SynchronizationContext.Current;
            TableA = ModuleSettings.Default.CalibrationTableA;
            TableB = ModuleSettings.Default.CalibrationTableB;
            StandardA = ModuleSettings.Default.CalibrationCylinderA;
            StandardB = ModuleSettings.Default.CalibrationCylinderB;
            C = new Calibrator(Context);
            G3D = new Graphics3D(Context.DepthWidth, Context.DepthHeight);
            G3D.Projection = Context.DepthIntrinsics;
        }

        void Context_DeviceDisposing(object sender, EventArgs e)
        {
            MessageBox.Show(LocalizedResources.DeviceRemoved, LocalizedResources.CalibrationTitle);
            Close();
        }

        int Step;
        private void SetStep(int i)
        {
            if (i < 1 || i > CalibrationSteps) return;
            Step = i;

            LStepNumber.Text = Step.ToString();
            LStepText.Text = RM.GetString("Step" + Step.ToString());
            BPrevious.Enabled = Step != 1;
            BNext.Enabled = Step != CalibrationSteps;
            PBCalibration.Value = 100 * Step / CalibrationSteps;
            SetState(States.None);
            switch (Step)
            {
                case 1:
                    Context.InitiateKinectDevice(Scanner.Modes.IR1024);
                    ClickState = States.None;
                    break;
                case 2:
                    Context.InitiateKinectDevice(Scanner.Modes.Depth480);
                    ClickState = States.None;
                    break;
                case 3:
                    ClickState = States.SelectTable;
                    Context.Scanner.RawFrameIn += Scanner_RawFrameIn;                    
                    break;
                case 4:
                    C.CalibrateTable(TableDepthData, TableA, TableB);
                    PImage.Refresh();
                    ClickState = States.None;                    
                    break;
                case 5:
                    ClickState = States.SelectStandard;
                    Context.Scanner.RawFrameIn += Scanner_RawFrameIn;
                    break;
                case 6:
                    C.CalibrateStandard(StandardDepthData, StandardA, StandardB);
                    PImage.Refresh();
                    ClickState = States.None;
                    break;
            }
            PImage.Refresh();
        }

        private void BNext_Click(object sender, EventArgs e)
        {
            SetStep(Step + 1);
        }

        private void BPrevious_Click(object sender, EventArgs e)
        {
            SetStep(Step - 1);
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
            byte[] depthData = (byte[])o;
            PImage.BackgroundImage = DepthToBitmap(depthData, Context.DepthWidth, Context.DepthHeight);
            switch (ClickState)
            {
                case States.SelectTable:
                    TableDepthData = depthData;
                    break;
                case States.SelectStandard:
                    StandardDepthData = depthData;
                    break;
            }
        }

        private void CalibrationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ModuleSettings.Default.CalibrationTableA = TableA;
            ModuleSettings.Default.CalibrationTableB = TableB;
            ModuleSettings.Default.CalibrationCylinderA = StandardA;
            ModuleSettings.Default.CalibrationCylinderB = StandardB;
            ModuleSettings.Default.Save();
        }        
        #endregion

        #region Mouse selection
        void SetState(States state)
        {
            State = state;
            switch (State)
            {
                case States.SelectTable:
                    TableA = TableB = Point.Empty;
                    break;
                case States.SelectStandard:
                    StandardA = StandardB = Point.Empty;
                    break;
            }
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

        private void CalibrationForm_Paint(object sender, PaintEventArgs e)
        {
            if (!TableA.IsEmpty && !TableB.IsEmpty && ClickState == States.SelectTable)
                e.Graphics.DrawEllipse(Pens.Red, TableA.X, TableA.Y, TableB.X - TableA.X, TableB.Y - TableA.Y);
            if (!StandardA.IsEmpty && !StandardB.IsEmpty && ClickState == States.SelectStandard)
                e.Graphics.DrawRectangle(Pens.Blue, StandardA.X, StandardA.Y, StandardB.X - StandardA.X, StandardB.Y - StandardA.Y);

            G3D.SetGraphics(e.Graphics);
            switch (Step)
            {
                case CalibrationSteps:
                    G3D.DrawPlane(C.StandardA, Color.Red);
                    G3D.DrawPlane(C.StandardB, Color.Green);
                    G3D.DrawPlane(C.Table, Color.Blue);
                    break;
                case 4:
                    G3D.DrawPoint(C.Ground.Origin, Color.Yellow);
                    G3D.DrawPlane(C.Ground, Color.Yellow);
                    break;
            }
        }

        private void CalibrationForm_MouseDown(object sender, MouseEventArgs e)
        {
            SetState(ClickState);
            switch (State)
            {
                case States.SelectTable:
                    TableA = e.Location;
                    break;
                case States.SelectStandard:
                    StandardA = e.Location;
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
                case States.SelectStandard:
                    StandardB = e.Location;
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
                case States.SelectStandard:
                    StandardB = e.Location;
                    SetState(States.None);
                    break;
            }

        }
        #endregion

        #region GDI3D
        class Graphics3D
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public Graphics3D(int width, int height)
            {
                Width = width;
                Height = height;
                world = view = projection = Matrix.Identity;
            }
            private Graphics Graphics;
            private Matrix world, view, projection;
            public Matrix WorldViewProjection { get; private set; }
            public Matrix World
            {
                get { return world; }
                set
                {
                    world = value;
                    WorldViewProjection = world * view * projection;
                }
            }
            public Matrix View
            {
                get { return view; }
                set
                {
                    view = value;
                    WorldViewProjection = world * view * projection;
                }
            }
            public Matrix Projection
            {
                get { return projection; }
                set
                {
                    projection = value;
                    WorldViewProjection = world * view * projection;
                }
            }
            public void SetGraphics(Graphics graphics)
            {
                Graphics = graphics;
            }

            private Vector3 Multiply(Matrix matrix,Vector3 position)
            {
                return new Vector3(
                    position.X * matrix.M11 + position.Y * matrix.M12 + position.Z * matrix.M13 + matrix.M14,
                    position.X * matrix.M21 + position.Y * matrix.M22 + position.Z * matrix.M23 + matrix.M24,
                    position.X * matrix.M31 + position.Y * matrix.M32 + position.Z * matrix.M33 + matrix.M34);
            }

            private PointF Project(Vector3 position)
            {
                Vector3 pos = Multiply(WorldViewProjection, position);
                return new PointF(pos.X / pos.Z / Width * Graphics.VisibleClipBounds.Width, pos.Y / pos.Z / Height * Graphics.VisibleClipBounds.Height);
            }

            public void DrawPoint(Vector3 position, Color color)
            {
                PointF pos = Project(position);
                Pen P = new Pen(color);
                Graphics.DrawLine(P, pos.X - 5, pos.Y, pos.X + 5, pos.Y);
                Graphics.DrawLine(P, pos.X, pos.Y - 5, pos.X, pos.Y + 5);
                P.Dispose();
            }

            public void DrawLine(Vector3 a, Vector3 b, Color color)
            {
                Pen P = new Pen(color);
                Graphics.DrawLine(P, Project(a), Project(b));
                P.Dispose();
            }

            const float Length = 0.1f;
            public void DrawPlane(Plane plane, Color color)
            {
                Vector3 u = Vector3.Cross(Vector3.UnitX, plane.Normal);
                Vector3 v = Vector3.Cross(plane.Normal, u);
                Vector3 o = plane.Origin;
                Vector3 x = plane.Origin + plane.Normal * Length/2;
                Vector3 a = plane.Origin + (u + v) * Length;
                Vector3 b = plane.Origin + (u - v) * Length;
                Vector3 c = plane.Origin + (-u - v) * Length;
                Vector3 d = plane.Origin + (-u + v) * Length;
                DrawPoint(o, color);
                DrawLine(o, x, color);
                DrawLine(a, b, color);
                DrawLine(b, c, color);
                DrawLine(c, d, color);
                DrawLine(d, a, color);
            }
        }
        #endregion

        #region Calibration
        public class Calibrator
        {
            KinectScanContext Context;
            public Plane Ground { get; private set; }
            public Plane StandardA { get; private set; }
            public Plane StandardB { get; private set; }
            public Plane Table { get; private set; }
            public Calibrator(KinectScanContext context)
            {
                Context = context;
            }
            public void CalibrateTable(byte[] data, Point tableA, Point tableB)
            {
                Ground = Plane.FromDepthData(data, Context.DepthWidth, Context.DepthHeight, new EllipseRegion(tableA, tableB), Context.GetWorldPosition);
            }
            public void CalibrateStandard(byte[] data, Point standardA, Point standardB)
            {
                int mX=(standardA.X + standardB.X) / 2;
                Point standardM1 = new Point(mX - 10, standardB.Y);
                Point standardM2 = new Point(mX + 10, standardA.Y);
                Plane a = Plane.FromDepthData(data, Context.DepthWidth, Context.DepthHeight, new RectangleRegion(standardA,standardM1), Context.GetWorldPosition);
                Plane b = Plane.FromDepthData(data, Context.DepthWidth, Context.DepthHeight, new RectangleRegion(standardM2, standardB), Context.GetWorldPosition);
                Line axis = Plane.Intersect(a, b);
                Vector3 origin = axis.Intersect(Ground);
                Table = new Plane(origin, axis.Direction);
                StandardA = a;
                StandardB = b;
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
