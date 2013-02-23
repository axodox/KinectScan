using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using KinectScan;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Globalization;

namespace KinectScan
{
    public partial class ProcessorForm : Form, IServiceProvider, IGraphicsDeviceService
    {
#if DEBUG
        const int DebugFrame = 300, DebugMinFrame=25, DebugMaxFrame=35;
        const int DebugLine = 474;
#endif
        const int DisplayLines = 12;
        public enum Views : byte { AntiDistort, Sum, Avg, Gauss, Fusion, ExCoreReprojection, ExCorePolarDepth };
        Views ViewMode = Views.Fusion;
        public enum ModellingModes : byte { ExCore, Fusion };
        ModellingModes ModellingMode = ModellingModes.Fusion;
        int DepthCacheSize = 21;
        int DepthMainItem = 10;
        int DepthIndex;
        int DepthCacheUsed;
        float debugValue = 1f;
        const float debugStep = 0.01f;
        RSSettingsForm SF;
        float RotationX, RotationZ, TranslationX, TranslationY, TranslationZ;
        ModelMaker MM;
        DepthContainer[] DepthCache;
        public string WorkingDirectory;
        public bool ProjectLoaded=false;
        public int FrameCount = -1;
        public void OnProjectLoad()
        {
            FrameCount = -1;
            FrameID = 0;
            ProjectLoaded = true;
            Settings.Default.PropertyValues.Import(WorkingDirectory + @"\Project.xml", "RotatingScanner");
        }
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
        bool ExCoreTick;
        GraphicsDevice XDevice;
        MainForm.KinectScanContext Context;
        ContentManager XContent;
        Effect XEffect;
        Texture2D TRawDepth, TDepth, TDepthCorrection;
        RenderTarget2D Vector2TargetA, Vector2TargetB, SingleTargetA, SingleTargetB, Vector4Target, ExCoreTargetA, ExCoreTargetB;
        EffectTechnique DepthAntiDistortTechnique, FusionDisplayTechnique, SumTechnique, AvgTechnique, PerspectiveReprojectionTechnique, FusionOutputTechnique, FusionLinesTechnique, VGaussTechnique, HGaussTechnique, CalibrationTechnique, DualTechnique, ColoredDepthTechnique, SimpleTechnique;
        EffectTechnique DepthVisualizationTechnique, SignedDepthVisualizationTechnique, PolarDisplayTechnique, PolarOutputTechnique, ColorModelTechnique, PolarAddTechnique, PolarAvgTechnique, PolarAvgDisplayTechnique;
        EffectParameter DepthSampler, DepthCorrectionSampler, DepthToColorTransform, ReprojectionTransform, DepthZLimit, TriangleRemoveLimit, DepthSamplerA, DepthSamplerB, SpaceTransform, MinX, MinY, MaxWidth, MaxHeight, SplitPlaneVector, FusionTranform, SegmentLength, FusionTransform, LinesTransform, ShadingColor, MaxClipRadius, MinClipRadius, ClipOn, MinAvgCount, DepthAvgLimit, MinClipY, SideSelector, MainSampler, ModelTransform, CorePos;
        XPlane MiniPlane, MaxiPlane;
        VertexBuffer LinesVertexBuffer;

        ToolStripItem[] MainMenuSet, CalibrationMenuSet;
        void SetMenuVisibility(ToolStripItem[] menu, bool visibility)
        {
            for (int i = 0; i < menu.Length; i++) menu[i].Visible = visibility;
        }

        public ProcessorForm(MainForm.KinectScanContext context)
        {
            Context = context;
            Context.TriangleRemoveLimitChanged += (object o, EventArgs e) => { TriangleRemoveLimit.SetValue(Context.TriangleRemoveLimit); };
            
            InitializeComponent();
            InitGUI();

            MiniPlane = new XPlane(2, 2);
            MaxiPlane = new XPlane(640, 480);

            MM = new ModelMaker(Context.DepthWidth, Context.DepthHeight);

            CreateXDevice();
            LoadSettings();            

            DepthCache = new DepthContainer[DepthCacheSize];
            DepthIndex = DepthCacheUsed = 0;

            CF = new CalibrationForm(Context);
        }

        MenuItem[] ModellingModeMenuItems;
        void InitGUI()
        {
            MainMenuSet = new ToolStripItem[] { TSBCalibration, TSDDBView, TSBBuildModel, TSBSaveModel, TSBSettings, TSS0, TSLLabel, TSTBLabel };
            CalibrationMenuSet = new ToolStripItem[] { TSBCalibrationSetA, TSBCalibrationSetB, TSBCalibrationClear, TSBCalibrationResetView, TSBExitMode };

            Array ViewModes = Enum.GetValues(typeof(Views));
            ToolStripMenuItem TSMI;
            foreach (Views mode in ViewModes)
            {
                TSMI = new ToolStripMenuItem(mode.ToString());
                TSMI.Click += TSMIViewMode_Click;
                TSMI.Tag = mode;
                TSDDBView.DropDownItems.Add(TSMI);
            }

            Array ModellingModes = Enum.GetValues(typeof(ModellingModes));
            ModellingModeMenuItems = new MenuItem[ModellingModes.Length];
            MenuItem MI;
            foreach (ModellingModes mode in ModellingModes)
            {
                MI = new MenuItem(mode.ToString());
                MI.RadioCheck = true;
                MI.Tag = mode;
                MI.Click += new EventHandler(MI_Click);
                MIModellingMode.MenuItems.Add(MI);
                ModellingModeMenuItems[(int)mode] = MI;
            }

            MouseWheel += (object o, MouseEventArgs e) =>
            {
                debugValue += debugStep * Math.Sign(e.Delta);
                Text = debugValue.ToString();
            };

            KeyDown += new KeyEventHandler(DebugForm_KeyDown);
        }

        void MI_Click(object sender, EventArgs e)
        {
            SetModellingMode((ModellingModes)(sender as MenuItem).Tag);
        }

        void SetModellingMode(ModellingModes mode)
        {
            ModellingModeMenuItems[(int)ModellingMode].Checked = false;
            ModellingMode = mode;
            ModellingModeMenuItems[(int)ModellingMode].Checked = true;
        }

        void DebugForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Space:
                    MM.SaveToSTLFile(@"x", "próba");
                    break;
            }
        }

        private void LoadSettings()
        {
            SF = new RSSettingsForm();
            SF.SettingsLoaded += new EventHandler(SF_SettingsLoaded);
            SF.LoadSettings();
            SF.NBZLimit.ValueChanged += (object o, EventArgs e) => { DepthZLimit.SetValue(SF.NBZLimit.Value); };
            SF.NBMinY.ValueChanged += (object o, EventArgs e) =>
            {
                MinY.SetValue(SF.NBMinY.Value);
                MaxHeight.SetValue(SF.NBMaxY.Value - SF.NBMinY.Value);
            };
            SF.NBMaxY.ValueChanged += (object o, EventArgs e) => { MaxHeight.SetValue(SF.NBMaxY.Value - SF.NBMinY.Value); };
            SF.NBProjectionWidth.ValueChanged += (object o, EventArgs e) =>
            {
                MinX.SetValue(-SF.NBProjectionWidth.Value / 2f);
                MaxWidth.SetValue(SF.NBProjectionWidth.Value);
            };
            SF.NBReprojectionRotationX.ValueChanged += (object o, EventArgs e) => { RotationX = SF.NBReprojectionRotationX.Value; };
            SF.NBReprojectionRotationZ.ValueChanged += (object o, EventArgs e) => { RotationZ = SF.NBReprojectionRotationZ.Value; };
            SF.NBReprojectionTranslationX.ValueChanged += (object o, EventArgs e) => { TranslationX = SF.NBReprojectionTranslationX.Value; };
            SF.NBReprojectionTranslationY.ValueChanged += (object o, EventArgs e) => { TranslationY = SF.NBReprojectionTranslationY.Value; };
            SF.NBReprojectionTranslationZ.ValueChanged += (object o, EventArgs e) => { TranslationZ = SF.NBReprojectionTranslationZ.Value; };
            SF.NBMaxClipRadius.ValueChanged += (object o, EventArgs e) => { MaxClipRadius.SetValue(SF.NBMaxClipRadius.Value); };
            SF.NBMinClipRadius.ValueChanged += (object o, EventArgs e) => { MinClipRadius.SetValue(SF.NBMinClipRadius.Value); };
            SF.NBFusionSpacing.ValueChanged += (object o, EventArgs e) => { FusionSpacing = SF.NBFusionSpacing.Int32Value; };
            SF.CBClip.CheckedChanged += (object o, EventArgs e) => { ClipOn.SetValue(SF.CBClip.Checked); };
            SF.NBMinAvgCount.ValueChanged += (object o, EventArgs e) => { MinAvgCount.SetValue(SF.NBMinAvgCount.Value - 0.1f); };
            SF.NBDepthAveragingLimit.ValueChanged += (object o, EventArgs e) => { DepthAvgLimit.SetValue(SF.NBDepthAveragingLimit.Value); };
            SF.NBMinClipY.ValueChanged += (object o, EventArgs e) => { MinClipY.SetValue(SF.NBMinClipY.Value); };

            DepthCacheSize = SF.NBAveragedFrames.Int32Value;
            if (DepthCacheSize % 2 == 1) DepthCacheSize++;
            DepthMainItem = DepthCacheSize / 2;
            SetBuildModel(ModuleSettings.Default.BuildModel);
            SetModellingMode((ModellingModes)ModuleSettings.Default.ModellingMode);
        }

        void SF_SettingsLoaded(object sender, EventArgs e)
        {
            RotationX = SF.NBReprojectionRotationX.Value;
            RotationZ = SF.NBReprojectionRotationZ.Value;
            TranslationX = SF.NBReprojectionTranslationX.Value;
            TranslationY = SF.NBReprojectionTranslationY.Value;
            TranslationZ = SF.NBReprojectionTranslationZ.Value;
            DepthZLimit.SetValue(SF.NBZLimit.Value);
            MinX.SetValue(-SF.NBProjectionWidth.Value / 2f);
            MaxWidth.SetValue(SF.NBProjectionWidth.Value);
            MinY.SetValue(SF.NBMinY.Value);
            MaxHeight.SetValue(SF.NBMaxY.Value - SF.NBMinY.Value);
            MaxClipRadius.SetValue(SF.NBMaxClipRadius.Value);
            MinClipRadius.SetValue(SF.NBMinClipRadius.Value);
            FusionSpacing = SF.NBFusionSpacing.Int32Value;
            ClipOn.SetValue(SF.CBClip.Checked);
            MinAvgCount.SetValue(SF.NBMinAvgCount.Value - 0.1f);
            DepthAvgLimit.SetValue(SF.NBDepthAveragingLimit.Value);
            MinClipY.SetValue(SF.NBMinClipY.Value);
        }

        public void Exit()
        {
            CloseProject();
            ModuleSettings.Default.BuildModel = BuildModel;
            if (!string.IsNullOrEmpty(WorkingDirectory)) ModuleSettings.Default.WorkingDirectory = WorkingDirectory;
            ModuleSettings.Default.ModellingMode = (byte)ModellingMode;
            ModuleSettings.Default.Save();

            SF.SaveSettings();
        }

        private void CreateXDevice()
        {
            ClientSize = new System.Drawing.Size(640, 480);
            XDevice = new GraphicsDevice(
                    GraphicsAdapter.DefaultAdapter,
                    GraphicsProfile.HiDef,
                    new PresentationParameters()
                    {
                        IsFullScreen = false,
                        DeviceWindowHandle = Handle,
                        PresentationInterval = PresentInterval.Immediate,
                        BackBufferWidth = 640,
                        BackBufferHeight = 480,
                        DepthStencilFormat=DepthFormat.Depth16,
                        RenderTargetUsage=RenderTargetUsage.PreserveContents
                    });
            XDevice.RasterizerState = RasterizerState.CullNone;
            XDevice.DepthStencilState = DepthStencilState.Default;
            InitGraphicsDeviceService();
            XContent = new ContentManager(this);
            XEffect = XContent.Load<Effect>("Content/RotatingScannerEffects");
            TRawDepth = new Texture2D(XDevice, Context.DepthWidth, Context.DepthHeight, false, SurfaceFormat.Bgra4444);
            TDepth = new Texture2D(XDevice, Context.DepthWidth, Context.DepthHeight, false, SurfaceFormat.Single);
            TDepthCorrection = new Texture2D(XDevice, Context.DepthWidth, Context.DepthHeight, false, SurfaceFormat.Vector2);
            TDepthCorrection.SetData<float>(Context.DepthCorrection);
            SingleTargetA = new RenderTarget2D(XDevice, Context.DepthWidth, Context.DepthHeight, false, SurfaceFormat.Single, DepthFormat.Depth16);
            SingleTargetB = new RenderTarget2D(XDevice, Context.DepthWidth, Context.DepthHeight, false, SurfaceFormat.Single, DepthFormat.Depth16);
            Vector2TargetA = new RenderTarget2D(XDevice, Context.DepthWidth, Context.DepthHeight, false, SurfaceFormat.Vector2, DepthFormat.Depth16);
            Vector2TargetB = new RenderTarget2D(XDevice, Context.DepthWidth, Context.DepthHeight, false, SurfaceFormat.Vector2, DepthFormat.Depth16);            
            Vector4Target = new RenderTarget2D(XDevice, Context.DepthWidth, Context.DepthHeight, false, SurfaceFormat.Vector4, DepthFormat.Depth16);
            LinesVertexBuffer = new VertexBuffer(XDevice, VertexPositionColor.VertexDeclaration, DisplayLines, BufferUsage.WriteOnly);
            ExCoreTargetA = new RenderTarget2D(XDevice, Context.DepthWidth, Context.DepthHeight, false, SurfaceFormat.Vector2, DepthFormat.None);
            ExCoreTargetB = new RenderTarget2D(XDevice, Context.DepthWidth, Context.DepthHeight, false, SurfaceFormat.Vector2, DepthFormat.None); 
            
            DepthAntiDistortTechnique = XEffect.Techniques["DepthAntiDistort"];
            PerspectiveReprojectionTechnique = XEffect.Techniques["PerspectiveReprojection"];
            FusionDisplayTechnique = XEffect.Techniques["FusionDisplay"];
            FusionOutputTechnique = XEffect.Techniques["FusionOutput"];
            SumTechnique = XEffect.Techniques["Sum"];
            AvgTechnique = XEffect.Techniques["Avg"];
            FusionLinesTechnique = XEffect.Techniques["FusionLines"];
            VGaussTechnique = XEffect.Techniques["DepthVGauss"];
            HGaussTechnique = XEffect.Techniques["DepthHGauss"];
            CalibrationTechnique = XEffect.Techniques["Calibration"];
            DualTechnique = XEffect.Techniques["Dual"];
            ColoredDepthTechnique = XEffect.Techniques["ColoredDepth"];
            SimpleTechnique = XEffect.Techniques["Simple"];
            DepthVisualizationTechnique = XEffect.Techniques["DepthVisualization"];
            SignedDepthVisualizationTechnique = XEffect.Techniques["SignedDepthVisualization"];
            PolarDisplayTechnique = XEffect.Techniques["PolarDisplay"];
            ColorModelTechnique = XEffect.Techniques["ColorModel"];
            PolarAddTechnique = XEffect.Techniques["PolarAdd"];
            PolarAvgTechnique = XEffect.Techniques["PolarAvg"];
            PolarOutputTechnique = XEffect.Techniques["PolarOutput"];
            PolarAvgDisplayTechnique = XEffect.Techniques["PolarAvgDisplay"];

            DepthSampler = XEffect.Parameters["DepthTexture"];
            DepthCorrectionSampler = XEffect.Parameters["DepthCorrectionTexture"];
            TriangleRemoveLimit = XEffect.Parameters["TriangleRemoveLimit"];
            DepthZLimit = XEffect.Parameters["DepthLimitZ"];
            DepthToColorTransform = XEffect.Parameters["DepthToColor"];
            ReprojectionTransform = XEffect.Parameters["ReprojectionTransform"];
            SpaceTransform = XEffect.Parameters["SpaceTransform"];
            DepthSamplerA = XEffect.Parameters["DepthTextureA"];
            DepthSamplerB = XEffect.Parameters["DepthTextureB"];
            MinX = XEffect.Parameters["ProjXMin"];
            MinY = XEffect.Parameters["ProjYMin"];
            MaxWidth = XEffect.Parameters["ProjWidthMax"];
            MaxHeight = XEffect.Parameters["ProjHeightMax"];
            SplitPlaneVector = XEffect.Parameters["SplitPlaneVector"];
            FusionTranform = XEffect.Parameters["FusionTranform"];
            SegmentLength = XEffect.Parameters["SegmentLength"];
            FusionTranform = XEffect.Parameters["FusionTransform"];
            LinesTransform = XEffect.Parameters["LinesTransform"];
            ShadingColor = XEffect.Parameters["ShadingColor"];
            MaxClipRadius = XEffect.Parameters["ClipRadiusMax"];
            MinClipRadius = XEffect.Parameters["ClipRadiusMin"];
            ClipOn = XEffect.Parameters["ClipOn"];
            MinAvgCount = XEffect.Parameters["DepthAvgCountMin"];
            DepthAvgLimit = XEffect.Parameters["DepthAvgLimit"];
            MinClipY = XEffect.Parameters["ClipYMin"];
            SideSelector = XEffect.Parameters["SideSelector"];
            MainSampler = XEffect.Parameters["MainTexture"];
            ModelTransform = XEffect.Parameters["ModelTransform"];
            CorePos = XEffect.Parameters["CorePos"];
            
            XEffect.Parameters["DepthCorrectionTextureSize"].SetValue(new Vector2(Context.DepthWidth, Context.DepthHeight));
            XEffect.Parameters["DepthInverseIntrinsics"].SetValue(Context.DepthInverseIntrinsics);
            XEffect.Parameters["NaN"].SetValue(float.NaN);
            XEffect.SetGaussCoeffs(Context.DepthWidth, Context.DepthHeight, 9, 1, 1);

            MiniPlane.SetDevice(XDevice);
            MaxiPlane.SetDevice(XDevice);
            TriangleRemoveLimit.SetValue(Context.TriangleRemoveLimit);
        }

        private void SetReprojection(float rotation)
        {
            Matrix T = Matrix.Transpose(Matrix.CreateTranslation(0, 0, -TranslationZ));
            Matrix T2 = Matrix.Transpose(Matrix.CreateTranslation(TranslationX, TranslationY, 0));
            Matrix Ti = Matrix.Invert(T);
            Matrix R = Matrix.CreateRotationY(rotation)*Matrix.CreateRotationZ(RotationZ / 180 * MathHelper.Pi) *
                    Matrix.CreateRotationX(RotationX / 180 * MathHelper.Pi);
            Matrix reproj = Context.DepthIntrinsics * Ti * R * T * T2 * Context.DepthInverseIntrinsics;
            ReprojectionTransform.SetValue(reproj); //->main
            Matrix space = R * T * T2 * Context.DepthInverseIntrinsics;
            SpaceTransform.SetValue(space); //->clip
        }

        private void SetFusionReprojection(float rotation, float fusionRotation)
        {
            Matrix T = Matrix.Transpose(Matrix.CreateTranslation(0, 0, -TranslationZ*debugValue));
            Matrix Ti = Matrix.Invert(T);
            Matrix R = Matrix.CreateRotationY(rotation);
            Matrix space = R * T * Context.DepthInverseIntrinsics;
            Matrix reproj = Context.DepthIntrinsics * Ti * R * T * Context.DepthInverseIntrinsics;
            ReprojectionTransform.SetValue(reproj);
            SpaceTransform.SetValue(space);
            double a = SF.NBSplitPlane.DoubleValue / 180d * Math.PI;
            SplitPlaneVector.SetValue(new Vector4((float)Math.Cos(a), 0f, (float)Math.Sin(a), 0f));
            FusionTranform.SetValue(Matrix.CreateRotationY(fusionRotation) * space);
        }

        private void SetExCoreReprojection(float rotation)
        {
            Matrix T = Matrix.Transpose(Matrix.CreateTranslation(0, 0, -TranslationZ));
            Matrix Ti = Matrix.Invert(T);
            Matrix R = Matrix.CreateRotationY(rotation);
            Matrix space = R * T * Context.DepthInverseIntrinsics;
            Matrix reproj = Context.DepthIntrinsics * Ti * R * T * Context.DepthInverseIntrinsics;
            ReprojectionTransform.SetValue(reproj);
            SpaceTransform.SetValue(space);
            SplitPlaneVector.SetValue(new Vector4(0f, 0f, 1f, 0f));
            FusionTranform.SetValue(Matrix.CreateRotationY((-SF.NBSplitPlane.Value+90f) / 180f * MathHelper.Pi) * space);
        }

        private void SetModelProjection(float rotation)
        {
            Matrix T = Matrix.Transpose(Matrix.CreateTranslation(0, 0, TranslationZ));
            Matrix R = Matrix.CreateRotationY(rotation);
            Matrix reproj = Context.DepthIntrinsics * T * R;
            ModelTransform.SetValue(reproj);
        }

        private void SetCalibrationReprojection(float rotation, float fusionRotation)
        {
            Matrix T = Matrix.Transpose(Matrix.CreateTranslation(0, 0, -TranslationZ));
            Matrix T2 = Matrix.Transpose(Matrix.CreateTranslation(TranslationX, TranslationY, 0));
            Matrix Ti = Matrix.Invert(T);
            Matrix R = Matrix.CreateRotationY(rotation) *
                Matrix.CreateRotationZ(RotationZ / 180 * MathHelper.Pi) *
                Matrix.CreateRotationX(RotationX / 180 * MathHelper.Pi);
            Matrix reproj = Context.DepthIntrinsics * Ti * R * T * T2 * Context.DepthInverseIntrinsics;
            Matrix space = Matrix.CreateRotationX(RotateX) * Matrix.CreateRotationY(RotateY) * R * T * T2 * Context.DepthInverseIntrinsics;
            ReprojectionTransform.SetValue(reproj);
            SpaceTransform.SetValue(space);
            FusionTranform.SetValue(Matrix.CreateRotationY(fusionRotation) * space);
        }

        int FrameID = 0;
        int FusionSpacing = 5;
        bool FusionTick = false;
        float FusionRotation;
        int LastDepthIndex;
        public void LoadDepth(byte[] data, float rotation)
        {
            LastDepthIndex = DepthIndex;
            DepthCache[DepthIndex].Data = data;
            DepthCache[DepthIndex].Rotation = rotation;
            DepthIndex++;
            if (DepthIndex == DepthCacheSize) DepthIndex = 0;
            if (DepthCacheUsed < DepthCacheSize) DepthCacheUsed++;
            switch (State)
            {
                case States.None:
                    ProcessFrame();
                    break;
                case States.Calibration:
                    Calibration();
                    break;
            }            
        }

        void Calibration()
        {
            if (DepthCacheUsed >= DepthCacheSize)
            {
                //TSBCalibrationSetA.Enabled = TSBCalibrationSetB.Enabled = true;
                if (ImageA.Data == null || ImageB.Data == null)
                {
                    DrawDepthTo(DepthCache[LastDepthIndex].Data, DepthCache[LastDepthIndex].Rotation, null, CalibrationTechnique, true);
                    XDevice.Present();
                }
                else
                {
                    //DrawDepthTo(ImageA.Data, ImageA.Rotation, SingleTargetA);
                    //DrawDepthTo(ImageB.Data, ImageB.Rotation, SingleTargetB);
                    //XDevice.SetRenderTarget(null);
                    //DepthSamplerA.SetValue(SingleTargetA);
                    //DepthSamplerB.SetValue(SingleTargetB);
                    //XEffect.CurrentTechnique = DualTechnique;
                    //XEffect.CurrentTechnique.Passes[0].Apply();
                    //MiniPlane.Draw();
                    //XDevice.Present();

                    ShadingColor.SetValue(new Vector4(1, 0, 0, 1));
                    DrawDepthTo(ImageA.Data, 0f, null, ColoredDepthTechnique, true);
                    ShadingColor.SetValue(new Vector4(0, 1, 0, 1));
                    DrawDepthTo(ImageB.Data, (ImageB.Rotation-ImageA.Rotation), null, ColoredDepthTechnique, false);
                    XDevice.Present();
                }
            }
        }

        void DrawDepthTo(byte[] data, float rotation, RenderTarget2D target, EffectTechnique technique, bool clear)
        {
            for (int i = 0; i < 16; i++) XDevice.Textures[i] = null;
            TRawDepth.SetData<byte>(data);
            XDevice.BlendState = BlendState.Opaque;
            XDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            XDevice.DepthStencilState = DepthStencilState.Default;

            XDevice.SetRenderTarget(Vector4Target);
            XDevice.Clear(Color.Black);
            XDevice.RasterizerState = RasterizerState.CullNone;
            XDevice.BlendState = BlendState.Opaque;
            XDevice.DepthStencilState = DepthStencilState.Default;

            DepthSampler.SetValue(TRawDepth);
            DepthCorrectionSampler.SetValue(TDepthCorrection);
            XEffect.CurrentTechnique = DepthAntiDistortTechnique;
            XEffect.CurrentTechnique.Passes[0].Apply();
            MiniPlane.Draw();

            XEffect.CurrentTechnique = technique;
            XDevice.SetRenderTarget(target);
            if (clear) XDevice.Clear(Color.White);
            SetCalibrationReprojection(rotation, 0f);
            DepthSampler.SetValue(Vector4Target);
            XEffect.CurrentTechnique.Passes[0].Apply();
            MaxiPlane.Draw();            
        }
#if DEBUG
        int PF = 0;
#endif
        private void Visualize(Texture texture, EffectTechnique technique, bool present = true)
        {
            XDevice.SetRenderTarget(null);
            XDevice.Clear(Color.White);
            MainSampler.SetValue(texture);
            XEffect.CurrentTechnique = technique;
            XEffect.CurrentTechnique.Passes[0].Apply();
            MiniPlane.Draw();
            if(present) XDevice.Present();
        }

        public void ProcessFrame()
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
                    float mainRotation = DepthCache[mainIndex].Rotation;
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

                    if (ViewMode == Views.Gauss)
                    {
                        XDevice.SetRenderTarget(null);
                        Visualize(FusionTick ? SingleTargetA : SingleTargetB, SignedDepthVisualizationTechnique, false);
                        VertexPositionColor[] VPC = new VertexPositionColor[10];
                        VPC[0] = new VertexPositionColor(new Vector3(0, -0.1f,0), Color.Yellow);
                        VPC[1] = new VertexPositionColor(new Vector3(0, 0.1f, 0), Color.Yellow);
                        VPC[2] = new VertexPositionColor(new Vector3(0.1f, 0f, 0), Color.Yellow);
                        VPC[3] = new VertexPositionColor(new Vector3(-0.1f, 0f, 0), Color.Yellow);
                        VPC[4] = new VertexPositionColor(new Vector3(0, 0f, 0.1f), Color.Yellow);
                        VPC[5] = new VertexPositionColor(new Vector3(0, 0f, -0.1f), Color.Yellow);
                        VPC[6] = new VertexPositionColor(new Vector3(-SF.NBCoreY.Value, -1, SF.NBCoreX.Value), Color.Green);
                        VPC[7] = new VertexPositionColor(new Vector3(-SF.NBCoreY.Value, 1, SF.NBCoreX.Value), Color.Green);
                        VPC[8] = new VertexPositionColor(new Vector3(-SF.NBCoreY.Value, -1, -SF.NBCoreX.Value), Color.Red);
                        VPC[9] = new VertexPositionColor(new Vector3(-SF.NBCoreY.Value, 1, -SF.NBCoreX.Value), Color.Red);
                        SetModelProjection(-mainRotation + (SF.NBSplitPlane.Value+90f) / 180f * MathHelper.Pi);
                        LinesVertexBuffer.SetData<VertexPositionColor>(VPC);
                        XDevice.SetVertexBuffer(LinesVertexBuffer);
                        XEffect.CurrentTechnique = ColorModelTechnique;
                        XEffect.CurrentTechnique.Passes[0].Apply();
                        XDevice.DrawPrimitives(PrimitiveType.LineList, 0, 5);
                        XDevice.Present();
                    }

                    if (ModellingMode==ModellingModes.ExCore)
                    {
                        XDevice.SetRenderTarget(FusionTick ? SingleTargetB : SingleTargetA);
                        XDevice.Clear(Color.Black);
                        DepthSampler.SetValue(FusionTick ? SingleTargetA : SingleTargetB);
                        XDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                        CorePos.SetValue(new Vector3(SF.NBCoreY.Value, 0, -SF.NBCoreX.Value));
                        SetExCoreReprojection(mainRotation);
                        XEffect.CurrentTechnique = PolarOutputTechnique;
                        XEffect.CurrentTechnique.Passes[0].Apply();
                        MaxiPlane.Draw();

                        if (ViewMode == Views.ExCoreReprojection)
                        {
                            XDevice.SetRenderTarget(null);
                            XDevice.Clear(Color.White);
                            XEffect.CurrentTechnique = PolarDisplayTechnique;
                            XEffect.CurrentTechnique.Passes[0].Apply();
                            MaxiPlane.Draw();
                            XDevice.Present();
                        }

                        XDevice.SetRenderTarget(ExCoreTick ? ExCoreTargetA : ExCoreTargetB);
                        if (NextExCoreClear)
                        {
                            XDevice.Clear(ClearOptions.Target, Color.Black, 1, 0);
                            NextExCoreClear = false;
                        }
                        else
                        {
                            DepthSamplerA.SetValue(FusionTick ? SingleTargetB : SingleTargetA);
                            DepthSamplerB.SetValue(ExCoreTick ? ExCoreTargetB : ExCoreTargetA);
                            XEffect.CurrentTechnique = PolarAddTechnique;
                            XEffect.CurrentTechnique.Passes[0].Apply();
                            MiniPlane.Draw();
                        }
                        if (ViewMode == Views.ExCorePolarDepth)
                        {
                            XDevice.SetRenderTarget(null);
                            XDevice.Clear(Color.White);
                            DepthSampler.SetValue(ExCoreTick ? ExCoreTargetA : ExCoreTargetB);
                            XEffect.CurrentTechnique = PolarAvgDisplayTechnique;
                            XEffect.CurrentTechnique.Passes[0].Apply();
                            MiniPlane.Draw();
                            XDevice.Present();
                        }

                        if(ExCoreSaveTo!=null)
                        {

                            XDevice.SetRenderTarget(FusionTick ? SingleTargetB : SingleTargetA);
                            XDevice.Clear(Color.Black);
                            DepthSampler.SetValue(ExCoreTick ? ExCoreTargetA : ExCoreTargetB);
                            XEffect.CurrentTechnique = PolarAvgTechnique;
                            XEffect.CurrentTechnique.Passes[0].Apply();
                            MiniPlane.Draw();

                            for (int i = 0; i < 16; i++)
                            {
                                XDevice.SetRenderTarget(FusionTick ? SingleTargetA : SingleTargetB);
                                DepthSampler.SetValue(FusionTick ? SingleTargetB : SingleTargetA);
                                XEffect.CurrentTechnique = VGaussTechnique;
                                XEffect.CurrentTechnique.Passes[0].Apply();
                                MiniPlane.Draw();
                                FusionTick = !FusionTick;

                                XDevice.SetRenderTarget(FusionTick ? SingleTargetA : SingleTargetB);
                                DepthSampler.SetValue(FusionTick ? SingleTargetB : SingleTargetA);
                                XEffect.CurrentTechnique = HGaussTechnique;
                                XEffect.CurrentTechnique.Passes[0].Apply();
                                MiniPlane.Draw();
                                FusionTick = !FusionTick;
                            }

                            XDevice.SetRenderTarget(null);
                            STLScreenshot(FusionTick ? SingleTargetB : SingleTargetA,ExCoreSaveTo, "Created by KinectScan.", MinY.GetValueSingle(), MaxHeight.GetValueSingle());
                            SSStatus.Text = "Model saved.";
                            ExCoreSaveTo = null;
                        }
                        ExCoreTick = !ExCoreTick;
                    }

                    if (FrameID != 0)
                    {
                        XDevice.BlendState = BlendState.Opaque;
                        XDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                        XDevice.DepthStencilState = DepthStencilState.Default;



                        //XDevice.SetRenderTarget(DepthTargetA);
                        //XDevice.Clear(Color.Black);
                        //SetFusionReprojection(-mainRotation + FusionRotation, mainRotation);

                        //DepthSampler.SetValue(FusionTick ? FusionDepthTargetB : FusionDepthTargetA);
                        //XEffect.CurrentTechnique.Passes[0].Apply();
                        //MaxiPlane.Draw();

                        //XDevice.SetRenderTarget(DepthTargetB);

                        XEffect.CurrentTechnique = FusionOutputTechnique;
                        XDevice.SetRenderTarget(Vector4Target);
                        XDevice.Clear(Color.Black);
                        SetFusionReprojection(0f, mainRotation);
                        DepthSampler.SetValue(FusionTick ? SingleTargetA : SingleTargetB);
                        XEffect.CurrentTechnique.Passes[0].Apply();
                        MaxiPlane.Draw();

                        if (ViewMode == Views.Fusion)
                        {
                            XEffect.CurrentTechnique = FusionDisplayTechnique;
                            XDevice.SetRenderTarget(null);
                            XDevice.Clear(Color.Black);
                            XEffect.CurrentTechnique.Passes[0].Apply();
                            MaxiPlane.Draw();
                            XDevice.Present();
                        }


                        //XEffect.CurrentTechnique = FusionDisplayTechnique;
                        //XDevice.SetRenderTarget(null);
                        //XDevice.Clear(Color.Black);
                        //SetFusionReprojection(0f, mainRotation);
                        //DepthSampler.SetValue(FusionTick ? FusionDepthTargetA : FusionDepthTargetB);
                        //XEffect.CurrentTechnique.Passes[0].Apply();
                        //MaxiPlane.Draw();


                        if (!MM.FirstFrame)
                        {
                            LinesTransform.SetValue(Matrix.CreateRotationY(-mainRotation));
                            XDevice.SetRenderTarget(SingleTargetA);
                            XEffect.CurrentTechnique = FusionLinesTechnique;
                            MM.DrawModel(XDevice, SegmentLength, XEffect.CurrentTechnique.Passes[0]);


                            //LinesTransform.SetValue(Matrix.CreateRotationX(0) * Matrix.CreateRotationY(-mainRotation));
                            //XDevice.SetRenderTarget(null);
                            //XEffect.CurrentTechnique = XEffect.Techniques["FusionLinesDisplay"];
                            //MM.DrawModel(XDevice, SegmentLength, XEffect.CurrentTechnique.Passes[0]);
                            //XDevice.Present();

                        }
                        XDevice.SetRenderTarget(null);
                        Vector3 direction = Vector3.Transform(Vector3.Backward, Matrix.CreateRotationY(-mainRotation));
                        if (BuildModel)
                        {
                            MM.ProcessFrame(Vector4Target, SingleTargetA, MinY.GetValueSingle(), MaxHeight.GetValueSingle(), true, direction);
                            MM.ProcessFrame(Vector4Target, SingleTargetA, MinY.GetValueSingle(), MaxHeight.GetValueSingle(), false, direction);
                        }
                        //MM.SaveToSTLFile("x", "dd");
                        //Debugger.Break();

                        //XEffect.Parameters["VideoTexture"].SetValue(DepthTargetA);
                        //XEffect.CurrentTechnique = XEffect.Techniques["Simple"];
                        //XEffect.CurrentTechnique.Passes[0].Apply();
                        //MiniPlane.Draw();

                        //DepthSamplerA.SetValue(DepthTargetA);
                        //DepthSamplerB.SetValue(DepthTargetB);
                        //XEffect.CurrentTechnique = XEffect.Techniques["DepthDiff"];
                        //XEffect.CurrentTechnique.Passes[0].Apply();
                        //MiniPlane.Draw();

                    }

                    FusionRotation = mainRotation;
                    //FusionTick = !FusionTick;
#if DEBUG
                    PF++;
                    //if ((DebugFrame < 0 && PF > DebugMinFrame && PF < DebugMaxFrame) || PF == DebugFrame)
                    //{
                    //    MM.SaveToSTLFile("d" + PF.ToString(), "debug");
                    //    MM.Triangulate2(false);
                    //    if (DebugFrame == PF) Close();
                    //}
#endif
                }

                //Display
                //XDevice.SetRenderTarget(null);
                //DepthSampler.SetValue(tick ? DepthTargetB : DepthTargetA);
                //XEffect.CurrentTechnique = RotationAvgTechnique;
                //XEffect.CurrentTechnique.Passes[0].Apply();
                //MiniPlane.Draw();


                FrameID++;


            }
        }

        int SaveBufferSize = 128;
        public unsafe IOEventArgs STLScreenshot(RenderTarget2D target, string path, string title, float minH, float maxHeight)
        {
            //try
            {
                int width = target.Width;
                int height = target.Height;

                FromPolarSettings FPS;
                FPS.width=width;
                FPS.height=height;
                FPS.minH=minH;
                FPS.stepH=maxHeight / Height;
                FPS.stepFi= MathHelper.TwoPi / width;
                float[] heightmap = new float[width * height];
                target.GetData<float>(heightmap);

                string header;
                while (title.StartsWith("solid", StringComparison.OrdinalIgnoreCase)) title = title.Remove(0, 5);
                if (title.Length > 80) header = title.Substring(0, 79);
                else header = title.PadRight(79);

                int i = 0, e = 0;
                long lenPos;
                byte[] buffer = new byte[SaveBufferSize * 100];
                
                using (FileStream FS = new FileStream(path, FileMode.Create, FileAccess.Write))
                using (BinaryWriter BW = new BinaryWriter(FS))
                {
                    BW.Write(header);
                    lenPos = FS.Position;
                    BW.Write(0u);

                    fixed (float* sHeight = &heightmap[0])
                    fixed (byte* sBuffer = &buffer[0])
                    {
                        FPS.sv = sHeight;
                        float* pHeight = sHeight, lHeight, eHeight = sHeight + (height - 1) * width, firstAHeight, firstBHeight, lastAHeight, lastBHeight;
                        Vector3 A, B, C;
                        byte* pBuffer = sBuffer;
                        byte* eBuffer = sBuffer + buffer.Length - 50;
                        float h;
                        for (int j = 0; j < height - 1; j++)
                        {
                            lHeight = pHeight + width;
                            firstAHeight = firstBHeight = lastAHeight = lastBHeight = null;
                            while (pHeight <= lHeight)
                            {
                                if (pHeight < lHeight)
                                {
                                    if (firstAHeight == null && *pHeight > 0) firstAHeight = pHeight;
                                    if (firstBHeight == null && *(pHeight + width) > 0) firstBHeight = pHeight + width;
                                    if (*pHeight > 0) lastAHeight = pHeight;
                                    if (*(pHeight + width) > 0) lastBHeight = pHeight + width;
                                    A = FromPolar(FPS, pHeight);
                                    B = FromPolar(FPS, pHeight + 1);
                                    C = FromPolar(FPS, pHeight + width);
                                    i += CheckAndWriteTriangle(A, B, C, ref pBuffer);
                                    C = FromPolar(FPS, pHeight + 1);
                                    B = FromPolar(FPS, pHeight + width);
                                    A = FromPolar(FPS, pHeight + width + 1);
                                    i += CheckAndWriteTriangle(A, B, C, ref pBuffer);
                                }
                                else if(firstAHeight!=null && firstBHeight!=null && lastAHeight!=null && lastBHeight!=null && lastAHeight-firstAHeight>width*0.97f && lastBHeight-firstBHeight>width*0.97f)
                                {
                                    A = FromPolar(FPS, lastAHeight);
                                    B = FromPolar(FPS, firstAHeight);
                                    C = FromPolar(FPS, lastBHeight);
                                    i += CheckAndWriteTriangle(A, B, C, ref pBuffer);
                                    C = FromPolar(FPS, firstAHeight);
                                    B = FromPolar(FPS, lastBHeight);
                                    A = FromPolar(FPS, firstBHeight);
                                    i += CheckAndWriteTriangle(A, B, C, ref pBuffer);
                                }
                                pHeight++;
                                if (pBuffer >= eBuffer || pHeight == eHeight)
                                {
                                    BW.Write(buffer, 0, i * 50);
                                    pBuffer = sBuffer;
                                    e += i;
                                    i = 0;
                                }
                                if (pHeight > lHeight)
                                {
                                    pHeight--;
                                    break;
                                }
                            }
                        }
                    }
                    FS.Position = lenPos;
                    BW.Write((uint)e);
                    BW.Flush();

                    BW.Close();
                    FS.Close();
                }
                return new IOEventArgs(path, true, null);
            }
            //catch (Exception e)
            //{
            //    return new IOEventArgs(path, true, null);
            //}
        }

        unsafe struct FromPolarSettings
        {
            public float minH, stepH, stepFi;
            public int width, height;
            public float* sv;
        }

        unsafe Vector3 FromPolar(FromPolarSettings s, float* d)
        {
            if (*d == 0)
                return new Vector3(float.NaN);
            else
            {
                int i = (int)(d - s.sv);
                int u = i % s.width;
                int v = i / s.width;
                float fi = u * MathHelper.TwoPi / s.width;
                float h = s.minH + s.stepH * v;
                float r = *d;
                return new Vector3((float)Math.Cos(fi) * r, h,(float)Math.Sin(fi) * r);
            }
        }
        float ExportScale=1000f;
        public unsafe int CheckAndWriteTriangle(Vector3 A, Vector3 B, Vector3 C, ref byte* p)
        {
            Vector3 N;
            Vector3* pVector3 = (Vector3*)p;
            if (float.IsNaN(A.X) || float.IsNaN(B.X) || float.IsNaN(C.X))
            {
                return 0;
            }
            else
            {
                A.Y = -A.Y;
                B.Y = -B.Y;
                C.Y = -C.Y;
                N = Vector3.Cross(B - A, C - A);
                N.Normalize();
                *pVector3++ = N;
                *pVector3++ = A * ExportScale;
                *pVector3++ = B * ExportScale;
                *pVector3++ = C * ExportScale;
                p += 50;
                return 1;
            }
        }

        public class ModelMaker
        {
            struct Vertex
            {
                public Vector3 Position;
                public Vector2 Direction;
                public Vertex(Vector3 position, Vector2 direction)
                {
                    Position = position;
                    Direction = direction;
                }
                public Vertex(float x, float y, float z, Vector2 direction)
                {
                    Position = new Vector3(x, y, z);
                    Direction = direction;
                }
            }
            LinkedList<Vertex>[] ModelDataA, ModelDataB;
            Vector4[] LocalFrameData;
            float[] LocalModelData;
            int Width, Height;
            public bool FirstFrame { get; private set; }
            public ModelMaker(int width, int height)
            {
                ModelDataA = new LinkedList<Vertex>[height];
                ModelDataB = new LinkedList<Vertex>[height];
                for (int i = 0; i < height; i++)
                {
                    ModelDataA[i] = new LinkedList<Vertex>();
                    ModelDataB[i] = new LinkedList<Vertex>();
                }
                LocalFrameData = new Vector4[width * height];
                LocalModelData = new float[width * height];
                Width = width;
                Height = height;
                FirstFrame = true;
                Vertices = new VertexPosition[MaxVertices];
            }

            public void NewModel()
            {
                for (int i = 0; i < Height; i++)
                {
                    ModelDataA[i].Clear();
                    ModelDataB[i].Clear();
                }
                FirstFrame = true;
            }

            const int MaxSegmentWidth = 2;
            public unsafe void ProcessFrame(RenderTarget2D frame, RenderTarget2D model, float minY, float maxHeight, bool side, Vector3 direction)
            {
                Vector2 dir = new Vector2(-direction.Z, direction.X);
                dir.Normalize();
                frame.GetData<Vector4>(LocalFrameData);
                if (!FirstFrame)
                {
                    model.GetData<float>(LocalModelData);
                }
                bool dataFound;
                float lastData, stepY, Y, vModel;
                stepY = maxHeight / Height;
                int index, target, segmentWidth, nodesAdded;
                float W = side ? 0.5f : 2f;
                Vector4 vFrame;
                LinkedList<Vertex> Model;
                LinkedList<Vertex>[] ModelData = side ? ModelDataA : ModelDataB;
                LinkedListNode<Vertex> Node, TempStartNode, TempEndNode;
                fixed (Vector4* sFrame = &LocalFrameData[0])
                fixed (float* sModel = &LocalModelData[0])
                {
                    Vector4* pFrame, lFrame;
                    float* pModel;
                    for (int j = 0; j < Height; j++)
                    {
                        Y = minY + j * stepY;
                        pFrame = sFrame + j * Width;
                        pModel = sModel + j * Width;
                        lFrame = pFrame + Width;
                        Model = ModelData[j];
                        if (Model.Count==0)
                        {
                            while (pFrame < lFrame)
                            {
                                if (pFrame->Z != 0f)
                                {
                                    if (pFrame->W == W)
                                        Model.AddLast(new Vertex(pFrame->X, Y, pFrame->Z, dir));
                                }
                                pFrame++;
                                pModel++;
                            }
                        }
                        else
                        {
                            lastData = 0;
                            Node = null;
                            index = 0;
                            segmentWidth = 0;
                            while (pFrame < lFrame)
                            {
                                vFrame = *pFrame;
                                if (side)
                                {
                                    vModel = *pModel;
                                }
                                else
                                {
                                    vModel = -*pModel;
                                }


                                if (vModel > 0f && vModel > lastData)
                                {
                                    dataFound = true;
                                    target = (int)Math.Ceiling(vModel);
                                    if (target == index) segmentWidth++;
                                    else
                                    {
                                        if (segmentWidth > MaxSegmentWidth && vFrame.W == W && Node.Next != null && Node.Previous!=null)
                                        {
                                            TempStartNode = Node.Previous;
                                            TempEndNode = Node;
                                            Node = TempStartNode.Previous;
                                            Model.Remove(TempStartNode);
                                            Model.Remove(TempEndNode);
                                            pFrame -= segmentWidth;
                                            nodesAdded = 0;
                                            for (int x = 0; x < segmentWidth; x++)
                                            {
                                                if (pFrame->W == W)
                                                {
                                                    nodesAdded++;
                                                    if (Node == null)
                                                    {
                                                        Model.AddFirst(new Vertex(pFrame->X, Y, pFrame->Z, dir));
                                                        Node = Model.First;
                                                    }
                                                    else
                                                    {
                                                        Model.AddAfter(Node, new Vertex(pFrame->X, Y, pFrame->Z, dir));
                                                        Node = Node.Next;
                                                    }
                                                }
                                                pFrame++;
                                            }
                                            if (nodesAdded == 0)
                                            {
                                                if (Node == null)
                                                {
                                                    Model.AddFirst(TempEndNode);
                                                    Model.AddFirst(TempStartNode);
                                                }
                                                else
                                                {
                                                    Model.AddAfter(Node, TempStartNode);
                                                    Model.AddAfter(TempStartNode, TempEndNode);
                                                }
                                            }
                                            else index++;
                                        }
                                        segmentWidth = 0;
                                    }
                                    if (target < index || Node == null)
                                    {
                                        Node = Model.First;
                                        index = 0;
                                    }
                                    if (Node != null && target < Model.Count)
                                        while (index < target)
                                        {
                                            Node = Node.Next;
                                            index++;
                                        }
                                    lastData = vModel;
                                }
                                else dataFound = false;

                                if (vFrame.W == W && !dataFound)
                                {
                                    if (Model.Count == 0)
                                    {
                                        Model.AddLast(new Vertex(vFrame.X, Y, vFrame.Z, dir));
                                        Node = Model.Last;
                                    }
                                    else if (Node == null)
                                    {
                                        //Model.AddFirst(new Vertex(vFrame.X, Y, vFrame.Z, dir));
                                        //Node = Model.First;
                                    }
                                    else
                                    {
                                        if (Node.Next == null)
                                        {
                                            Model.AddLast(new Vertex(vFrame.X, Y, vFrame.Z, dir));
                                            Node = Model.Last;
                                        }
                                        else if (Node.Next.Next == null)
                                        {
                                            Node = Node.Next;
                                            Node.Value = new Vertex(vFrame.X, Y, vFrame.Z, dir);
                                            index++;
                                        }
                                    }
                                }

                                pFrame++;
                                pModel++;
                            }
                        }
                    }
                }
                FirstFrame = false;
            }

            const float MaxEdgeLength = 0.02f;
            const float MaxLongEdgeLength = 0.05f;
            const int MaxLongEdges = 13;
            const int refPointHistory = 5;
           /* public unsafe void Triangulate2(bool side, out Vector3[] vertices, out int[] indicies)
            {

                LinkedList<Vector3>[] model = side ? ModelDataA : ModelDataB;
                LinkedList<Vector3> slice, lastSlice = model[0];
                LinkedListNode<Vector3> node;
                //float len;
                //int divs;
                //Vector3 divStep;
                //for (int i = 0; i < model.Length; i++)
                //{
                //    slice = model[i];
                //    node = slice.First;
                //    while (node != null)
                //    {
                //        if (node.Next != null)
                //        {
                //            len = (node.Value - node.Next.Value).Length();
                //            if (len > MaxHEdgeLength)
                //            {
                //                divs = (int)Math.Ceiling(len / MaxHEdgeLength);
                //                divStep = (node.Next.Value - node.Value) / divs;
                //                for (int j = divs-1; j > 0; j--)
                //                {
                //                    slice.AddAfter(node, node.Value + j * divStep);
                //                }
                //            }
                //        }
                //        node = node.Next;
                //    }
                //}
                
                
                int vertexCount = model[0].Count, triangleCount = 0;
                
                bool lastSliceAdded = false;
                for (int i = 1; i < model.Length; i++)
                {
                    slice = model[i];
                    if (slice.Count > 0 && lastSlice.Count > 0)
                    {
                        if (!lastSliceAdded) vertexCount += slice.Count;
                        vertexCount += slice.Count;
                        triangleCount += lastSlice.Count + slice.Count+3;
                        lastSliceAdded = true;
                    }
                    else
                        lastSliceAdded = false;
                    lastSlice = slice;
                }
                Vector3 refPoint;
                lastSliceAdded = false;
                indicies = new int[triangleCount * 3];
                vertices = new Vector3[vertexCount];
                if (vertexCount == 0) return;
                lastSlice = model[0];
                
                float minLength, length, lenA, lenB;
                int n = 0, plusA, plusB, ic;
                int longEdges, refPointCount;
                bool lastA, lastB, noFound;
                Vector3[] refPointBuffer = new Vector3[refPointHistory];
                fixed (int* sIndicies = &indicies[0])
                fixed (Vector3* sVertices = &vertices[0])
                fixed(Vector3* sRefPoint=&refPointBuffer[0])
                {
                    int* pIndicies=sIndicies;
                    int* lastShortTriangle;
                    Vector3* pVertices, startVertexA = sVertices, startVertexB, lVertices = sVertices, vertexA, vertexB, minVertex, nextVertexA, nextVertexB, firstA, firstB, baseB, lastShortA, lastShortB, lastRefPoint, newRefPoint, endRefPoint;
                    endRefPoint = sRefPoint + refPointBuffer.Length;

                    for (int i = 1; i < model.Length; i++)
                    {

                        slice = model[i];
                        if (slice.Count > 1 && lastSlice.Count > 1)
                        {
                            //Add new vertices to vertex buffer
                            pVertices = lVertices;
                            if (!lastSliceAdded)
                            {
                                startVertexA = pVertices;
                                node = lastSlice.First;
                                while (node != null)
                                {
                                    *pVertices = node.Value;
                                    node = node.Next;
                                    pVertices++;
                                }
                            }

                            startVertexB = pVertices;
                            node = slice.First;
                            while (node != null)
                            {
                                *pVertices = node.Value;
                                node = node.Next;
                                pVertices++;
                            }
                            lVertices = pVertices;

                            //Search for shortest edge
                            vertexA = startVertexA;
                            vertexB = startVertexB;
                            minVertex = null;
                            minLength = float.PositiveInfinity;
                            while (vertexB < lVertices-1)
                            {
                                length = (*vertexA - *vertexB).Length();
                                if (length < minLength)
                                {
                                    minVertex = vertexB;
                                    minLength = length;
                                }
                                vertexB++;
                            }
                            vertexB = minVertex;

                            //Build index buffer
                            firstA = vertexA;
                            firstB = vertexB;
                            refPoint = (*vertexA + *vertexB) / 2f;
                            lastA = false;
                            lastB = false;
                            longEdges = 0;
                            lastShortA = null;
                            lastShortB = null;
                            noFound = false;
                            refPointCount = 1;
                            newRefPoint = sRefPoint+1;
                            lastRefPoint = sRefPoint;
                            *endRefPoint = refPoint;
                            ic = 0;
                            while (!noFound)
                            {
                                //ic++;
                                //if (ic > 1000) System.Diagnostics.Debugger.Break();
                                n += 3;
                                nextVertexA = vertexA + 1;
                                if (nextVertexA == startVertexB)
                                    break;
                                   // nextVertexA = startVertexA;
                                nextVertexB = vertexB + 1;
                                if (nextVertexB == lVertices) nextVertexB = startVertexB;

                                if (refPointCount > refPointHistory) lastRefPoint = newRefPoint;


                                if (pIndicies - sIndicies > triangleCount) break;
                                lenA = (*nextVertexA - *vertexB).Length();
                                lenB = (*nextVertexB - *vertexA).Length();
                                if ((lenA < MaxLongEdgeLength || lenB < MaxLongEdgeLength) && longEdges < MaxLongEdges)
                                {
                                    if (lenA > MaxEdgeLength && lenB > MaxEdgeLength)
                                    {
                                        longEdges++;
                                    }
                                    else
                                    {
                                        longEdges = 0;
                                        lastShortA = vertexA;
                                        lastShortB = vertexB;
                                    }
                                    *pIndicies++ = (int)(vertexA - sVertices);
                                    *pIndicies++ = (int)(vertexB - sVertices);
                                    if ((*nextVertexA - refPoint).Length() < (*nextVertexB - refPoint).Length())
                                    {
                                        *pIndicies++ = (int)(nextVertexA - sVertices);
                                        vertexA = nextVertexA;
                                        if (lastB)
                                        {
                                            refPoint = (*vertexA + *vertexB) / 2f;
                                        }
                                        lastA = true;
                                        lastB = false;
                                    }
                                    else
                                    {
                                        *pIndicies++ = (int)(nextVertexB - sVertices);
                                        vertexB = nextVertexB;
                                        if (lastA)
                                        {
                                            refPoint = (*vertexA + *vertexB) / 2f;
                                        }
                                        lastA = false;
                                        lastB = true;
                                    }

                                    refPointCount++;
                                    *newRefPoint++ = refPoint;
                                    if (newRefPoint == endRefPoint) newRefPoint = sRefPoint;
                                }
                                else
                                {
                                    if (longEdges >= MaxLongEdges)
                                    {
                                        if (lastShortA != null && lastShortB != null)
                                        {
                                            vertexA = lastShortA + 1;
                                            vertexB = lastShortB + 1;
                                            pIndicies--;
                                            for (int k = 0; k < longEdges * 3; k++)
                                            {
                                                *pIndicies-- = 0;
                                            }
                                            pIndicies++;
                                        }                                        
                                    }
                                    else
                                    {
                                        
                                        if (!lastA) vertexA = nextVertexA;
                                        if (!lastB) vertexB = nextVertexB;
                                    }
                                    longEdges = 0;
                                    baseB = vertexB;
                                    plusA = 0;
                                    if (vertexA > startVertexB || vertexB > lVertices) System.Diagnostics.Debugger.Break();
                                    while (vertexA < startVertexB)
                                    {
                                        nextVertexA = vertexA + 1;
                                        minLength = float.PositiveInfinity;
                                        vertexB = baseB;
                                        minVertex = null;
                                        nextVertexB = vertexB + 1;
                                        while (nextVertexB < lVertices)
                                        {
                                            length = (*vertexA - *vertexB).Length();
                                            if (length < minLength && ((*nextVertexB-*vertexA).Length()<minLength || (*nextVertexA-*vertexB).Length()<minLength))
                                            {
                                                minVertex = vertexB;
                                                minLength = length;
                                            }
                                            vertexB++;
                                            nextVertexB = vertexB + 1;
                                        }
                                        if (minLength < MaxEdgeLength)
                                        {
                                            vertexB = minVertex;
                                            refPoint = (*vertexA + *vertexB) / 2f;
                                            lastShortA = vertexA;
                                            lastShortB = vertexB;
                                            refPointCount = 1;
                                            newRefPoint = sRefPoint + 1;
                                            lastRefPoint = sRefPoint;
                                            *endRefPoint = refPoint;
                                            break;
                                        }

                                        vertexA++;
                                        plusA++;
                                        if (vertexA == startVertexB)
                                        {
                                            noFound = true;
                                        }
                                    }

                                }

                                
                            }
                                
                            //Finish iteration step
                            startVertexA = startVertexB;
                            lastSliceAdded = true;
                        }
                        else
                            lastSliceAdded = false;
                        lastSlice = slice;
                    }
                }
            }*/
            unsafe struct LineInfo
            {
                public Vector3* FirstA, LastA, FirstB, LastB, StartA, EndA, StartB, EndB;
            }
            static readonly float MaxReturnLengthSquared = (float)Math.Pow(0.02, 2d);
            const int MinLineLength = 40;
            const int SpikeRemoveLength = 20;



            unsafe static void PointArrayFromList(LinkedList<Vertex> list, out CurveFitting.Point[] arrayX, out CurveFitting.Point[] arrayZ)
            {
                arrayX = new CurveFitting.Point[list.Count + 1];
                arrayZ = new CurveFitting.Point[list.Count + 1];
                LinkedListNode<Vertex> node = list.First;
                float t = 0;
                fixed (CurveFitting.Point* sArrayX = &arrayX[0])
                fixed (CurveFitting.Point* sArrayZ = &arrayZ[0])
                {
                    CurveFitting.Point* pArrayX = sArrayX;
                    CurveFitting.Point* pArrayZ = sArrayZ;
                    while (node != null)
                    {
                        pArrayX->Y = node.Value.Position.X;
                        pArrayZ->Y = node.Value.Position.Z;
                        if (node!=list.First)
                        {
                            t += (node.Value.Position - node.Previous.Value.Position).Length();
                        }
                        pArrayX->X = t;
                        pArrayZ->X = t;
                        pArrayX++;
                        pArrayZ++;
                        node = node.Next;
                    }

                    pArrayX->Y = list.First.Value.Position.X;
                    pArrayZ->Y = list.First.Value.Position.Z;
                    t += (list.First.Value.Position - list.Last.Value.Position).Length();
                    pArrayX->X = t;
                    pArrayZ->X = t;
                }
            }

            public unsafe void Triangulate2(bool side)
            {
                LinkedList<Vertex>[] model = side ? ModelDataA : ModelDataB;
                LinkedList<Vertex> slice;
                CurveFitting.Point[] ax, az;
                double[] p;
                for (int i = 0; i < model.Length; i++)
                {
                    slice = model[i];
                    if (slice.Count > MinLineLength)
                    {
                        PointArrayFromList(slice, out ax, out az);
                        p = CurveFitting.PolynomialClosedFit(ax, 10, 3);
                        Clipboard.SetText(p.ToMathematicaFunction("u"));
                        p = CurveFitting.PolynomialClosedFit(az, 10, 3);
                        Clipboard.SetText(p.ToMathematicaFunction("v"));
                        Clipboard.SetText(ax[ax.Length-1].X.ToString(new CultureInfo("en-US")));
                        p = null;
                        
                    }
                }
            }

            const int SmoothVertexCount = 100;
            const int SmoothVertexDegree = 5;
            const int SmoothVertexJoinDegree = 1;
            unsafe void SmoothWriteVertices(LinkedList<Vertex> model, ref Vector3* pVertices, ref Vector2* pDirections, int degree, int joinDegree, int count)
            {
                CurveFitting.Point[] ax, az;
                double[] fx, fz;
                PointArrayFromList(model, out ax, out az);
                fx = CurveFitting.PolynomialClosedFit(ax, degree, joinDegree);
                fz = CurveFitting.PolynomialClosedFit(az, degree, joinDegree);
                double length = ax[ax.Length - 1].X;
                double step = length / count;
                double x, z, t, dx, dz;
                float y = model.First.Value.Position.Y;
                for (int i = 0; i < count; i++)
                {
                    t = i * step;
                    x = fx.SubstituteToPolynomial(t);
                    z = fz.SubstituteToPolynomial(t);
                    dx = fx.SubstituteToDerivatedPolynomial(t);
                    dz = fx.SubstituteToDerivatedPolynomial(t);
                    pVertices->X = (float)x;
                    pVertices->Y = y;
                    pVertices->Z = (float)z;
                    pDirections->X = (float)dx;
                    pDirections->Y = (float)dz;
                    pVertices++;
                    pDirections++;
                }

            }

            unsafe void SmoothWriteVertices(LinkedList<Vertex> model, ref Vector3* pVertices, int degree, int joinDegree, int count)
            {
                CurveFitting.Point[] ax, az;
                double[] fx, fz;
                PointArrayFromList(model, out ax, out az);
                fx = CurveFitting.PolynomialClosedFit(ax, degree, joinDegree);
                fz = CurveFitting.PolynomialClosedFit(az, degree, joinDegree);
                double length = ax[ax.Length - 1].X;
                double step = length / count;
                double x, z, t, dx, dz;
                float y = model.First.Value.Position.Y;
                for (int i = 0; i < count; i++)
                {
                    t = i * step;
                    x = fx.SubstituteToPolynomial(t);
                    z = fz.SubstituteToPolynomial(t);
                    dx = fx.SubstituteToDerivatedPolynomial(t);
                    dz = fx.SubstituteToDerivatedPolynomial(t);
                    pVertices->X = (float)x;
                    pVertices->Y = y;
                    pVertices->Z = (float)z;
                    pVertices++;
                }

            }


            const int VerticalSmoothing = 0;
            const int KeepEveryNVertex = 4;
            public unsafe int FastTriangulate(bool side, out Vector3[] vertices, out int[] indicies)
            {
                LinkedList<Vertex>[] model = side ? ModelDataA : ModelDataB;
                LinkedList<Vertex> slice, lastSlice = model[0];
                int sliceCount = 0, triangleCount = 0;
                bool lastSliceAdded = false;
                int kj;
                LinkedListNode<Vertex> vl;
                for (int i = 1; i < model.Length; i++)
                {
                    slice = model[i];
                    vl = slice.First;
                    kj = 1;
                    while (vl != slice.Last)
                    {
                        if (kj != KeepEveryNVertex)
                        {
                            slice.Remove(vl.Next);
                            kj++;
                        }
                        else
                        {
                            vl = vl.Next;
                            kj = 1;
                        }
                    }
                    if (slice.Count > MinLineLength) RemoveTail(slice);
                    if (slice.Count > MinLineLength && lastSlice.Count > MinLineLength)
                    {
                        if (!lastSliceAdded) sliceCount++;
                        sliceCount++;
                        triangleCount += SmoothVertexCount * 2;
                        lastSliceAdded = true;
                    }
                    else lastSliceAdded = false;
                    lastSlice = slice;
                }

                RemoveSpikes(model);

                vertices = new Vector3[sliceCount * SmoothVertexCount];
                indicies = new int[triangleCount * 3];
                lastSliceAdded = false;
                fixed (Vector3* sVertices = &vertices[0])
                fixed (int* sIndicies = &indicies[0])
                {
                    Vector3* pVertices = sVertices;
                    Vector3* startVertexA = null, startVertexB,lVertices, vertexA, vertexB, minVertex, nextVertexA, nextVertexB;
                    float minLength, length;
                    int* pIndicies = sIndicies;
                    int startSlice=-1, endSlice, slices;
                    Vector3*[] verticalVertices = new Vector3*[VerticalSmoothing];
                    Vector3*[] verticalStartVertices = new Vector3*[VerticalSmoothing];
                    Vector3*[] verticalEndVertices = new Vector3*[VerticalSmoothing];
                    int[] rotations = new int[model.Length];
                    for (int i = 1; i < model.Length; i++)
                    {
                        slice = model[i];
                        if (slice.Count > MinLineLength && lastSlice.Count > MinLineLength)
                        {
                            if (!lastSliceAdded)
                            {
                                startSlice = i - 1;
                                startVertexA = pVertices;
                                SmoothWriteVertices(model[i - 1], ref pVertices, SmoothVertexDegree, SmoothVertexJoinDegree, SmoothVertexCount);
                            }
                            startVertexB = pVertices;
                            SmoothWriteVertices(model[i], ref pVertices, SmoothVertexDegree, SmoothVertexJoinDegree, SmoothVertexCount);
                            lVertices = pVertices;


                            minVertex = null;
                            vertexA = startVertexA;
                            vertexB = null;
                            minLength = float.PositiveInfinity;
                            while (vertexA != startVertexB)
                            {
                                vertexB = startVertexB;
                                while (vertexB != lVertices)
                                {
                                    length = (*vertexA - *vertexB).Length();
                                    if (length < minLength)
                                    {
                                        minLength = length;
                                        minVertex = vertexB;
                                    }
                                    vertexB++;
                                }

                                if (minLength < MaxEdgeLength)
                                {
                                    vertexB = minVertex;
                                    break;
                                }
                                vertexA++;
                            }

                            rotations[i] = (int)((vertexB - startVertexB) - (vertexA - startVertexA));
                            for (int j = 0; j < SmoothVertexCount; j++)
                            {
                                nextVertexA = vertexA + 1;
                                nextVertexB = vertexB + 1;
                                if (nextVertexA == startVertexB) nextVertexA = startVertexA;
                                if (nextVertexB == lVertices) nextVertexB = startVertexB;
                                *pIndicies++ = (int)(vertexA - sVertices);
                                *pIndicies++ = (int)(vertexB - sVertices);
                                *pIndicies++ = (int)(nextVertexB - sVertices);

                                *pIndicies++ = (int)(vertexA - sVertices);
                                *pIndicies++ = (int)(nextVertexB - sVertices);
                                *pIndicies++ = (int)(nextVertexA - sVertices);
                                vertexA=nextVertexA;
                                vertexB=nextVertexB;
                                
                            }

                            startVertexA = startVertexB;
                            lastSliceAdded = true;
                        }
                        //else
                        //{
                        //    if (startSlice != -1)
                        //    {
                        //        endSlice = i;
                        //        slices = endSlice - startSlice;
                        //        int rotation = 0;
                        //        int d = VerticalSmoothing / 2, jdk;
                        //        Vector3* slicesStart = pVertices - slices * SmoothVertexCount;
                        //        for (int j = 0; j < slices; j++)
                        //        {
                        //            rotation = 0;
                        //            for (int k = 0; k < VerticalSmoothing; k++)
                        //            {
                        //                jdk = j - d + k;
                        //                if (jdk < 0 || jdk >= slices)
                        //                {
                        //                    verticalVertices[k] = null;
                        //                }
                        //                else
                        //                {
                        //                    verticalStartVertices[k] = slicesStart + jdk * SmoothVertexCount;
                        //                    verticalEndVertices[k] = verticalStartVertices[k] + SmoothVertexCount;
                        //                    verticalVertices[k] = verticalStartVertices[k] + rotation;
                        //                    rotation -= rotations[startSlice + jdk];
                        //                    if (rotation < 0) rotation += SmoothVertexCount;
                        //                    if (rotation > SmoothVertexCount) rotation -= SmoothVertexCount;
                        //                }
                        //            }

                        //            Vector3 v;
                        //            Vector3* vertex;
                        //            int n;
                        //            for (int t = 0; t < SmoothVertexCount; t++)
                        //            {
                        //                v = Vector3.Zero;
                        //                n = 0;
                        //                for (int k = 0; k < VerticalSmoothing; k++)
                        //                {
                        //                    if (verticalVertices[k] != null)
                        //                    {
                        //                        v += *verticalVertices[k];
                        //                        n++;
                        //                        verticalVertices[k]++;
                        //                        if (verticalVertices[k] == verticalEndVertices[k]) verticalVertices[k] = verticalStartVertices[k];
                        //                    }
                        //                }
                        //                v /= n;
                        //                vertex = verticalVertices[d];
                        //                vertex->X = v.X;
                        //                vertex->Z = v.Z;
                        //            }
                        //        }
                        //        startSlice = -1;
                        //    }
                            
                        //    lastSliceAdded = false;
                        //}
                        lastSlice = slice;
                    }
                }
                return triangleCount * 3;
            }

            private void RemoveTail(LinkedList<Vertex> slice)
            {
                LinkedListNode<Vertex> node, minNode;
                float length, minLength;
                bool firstBlood;
                int nearCount, minPos;

                if (slice.Count > MinLineLength)
                {
                    minNode = null;
                    minLength = 0;
                    firstBlood = false;
                    nearCount = 0;
                    node = slice.First;
                    minPos = 0;
                    while (node != null)
                    {
                        length = (node.Value.Position - slice.Last.Value.Position).LengthSquared();
                        if (length < MaxReturnLengthSquared)
                        {
                            nearCount++;
                            if (firstBlood)
                            {
                                if (length < minLength)
                                {
                                    minLength = length;
                                    minNode = node;
                                }
                            }
                            else
                            {
                                firstBlood = true;
                                minLength = length;
                                minNode = node;
                            }


                        }
                        else
                        {
                            if (firstBlood && nearCount > 3) break;
                        }
                        node = node.Next;
                        minPos++;
                    }

                    if (minNode != null && minPos < slice.Count * 2 / 3)
                        while (slice.First != minNode) slice.RemoveFirst();
                }
            }

            private void RemoveSpikes(LinkedList<Vertex>[] model)
            {
                LinkedList<Vertex> slice;
                LinkedListNode<Vertex> node, tempNode, minNode;
                float minLength, length;
                for (int i = 1; i < model.Length; i++)
                {
                    slice = model[i];
                    if (slice.Count > MinLineLength)
                    {
                        node = slice.First;
                        while (node != null)
                        {
                            tempNode = node.Previous;
                            minLength = float.PositiveInfinity;
                            minNode = null;
                            for (int k = 0; k < SpikeRemoveLength; k++)
                            {
                                if (tempNode == null) break;
                                else
                                {
                                    length = (tempNode.Value.Position - node.Value.Position).Length();
                                    if (length < minLength)
                                    {
                                        minLength = length;
                                        minNode = tempNode;
                                    }
                                }
                                tempNode = tempNode.Previous;
                            }
                            if (minNode != null)
                            {
                                while (minNode != node.Previous)
                                {
                                    slice.Remove(node.Previous);
                                }
                            }
                            node = node.Next;
                        }
                    }
                }
            }

            public unsafe int Triangulate(bool side, out Vector3[] vertices, out int[] indicies)
            {
#if DEBUG
                int verticesAdded;
                int* lastIndexPointer;
#endif

                LinkedList<Vertex>[] model = side ? ModelDataA : ModelDataB;
                LinkedList<Vertex> slice, lastSlice = model[0];
                LinkedListNode<Vertex> node, minNode;

                //Remove tail
                int vertexCount = model[0].Count, triangleCount = 0, nearCount, minPos, sliceCount = 0;
                bool lastSliceAdded = false, firstBlood;
                float minLength, length;
                for (int i = 1; i < model.Length; i++)
                {
                    slice = model[i];
                    RemoveTail(slice);
                    //if (slice.Count > MinLineLength)
                    //{
                    //    minNode = null;
                    //    minLength = 0;
                    //    firstBlood = false;
                    //    nearCount = 0;
                    //    node = slice.First;
                    //    minPos = 0;
                    //    while (node != null)
                    //    {
                    //        length = (node.Value.Position - slice.Last.Value.Position).LengthSquared();
                    //        if (length < MaxReturnLengthSquared)
                    //        {
                    //            nearCount++;
                    //            if (firstBlood)
                    //            {
                    //                if (length < minLength)
                    //                {
                    //                    minLength = length;
                    //                    minNode = node;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                firstBlood = true;
                    //                minLength = length;
                    //                minNode = node;
                    //            }


                    //        }
                    //        else
                    //        {
                    //            if (firstBlood && nearCount > 3) break;
                    //        }
                    //        node = node.Next;
                    //        minPos++;
                    //    }

                    //    if (minNode != null && minPos < slice.Count * 2 / 3)
                    //        while (slice.First != minNode) slice.RemoveFirst();
                    //}

                    if (slice.Count > MinLineLength && lastSlice.Count > MinLineLength)
                    {
                        if (!lastSliceAdded)
                        {
                            vertexCount += lastSlice.Count;
                            sliceCount++;
                        }
                        vertexCount += slice.Count;
                        triangleCount += lastSlice.Count + slice.Count;
                        lastSliceAdded = true;
                        sliceCount++;
                    }
                    else
                        lastSliceAdded = false;
                    
                    lastSlice = slice;
                }

                //Remove spikes
                RemoveSpikes(model);
                //LinkedListNode<Vertex> tempNode;
                //for (int i = 1; i < model.Length; i++)
                //{
                //    slice = model[i];
                //    if (slice.Count > MinLineLength)
                //    {
                //        node = slice.First;
                //        while (node != null)
                //        {
                //            tempNode = node.Previous;
                //            minLength = float.PositiveInfinity;
                //            minNode = null;
                //            for (int k = 0; k < SpikeRemoveLength; k++)
                //            {
                //                if (tempNode == null) break;
                //                else
                //                {
                //                    length = (tempNode.Value.Position - node.Value.Position).Length();
                //                    if (length < minLength)
                //                    {
                //                        minLength = length;
                //                        minNode = tempNode;
                //                    }
                //                }
                //                tempNode = tempNode.Previous;
                //            }
                //            if (minNode != null)
                //            {
                //                while(minNode != node.Previous)
                //                {
                //                    slice.Remove(node.Previous);
                //                }
                //            }
                //            node = node.Next;
                //        }
                //    }
                //}

                Vector2[] directions;
                indicies = new int[triangleCount * 3];
                vertices = new Vector3[vertexCount];
                directions = new Vector2[vertexCount];
                if (vertexCount == 0) return 0;


                Vector3[] refPoints = new Vector3[refPointHistory];

                bool lineFinished;

                LineInfo[] lines = new LineInfo[model.Length];
                float lenA, lenB;
                int longEdges;
                Vector3 refPoint;
                Vector3 vA, vB;
                fixed (int* sIndicies = &indicies[0])
                fixed (Vector3* sVertices = &vertices[0])
                fixed (Vector3* sRefPoints = &refPoints[0])
                fixed (Vector2* sDirections = &directions[0])
                fixed(LineInfo* sLines = &lines[0])
                {
                    int* pIndicies=sIndicies, lIndicies;
                    Vector3* pVertices, lVertices = sVertices;
                    Vector3* startVertexA = null, startVertexB;
                    Vector3* vertexA, vertexB, nextVertexA, nextVertexB, baseA, baseB, limitA, limitB;
                    Vector3* minVertex, trapB;
                    Vector2* pDirections = sDirections;
                    Vector2* directionA, directionB, startDirectionA = null, startDirectionB;
                    Vector3* pRefPoint, eRefPoints = sRefPoints + refPoints.Length;
                    Vector3* firstA, firstB, lastA, lastB;
                    //LineInfo* pLines = sLines;
#if DEBUG
                    lastIndexPointer = pIndicies;
#endif

                    lastSliceAdded = false;
                    for (int row = 1; row < model.Length; row++)
                    {
                        slice = model[row];
                        if (slice.Count > MinLineLength && lastSlice.Count > MinLineLength)
                        {
                            //Add new vertices
                            //->last slice
                            pVertices = lVertices;
                            if (!lastSliceAdded)
                            {
                                startVertexA = pVertices;
                                startDirectionA = pDirections;
                                node = lastSlice.First;
                                while (node != null)
                                {
                                    *pVertices++ = node.Value.Position;
                                    *pDirections++ = node.Value.Direction;
                                    node = node.Next;
                                }
                                
                            }

                            //->current slice
                            startVertexB = pVertices;
                            startDirectionB = pDirections;
                            node = slice.First;
                            while (node != null)
                            {
                                *pVertices++ = node.Value.Position;
                                *pDirections++ = node.Value.Direction;
                                node = node.Next;
                            }
                            lVertices = pVertices;

                            //Process line
                            lineFinished = false;
                            vertexA = baseA = startVertexA;
                            vertexB = baseB = startVertexB;
                            baseA = vertexA + 1;
                            baseB = vertexB + 1;
                            longEdges = 0;

                            refPoint = (*vertexA + *vertexB) / 2f;
                            for (pRefPoint = sRefPoints; pRefPoint < eRefPoints; pRefPoint++)
                            {
                                *pRefPoint = refPoint;
                            }
                            pRefPoint = sRefPoints;
                            trapB = null;

                            //pLines->StartA = startVertexA;
                            //pLines->EndA = startVertexB - 1;
                            //pLines->StartB = startVertexB;
                            //pLines->EndB = lVertices - 1;
                            lIndicies = pIndicies;
                            firstA = null;
                            firstB = null;
                            lastA = null;
                            lastB = null;
#if DEBUG
                            //if (row == 376) Debugger.Break();
                            //if (DebugLine == row) 
                            //if (row > 480 - 18 && row < 480 - 14)
                            {
#endif
                                //Debugger.Break();
                                while (!lineFinished)
                                {
                                    //->set next
                                    nextVertexA = vertexA + 1;
                                    if (nextVertexA == startVertexB)
                                        nextVertexA = startVertexA;
                                    //if (nextVertexA == startVertexB) 
                                    //    break;
                                    nextVertexB = vertexB + 1;
                                    if (nextVertexB == lVertices)
                                        nextVertexB = startVertexB;
                                    //if (nextVertexB == trapB) 
                                    //    break;
                                    if (nextVertexA == firstA && nextVertexB == firstB && firstB != null)
                                    {
                                        *pIndicies++ = (int)(vertexA - sVertices);
                                        *pIndicies++ = (int)(vertexB - sVertices);
                                        *pIndicies++ = (int)(nextVertexA - sVertices);

                                        *pIndicies++ = (int)(vertexB - sVertices);
                                        *pIndicies++ = (int)(nextVertexB - sVertices);
                                        *pIndicies++ = (int)(nextVertexA - sVertices);
                                        break;
                                    }

                                    directionA = startDirectionA + (vertexA - startVertexA);
                                    directionB = startDirectionB + (vertexB - startVertexB);

                                    //->lengths of possible edges
                                    lenA = (*nextVertexA - *vertexB).Length();
                                    lenB = (*nextVertexB - *vertexA).Length();
                                    if ((lenA < MaxLongEdgeLength || lenB < MaxLongEdgeLength) && longEdges < MaxLongEdges)
                                    {
                                        if (lenA > MaxEdgeLength && lenB > MaxEdgeLength)
                                        {
                                            longEdges++;
                                        }
                                        else
                                        {
                                            longEdges = 0;
                                            //!!!!!!!!!!!!!!!!!!
                                            baseA = nextVertexA;
                                            baseB = nextVertexB;
                                            //!!!!!!!!!!!!!!!!!!
                                        }
                                        if (trapB == null) trapB = vertexB;
                                        *pIndicies++ = (int)(vertexA - sVertices);
                                        *pIndicies++ = (int)(vertexB - sVertices);

                                        if (nextVertexA == firstA)
                                        {
                                            *pIndicies++ = (int)(nextVertexB - sVertices);
                                            vertexB = nextVertexB;
                                        }
                                        else if (nextVertexB == firstB)
                                        {
                                            *pIndicies++ = (int)(nextVertexA - sVertices);
                                            vertexA = nextVertexA;
                                        }
                                        else
                                        {

                                            //vA = *nextVertexA - *pRefPoint;
                                            //vB = *nextVertexB - *pRefPoint;
                                            

                                            if (firstA == null)
                                            {
                                                firstA = vertexA;
                                                firstB = vertexB;
                                            }

                                            Vector2 dir = (*directionA + *directionB) / 2;
                                            Vector3 dir3 = new Vector3(dir.X,0,dir.Y);
                                            //if (Math.Abs(vA.X * dir.X + vA.Z * dir.Y) < Math.Abs(vB.X * dir.X + vB.Z * dir.Y))
                                            //if((*nextVertexA-*vertexA).Length()+(*nextVertexA-*vertexB).Length()<
                                            //    (*nextVertexB - *vertexA).Length() + (*nextVertexB - *vertexB).Length())
                                            //if (TriangleArea(*vertexA, *vertexB, *nextVertexA) > TriangleArea(*vertexA, *vertexB, *nextVertexB))
                                            if((*nextVertexA-*vertexB).Length()<(*nextVertexB-*vertexA).Length())
                                            {
                                                *pIndicies++ = (int)(nextVertexA - sVertices);
                                                vertexA = nextVertexA;
                                            }
                                            else
                                            {
                                                *pIndicies++ = (int)(nextVertexB - sVertices);
                                                vertexB = nextVertexB;
                                            }
                                        }
                                        *pRefPoint++ = (*vertexA + *vertexB) / 2f;
                                        if (pRefPoint == eRefPoints) pRefPoint = sRefPoints;
                                    }
                                    else
                                    {
                                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                        if (pIndicies > lIndicies)
                                        {
                                            if (longEdges == 0) longEdges = 1;
                                            pIndicies--;
                                            for (int k = 0; k < longEdges * 3; k++)
                                            {
                                                *pIndicies-- = 0;
                                            }
                                            if (pIndicies > lIndicies)
                                            {
                                                pIndicies -= 2;
                                                float y;
                                                for (int k = 0; k < 3; k++)
                                                {
                                                    y = (sVertices + *pIndicies)->Y;
                                                    if (y == startVertexA->Y) lastA = sVertices + *pIndicies;
                                                    else lastB = sVertices + *pIndicies;
                                                    pIndicies++;
                                                }                                                
                                            }
                                            else
                                            {
                                                pIndicies++;
                                                firstA = null;
                                                firstB = null;
                                            }
                                        }

                                        longEdges = 0;
                                        if (nextVertexA == firstA || nextVertexA == firstB) break;
                                        vertexA = baseA;
                                        vertexB = baseB;
                                        if (firstA == null)
                                            limitA = startVertexA;
                                        else
                                            limitA = firstA;
                                        if (firstB == null)
                                            limitB = startVertexB;
                                        else
                                            limitB = firstB;
                                        lineFinished = true;
                                        if (baseA != limitA && baseB != limitB)
                                            while (true)
                                            {
                                                nextVertexA = vertexA + 1;
                                                if (nextVertexA == startVertexB) nextVertexA = startVertexA;
                                                if (nextVertexA == limitA) break;
                                                vertexB = baseB;
                                                nextVertexB = vertexB + 1;
                                                if (nextVertexB == lVertices) nextVertexB = startVertexB;

                                                minLength = float.PositiveInfinity;
                                                minVertex = null;
                                                while (nextVertexB != limitB)
                                                {
                                                    length = (*vertexA - *vertexB).Length();
                                                    if (length < minLength && ((*nextVertexB - *vertexA).Length() < minLength || (*nextVertexA - *vertexB).Length() < minLength))
                                                    {
                                                        minVertex = vertexB;
                                                        minLength = length;
                                                    }
                                                    vertexB = nextVertexB;
                                                    nextVertexB = vertexB + 1;
                                                    if (nextVertexB == lVertices) nextVertexB = startVertexB;
                                                }
                                                if (minLength < MaxEdgeLength)
                                                {
                                                    vertexB = minVertex;
                                                    refPoint = (*vertexA + *vertexB) / 2f;
                                                    for (pRefPoint = sRefPoints; pRefPoint < eRefPoints; pRefPoint++)
                                                    {
                                                        *pRefPoint = refPoint;
                                                    }
                                                    pRefPoint = sRefPoints;
                                                    baseA = nextVertexA;
                                                    baseB = minVertex + 1;
                                                    if (baseB == lVertices) baseB = startVertexB;
                                                    lineFinished = false;
                                                    break;
                                                }
                                                vertexA = nextVertexA;
                                            }
                                    }
                                }

                                if (pIndicies > lIndicies && lineFinished)
                                {
                                    pIndicies -= 3;
                                    float y;
                                    for (int k = 0; k < 3; k++)
                                    {
                                        y = (sVertices + *pIndicies)->Y;
                                        if (y == startVertexA->Y) lastA = sVertices + *pIndicies;
                                        else lastB = sVertices + *pIndicies;
                                        pIndicies++;
                                    }

                                    if ((*firstA - *lastA).Length() < MaxEdgeLength && (*firstB - *lastB).Length() < MaxEdgeLength)
                                    {
                                        *pIndicies++ = (int)(lastA - sVertices);
                                        *pIndicies++ = (int)(lastB - sVertices);
                                        *pIndicies++ = (int)(firstB - sVertices);

                                        *pIndicies++ = (int)(lastA - sVertices);
                                        *pIndicies++ = (int)(firstB - sVertices);
                                        *pIndicies++ = (int)(firstA - sVertices);
                                    }
                                }


#if DEBUG

                                //if (pIndicies - lastIndexPointer > (slice.Count + lastSlice.Count) * 3) Debugger.Break();
                                //if (pIndicies - lastIndexPointer < (slice.Count + lastSlice.Count) *3/8) Debugger.Break();
                            }
                            lastIndexPointer = pIndicies;
#endif

                            //Prepare next round
                            lastSliceAdded = true;
                            startVertexA = startVertexB;
                            startDirectionA = startDirectionB;
                        }
                        else
                            lastSliceAdded = false;
                        lastSlice = slice;

                        //pLines++;
                    }



                    //pLines = sLines;
                    //LineInfo* prevLines, nextLines;
                    //Vector3* jumpVertexAs, jumpVertexAt, jumpVertexBs, jumpVertexBt, endVertexA, endVertexB;
                    //for (int row = 0; row < 0/*model.Length*/; row++)
                    //{
                    //    if (row == 0) prevLines = null;
                    //    else
                    //    {
                    //        prevLines = pLines - 1;
                    //        if (prevLines->FirstA == null) prevLines = null;
                    //    }

                    //    if (row == model.Length - 1) nextLines = null;
                    //    else
                    //    {
                    //        nextLines = pLines + 1;
                    //        if (nextLines->FirstA == null) nextLines = null;
                    //    }

                        


                    //    if (pLines->FirstA != null)
                    //    {
                    //        startVertexA = pLines->LastA;
                    //        endVertexA = pLines->FirstA;
                    //        startVertexB = pLines->LastB;
                    //        endVertexB = pLines->FirstB;

                            

                    //        if (prevLines == null || prevLines->LastB < pLines->LastA)
                    //            jumpVertexAs = pLines->LastA;
                    //        else
                    //            jumpVertexAs = prevLines->LastB;

                    //        if (prevLines == null || prevLines->FirstB > pLines->FirstA)
                    //            jumpVertexAt = pLines->FirstA;
                    //        else
                    //            jumpVertexAt = prevLines->FirstB;

                    //        if (nextLines == null || nextLines->LastA < pLines->LastB || nextLines->FirstA<pLines->LastB)
                    //            jumpVertexBs = pLines->LastB;
                    //        else
                    //            jumpVertexBs = nextLines->LastA;

                    //        if (nextLines == null || nextLines->FirstA > pLines->FirstB)
                    //            jumpVertexBt = pLines->FirstB;
                    //        else
                    //            jumpVertexBt = nextLines->FirstA;

                    //        if (!(startVertexA <= jumpVertexAs && jumpVertexAt <= endVertexA && jumpVertexAt < jumpVertexAs && endVertexA<jumpVertexAs))
                    //            System.Diagnostics.Debugger.Break();
                    //        if (!(startVertexB <= jumpVertexBs && jumpVertexBt <= endVertexB && jumpVertexBt < jumpVertexBs && endVertexB<jumpVertexBs))
                    //            System.Diagnostics.Debugger.Break();

                    //        triangleCount = 0;
                    //        int tm = (int)(jumpVertexAs - startVertexA + endVertexA - jumpVertexAt) + (int)(jumpVertexBs - startVertexB + endVertexB - jumpVertexBt);

                    //        vertexA = startVertexA;
                    //        vertexB = startVertexB;
                    //        while (vertexA != endVertexA || vertexB != endVertexB)
                    //        {

                    //            if (vertexA == jumpVertexAs) nextVertexA = jumpVertexAt;
                    //            else if (vertexA == pLines->EndA) nextVertexA = pLines->StartA;
                    //            else nextVertexA = vertexA + 1;

                    //            if (vertexB == jumpVertexBs) nextVertexB = jumpVertexBt;
                    //            else if (vertexB == pLines->EndB) nextVertexB = pLines->StartB;
                    //            else nextVertexB = vertexB + 1;
                    //            triangleCount++;

                    //            lenA = (*vertexB - *nextVertexA).LengthSquared();
                    //            lenB = (*vertexA - *nextVertexB).LengthSquared();
                    //            *pIndicies++ = (int)(vertexA - sVertices);
                    //            *pIndicies++ = (int)(vertexB - sVertices);
                    //            if ((nextVertexB == endVertexB + 1 || lenA < lenB) && nextVertexA != endVertexA + 1)
                    //            {
                    //                *pIndicies++ = (int)(nextVertexA - sVertices);
                    //                vertexA = nextVertexA;
                    //            }
                    //            else
                    //            {
                    //                *pIndicies++ = (int)(nextVertexB - sVertices);
                    //                vertexB = nextVertexB;
                    //            }

                    //            if (1000 < triangleCount) 
                    //                System.Diagnostics.Debugger.Break();
                    //        }
                    //    }
                    //    pLines++;
                    //}


                    return (int)(pIndicies - sIndicies);
                }
            }


            float TriangleArea(Vector3 a, Vector3 b, Vector3 c)
            {
                return Vector3.Cross(b - a, c - a).Length() / 2f;
            }

            const int SaveBufferSize = 128;
            const float ExportScale = 500;
            public unsafe void SaveToSTLFile(string path, string title)
            {
                string[] filename = { path + "_1a.stl", path + "_1b.stl" };
                bool[] side = { false, true };
                for (int k = 0; k < 2; k++)
                {
                    Vector3[] vertices;
                    int[] indicies;
                    int count = Triangulate(side[k], out vertices, out indicies);
                    if (vertices.Length == 0) continue;
                    if (File.Exists(filename[k]))
                        File.Delete(filename[k]);
                    FileStream FS = new FileStream(filename[k], FileMode.Create, FileAccess.Write);
                    BinaryWriter BW = new BinaryWriter(FS, Encoding.ASCII);
                    string header;
                    while (title.StartsWith("solid", StringComparison.OrdinalIgnoreCase)) title = title.Remove(0, 5);
                    if (title.Length > 80) header = title.Substring(0, 79);
                    else header = title.PadRight(79);
                    BW.Write(header);
                    BW.Write((UInt32)(count/3));
                    byte[] buffer = new byte[SaveBufferSize * (3 * 4 * 4 + 2)];
                    int i = 0;
                    Vector3 A, B, C, N;
                    fixed (Vector3* sVertices = &vertices[0])
                    fixed (int* sIndicies = &indicies[0])
                    fixed (byte* sBuffer = &buffer[0])
                    {
                        int* pIndicies = sIndicies;
                        int* eIndicies = sIndicies + count;
                        while (pIndicies < eIndicies)
                        {
                            Vector3* vBuffer = (Vector3*)(sBuffer + i * 50);
                            short* pBuffer = (short*)(sBuffer + i * 50 + 48);

                            A = *(sVertices + *pIndicies++);
                            A.Y = -A.Y;
                            B = *(sVertices + *pIndicies++);
                            B.Y = -B.Y;
                            C = *(sVertices + *pIndicies++);
                            C.Y = -C.Y;
                            N = Vector3.Cross(B - A, C - A);
                            N.Normalize();
                            *vBuffer++ = N;
                            *vBuffer++ = A * ExportScale;
                            *vBuffer++ = B * ExportScale;
                            *vBuffer++ = C * ExportScale;

                            *pBuffer = 0;
                            i++;
                            if (i == SaveBufferSize || pIndicies == eIndicies)
                            {
                                BW.Write(buffer, 0, i * 50);
                                i = 0;
                            }
                        }
                    }
                    BW.Flush();
                    BW.Close();
                    BW.Dispose();
                    FS.Dispose();
                }
            }

            struct VertexPosition : IVertexType
            {
                public Vector3 Position;
                public float Segment;
                public VertexPosition(Vector3 position, int segment)
                {
                    Position = position;
                    Segment = segment;
                }

                public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
                (
                    new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                    new VertexElement(12, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 0)
                );

                VertexDeclaration IVertexType.VertexDeclaration
                {
                    get { return VertexDeclaration; }
                }
            }

            VertexBuffer VB;
            VertexPosition[] Vertices;
            const int MaxVertices = 4096;
            public void DrawModel(GraphicsDevice device, EffectParameter segmentLength, EffectPass pass)
            {
                if (VB != null && VB.GraphicsDevice != device)
                {
                    VB.Dispose();
                    VB = null;
                }
                if (VB == null)
                {
                    VB = new VertexBuffer(device, typeof(VertexPosition), MaxVertices, BufferUsage.WriteOnly);
                }
                LinkedList<Vertex> Model;
                int i;
                int mab;
                device.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.Transparent, 1, 0);
                for (int j = 0; j < Height; j++)
                {
                    for (mab = 0; mab < 2; mab++)
                    {
                        Model = mab == 0 ? ModelDataA[j] : ModelDataB[j];
                        if (Model.Count > 1)
                        {
                            i = 0;
                            foreach (Vertex v in Model)
                            {
                                Vertices[i].Position = v.Position;
                                Vertices[i].Segment = mab == 0 ? i : -i;
                                i++;
                                if (i == MaxVertices) break;
                            }
                            i--;
                            segmentLength.SetValue(i);
                            pass.Apply();
                            device.SetVertexBuffer(null);
                            VB.SetData<VertexPosition>(Vertices);
                            device.SetVertexBuffer(VB);
                            device.DrawPrimitives(PrimitiveType.LineStrip, 0, i);
                        }
                    }
                }
            }
        }



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

        enum States { None, Calibration };
        States State;

        CalibrationForm CF;
        private void TSBCalibration_Click(object sender, EventArgs e)
        {
            //SwitchState(States.Calibration);
            CF.Show();
            
        }

        void SwitchState(States state)
        {
            switch (State)
            {
                case States.None:
                    SetMenuVisibility(MainMenuSet, false);
                    TSBCalibration.Visible = false;
                    break;
                case States.Calibration:
                    SetMenuVisibility(CalibrationMenuSet, false);
                    break;
            }
            State = state;
            switch (State)
            {
                case States.None:
                    SetMenuVisibility(MainMenuSet, true);
                    break;
                case States.Calibration:
                    SetMenuVisibility(CalibrationMenuSet, true);
                    TSBCalibrationSetA.Enabled = TSBCalibrationSetB.Enabled = false;
                    break;
            }
        }

        private void TSBExitMode_Click(object sender, EventArgs e)
        {
            SwitchState(States.None);
        }

        DepthContainer ImageA, ImageB;
        private void TSBCalibrationSetA_Click(object sender, EventArgs e)
        {
            ImageA = DepthCache[LastDepthIndex];
        }

        private void TSBCalibrationSetB_Click(object sender, EventArgs e)
        {
            ImageB = DepthCache[LastDepthIndex];
        }

        private void TSBCalibrationClear_Click(object sender, EventArgs e)
        {
            ImageA.Data = null;
            ImageB.Data = null;
        }

        float RotateX, RotateY;
        const float RotationStep = 0.01f;
        private void TSBCalibrationResetView_Click(object sender, EventArgs e)
        {
            RotateX = RotateY = 0;
        }

        bool Rotation = false;
        System.Drawing.Point LastPoint;
        private void DebugForm_MouseDown(object sender, MouseEventArgs e)
        {
            LastPoint = e.Location;
            Rotation = true;
        }

        private void DebugForm_MouseUp(object sender, MouseEventArgs e)
        {
            Rotation = false;
        }

        private void DebugForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (Rotation)
            {
                RotateY += (e.Location.X - LastPoint.X) * RotationStep;
                RotateX += -(e.Location.Y - LastPoint.Y) * RotationStep;
                LastPoint = e.Location;
            }
        }

        private void TSMIViewMode_Click(object sender, EventArgs e)
        {
            (TSDDBView.DropDownItems[(int)ViewMode] as ToolStripMenuItem).Checked = false;
            ViewMode = (Views)(sender as ToolStripMenuItem).Tag;
            (TSDDBView.DropDownItems[(int)ViewMode] as ToolStripMenuItem).Checked = true;
        }

        string ExCoreSaveTo=null;
        private void TSBSaveModel_Click(object sender, EventArgs e)
        {
            string fileName = Context.WorkingDirectory + '\\';
            if (string.IsNullOrEmpty(TSTBLabel.Text))
            {
                fileName += DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            }
            else
            {
                fileName += TSTBLabel.Text.ToFileName();
            }
            fileName += ".stl";
            switch (ModellingMode)
            {
                case ModellingModes.Fusion:                    
                    MM.SaveToSTLFile(fileName, TSTBLabel.Text);
                    break;
                case ModellingModes.ExCore:
                    ExCoreSaveTo = fileName;
                    break;
            }
        }

        private void TSBSettings_Click(object sender, EventArgs e)
        {
            SF.Show();
        }

        bool BuildModel;
        private void SetBuildModel(bool enabled)
        {
            TSBBuildModel.Checked = BuildModel = enabled;
        }

        private void TSBBuildModel_Click(object sender, EventArgs e)
        {
            SetBuildModel(!BuildModel);
        }

        public event EventHandler LoadDirectory;
        private void MISelectWorkingDirectory_Click(object sender, EventArgs e)
        {
            using(FolderBrowserDialog FBD=new FolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(WorkingDirectory))
                    FBD.SelectedPath = WorkingDirectory;
                else if (!string.IsNullOrEmpty(ModuleSettings.Default.WorkingDirectory))
                    FBD.SelectedPath = ModuleSettings.Default.WorkingDirectory;
                if (FBD.ShowDialog() == DialogResult.OK)
                {
                    WorkingDirectory = FBD.SelectedPath;
                    if (LoadDirectory != null) LoadDirectory(this, null);
                }
            }
        }

        void CloseProject()
        {
            if (ProjectLoaded)
            {
                ProjectLoaded = true;
                Settings.Default.PropertyValues.Export(WorkingDirectory + @"\Project.xml", "RotatingScanner");
            }
        }

        bool NextExCoreClear;
        private void MIClearModel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        public void Clear()
        {
            if (ModellingMode == ModellingModes.ExCore) NextExCoreClear = true;
            MM.NewModel();
        }
    }

    public static class LocalExtensions
    {
        public static LinkedListNode<Vector3> CircularNext(this LinkedListNode<Vector3> node)
        {
            if (node.Next == null) return node.List.First;
            else return node.Next;
        }

        public static LinkedListNode<Vector3> CircularPrevious(this LinkedListNode<Vector3> node)
        {
            if (node.Previous == null) return node.List.Last;
            else return node.Previous;
        }
    }
}