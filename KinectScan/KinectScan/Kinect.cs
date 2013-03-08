using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using freenect;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System;
using System.ComponentModel;

namespace KinectScan
{
    public abstract class Scanner
    {
        protected SynchronizationContext SC;
        internal Scanner()
        {
            SC = SynchronizationContext.Current;  
        }
        internal abstract bool AllTexturesReady { get; }
        public enum Modes : byte { RGB1024 = 0, RGB480, IR1024, IR480, Depth480 }
        public Modes Mode { get; protected set; }
        internal abstract TextureDoubleBuffer DepthTexture { get; }
        internal abstract Texture2D VideoTexture { get; }
        internal abstract void InitXNA(GraphicsDevice device);
        internal abstract void PrepareXNAResources();
        internal abstract void EnableBufferUsage();
        public readonly int DepthWidth = 640, DepthHeight = 480;
        protected internal int DepthAvgFrameCount { get; protected set; }
        public Size GetVideoSize(Modes mode)
        {
            switch (mode)
            {
                case Modes.IR1024:
                case Modes.RGB1024:
                    return new Size(1280, 1024);
                case Modes.IR480:
                case Modes.RGB480:
                case Modes.Depth480:
                    return new Size(640, 480);
                default:
                    return new Size(-1, -1);
            }
        }
        internal abstract void DisableBufferUsage();
        internal abstract void ReleaseXNAResources();
        public int Index { get; protected set; }
        public event IOEventHandler IOEvent;
        internal abstract void Start(Modes mode);
        internal abstract void Stop();
        public abstract double Tilt { get; set; }
        internal abstract void Destroy();
        public abstract void SaveRawData(string path);
        protected void SendIOEvent(IOEventArgs e)
        {
            SC.Post(IOEventCallback, e);
        }
        protected void IOEventCallback(object o)
        {
            if (IOEvent != null) IOEvent(this, o as IOEventArgs);
        }
        public abstract bool IsModeSupported(Modes mode);
        protected internal bool BufferUsageDisabled { get; protected set; }

        protected unsafe void ConvertColorData(byte[] source, byte[] target)
        {
            fixed (byte* sSource = &source[0])
            fixed (byte* sTarget = &target[0])
            {
                byte* pSource = sSource;
                byte* pTarget = sTarget;
                byte* eSource = pSource + source.Length;
                while (pSource < eSource)
                {
                    *pTarget = *pSource;
                    pTarget++; pSource++;
                    *pTarget = *pSource;
                    pTarget++; pSource++;
                    *pTarget = *pSource;
                    pTarget += 2; pSource++;
                }
            }
        }

        public unsafe static void SaveColorData(byte[] source, int w, int h, string path)
        {
            using (Bitmap B = new Bitmap(w, h, PixelFormat.Format24bppRgb))
            {
                BitmapData BD = B.LockBits(new System.Drawing.Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                byte* sTarget = (byte*)BD.Scan0.ToPointer();
                int jump = BD.Stride - BD.Width * 3;
                fixed (byte* sSource = &source[0])
                {
                    int i = 0;
                    byte* pSource = sSource;
                    byte* pTarget = sTarget;
                    byte* eSource = pSource + source.Length;
                    while (pSource < eSource)
                    {
                        *pTarget++ = *(pSource+2);
                        *pTarget++ = *(pSource+1);
                        *pTarget++ = *pSource;
                        pSource += 3;
                        i++;
                        if (i == w)
                        {
                            i = 0;
                            pTarget += jump;
                        }
                    }
                }
                B.UnlockBits(BD);
                B.Save(path, ImageFormat.Bmp);
            }
        }

        protected int FrameID;
        public enum FrameTypes { Unknown, Depth, Color };
        public class RawFrameEventArgs : EventArgs
        {
            public byte[] Data;
            public FrameTypes FrameType;
            public int FrameID;
        }
        public delegate void RawFrameEventHandler(object sender, RawFrameEventArgs e);
        public event RawFrameEventHandler RawFrameIn;
        protected void OnRawFrameIn(byte[] data, FrameTypes type)
        {
            if (RawFrameIn != null)
            {
                RawFrameIn(this, new RawFrameEventArgs() { Data = data, FrameType = type, FrameID = FrameID });
            }
        }
    }

    public class FileScanner : Scanner
    {
        TextureDoubleBuffer VideoTextureBuffer, DepthTextureBuffer;
        GraphicsDevice XDevice;
        
        public string FilePath { get; private set; }
        internal FileScanner(string path = null)
        {
            Index = -1;
            FileCount = 0;
            fileIndex = -1;
            FSW = new FileSystemWatcher();
            FSW.Changed += FSW_Changed;
            FSW.Created += FSW_Changed;
            FSW.Deleted += FSW_Changed;
            FSW.Renamed += FSW_Changed;
            Files = new string[0];
            if (path != null) LoadImage(path);
        }

        void FSW_Changed(object sender, FileSystemEventArgs e)
        {
            SC.Post(FileSystemCallback, null);               
        }

        bool FSWOn = false;
        void FileSystemCallback(object o)
        {
            if (!FSWOn)
            {
                FSWOn = true;
                string dir = CurrentDirectory;
                CurrentDirectory = "";
                LoadDirectory(dir);
                FSWOn = false;
            }
        }



        bool FileLoaded;
        internal override bool AllTexturesReady
        {
            get
            {
                return FileLoaded;
            }
        }

        internal override TextureDoubleBuffer DepthTexture
        {
            get { return DepthTextureBuffer; }
        }

        internal override Texture2D VideoTexture
        {
            get { return VideoTextureBuffer.FrontTexture; }
        }

        internal override void InitXNA(GraphicsDevice device)
        {
            XDevice = device;
        }

        bool XNAReady;
        internal override void PrepareXNAResources()
        {
            DepthTextureBuffer = new TextureDoubleBuffer(XDevice, 640, 480, SurfaceFormat.Bgra4444);
            VideoTextureBuffer = new TextureDoubleBuffer(XDevice, 640, 480, SurfaceFormat.Color);
            XNAReady = true;
            UploadTextures();
        }

        internal override void EnableBufferUsage()
        {
            BufferUsageDisabled = false;
            UploadTextures();
        }

        internal override void DisableBufferUsage()
        {
            BufferUsageDisabled = true;
        }

        internal override void ReleaseXNAResources()
        {
            XNAReady = false;
            if (VideoTextureBuffer != null)
            {
                VideoTextureBuffer.Dispose();
                VideoTextureBuffer = null;
            }
            if (DepthTextureBuffer != null)
            {
                DepthTextureBuffer.Dispose();
                DepthTextureBuffer = null;
            }
            FileLoaded = false;
        }

        internal override void Start(Scanner.Modes mode)
        {
            Mode = mode;
            PrepareXNAResources();
            FrameID = 0;
        }

        internal override void Stop()
        {
            
        }

        public override double Tilt {get; set; }

        internal override void Destroy()
        {
            
        }

        public override void SaveRawData(string path)
        {
            
        }

        public string CurrentDirectory { get; private set; }
        public int FileCount { get; private set; }
        private string[] Files;
        private int fileIndex;
        public int FileIndex
        {
            get
            {
                return fileIndex;
            }
            set
            {
                if (value >= 0 && value < Files.Length)
                {
                    fileIndex = value;
                    LoadImage(Files[value].Remove(Files[value].LastIndexOf('.')));
                    if (FileIndexChanged != null) FileIndexChanged(this, null);
                }
            }
        }

        public event EventHandler FileIndexChanged;

        FileSystemWatcher FSW;
        public void LoadDirectory(string path)
        {
            
            string newPath = path;            
            if (newPath != CurrentDirectory)
            {
                try
                {
                    Files = Directory.GetFiles(newPath, "*.rwd");
                    FileCount = Files.Length;
                }
                catch
                {
                    Files = new string[0];
                    FileCount = 0;
                }
                if (FileIndexChanged != null) FileIndexChanged(this, null);
            }
            string filePath = FilePath;
            CurrentDirectory = newPath;
            FSW.Path = newPath;
            try
            {
                FSW.EnableRaisingEvents = true;
            }
            catch { }
            if (fileIndex < 0 || fileIndex >= Files.Length || Files[fileIndex] != filePath)
            {
                fileIndex = -1;
                for (int i = 0; i < Files.Length; i++)
                {
                    if (Files[i] == filePath)
                    {
                        fileIndex = i;
                        break;
                    }
                }
                if (FileIndexChanged != null) FileIndexChanged(this, null);
            }
            
        }


        byte[] DepthData, ColorData;
        public unsafe void LoadImage(string path)
        {
            FilePath = path + ".rwd";
            LoadDirectory(Path.GetDirectoryName(path));
            bool loadOK = false;
            byte[] depthdata = null;
            byte[] colordata = null;
            try
            {
                depthdata = File.ReadAllBytes(path + ".rwd");
                if (depthdata.Length != 640 * 480 * 2) return;
                DepthData = depthdata;

                ColorData = null;
                if (File.Exists(path + ".rwc"))
                {
                    colordata = File.ReadAllBytes(path + ".rwc");
                    if (colordata.Length == 640 * 480 * 3)
                    {
                        byte[] prebuffer = new byte[640 * 480 * 4];
                        ConvertColorData(colordata, prebuffer);
                        ColorData = prebuffer;
                    }
                }
                if(ColorData==null) ColorData = new byte[640 * 480 * 4];
                UploadTextures();
                loadOK = true;
            }
            catch
            {

            }
            if (loadOK)
            {
                FrameID++;
                OnRawFrameIn(depthdata, FrameTypes.Depth);
                OnRawFrameIn(colordata, FrameTypes.Color);
            }
        }



        void UploadTextures()
        {
            if (ColorData != null && DepthData != null && !BufferUsageDisabled && XNAReady)
            {
                VideoTextureBuffer.SetData(ColorData);
                DepthTextureBuffer.SetData(DepthData);
                VideoTextureBuffer.SetData(ColorData);
                DepthTextureBuffer.SetData(DepthData);
                FileLoaded = true;
            }
        }

        public override bool IsModeSupported(Scanner.Modes mode)
        {
            return mode == Modes.Depth480;
        }
    }

    public class KinectScanner : Scanner
    {
        public static int DeviceCount
        {
            get
            {
                return Kinect.DeviceCount;
            }
        }
        
        bool VideoBufferInUse, DepthBufferInUse;
        Kinect XKinect;
        Thread ProcessingThread;
        
        bool ThreadsOn;
        Timer FPSTimer;
        public int FPS { get; private set; }
        int FrameCounter;


        internal override void DisableBufferUsage()
        {
            BufferUsageDisabled = true;
            while (VideoBufferInUse || DepthBufferInUse)
            {
                BufferReleaseARE.WaitOne(100);
            }
        }

        internal override void EnableBufferUsage()
        {
            BufferUsageDisabled = false;
        }

        internal KinectScanner(int index = 0, int avgframe = 2)
        {
                      
            DepthAvgFrameCount = avgframe;
            Index = index;
            XKinect = new Kinect(index);
            try
            {
                XKinect.Open();
            }
            catch
            {
                throw new Exception("Device can not be opened.");
            }
            XKinect.LED.Color = LEDColor.Yellow;
            XKinect.VideoCamera.DataReceived += DataReceived;
            XKinect.DepthCamera.DataReceived += DepthDataReceived;
            StopARE = new AutoResetEvent(false);
            BufferReleaseARE = new AutoResetEvent(false);
            FPSTimer = new Timer(FPSCallback, null, 1000, 1000);
        }

        string RawSavePath;
        bool RawSaveDataNext, RawSaveDepthNext;
        
        public override void SaveRawData(string path)
        {
            RawSavePath = path;
            RawSaveDataNext = RawSaveDepthNext = true;
        }

        void FPSCallback(object o)
        {
            FPS = FrameCounter;
            FrameCounter = 0;
        }

        AutoResetEvent StopARE, BufferReleaseARE;
        internal override void Stop()
        {
            ThreadsOn = false;
            StopARE.WaitOne();
            ReleaseXNAResources();            
        }

        internal override void Start(Modes mode)
        {
            Mode = mode;
            PrepareXNAResources();
            switch (Mode)
            {
                case Modes.IR1024:
                    XKinect.VideoCamera.Mode = XKinect.VideoCamera.Modes[6];
                    XKinect.VideoCamera.Start();
                    break;
                case Modes.IR480:
                    XKinect.VideoCamera.Mode = XKinect.VideoCamera.Modes[7];
                    XKinect.VideoCamera.Start();
                    break;
                case Modes.RGB1024:
                    XKinect.VideoCamera.Mode = XKinect.VideoCamera.Modes[0];
                    XKinect.VideoCamera.Start();
                    break;
                case Modes.RGB480:
                    XKinect.VideoCamera.Mode = XKinect.VideoCamera.Modes[1];
                    XKinect.VideoCamera.Start();
                    break;
                case Modes.Depth480:
                    XKinect.DepthCamera.Mode = XKinect.DepthCamera.Modes[0];
                    XKinect.DepthCamera.Start();
                    XKinect.VideoCamera.Mode = XKinect.VideoCamera.Modes[1];
                    XKinect.VideoCamera.Start();
                    break;
            }
            FrameID = 0;
            ProcessingThread = new Thread(ProcessingWorker);
            ProcessingThread.Name = "Kinect control thread";
            ProcessingThread.Start();
        }





        internal System.Drawing.Rectangle? DepthMeasurementWindow { get; set; }
        internal int Depth { get; private set; }
        private int DepthSum, DepthCount;
        internal double DepthAverage { get; private set; }
        internal void StartDepthMeasurement() { DepthSum = 0; DepthCount = 0; }

        void ProcessingWorker()
        {
            XKinect.LED.Color = LEDColor.Red;
            ThreadsOn = true;
            while (ThreadsOn)
            {
                Kinect.ProcessEvents();
                XKinect.UpdateStatus();
                Thread.Sleep(1);
            }

            switch (Mode)
            {
                case Modes.RGB1024:
                case Modes.RGB480:
                    XKinect.VideoCamera.Stop();
                    VideoTextureBuffer.Dispose();
                    break;
                case Modes.IR1024:
                case Modes.IR480:
                    XKinect.VideoCamera.Stop();
                    VideoTextureBuffer.Dispose();
                    break;
                case Modes.Depth480:
                    XKinect.DepthCamera.Stop();
                    DepthTextureBuffer.Dispose();
                    XKinect.VideoCamera.Stop();
                    VideoTextureBuffer.Dispose();
                    break;
            }
            Prebuffer = null;
            XKinect.LED.Color = LEDColor.Yellow;
            StopARE.Set();
            
        }

        void DataReceived(object sender, BaseCamera.DataReceivedEventArgs e)
        {
            if (!BufferUsageDisabled)
            {
                VideoBufferInUse = true;
                switch (Mode)
                {
                    case Modes.IR1024:
                    case Modes.IR480:
                        VideoTextureBuffer.SetData(e.Data.Data);
                        break;
                    case Modes.RGB1024:
                    case Modes.RGB480:
                    case Modes.Depth480:
                        ConvertColorData(e.Data.Data, Prebuffer);    
                        VideoTextureBuffer.SetData(Prebuffer);
                        break;
                }
                if (Mode != Modes.Depth480)
                    FrameCounter++;
                VideoBufferInUse = false;
            }
            else
            {
                BufferReleaseARE.Set();
            }
            FrameID++;
            OnRawFrameIn(e.Data.Data, FrameTypes.Color);
            if (RawSaveDataNext)
            {
                string path = RawSavePath + ".rwc";
                try
                {
                    File.WriteAllBytes(path, e.Data.Data);
                    SendIOEvent(new IOEventArgs(path, true, null));
                }
                catch (Exception t)
                {
                    SendIOEvent(new IOEventArgs(path, false, t.Message));
                }
                RawSaveDataNext = false;
            }
        }



        void DepthDataReceived(object sender, BaseCamera.DataReceivedEventArgs e)
        {
            if (!BufferUsageDisabled)
            {
                DepthBufferInUse = true;
                switch (Mode)
                {
                    case Modes.Depth480:
                        if (DepthMeasurementWindow != null)
                        {
                            int x1 = DepthMeasurementWindow.Value.X, y1 = DepthMeasurementWindow.Value.Y;
                            int x2 = DepthMeasurementWindow.Value.Width + x1, y2 = DepthMeasurementWindow.Value.Height + y1;
                            int n, dmax = 10000, w = x2 - x1 + 1, h = y2 - y1 + 1, pos, x;

                            byte[] data = e.Data.Data;
                            for (int i = x1; i <= x2; i++)
                            {
                                n = 0;
                                for (int j = y1; j <= y2; j++)
                                {
                                    pos = (j * DepthWidth + i) * 2;
                                    x = data[pos + 1] << 8;
                                    x += data[pos];
                                    n += x;
                                }
                                n /= h;
                                if (n < dmax) dmax = n;
                            }
                            Depth = dmax;
                            DepthCount++;
                            DepthSum += Depth;
                            DepthAverage = (double)DepthSum / DepthCount;
                        }
                        FrameCounter++;
                        DepthTextureBuffer.SetData(e.Data.Data);
                        break;
                }
                DepthBufferInUse = false;
            }
            else
            {
                BufferReleaseARE.Set();
            }
            OnRawFrameIn(e.Data.Data, FrameTypes.Depth);
            if (RawSaveDepthNext)
            {
                string path = RawSavePath + ".rwd";
                try
                {
                    File.WriteAllBytes(path, e.Data.Data);
                    SendIOEvent(new IOEventArgs(path, true, null));
                }
                catch (Exception t)
                {
                    SendIOEvent(new IOEventArgs(path, false, t.Message));
                }
                RawSaveDepthNext = false;
            }
        }

        void BBB(byte[] data, string path)
        {
            using (Bitmap B = new Bitmap(640, 480, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
            {
                BitmapData BD = B.LockBits(new System.Drawing.Rectangle(0, 0, 640, 480), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                unsafe
                {
                    byte* sTarget = (byte*)BD.Scan0.ToPointer();
                    byte a;
                    int x;
                    fixed (byte* sSource = &data[0])
                    {
                        byte* pTarget = sTarget;
                        byte* pSource = sSource;
                        byte* eSource = sSource + data.Length;
                        while (pSource < eSource)
                        {
                            pSource++;
                            x = *pSource<<8;
                            pSource++;
                            x += *pSource;
                            a = (byte)(x >> 3);
                            *pTarget = a;
                            pTarget++;
                            *pTarget = a;
                            pTarget++;
                            *pTarget = a;
                            pTarget++;
                        }
                    }
                }
                B.UnlockBits(BD);
                B.Save(path, ImageFormat.Bmp);
            }
        }

        public double AccelerationX
        {
            get
            {
                return XKinect.Accelerometer.X;
            }
        }

        public double AccelerationY
        {
            get
            {
                return XKinect.Accelerometer.Y;
            }
        }

        public double AccelerationZ
        {
            get
            {
                return XKinect.Accelerometer.Z;
            }
        }

        public override double Tilt
        {
            get
            {
                return XKinect.Motor.Tilt;
            }
            set
            {
                if (value <= 1 && value >= -1)
                {
                    XKinect.Motor.Tilt = value;
                }
            }
        }

        bool Destroyed = false;
        internal override void Destroy()
        {
            if (!Destroyed)
            {
                FPSTimer.Dispose();
                XKinect.LED.Color = LEDColor.BlinkGreen;
                XKinect.Close();
                StopARE.Dispose();
                BufferReleaseARE.Dispose();
                Destroyed = true;
            }
        }


        #region XNA
        GraphicsDevice XDevice;
        TextureDoubleBuffer VideoTextureBuffer, DepthTextureBuffer;
        byte[] Prebuffer;

        internal override void InitXNA(GraphicsDevice device)
        {
            XDevice = device;
        }

        internal override void PrepareXNAResources()
        {
            ReleaseXNAResources();
            switch (Mode)
            {
                case Modes.IR1024:
                    VideoTextureBuffer = new TextureDoubleBuffer(XDevice, 1280, 1024, SurfaceFormat.Bgra4444);
                    break;
                case Modes.IR480:
                    VideoTextureBuffer = new TextureDoubleBuffer(XDevice, 640, 488, SurfaceFormat.Bgra4444);
                    break;
                case Modes.RGB1024:
                    VideoTextureBuffer = new TextureDoubleBuffer(XDevice, 1280, 1024, SurfaceFormat.Color);
                    Prebuffer = new byte[1280 * 1024 * 4];
                    break;
                case Modes.RGB480:
                    VideoTextureBuffer = new TextureDoubleBuffer(XDevice, 640, 480, SurfaceFormat.Color);
                    Prebuffer = new byte[640 * 480 * 4];
                    break;
                case Modes.Depth480:
                    DepthTextureBuffer = new TextureDoubleBuffer(XDevice, 640, 480, SurfaceFormat.Bgra4444, DepthAvgFrameCount);
                    VideoTextureBuffer = new TextureDoubleBuffer(XDevice, 640, 480, SurfaceFormat.Color);
                    Prebuffer = new byte[640 * 480 * 4];
                    break;
            }
        }

        internal override void ReleaseXNAResources()
        {
            if (VideoTextureBuffer != null)
            {
                VideoTextureBuffer.Dispose();
            }
            if (DepthTextureBuffer != null)
            {
                DepthTextureBuffer.Dispose();
            }
        }

        internal override TextureDoubleBuffer DepthTexture
        {
            get
            {
                return DepthTextureBuffer;
            }
        }

        internal override Texture2D VideoTexture
        {
            get
            {
                return VideoTextureBuffer.FrontTexture;
            }
        }

        internal override bool AllTexturesReady
        {
            get
            {

                switch (Mode)
                {
                    case Modes.RGB1024:
                    case Modes.RGB480:
                    case Modes.IR1024:
                    case Modes.IR480:
                        return VideoTexture != null;
                    case Modes.Depth480:
                        return DepthTexture != null && VideoTexture != null;
                    default:
                        return false;
                }

            }
        }
        #endregion

        public override bool IsModeSupported(Scanner.Modes mode)
        {
            return true;
        }
    }
}