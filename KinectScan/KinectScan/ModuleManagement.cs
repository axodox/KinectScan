using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Modules;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace KinectScan
{
    public partial class MainForm
    {
        ModuleManager MM;
        KinectScanContext KSC;
        private void InitModules()
        {
            KSC = new KinectScanContext();
            KSC.DepthA = DepthA;
            KSC.DepthB = DepthB;
            KSC.DepthC = DepthC;
            KSC.DepthWidth = 640;
            KSC.DepthHeight = 480;
            KSC.DepthCalibrationWidth = 1280;
            KSC.DepthCalibrationHeight = 1024;
            KSC.DepthIntrinsics = DepthIntrinsics;
            KSC.DepthInverseIntrinsics = DepthInverseIntrinsics;
            KSC.DepthCorrection = DepthCorrection;
            KSC.ColorCorrection = VideoCorrection;
            KSC.OnDepthZLimitChanged(SF.NBZLimit.Value);
            KSC.OnTriangleRemoveLimitChanged(SF.NBTriangleRemove.Value);
            KSC.OnReprojectionChanged(SF.NBReprojectionTranslationX.Value, SF.NBReprojectionTranslationY.Value, SF.NBReprojectionTranslationZ.Value, SF.NBReprojectionRotationX.Value, SF.NBReprojectionRotationY.Value, SF.NBReprojectionRotationZ.Value);
            KSC.WorkingDirectory = WorkingDirectory;
            KSC.StartVirtualDevice += KSC_StartVirtualDevice;
            KSC.StartKinectDevice += KSC_StartKinectDevice;
            KSC.ChangeUIMode += KSC_ChangeUIMode;
            KSC.MessageReceived += KSC_MessageReceived;
            MM = new ModuleManager(KSC);
            MM.Load(@"..\..\..\RotatingScanner\bin\Release\RotatingScanner.dll");
        }

        void KSC_MessageReceived(object sender, KinectScanContext.MessageEventArgs e)
        {
            NotifyIcon.ShowBalloonTip(3000, e.Title, e.Text, e.Icon);
        }

        void KSC_ChangeUIMode(object sender, KinectScanContext.UIModeEventArgs e)
        {
            TSBStartStop.Enabled = MIDevices.Enabled = MIOpenRawData.Enabled = MIOpenSavedModel.Enabled = e.StartStopEnabled;
        }

        void KSC_StartKinectDevice(object sender, KinectScanContext.KinectEventArgs e)
        {
            Stop();
            SetMode(e.Mode);
            if (e.Id != -1) SetActiveDevice(e.Id);
            Start();
        }

        void KSC_StartVirtualDevice(object sender, EventArgs e)
        {
            Stop();
            SetActiveDevice(-1);
            Start();
        }
    }

    public class KinectScanContext
    {
        public Scanner Scanner { get; private set; }
        public event EventHandler ScannerCreated, ScannerDisposing;
        public event EventHandler ProcessingStarted, ProcessingStopping;
        public event EventHandler ProgramClosing;
        public GraphicsDevice XDevice { get; internal set; }
        public SpriteBatch XSprite { get; internal set; }
        public Effect XEffect { get; internal set; }
        public ContentManager XContent { get; internal set; }
        public float DepthA { get; internal set; }
        public float DepthB { get; internal set; }
        public float DepthC { get; internal set; }
        public float TriangleRemoveLimit { get; private set; }
        public float DepthZLimit { get; private set; }
        public float ReprojectionTranslationX { get; private set; }
        public float ReprojectionTranslationY { get; private set; }
        public float ReprojectionTranslationZ { get; private set; }
        public float ReprojectionRotationX { get; private set; }
        public float ReprojectionRotationY { get; private set; }
        public float ReprojectionRotationZ { get; private set; }
        public event EventHandler DepthZLimitChanged, TriangleRemoveLimitChanged, ReprojectionChanged;
        public int DepthWidth { get; internal set; }
        public int DepthHeight { get; internal set; }
        public int DepthCalibrationWidth { get; internal set; }
        public int DepthCalibrationHeight { get; internal set; }
        public Matrix DepthIntrinsics { get; internal set; }
        public Matrix DepthInverseIntrinsics { get; internal set; }
        public float[] DepthCorrection { get; internal set; }
        public float[] ColorCorrection { get; internal set; }
        public string WorkingDirectory { get; internal set; }
        public event EventHandler DeviceCreated, DeviceDisposing;
        public event EventHandler DrawFrame;
        internal void OnDeviceCreated(GraphicsDevice device, SpriteBatch sprite, Effect effect, ContentManager content)
        {
            XDevice = device;
            XSprite = sprite;
            XEffect = effect;
            XContent = content;
            if (DeviceCreated != null)
            {
                DeviceCreated(this, null);
            }
        }
        internal void OnDeviceDisposing()
        {
            if (DeviceDisposing != null)
            {
                DeviceDisposing(this, null);
            }
        }
        internal void OnDrawFrame()
        {
            if (DrawFrame != null) DrawFrame(this, null);
        }
        internal void OnScannerCreated(Scanner scanner)
        {
            Scanner = scanner;
            if (ScannerCreated != null) ScannerCreated(this, null);
        }
        internal void OnScannerDisposing()
        {
            if (ScannerDisposing != null) ScannerDisposing(this, null);
        }
        internal void OnProcessingStarted()
        {
            if (ProcessingStarted != null) ProcessingStarted(this, null);
        }
        internal void OnProcessingStopping()
        {
            if (ProcessingStopping != null) ProcessingStopping(this, null);
        }
        internal void OnDepthZLimitChanged(float value)
        {
            DepthZLimit = value;
            if (DepthZLimitChanged != null) DepthZLimitChanged(this, null);
        }
        internal void OnTriangleRemoveLimitChanged(float value)
        {
            TriangleRemoveLimit = value;
            if (TriangleRemoveLimitChanged != null) TriangleRemoveLimitChanged(this, null);
        }
        internal void OnReprojectionChanged(float translationX, float translationY, float translationZ, float rotationX, float rotationY, float rotationZ)
        {
            ReprojectionTranslationX = translationX;
            ReprojectionTranslationY = translationY;
            ReprojectionTranslationZ = translationZ;
            ReprojectionRotationX = rotationX;
            ReprojectionRotationY = rotationY;
            ReprojectionRotationZ = rotationZ;
            if (ReprojectionChanged != null) ReprojectionChanged(this, null);
        }
        internal void OnProgramClosing()
        {
            if (ProgramClosing != null) ProgramClosing(this, null);
        }

        public void InitiateVirtualDevice()
        {
            if (StartVirtualDevice != null) StartVirtualDevice(this, null);
        }

        public void InitiateKinectDevice(KinectScanner.Modes mode, int id = -1)
        {
            if (StartKinectDevice != null) StartKinectDevice(this, new KinectEventArgs(id, mode));
        }

        internal class KinectEventArgs
        {
            public int Id;
            public Scanner.Modes Mode;
            public KinectEventArgs(int id, Scanner.Modes mode)
            {
                Id = id;
                Mode = mode;
            }
        }
        internal delegate void KinectEventHandler(object sender, KinectEventArgs e);

        internal event EventHandler StartVirtualDevice;
        internal event KinectEventHandler StartKinectDevice;

        internal class UIModeEventArgs
        {
            public bool StartStopEnabled;
            public UIModeEventArgs(bool startStopEnabled)
            {
                StartStopEnabled = startStopEnabled;
            }
        }
        internal delegate void UIModeEventHandler(object sender, UIModeEventArgs e);
        internal event UIModeEventHandler ChangeUIMode;
        public void SetUIMode(bool startStopEnabled)
        {
            if (ChangeUIMode != null) ChangeUIMode(this, new UIModeEventArgs(startStopEnabled));
        }

        public float GetDepth(ushort depth)
        {
            return DepthA - DepthB / (depth - DepthC);
        }

        public Vector4 GetWorldPosition(int x, int y, ushort rawDepth)
        {
            float depth = GetDepth(rawDepth);
            Vector4 imagePos = new Vector4(x * depth, y * depth, depth, 1f);
            return Vector4.Transform(imagePos, Matrix.Transpose(DepthInverseIntrinsics));
        }

        internal class MessageEventArgs : EventArgs
        {
            public string Title, Text;
            public ToolTipIcon Icon;
            public MessageEventArgs(string title, string text, ToolTipIcon icon)
            {
                Title = title;
                Text = text;
                Icon = icon;
            }
        }
        internal delegate void MessageEventHandler(object sender, MessageEventArgs e);
        internal event MessageEventHandler MessageReceived;
        public void ShowTrayMessage(string title, string text, ToolTipIcon icon)
        {
            if (MessageReceived != null) MessageReceived(this, new MessageEventArgs(title, text, icon));
        }
    }
}
