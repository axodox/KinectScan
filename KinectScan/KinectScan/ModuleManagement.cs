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
            KSC.StartVirtualDevice += new EventHandler(KSC_StartVirtualDevice);
            MM = new ModuleManager(KSC);
            MM.Load(@"..\..\..\RotatingScanner\bin\Release\RotatingScanner.dll");
        }

        void KSC_StartVirtualDevice(object sender, EventArgs e)
        {
            Stop();
            SetActiveDevice(-1);
            Start();
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
            public float DepthB  { get; internal set; }
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

            public void InitiateKinectDevice()
            {
                if (StartKinectDevice != null) StartKinectDevice(this, null);
            }

            internal event EventHandler StartVirtualDevice;
            internal event EventHandler StartKinectDevice;
        }
    }
}
