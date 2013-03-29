using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading;
using System.Windows.Forms;
namespace KinectScan
{
    public abstract class Modeller : IServiceProvider, IGraphicsDeviceService, IDisposable
    {
        protected KinectScanContext Context;
        public Modeller(KinectScanContext context)
        {
            Context = context;
            InitHardwareAcceleration();
            StopARE = new AutoResetEvent(false);
            //RefreshTimer = new System.Threading.Timer(Refresh, null, 1000, 20);
        }

        #region Depth cache
        struct DepthContainer
        {
            public byte[] Data;
            public float Rotation;
            public DepthContainer(byte[] data, float rotation)
            {
                Data = data;
                Rotation = rotation;
            }
        }
        private DepthContainer[] DepthCache;
        private int DepthCacheUsed, DepthIndex, DepthMainItem, LastDepthIndex;
        private int depthCacheSize;
        public int DepthAveragingCacheSize
        {
            get { return depthCacheSize; }
            set
            {
                if (value < 1) return;
                depthCacheSize = value;
                if (DepthAveragingCacheSize % 2 == 0) depthCacheSize++;
                DepthMainItem = DepthAveragingCacheSize / 2;
                DepthCache = new DepthContainer[DepthAveragingCacheSize];
                for (int i = 0; i < DepthAveragingCacheSize; i++)
                {
                    DepthCache[i].Data = new byte[Context.DepthLength * 2];
                }
                DepthIndex = DepthCacheUsed = 0;
            }
        }

        public void LoadDepth(byte[] data, float rotation)
        {
            LastDepthIndex = DepthIndex;
            data.FastCopy4To(DepthCache[DepthIndex].Data);
            DepthCache[DepthIndex].Rotation = rotation;
            DepthIndex++;
            if (DepthIndex == DepthAveragingCacheSize) DepthIndex = 0;
            if (DepthCacheUsed < DepthAveragingCacheSize) DepthCacheUsed++;
            ProcessFrame();
        }
        #endregion

        #region GraphicsService
        private void InitGraphicsDeviceService()
        {
            if (DeviceCreated != null)
            {
                DeviceCreated(this, null);
            }
            XDevice.Disposing += new EventHandler<EventArgs>(XDevice_Disposing);
            XDevice.DeviceReset += new EventHandler<EventArgs>(XDevice_DeviceReset);
            XDevice.DeviceResetting += new EventHandler<EventArgs>(XDevice_DeviceResetting);
        }

        public new object GetService(Type serviceType)
        {
            if (serviceType == typeof(IGraphicsDeviceService))
            {
                return this;
            }
            else
            {
                return null;
            }
        }

        public event EventHandler<EventArgs> DeviceCreated;

        public event EventHandler<EventArgs> DeviceDisposing;
        void XDevice_Disposing(object sender, EventArgs e)
        {
            if (DeviceDisposing != null)
            {
                DeviceDisposing(this, null);
            }
        }

        public event EventHandler<EventArgs> DeviceReset;
        void XDevice_DeviceReset(object sender, EventArgs e)
        {
            if (DeviceReset != null)
            {
                DeviceReset(this, null);
            }
        }

        public event EventHandler<EventArgs> DeviceResetting;
        void XDevice_DeviceResetting(object sender, EventArgs e)
        {
            if (DeviceResetting != null)
            {
                DeviceResetting(this, null);
            }
        }

        public GraphicsDevice GraphicsDevice
        {
            get { return XDevice; }
        }
        #endregion

        #region Hardware acceleration
        protected GraphicsDevice XDevice;
        protected ContentManager XContent;
        protected Effect XEffect;

        Texture2D TRawDepth, TDepth, TDepthCorrection;
        protected RenderTarget2D Vector2TargetA, Vector2TargetB, SingleTargetA, SingleTargetB, Vector4Target;
        protected EffectParameter DepthSampler, DepthCorrectionSampler, ReprojectionTransform, SpaceTransform, TriangleRemoveLimit, MainSampler, DepthSamplerA, DepthSamplerB, SideSelector, SplitPlaneVector, FusionTransform, MinY, MaxHeight, DepthZLimit, ShadingColor, MaxClipRadius, MinClipRadius, ClipOn, MinAvgCount, DepthAvgLimit, MinClipY;
        protected EffectTechnique DepthAntiDistortTechnique, PerspectiveReprojectionTechnique, DepthVisualizationTechnique, SumTechnique, AvgTechnique, SignedDepthVisualizationTechnique, VGaussTechnique, HGaussTechnique;

        protected XPlane MiniPlane, MaxiPlane;
        private void InitHardwareAcceleration()
        {
            MiniPlane = new XPlane(2, 2);
            MaxiPlane = new XPlane(640, 480);
        }

        public virtual void CreateDevice(Control control)
        {
            //Device
            XDevice = new GraphicsDevice(
                    GraphicsAdapter.DefaultAdapter,
                    GraphicsProfile.HiDef,
                    new PresentationParameters()
                    {
                        IsFullScreen = false,
                        DeviceWindowHandle = control.Handle,
                        PresentationInterval = PresentInterval.Immediate,
                        BackBufferWidth = control.Width,
                        BackBufferHeight = control.Height,
                        DepthStencilFormat = DepthFormat.Depth16,
                        RenderTargetUsage = RenderTargetUsage.PreserveContents
                    });
            XDevice.RasterizerState = RasterizerState.CullNone;
            XDevice.DepthStencilState = DepthStencilState.Default;
            InitGraphicsDeviceService();
            XContent = new ContentManager(this);
            XEffect = XContent.Load<Effect>("Content/RotatingScannerEffects");

            //Textures
            TRawDepth = new Texture2D(XDevice, Context.DepthWidth, Context.DepthHeight, false, SurfaceFormat.Bgra4444);
            TDepth = new Texture2D(XDevice, Context.DepthWidth, Context.DepthHeight, false, SurfaceFormat.Single);
            TDepthCorrection = new Texture2D(XDevice, Context.DepthWidth, Context.DepthHeight, false, SurfaceFormat.Vector2);
            TDepthCorrection.SetData<float>(Context.DepthCorrection);

            //Render targets
            SingleTargetA = new RenderTarget2D(XDevice, Context.DepthWidth, Context.DepthHeight, false, SurfaceFormat.Single, DepthFormat.Depth16);
            SingleTargetB = new RenderTarget2D(XDevice, Context.DepthWidth, Context.DepthHeight, false, SurfaceFormat.Single, DepthFormat.Depth16);
            Vector2TargetA = new RenderTarget2D(XDevice, Context.DepthWidth, Context.DepthHeight, false, SurfaceFormat.Vector2, DepthFormat.Depth16);
            Vector2TargetB = new RenderTarget2D(XDevice, Context.DepthWidth, Context.DepthHeight, false, SurfaceFormat.Vector2, DepthFormat.Depth16);
            Vector4Target = new RenderTarget2D(XDevice, Context.DepthWidth, Context.DepthHeight, false, SurfaceFormat.Vector4, DepthFormat.Depth16);

            //Samplers
            MainSampler = XEffect.Parameters["MainTexture"];
            DepthSampler = XEffect.Parameters["DepthTexture"];
            DepthCorrectionSampler = XEffect.Parameters["DepthCorrectionTexture"];
            DepthSamplerA = XEffect.Parameters["DepthTextureA"];
            DepthSamplerB = XEffect.Parameters["DepthTextureB"];

            //Transforms
            ReprojectionTransform = XEffect.Parameters["ReprojectionTransform"];
            SpaceTransform = XEffect.Parameters["SpaceTransform"];
            FusionTransform = XEffect.Parameters["FusionTransform"];

            //Parameters
            TriangleRemoveLimit = XEffect.Parameters["TriangleRemoveLimit"];
            SideSelector = XEffect.Parameters["SideSelector"];
            SplitPlaneVector = XEffect.Parameters["SplitPlaneVector"];
            MinY = XEffect.Parameters["ProjYMin"];
            MaxHeight = XEffect.Parameters["ProjHeightMax"];
            DepthZLimit = XEffect.Parameters["DepthLimitZ"];
            ShadingColor = XEffect.Parameters["ShadingColor"];
            MaxClipRadius = XEffect.Parameters["ClipRadiusMax"];
            MinClipRadius = XEffect.Parameters["ClipRadiusMin"];
            ClipOn = XEffect.Parameters["ClipOn"];
            MinAvgCount = XEffect.Parameters["DepthAvgCountMin"];
            DepthAvgLimit = XEffect.Parameters["DepthAvgLimit"];
            MinClipY = XEffect.Parameters["ClipYMin"];
            
            //Techniques
            DepthAntiDistortTechnique = XEffect.Techniques["DepthAntiDistort"];
            PerspectiveReprojectionTechnique = XEffect.Techniques["PerspectiveReprojection"];
            DepthVisualizationTechnique = XEffect.Techniques["DepthVisualization"];
            SumTechnique = XEffect.Techniques["Sum"];
            AvgTechnique = XEffect.Techniques["Avg"];
            SignedDepthVisualizationTechnique = XEffect.Techniques["SignedDepthVisualization"];
            VGaussTechnique = XEffect.Techniques["DepthVGauss"];
            HGaussTechnique = XEffect.Techniques["DepthHGauss"];

            //Constants
            XEffect.Parameters["DepthCorrectionTextureSize"].SetValue(new Vector2(Context.DepthWidth, Context.DepthHeight));
            XEffect.Parameters["DepthInverseIntrinsics"].SetValue(Context.DepthInverseIntrinsics);
            XEffect.Parameters["NaN"].SetValue(float.NaN);
            XEffect.SetGaussCoeffs(Context.DepthWidth, Context.DepthHeight, 9, 1, 1);

            //Models
            MiniPlane.SetDevice(XDevice);
            MaxiPlane.SetDevice(XDevice);
        }

        AutoResetEvent StopARE;
        bool StopPending;
        int Processing = 0;
        public virtual void DestroyDevice()
        {
            while (Processing > 0)
            {
                StopPending = true;
                StopARE.WaitOne();
            }
            TRawDepth.Dispose(); TRawDepth = null;
            TDepth.Dispose(); TDepth = null;
            TDepthCorrection.Dispose(); TDepthCorrection = null;

            SingleTargetA.Dispose(); SingleTargetA = null;
            SingleTargetB.Dispose(); SingleTargetB = null;
            Vector2TargetA.Dispose(); Vector2TargetA = null;
            Vector2TargetB.Dispose(); Vector2TargetB = null;
            Vector4Target.Dispose(); Vector4Target = null;

            MiniPlane.Dispose(); MiniPlane = null;
            MaxiPlane.Dispose(); MaxiPlane = null;

            XContent.Unload(); XContent.Dispose(); XContent = null;
            XEffect=null;
            XDevice.Dispose(); XDevice = null;
        }

        public float TriangleClippingLimit
        {
            get { return TriangleRemoveLimit.GetValueSingle(); }
            set { TriangleRemoveLimit.SetValue(value); }
        }

        public float ProjectionYMin
        {
            get { return MinY.GetValueSingle(); }
            set
            {
                float oldMaxY = ProjectionYMax;
                MinY.SetValue(value);
                ProjectionYMax = oldMaxY;
            }
        }

        public float ProjectionYMax
        {
            get { return MaxHeight.GetValueSingle() + ProjectionYMin; }
            set { MaxHeight.SetValue(value - ProjectionYMin); }
        }

        public float ClippingDepthMax
        {
            get { return DepthZLimit.GetValueSingle(); }
            set { DepthZLimit.SetValue(value); }
        }

        public float ClippingRadiusMax
        {
            get { return MaxClipRadius.GetValueSingle(); }
            set { MaxClipRadius.SetValue(value); }
        }

        public float ClippingRadiusMin
        {
            get { return MinClipRadius.GetValueSingle(); }
            set { MinClipRadius.SetValue(value); }
        }

        public bool ClippingEnabled
        {
            get { return ClipOn.GetValueBoolean(); }
            set { ClipOn.SetValue(value); }
        }

        public int DepthAveragingMinCount //Numeric error proof
        {
            get { return (int)(MinAvgCount.GetValueSingle()+0.2f); }
            set { MinAvgCount.SetValue(value - 0.1f); }
        }

        public float DepthAveragingLimit
        {
            get { return DepthAvgLimit.GetValueSingle(); }
            set { DepthAvgLimit.SetValue(value); }
        }

        public float ClippingYMin
        {
            get { return MinClipY.GetValueSingle(); }
            set { MinClipY.SetValue(value); }
        }
        #endregion

        #region Visualization
        public enum Views : byte { AntiDistort, Sum, Avg, Gauss, Special };
        public Views ViewMode { get; set; }
        public abstract string[] SpecialViewModes { get; }
        public abstract void SetSpecialViewMode(int index);
        public int VisualizedLeg { get; set; }
        protected void Visualize(Texture texture, EffectTechnique technique, bool present = true)
        {
            XDevice.SetRenderTarget(null);
            XDevice.Clear(Color.White);
            MainSampler.SetValue(texture);
            XEffect.CurrentTechnique = technique;
            XEffect.CurrentTechnique.Passes[0].Apply();
            MiniPlane.Draw();            
            if (present) XDevice.Present();
        }
        private System.Threading.Timer RefreshTimer;
        private bool Refreshing;
        private void Refresh(object o) 
        { 
            if (XDevice == null || Disposed || Refreshing) 
                return;
            Refreshing = true;
            OnRefresh();
            Refreshing = false;
        }
        protected virtual void OnRefresh() { }
        #endregion

        #region Transformations
        public float RotationX { get; set; }
        public float SplitPlaneAngle { get; set; }
        public float RotationZ { get; set; }
        public float TranslationX { get; set; }
        public float TranslationY { get; set; }
        public float TranslationZ { get; set; }        
        private void SetReprojection(float rotation)
        {
            Matrix T = Matrix.Transpose(Matrix.CreateTranslation(0, 0, -TranslationZ));
            Matrix T2 = Matrix.Transpose(Matrix.CreateTranslation(TranslationX, TranslationY, 0));
            Matrix Ti = Matrix.Invert(T);
            Matrix R = Matrix.CreateRotationY(rotation) * Matrix.CreateRotationZ(RotationZ) *
                    Matrix.CreateRotationX(RotationX);
            Matrix reproj = Context.DepthIntrinsics * Ti * R * T * T2 * Context.DepthInverseIntrinsics;
            ReprojectionTransform.SetValue(reproj); //->main
            Matrix space = R * T * T2 * Context.DepthInverseIntrinsics;
            SpaceTransform.SetValue(space); //->clip
        }

        private void SetFusionReprojection(float rotation, float fusionRotation)
        {
            Matrix T = Matrix.Transpose(Matrix.CreateTranslation(0, 0, -TranslationZ));
            Matrix Ti = Matrix.Invert(T);
            Matrix R = Matrix.CreateRotationY(rotation);
            Matrix space = R * T * Context.DepthInverseIntrinsics;
            Matrix reproj = Context.DepthIntrinsics * Ti * R * T * Context.DepthInverseIntrinsics;
            ReprojectionTransform.SetValue(reproj);
            SpaceTransform.SetValue(space);
            SplitPlaneVector.SetValue(new Vector4((float)Math.Cos(SplitPlaneAngle), 0f, (float)Math.Sin(SplitPlaneAngle), 0f));
            FusionTransform.SetValue(Matrix.CreateRotationY(fusionRotation) * space);
        }
        #endregion

        #region Processing
        public int FusionSpacing { get; set; }
        protected int FrameID;
        protected bool FusionTick = false;
        protected float mainRotation;
        protected void ProcessFrame()
        {
            if (XDevice == null) return;

            Processing++;
            //try
            {
                if (DepthCacheUsed >= DepthAveragingCacheSize)
                {
                    if (FrameID % FusionSpacing == 0)
                    {
                        XDevice.SetRenderTarget(Vector2TargetA);
                        XDevice.Clear(Color.Black);
                        XDevice.SetRenderTarget(Vector2TargetB);
                        XDevice.Clear(Color.Black);

                        int mainIndex = (DepthIndex + DepthMainItem) % DepthAveragingCacheSize;
                        mainRotation = DepthCache[mainIndex].Rotation;
                        byte[] deltaData;
                        float deltaRotation;
                        bool tick = false;
                        for (int i = 0; i < DepthAveragingCacheSize; i++)
                        {
                            deltaData = DepthCache[i].Data;
                            deltaRotation = DepthCache[i].Rotation - mainRotation;
                            if (deltaRotation > MathHelper.Pi) deltaRotation -= MathHelper.TwoPi;
                            if (deltaRotation < -MathHelper.Pi) deltaRotation += MathHelper.TwoPi;

                            for (int j = 0; j < 16; j++)
                                XDevice.Textures[j] = null;
                            TRawDepth.SetData<byte>(deltaData);

                            //Anti-distort
                            XDevice.SetRenderTarget(SingleTargetA);
                            XDevice.Clear(Color.Black);
                            XDevice.RasterizerState = RasterizerState.CullNone;
                            XDevice.BlendState = BlendState.Opaque;
                            XDevice.DepthStencilState = DepthStencilState.None;

                            DepthSampler.SetValue(TRawDepth);
                            DepthCorrectionSampler.SetValue(TDepthCorrection);
                            XEffect.CurrentTechnique = DepthAntiDistortTechnique; //<----
                            XEffect.CurrentTechnique.Passes[0].Apply();
                            MiniPlane.Draw();
                            if (ViewMode == Views.AntiDistort) Visualize(SingleTargetA, DepthVisualizationTechnique);

                            //Reprojection + transform
                            SetReprojection(deltaRotation);
                            TriangleRemoveLimit.SetValue(Context.TriangleRemoveLimit);
                            XDevice.SetRenderTarget(SingleTargetB);
                            XDevice.Clear(Color.Black);
                            XDevice.BlendState = BlendState.Opaque;
                            XDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                            XDevice.DepthStencilState = DepthStencilState.Default;

                            DepthSampler.SetValue(SingleTargetA);
                            XEffect.CurrentTechnique = PerspectiveReprojectionTechnique; //<----
                            XEffect.CurrentTechnique.Passes[0].Apply();
                            MaxiPlane.Draw();

                            if (ViewMode == Views.Sum) Visualize(SingleTargetB, DepthVisualizationTechnique);

                            //Sum
                            XDevice.SetRenderTarget(tick ? Vector2TargetA : Vector2TargetB);
                            XDevice.Clear(Color.Black);
                            XDevice.RasterizerState = RasterizerState.CullNone;
                            XDevice.BlendState = BlendState.Opaque;
                            XDevice.DepthStencilState = DepthStencilState.None;

                            DepthSamplerA.SetValue(SingleTargetB);
                            DepthSamplerB.SetValue(tick ? Vector2TargetB : Vector2TargetA);
                            XEffect.CurrentTechnique = SumTechnique; //<----
                            XEffect.CurrentTechnique.Passes[0].Apply();
                            MiniPlane.Draw();
                            tick = !tick;
                        }


                        for (int leg = 0; leg < 2; leg++)
                        {
                            SideSelector.SetValue(leg == 0 ? 1f : -1f);

                            //Fusion + transform
                            SetFusionReprojection(0f, mainRotation);
                            XDevice.SetRenderTarget(FusionTick ? SingleTargetA : SingleTargetB);
                            DepthSampler.SetValue(tick ? Vector2TargetB : Vector2TargetA);
                            XEffect.CurrentTechnique = AvgTechnique; //<----
                            XEffect.CurrentTechnique.Passes[0].Apply();
                            MiniPlane.Draw();

                            if (ViewMode == Views.Avg && VisualizedLeg == leg) Visualize(FusionTick ? SingleTargetA : SingleTargetB, SignedDepthVisualizationTechnique);

                            //Gauss
                            for (int i = 0; i < 0; i++)
                            {
                                XDevice.SetRenderTarget(FusionTick ? SingleTargetB : SingleTargetA);
                                DepthSampler.SetValue(FusionTick ? SingleTargetA : SingleTargetB);
                                XEffect.CurrentTechnique = VGaussTechnique;
                                XEffect.CurrentTechnique.Passes[0].Apply();
                                MiniPlane.Draw();
                                FusionTick = !FusionTick;

                                XDevice.SetRenderTarget(FusionTick ? SingleTargetB : SingleTargetA);
                                DepthSampler.SetValue(FusionTick ? SingleTargetA : SingleTargetB);
                                XEffect.CurrentTechnique = HGaussTechnique;
                                XEffect.CurrentTechnique.Passes[0].Apply();
                                MiniPlane.Draw();
                                FusionTick = !FusionTick;
                            }

                            if (ViewMode == Views.Gauss && VisualizedLeg == leg) Visualize(FusionTick ? SingleTargetA : SingleTargetB, SignedDepthVisualizationTechnique);

                            ProcessLeg(leg);
                        }
                    }
                    FrameID++;
                }
                if (StopPending)
                {
                    StopARE.Set();
                    StopPending = false;
                }
            }
            //catch
            //{
            //    System.Diagnostics.Debugger.Break();
            //}
            Processing--;
        }

        protected abstract void ProcessLeg(int leg);

        public abstract void Save(string fileName);

        public virtual void DebugSave() { }
        public virtual void DebugLoad() { }

        public virtual void Clear()
        {
            FrameID = 0;
        }
        #endregion

        bool Disposed = false;
        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            DestroyDevice();
        }
    }
}