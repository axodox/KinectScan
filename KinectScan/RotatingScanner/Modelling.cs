using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace KinectScan
{
    abstract class Modeller : IServiceProvider, IGraphicsDeviceService
    {
        protected MainForm.KinectScanContext Context;
        public Modeller(MainForm.KinectScanContext context)
        {
            Context = context;
            InitHardwareAcceleration();
            
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
        public int DepthCacheSize
        {
            get { return depthCacheSize; }
            set
            {
                if (value < 1) return;
                depthCacheSize = value;
                if (DepthCacheSize % 2 == 1) depthCacheSize++;
                DepthMainItem = DepthCacheSize / 2;
                DepthCache = new DepthContainer[DepthCacheSize];
                DepthIndex = DepthCacheUsed = 0;
            }
        }
        public void LoadDepth(byte[] data, float rotation)
        {
            LastDepthIndex = DepthIndex;
            DepthCache[DepthIndex].Data = data;
            DepthCache[DepthIndex].Rotation = rotation;
            DepthIndex++;
            if (DepthIndex == DepthCacheSize) DepthIndex = 0;
            if (DepthCacheUsed < DepthCacheSize) DepthCacheUsed++;
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
        protected EffectParameter DepthSampler, DepthCorrectionSampler, ReprojectionTransform, SpaceTransform, TriangleRemoveLimit, MainSampler, DepthSamplerA, DepthSamplerB, SideSelector, SplitPlaneVector, FusionTranform, MinY, MaxHeight, DepthZLimit, ShadingColor, MaxClipRadius, MinClipRadius, ClipOn, MinAvgCount, DepthAvgLimit, MinClipY;
        protected EffectTechnique DepthAntiDistortTechnique, PerspectiveReprojectionTechnique, DepthVisualizationTechnique, SumTechnique, AvgTechnique, SignedDepthVisualizationTechnique, VGaussTechnique, HGaussTechnique;

        protected XPlane MiniPlane, MaxiPlane;
        private void InitHardwareAcceleration()
        {
            MiniPlane = new XPlane(2, 2);
            MaxiPlane = new XPlane(640, 480);
        }

        public virtual void CreateDevice(IntPtr windowHandle)
        {
            //Device
            XDevice = new GraphicsDevice(
                    GraphicsAdapter.DefaultAdapter,
                    GraphicsProfile.HiDef,
                    new PresentationParameters()
                    {
                        IsFullScreen = false,
                        DeviceWindowHandle = windowHandle,
                        PresentationInterval = PresentInterval.Immediate,
                        BackBufferWidth = 640,
                        BackBufferHeight = 480,
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
            FusionTranform = XEffect.Parameters["FusionTranform"];

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

            //Models
            MiniPlane.SetDevice(XDevice);
            MaxiPlane.SetDevice(XDevice);
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

        public int AveragingMinCount //Numeric error proof
        {
            get { return (int)(MinAvgCount.GetValueSingle()+0.2f); }
            set { MinAvgCount.SetValue(value - 0.1f); }
        }

        public float DepthAverageLimit
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
        #endregion

        #region Transformations
        public float RotationX { get; set; }
        public float RotationZ { get; set; }
        public float TranslationX { get; set; }
        public float TranslationY { get; set; }
        public float TranslationZ { get; set; }
        public float SplitPlaneAngle { get; set; }
        private void SetReprojection(float rotation)
        {
            Matrix T = Matrix.Transpose(Matrix.CreateTranslation(0, 0, -TranslationZ));
            Matrix T2 = Matrix.Transpose(Matrix.CreateTranslation(TranslationX, TranslationY, 0));
            Matrix Ti = Matrix.Invert(T);
            Matrix R = Matrix.CreateRotationY(rotation) * Matrix.CreateRotationZ(RotationZ / 180 * MathHelper.Pi) *
                    Matrix.CreateRotationX(RotationX / 180 * MathHelper.Pi);
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
            FusionTranform.SetValue(Matrix.CreateRotationY(fusionRotation) * space);
        }
        #endregion

        #region Processing
        public int FusionSpacing { get; set; }
        protected int FrameID;
        protected bool FusionTick = false;
        protected float mainRotation;
        public virtual void ProcessFrame()
        {
            if (DepthCacheUsed >= DepthCacheSize)
            {
                if (FrameID % FusionSpacing == 0)
                {
                    XDevice.SetRenderTarget(Vector2TargetA);
                    XDevice.Clear(Color.Black);
                    XDevice.SetRenderTarget(Vector2TargetB);
                    XDevice.Clear(Color.Black);

                    int mainIndex = (DepthIndex + DepthMainItem) % DepthCacheSize;
                    mainRotation = DepthCache[mainIndex].Rotation;
                    byte[] deltaData;
                    float deltaRotation;
                    bool tick = false;
                    for (int i = 0; i < DepthCacheSize; i++)
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
                        if (ViewMode == Views.AntiDistort) Visualize(SingleTargetA, DepthVisualizationTechnique);
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

                    SideSelector.SetValue(-1f);

                    //Fusion + transform
                    SetFusionReprojection(0f, mainRotation);
                    XDevice.SetRenderTarget(FusionTick ? SingleTargetA : SingleTargetB);
                    DepthSampler.SetValue(tick ? Vector2TargetB : Vector2TargetA);
                    XEffect.CurrentTechnique = AvgTechnique; //<----
                    XEffect.CurrentTechnique.Passes[0].Apply();
                    MiniPlane.Draw();

                    if (ViewMode == Views.Avg) Visualize(FusionTick ? SingleTargetA : SingleTargetB, SignedDepthVisualizationTechnique);

                    //Gauss
                    for (int i = 0; i < 4; i++)
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

                    if (ViewMode == Views.Gauss) Visualize(FusionTick ? SingleTargetA : SingleTargetB, SignedDepthVisualizationTechnique);
                    
                }
                FrameID++;
            }
        }

        public abstract void Save(string fileName);

        public virtual void Clear()
        {
            FrameID = 0;
        }
        #endregion
    }
}