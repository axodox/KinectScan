using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Drawing;
using Kinect = freenect.Kinect;
using KinectScan.Properties;
using Modules;

namespace KinectScan
{
    public partial class MainForm : Form, IServiceProvider, IGraphicsDeviceService
    {
        public MainForm(string file = null)
        {
            //new XPlane(2, 2);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("hu-HU");
            NextMode = KinectScanner.Modes.Depth480;
            InitializeComponent();
            InitGUI();
            InitProbes();

            LoadSettings();
            LoadHardwareSettings("HardwareSettings.xml");
            InitModules();
            InitRendering();
            InitSequence();

            if (file != null)
            {
                string ext = Path.GetExtension(file).ToLower();
                switch (ext)
                {
                    case ".vector4":
                        LoadModel(file);
                        break;
                    case ".rwd":
                    case ".rwc":
                        LoadRawModel(file);
                        break;
                }
            }
        }

        string WorkingDirectory;
        [Flags]
        enum SaveModes : byte { None = 0, Vector4 = 1, Raw = 2, STL = 4, Screenshot = 8, All = 255 };
        SaveModes SaveMode = SaveModes.Vector4;
        readonly SaveModes DepthSupportedSaveModes = SaveModes.All;
        readonly SaveModes ColorSupportedSaveModes = SaveModes.Screenshot;
        readonly SaveModes IRSupportedSaveModes = SaveModes.Screenshot;


        #region GUI
        Timer GUITimer;
        int DevicesMenuLength;
        Timer ResizeTimer = new Timer();
        Size PreviousXPanelSize;
        const float ZoomStep = 1.1f;
        private void InitGUI()
        {
            GUITimer = new Timer();
            GUITimer.Interval = 200;
            GUITimer.Start();
            ResizeTimer = new Timer();
            ResizeTimer.Interval = 500;
            ResizeTimer.Tick += (object sender, EventArgs e) =>
            {
                if (WindowState != FormWindowState.Minimized)
                {
                    DestroyXDevice();
                    CreateXDevice();
                }
                ResizeTimer.Stop();
            };
            XPanel.Resize += (object sender, EventArgs e) =>
            {
                if (PreviousXPanelSize != XPanel.Size)
                {
                    ResizeTimer.Stop();
                    ResizeTimer.Start();
                }
                PreviousXPanelSize = XPanel.Size;
            };
            MenuItem MI;
            Array Modes = Enum.GetValues(typeof(KinectScanner.Modes));
            foreach (KinectScanner.Modes mode in Modes)
            {
                MI = new MenuItem(mode.ToString(), OnModeClick);
                MI.Tag = mode;
                MI.RadioCheck = true;
                MIModes.MenuItems.Add(MI);
            }
            MIModes.MenuItems[(int)NextMode].Checked = true;
            DevicesMenuLength = MIDevices.MenuItems.Count;
            RefreshKinectList();

            MouseWheel += (object o, MouseEventArgs e) =>
                {
                    if (EFScale != null && !ProbesOn)
                    {
                        if (e.Delta > 0)
                            ReprojectionScale *= ZoomStep;
                        else
                            ReprojectionScale /= ZoomStep;
                        EFScale.SetValue(ReprojectionScale);
                    }
                };

            ToolStripMenuItem TSMI;
            Modes = Enum.GetValues(typeof(SaveModes));
            foreach (SaveModes mode in Modes)
            {
                if (mode == SaveModes.None || mode==SaveModes.All) continue;
                MI = new MenuItem(mode.ToString(), TSMISave_Click);
                MI.Tag = mode;
                MISaveTo.MenuItems.Add(MI);
                TSMI = new ToolStripMenuItem(mode.ToString());
                TSMI.Click += TSMISave_Click;
                TSMI.Tag = mode;
                TSBSave.DropDownItems.Add(TSMI);
            }
        }

        private void SetMode(KinectScanner.Modes mode)
        {
            MIModes.MenuItems[(int)NextMode].Checked = false;
            NextMode = mode;
            MIModes.MenuItems[(int)NextMode].Checked = true;
        }

        Size GetFitSize(Size InnerSize, Size OuterSize)
        {
            Size returnSize = new Size();
            if ((double)OuterSize.Width / (double)OuterSize.Height > (double)InnerSize.Width / (double)InnerSize.Height)
            {
                returnSize.Height = OuterSize.Height;
                returnSize.Width = InnerSize.Width * OuterSize.Height / InnerSize.Height;
            }
            else
            {
                returnSize.Height = InnerSize.Height * OuterSize.Width / InnerSize.Width;
                returnSize.Width = OuterSize.Width;
            }
            return returnSize;
        }

        private void OnModeClick(object sender, EventArgs e)
        {
            SwitchToMode((Scanner.Modes)(sender as MenuItem).Tag);
        }

        public void SwitchToMode(Scanner.Modes mode)
        {
            if (KS==null || mode != KS.Mode)
            {
                Stop();
                SetMode(mode);
                Start();
            }
        }

        void SetStatus(string text)
        {
            SBStatus.Text = text;
        }
        #endregion

        #region Processing & visualization
        int FrameID;
        const int GaussCoeffCount = 9;
        float ReprojectionTranslationX, ReprojectionTranslationY, ReprojectionTranslationZ, ReprojectionRotationX, ReprojectionRotationY, ReprojectionRotationZ, GaussSigma = 3f, ReprojectionScale = 1f;
        Vector2 ReprojectionMove = Vector2.Zero;
        bool Reprojection;
        int VideoDismapWidth, DepthDismapWidth;
        int VideoDismapHeight, DepthDismapHeight;
        int GaussPasses, ShadingMode, Rotation = 0, BufferedFrames;
        float[] DepthCorrection, VideoCorrection;
        Timer XTimer;
        GraphicsDevice XDevice;
        ContentManager XContent;
        Effect XEffect;
        SpriteBatch XSprite;
        SpriteFont XFont;
        XPlane Plane, MiniPlane;
        RasterizerState WireFrame;
        RenderTarget2D DepthTarget, DepthNormals, VideoOutput, DepthOutput, SecondaryTarget;
        Texture2D DepthCorrectionTexture, VideoCorrectionTexture, ScaleTexture, LogoTexture;
        Vector2 LogoPosition, LogoOrigin;
        const float LogoSpacing = 0.8f;
        float LogoScale;
        Matrix DepthIntrinsics, DepthInverseIntrinsics, DepthInverseExtrinsics, VideoIntrinsics, VideoExtrinsics, DepthToColorTransfrom;
        BlendState BSAdd;
        EffectParameter EPDepthTextureA, EPDepthTextureB, EFDepthTexture, EFDepthNormalTexture, EFVideoTexture, EFMove, EFScale, EPDepthScale, EPDepthDisp, EFReprojectionTransform, EFStereoFocus;
        EffectTechnique SimpleTechnique, DepthAddTechnique, DepthAntiDistortAndAverageTechnique, DepthAverageTechnique, DepthHGaussTechnique, DepthVGaussTechnique, PositionOutputShadingTechnique, DepthNormalShadingTechnique, DepthReprojectionTechnique, AnaglyphDepthReprojectionTechnique, DepthDisplayTechnique, ModelReprojectionTechnique, ReprojectionOutputTechnique;

        public enum StereoModes { LeftRight, UpDown, Anaglyph };
        StereoModes StereoMode = StereoModes.LeftRight;
        Viewport DefaultViewport, LeftViewport, RightViewport;
        float CameraOffset = 0.025f;
        float FocusedDepth = 0.2f;
        Vector2 StereoFocus;
        EffectParameter EPAnaglyphColor;
        EffectTechnique AnaglyphTechnique;
        //Vector3[] AnaglyphColors = new Vector3[] { new Vector3(0, 28, 105), new Vector3(105, 0, 0) };
        Vector3[] AnaglyphColors = new Vector3[] { new Vector3(1, 0, 0), new Vector3(0, 1, 1) };
        private void InitRendering()
        {            
            Plane = new XPlane(640, 480);
            MiniPlane = new XPlane(2, 2);

            BSAdd = new BlendState();
            BSAdd.ColorBlendFunction = BlendFunction.Add;
            BSAdd.ColorSourceBlend = Blend.One;
            BSAdd.ColorDestinationBlend = Blend.One;
            BSAdd.AlphaBlendFunction = BlendFunction.Add;
            BSAdd.AlphaSourceBlend = Blend.One;
            BSAdd.AlphaDestinationBlend = Blend.One;
            BSAdd.ColorWriteChannels = ColorWriteChannels.All;

            WireFrame = new RasterizerState();
            WireFrame.CullMode = CullMode.None;

            XTimer = new Timer();
            XTimer.Interval = 31;
            XTimer.Tick += new EventHandler(XTimer_Tick);

            for (int i = 0; i < AnaglyphColors.Length; i++)
                AnaglyphColors[i].Normalize();

            DepthToColorTransfrom = VideoIntrinsics * VideoExtrinsics * DepthInverseExtrinsics * DepthInverseIntrinsics;
            CreateXDevice();
        }


        void XTimer_Tick(object sender, EventArgs e)
        {
            if (XTimer.Enabled)
            {
                bool CalibrationSave = Calibration && screenshotNext;
                FrameID++;
                XDevice.SetRenderTarget(null);
                XDevice.Clear(Microsoft.Xna.Framework.Color.White);

                XDevice.BlendState = BlendState.Opaque;
                if (Processing)
                {
                    string path = null;
                    if (screenshotNext)
                    {
                        if (Calibration) path = CalibrationSavePath;
                        else path = WorkingDirectory + "\\" + (TSTBLabel.Text == "" ? "" : TSTBLabel.Text.ToFileName() + "_") + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                    }

                    if (KS.AllTexturesReady)
                    {
                        if (KS.Mode == KinectScanner.Modes.Depth480)
                        {
                            XDevice.SetRenderTarget(DepthVectorTarget1);
                            XDevice.Clear(ClearOptions.Target, Microsoft.Xna.Framework.Color.Black, 0, 0);
                            XDevice.SetRenderTarget(DepthVectorTarget2);
                            XDevice.Clear(ClearOptions.Target, Microsoft.Xna.Framework.Color.Black, 0, 0);

                            Texture2D T;
                            bool tick = false;
                            if (KS.DepthTexture.TextureCount == 2)
                            {
                                T = KS.DepthTexture.BeginTextureUse(KS.DepthTexture.FrontTextureIndex);
                                if (T != null)
                                {
                                    XDevice.SetRenderTarget(tick ? DepthVectorTarget2 : DepthVectorTarget1);
                                    EPDepthTextureA.SetValue(tick ? DepthVectorTarget1 : DepthVectorTarget2);
                                    EPDepthTextureB.SetValue(T);
                                    XEffect.CurrentTechnique = DepthAddTechnique;
                                    XEffect.CurrentTechnique.Passes[0].Apply();
                                    MiniPlane.Draw();
                                    tick = !tick;
                                    KS.DepthTexture.EndTextureUse();
                                }
                            }
                            else
                            {
                                for (int i = 0; i < KS.DepthTexture.TextureCount; i++)
                                {
                                    T = KS.DepthTexture.BeginTextureUse(i);
                                    if (T != null)
                                    {
                                        XDevice.SetRenderTarget(tick ? DepthVectorTarget2 : DepthVectorTarget1);
                                        EPDepthTextureA.SetValue(tick ? DepthVectorTarget1 : DepthVectorTarget2);
                                        EPDepthTextureB.SetValue(T);
                                        XEffect.CurrentTechnique = DepthAddTechnique;
                                        XEffect.CurrentTechnique.Passes[0].Apply();
                                        MiniPlane.Draw();
                                        tick = !tick;
                                        KS.DepthTexture.EndTextureUse();
                                    }
                                }
                            }

                            XDevice.SetRenderTarget(tick ? DepthVectorTarget2 : DepthVectorTarget1);
                            EPDepthTextureA.SetValue(tick ? DepthVectorTarget1 : DepthVectorTarget2);
                            XEffect.CurrentTechnique = Reprojection? DepthAntiDistortAndAverageTechnique:DepthAverageTechnique;
                            XEffect.CurrentTechnique.Passes[0].Apply();
                            MiniPlane.Draw();
                            tick = !tick;

                            for (int i = 0; i < GaussPasses; i++)
                            {
                                XDevice.SetRenderTarget(tick ? DepthVectorTarget2 : DepthVectorTarget1);
                                XEffect.CurrentTechnique = DepthHGaussTechnique;
                                EPDepthTextureA.SetValue(tick ? DepthVectorTarget1 : DepthVectorTarget2);
                                XEffect.CurrentTechnique.Passes[0].Apply();
                                MiniPlane.Draw();

                                XDevice.SetRenderTarget(tick ? DepthVectorTarget1 : DepthVectorTarget2);
                                XEffect.CurrentTechnique = DepthVGaussTechnique;
                                EPDepthTextureA.SetValue(tick ? DepthVectorTarget2 : DepthVectorTarget1);
                                XEffect.CurrentTechnique.Passes[0].Apply();
                                MiniPlane.Draw();
                            }

                            if (Reprojection)
                            {
                                if (screenshotNext && SaveMode!=SaveModes.Screenshot)
                                {
                                    XDevice.SetRenderTarget(DepthOutput);
                                    EFDepthTexture.SetValue(tick ? DepthVectorTarget1 : DepthVectorTarget2);
                                    XEffect.CurrentTechnique = PositionOutputShadingTechnique;
                                    XEffect.CurrentTechnique.Passes[0].Apply();
                                    MiniPlane.Draw();

                                    XDevice.SetRenderTarget(VideoOutput);
                                    EFVideoTexture.SetValue(KS.VideoTexture);
                                    XEffect.CurrentTechnique = SimpleTechnique;
                                    XEffect.CurrentTechnique.Passes[0].Apply();
                                    MiniPlane.Draw();

                                    XDevice.SetRenderTarget(null);
                                    switch (SaveMode)
                                    {
                                        case SaveModes.Vector4:
                                            OnIOEvent(this, DepthOutput.Vector4Screenshot(path + ".vector4"));
                                            break;
                                        case SaveModes.STL:
                                            OnIOEvent(this, DepthOutput.STLScreenshot(path + ".stl", TSTBLabel.Text));
                                            break;
                                    }
                                    OnIOEvent(this, VideoOutput.Screenshot(path + "_tex.png"));
                                }

                                if (ShadingMode == 2)
                                {
                                    XDevice.SetRenderTarget(DepthNormals);
                                    EFDepthTexture.SetValue(tick ? DepthVectorTarget1 : DepthVectorTarget2);
                                    XEffect.CurrentTechnique = DepthNormalShadingTechnique;
                                    XEffect.CurrentTechnique.Passes[0].Apply();
                                    MiniPlane.Draw();
                                }

                                ProcessProbes(tick ? DepthVectorTarget1 : DepthVectorTarget2);

                                XDevice.SetRenderTarget(null);
                                XDevice.DepthStencilState = DepthStencilState.Default;
                                XDevice.RasterizerState = RasterizerState.CullNone;
                                if (StereoMode == StereoModes.Anaglyph && StereoReady)
                                {
                                    XDevice.Clear(Microsoft.Xna.Framework.Color.Black);
                                    XDevice.SetRenderTarget(SecondaryTarget);
                                    XDevice.Clear(Microsoft.Xna.Framework.Color.Black);
                                }
                                XEffect.CurrentTechnique = DepthReprojectionTechnique;
                                                                
                                EFDepthTexture.SetValue(tick ? DepthVectorTarget1 : DepthVectorTarget2);
                                EFDepthNormalTexture.SetValue(DepthNormals);
                                EFVideoTexture.SetValue(KS.VideoTexture);
                                if (StereoReady)
                                {
                                    for (int vp = 0; vp < 2; vp++)
                                    {
                                        if (StereoMode != StereoModes.Anaglyph)
                                            XDevice.Viewport = vp == 0 ? LeftViewport : RightViewport;
                                        
                                        EFReprojectionTransform.SetValue(vp == 0 ? LeftReprojection : RightReprojection);
                                        EFStereoFocus.SetValue(vp == 0 ? -StereoFocus : StereoFocus);
                                        
                                        XEffect.CurrentTechnique.Passes[ShadingMode].Apply();
                                        Plane.Draw();

                                        if (StereoMode == StereoModes.Anaglyph)
                                        {
                                            XDevice.SetRenderTarget(null);
                                            XDevice.BlendState = BlendState.Additive;
                                            EPAnaglyphColor.SetValue(AnaglyphColors[vp]);
                                            XEffect.CurrentTechnique = AnaglyphTechnique;
                                            EFVideoTexture.SetValue(SecondaryTarget);
                                            XEffect.CurrentTechnique.Passes[0].Apply();
                                            MiniPlane.Draw();
                                            XEffect.CurrentTechnique = DepthReprojectionTechnique;
                                            EFVideoTexture.SetValue(KS.VideoTexture);
                                            XDevice.BlendState = BlendState.Opaque;
                                            XDevice.SetRenderTarget(SecondaryTarget);
                                            XDevice.Clear(Microsoft.Xna.Framework.Color.Black);
                                        }
                                     
                                    }
                                    XDevice.Viewport = DefaultViewport;
                                    XDevice.BlendState = BlendState.Opaque;
                                    EFStereoFocus.SetValue(Vector2.Zero);
                                }
                                else
                                {
                                    XEffect.CurrentTechnique.Passes[ShadingMode].Apply();
                                    Plane.Draw();
                                }
                                
                            }
                            else
                            {
                                XDevice.SetRenderTarget(VideoOutput);
                                EFDepthTexture.SetValue(tick ? DepthVectorTarget1 : DepthVectorTarget2);
                                XEffect.CurrentTechnique = DepthDisplayTechnique;
                                XEffect.CurrentTechnique.Passes[0].Apply();
                                MiniPlane.Draw();

                                if (Calibration && screenshotNext)
                                {
                                    (tick ? DepthVectorTarget1 : DepthVectorTarget2).DepthScreenshot(path+".float", DepthA, DepthB, DepthC);
                                }
                            }
                        }
                        else
                        {
                            XDevice.SetRenderTarget(VideoOutput);
                            switch (KS.Mode)
                            {
                                case Scanner.Modes.IR1024:
                                case Scanner.Modes.IR480:
                                    if (SF.CBAntidistortTest.Checked)
                                    {
                                        XEffect.CurrentTechnique = XEffect.Techniques["IRAntiDistort"];
                                    }
                                    else XEffect.CurrentTechnique = XEffect.Techniques["IR"];
                                    break;
                                case Scanner.Modes.RGB1024:
                                case Scanner.Modes.RGB480:
                                    if (SF.CBAntidistortTest.Checked)
                                    {
                                        XEffect.CurrentTechnique = XEffect.Techniques["RGBAntiDistort"];
                                    }
                                    else XEffect.CurrentTechnique = XEffect.Techniques["RGB"];
                                    break;
                            }
                            
                            EFVideoTexture.SetValue(KS.VideoTexture);
                            XEffect.CurrentTechnique.Passes[0].Apply();
                            MiniPlane.Draw();                            
                        }
                    }

                    if (!Reprojection || KS.Mode != KinectScanner.Modes.Depth480)
                    {
                        //int hDisp, vDisp;
                        //if (Rotation % 2 == 0)
                        //{
                        //    hDisp = (XDevice.PresentationParameters.BackBufferWidth - VideoOutput.Width) / 2;
                        //    vDisp = (XDevice.PresentationParameters.BackBufferHeight - VideoOutput.Height) / 2;
                        //}
                        //else
                        //{
                        //    hDisp = (XDevice.PresentationParameters.BackBufferWidth - VideoOutput.Height) / 2;
                        //    vDisp = (XDevice.PresentationParameters.BackBufferHeight - VideoOutput.Width) / 2;
                        //}
                        XDevice.SetRenderTarget(null);
                        XSprite.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
                        XSprite.Draw(VideoOutput, new Vector2(XDevice.PresentationParameters.BackBufferWidth / 2, XDevice.PresentationParameters.BackBufferHeight / 2), null, Microsoft.Xna.Framework.Color.White, Rotation * MathHelper.PiOver2, new Vector2(VideoOutput.Width, VideoOutput.Height) / 2, VideoScale, SpriteEffects.None, 0);
                        XSprite.End();
                    }
                    
                    if (screenshotNext && SaveMode == SaveModes.Screenshot)
                    {
                        if (KS.Mode == Scanner.Modes.Depth480)
                        {
                            XDevice.SetRenderTarget(null);
                            if(Reprojection) OnIOEvent(this, XDevice.Screenshot(path + ".bmp"));
                            else if (!Calibration) OnIOEvent(this, VideoOutput.ColorScreenshot(path + ".bmp"));
                        }
                        else 
                        {
                            OnIOEvent(this, VideoOutput.ColorScreenshot(path + ".bmp"));
                        }
                    }
                }
                else
                {
                    XDevice.SetRenderTarget(null);
                    if (ModelVertices != null)
                    {
                        XDevice.DepthStencilState = DepthStencilState.Default;
                        XDevice.RasterizerState = RasterizerState.CullNone;
                        XEffect.CurrentTechnique = ModelReprojectionTechnique;
                        EFVideoTexture.SetValue(ModelTexture);
                        XEffect.CurrentTechnique.Passes[ShadingMode].Apply();
                        XDevice.SetVertexBuffer(ModelVertexBuffer);
                        XDevice.Indices = ModelIndexBuffer;
                        XDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 640 * 480, 0, 639 * 479 * 2);
                        XDevice.SetVertexBuffer(null);
                        XDevice.Indices = null;
                    }
                    else
                    {
                        XSprite.Begin();
                        XSprite.Draw(LogoTexture, LogoPosition, null, Microsoft.Xna.Framework.Color.White, 0, LogoOrigin, LogoScale, SpriteEffects.None, 0f);
                        XSprite.End();
                    }
                }

                screenshotNext = false;
                if (RotationMode)
                {
                    System.Drawing.Point cursorPos = XPanel.PointToClient(Cursor.Position);
                    Vector2 delta = new Vector2(cursorPos.X - BaseCursorPos.X, cursorPos.Y - BaseCursorPos.Y)*0.2f;
                    ReprojectionRotationX = BaseReprojectionRotationX - delta.Y;
                    ReprojectionRotationY = BaseReprojectionRotationY + delta.X;
                    if (ReprojectionRotationX < -180) ReprojectionRotationX += 360;
                    if (ReprojectionRotationX > 180) ReprojectionRotationX -= 360;
                    if (ReprojectionRotationY < -180) ReprojectionRotationY += 360;
                    if (ReprojectionRotationY > 180) ReprojectionRotationY -= 360;
                    SF.NBReprojectionRotationX.ValueChangeEventEnabled = SF.NBReprojectionRotationY.ValueChangeEventEnabled = false;
                    SF.NBReprojectionRotationX.Value = ReprojectionRotationX;
                    SF.NBReprojectionRotationY.Value = ReprojectionRotationY;
                    SF.NBReprojectionRotationX.ValueChangeEventEnabled = SF.NBReprojectionRotationY.ValueChangeEventEnabled = true;
                    SetReprojection();
                }

                if (MoveMode)
                {
                    System.Drawing.Point cursorPos = XPanel.PointToClient(Cursor.Position);
                    Vector2 delta = new Vector2(cursorPos.X - BaseCursorPos.X, cursorPos.Y - BaseCursorPos.Y) * 20f / ReprojectionScale;
                    delta.X /= XPanel.Width;
                    delta.Y /= -XPanel.Height;
                    ReprojectionMove = BaseReprojectionMove + delta;
                    EFMove.SetValue(ReprojectionMove);
                }


                if (ProbesOn)
                {
                    Probes.Draw();
                }

                XDevice.SetRenderTarget(null);
                KSC.OnDrawFrame();
                XDevice.Present();

                //SBStatus.Text = string.Format("X{0} Y{1} Z{2} FPS{3}", new object[] { KS.AccelerationX.ToString(AccelerationFormat), KS.AccelerationY.ToString(AccelerationFormat), KS.AccelerationZ.ToString(AccelerationFormat), KS.FPS });
                if (CalibrationSave && CalibrationSaveCompleted != null) CalibrationSaveCompleted(this, null); 
            }

        }

        const float DefaultAspectRatio = 4f/3f;
        private void CreateXDevice()
        {
            XDevice = new GraphicsDevice(
                    GraphicsAdapter.DefaultAdapter,
                    GraphicsProfile.HiDef,
                    new PresentationParameters()
                    {
                        IsFullScreen = false,
                        DeviceWindowHandle = XPanel.Handle,
                        PresentationInterval = PresentInterval.One,
                        BackBufferWidth = XPanel.Width,
                        BackBufferHeight = XPanel.Height,
                        DepthStencilFormat = DepthFormat.Depth16,
                        BackBufferFormat=SurfaceFormat.Color,
                        RenderTargetUsage = RenderTargetUsage.PreserveContents
                    });
            SecondaryTarget = new RenderTarget2D(XDevice, XPanel.Width, XPanel.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            XDevice.RasterizerState = RasterizerState.CullNone;
            DefaultViewport = XDevice.Viewport;
            switch (StereoMode)
            {
                case StereoModes.LeftRight:
                    LeftViewport = new Viewport(0, 0, DefaultViewport.Width / 2, DefaultViewport.Height);
                    RightViewport = new Viewport(DefaultViewport.Width / 2, 0, DefaultViewport.Width / 2, DefaultViewport.Height);

                    break;
                case StereoModes.UpDown:
                    LeftViewport = new Viewport(0, 0, DefaultViewport.Width, DefaultViewport.Height / 2);
                    RightViewport = new Viewport(0, DefaultViewport.Height / 2, DefaultViewport.Width, DefaultViewport.Height / 2);
                    break;
            }
            StereoReady = StereoOn;
            InitGraphicsDeviceService();
            
            XContent = new ContentManager(this, Environment.CurrentDirectory + @"\Content");
            XEffect = XContent.Load<Effect>("Effects");            
            XFont = XContent.Load<SpriteFont>("font20");
            XSprite = new SpriteBatch(XDevice);
            ScaleTexture = XContent.Load<Texture2D>("scale");
            
            LogoTexture = XContent.Load<Texture2D>("logo");
            LogoOrigin = new Vector2(LogoTexture.Width / 2f, LogoTexture.Height / 2f);
            LogoPosition = new Vector2(XPanel.Width / 2f, XPanel.Height / 2f);
            if (LogoTexture.Width < XPanel.Width * LogoSpacing && LogoTexture.Height < XPanel.Height * LogoSpacing)
            {
                LogoScale = 1f;
            }
            else
            {
                Size fitSize = GetFitSize(new Size(LogoTexture.Width, LogoTexture.Height), new Size(XPanel.Width, XPanel.Height));
                LogoScale = (fitSize.Width / (float)LogoTexture.Width) * LogoSpacing;
            }


            Plane.SetDevice(XDevice);
            MiniPlane.SetDevice(XDevice);
            ModelVertexBuffer = new VertexBuffer(XDevice, VertexPositionTexture.VertexDeclaration, 640 * 480, BufferUsage.WriteOnly);
            ModelIndexBuffer = new IndexBuffer(XDevice, IndexElementSize.ThirtyTwoBits, 640 * 480 * 2 * 3, BufferUsage.WriteOnly);

            XEffect.Parameters["DepthHSLColoringPeriod"].SetValue(SF.NBRainbowPeriod.Value);
            XEffect.Parameters["DepthZLimit"].SetValue(SF.NBZLimit.Value);
            XEffect.Parameters["TriangleRemoveLimit"].SetValue(SF.NBTriangleRemove.Value);
            XEffect.Parameters["ScaleTexture"].SetValue(ScaleTexture);
            XEffect.Parameters["MinColoringDepth"].SetValue(SF.NBMinColoringDepth.Value);
            XEffect.Parameters["DepthToIR"].SetValue(DepthToIR);
            XEffect.Parameters["IR640To1280"].SetValue(IR640To1280);
            XEffect.Parameters["Color640To1280"].SetValue(Color640To1280);

            EFDepthTexture = XEffect.Parameters["DepthTexture"];
            EPDepthTextureA = XEffect.Parameters["DepthTextureA"];
            EPDepthTextureB = XEffect.Parameters["DepthTextureB"];
            EFDepthNormalTexture = XEffect.Parameters["DepthNormalTexture"];
            EFVideoTexture = XEffect.Parameters["VideoTexture"];
            EFMove = XEffect.Parameters["Move"];
            EFScale = XEffect.Parameters["Scale"];
            EPDepthScale = XEffect.Parameters["DepthScale"];
            EPDepthDisp = XEffect.Parameters["DepthDisp"];
            EFReprojectionTransform = XEffect.Parameters["ReprojectionTransform"];
            EFStereoFocus = XEffect.Parameters["StereoFocus"];
            EPAnaglyphColor = XEffect.Parameters["AnaglyphColor"];

            ModelReprojectionTechnique = XEffect.Techniques["ModelReprojection"];
            DepthAddTechnique = XEffect.Techniques["DepthAdd"];
            DepthAntiDistortAndAverageTechnique = XEffect.Techniques["DepthAntiDistortAndAverage"];
            DepthHGaussTechnique = XEffect.Techniques["DepthHGauss"];
            DepthVGaussTechnique = XEffect.Techniques["DepthVGauss"];
            PositionOutputShadingTechnique = XEffect.Techniques["PositionOutputShading"];
            DepthNormalShadingTechnique = XEffect.Techniques["DepthNormalShading"];
            DepthReprojectionTechnique = XEffect.Techniques["DepthReprojection"];
            AnaglyphDepthReprojectionTechnique = XEffect.Techniques["AnaglyphDepthReprojection"];
            DepthDisplayTechnique = XEffect.Techniques["DepthDisplay"];
            SimpleTechnique = XEffect.Techniques["Simple"];
            ReprojectionOutputTechnique = XEffect.Techniques["DepthReprojectionOutput"];
            DepthAverageTechnique = XEffect.Techniques["DepthAverage"];
            AnaglyphTechnique = XEffect.Techniques["Anaglyph"];


            EFScale.SetValue(ReprojectionScale);
            EFMove.SetValue(ReprojectionMove);

            XInitProbes();
            SetReprojection();
            SendModelToGraphicsMemory();
            XTimer.Start();
            
            if (Processing)
            {
                SetStatus(LocalizedStrings.RecreatingGPUProcessingPipeline);
                PrepareKinectGPUProcessingPipeline();
                KS.InitXNA(XDevice);
                KS.PrepareXNAResources();
                KS.EnableBufferUsage();
                SetStatus(LocalizedStrings.ProcessingResumed);
            }
            KSC.OnDeviceCreated(XDevice, XSprite, XEffect, XContent);
        }

        RenderTarget2D DepthVectorTarget1, DepthVectorTarget2;
        void PrepareKinectGPUProcessingPipeline()
        {
            ResetVideoOutput();
            DepthTarget = new RenderTarget2D(XDevice, 640, 480, false, SurfaceFormat.Vector4, DepthFormat.None);
            VideoCorrectionTexture = new Texture2D(XDevice, VideoDismapWidth, VideoDismapHeight, false, SurfaceFormat.Vector2);
            VideoCorrectionTexture.SetData<float>(VideoCorrection);
            DepthCorrectionTexture = new Texture2D(XDevice, DepthDismapWidth, DepthDismapHeight, false, SurfaceFormat.Vector2);
            DepthCorrectionTexture.SetData<float>(DepthCorrection);
            DepthVectorTarget1 = new RenderTarget2D(XDevice, KS.DepthWidth, KS.DepthHeight, false, SurfaceFormat.Vector2, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            DepthVectorTarget2 = new RenderTarget2D(XDevice, KS.DepthWidth, KS.DepthHeight, false, SurfaceFormat.Vector2, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            DepthNormals = new RenderTarget2D(XDevice, KS.DepthWidth, KS.DepthHeight, false, SurfaceFormat.Vector4, DepthFormat.None);            
            DepthOutput = new RenderTarget2D(XDevice, KS.DepthWidth, KS.DepthHeight, false, SurfaceFormat.Vector4, DepthFormat.None);

            XEffect.Parameters["NaN"].SetValue(float.NaN);
            XEffect.Parameters["DepthInverseIntrinsics"].SetValue(DepthInverseIntrinsics);
            XEffect.Parameters["DepthToColor"].SetValue(DepthToColorTransfrom);
            XEffect.Parameters["VideoCorrectionTexture"].SetValue(VideoCorrectionTexture);
            XEffect.Parameters["DepthCorrectionTexture"].SetValue(DepthCorrectionTexture);
            XEffect.Parameters["VideoCorrectionTextureSize"].SetValue(new Vector2(VideoDismapWidth, VideoDismapHeight));
            XEffect.Parameters["DepthCorrectionTextureSize"].SetValue(new Vector2(DepthDismapWidth, DepthDismapHeight));
            XEffect.Parameters["SimpleTransform"].SetValue(Matrix.Multiply(Matrix.CreateScale(2f, -2f, 1f), Matrix.CreateTranslation(-1, 1, 0)));
            XEffect.Parameters["DepthAvgCount"].SetValue((float)KS.DepthAvgFrameCount);
            XEffect.Parameters["DepthZLimit"].SetValue(SF.NBZLimit.Value);
            XEffect.Parameters["TriangleRemoveLimit"].SetValue(SF.NBTriangleRemove.Value);
            XEffect.Parameters["DepthHSLColoringPeriod"].SetValue(SF.NBRainbowPeriod.Value);
            XEffect.Parameters["DepthA"].SetValue(DepthA);
            XEffect.Parameters["DepthB"].SetValue(DepthB);
            XEffect.Parameters["DepthC"].SetValue(DepthC);

            EPDepthDisp.SetValue(new Vector2(SF.NBDepthDispX.Value,SF.NBDepthDispY.Value));
            EPDepthScale.SetValue(new Vector2(SF.NBDepthScaleX.Value, SF.NBDepthScaleY.Value));

            SetReprojection();
            SetGauss();
            KS.InitXNA(XDevice);

            PipelineInitProbes();
        }

        void DisposeKinectGPUProcessingPipeline()
        {            
            if (DepthTarget != null)
            {
                DepthTarget.Dispose();
                DepthTarget = null;
            }
            if (VideoCorrectionTexture != null)
            {
                VideoCorrectionTexture.Dispose();
                VideoCorrectionTexture = null;
            }
            if (DepthCorrectionTexture != null)
            {
                DepthCorrectionTexture.Dispose();
                DepthCorrectionTexture = null;
            }
            if (DepthVectorTarget1 != null)
            {
                DepthVectorTarget1.Dispose();
                DepthVectorTarget1 = null;
            }
            if (DepthVectorTarget2 != null)
            {
                DepthVectorTarget2.Dispose();
                DepthVectorTarget2 = null;
            }
            if (DepthNormals != null)
            {
                DepthNormals.Dispose();
                DepthNormals = null;
            }
            if (DepthOutput!=null)
            {
                DepthOutput.Dispose();
                DepthOutput = null;
            }
            if (VideoOutput != null)
            {
                VideoOutput.Dispose();
                VideoOutput = null;
            }

            PipelineShutdownProbes();
        }

        Microsoft.Xna.Framework.Rectangle VideoOutputRectangle;
        float VideoScale;
        void ResetVideoOutput()
        {
            if (VideoOutput != null)
            {
                VideoOutput.Dispose();
            }
            if (KS != null)
            {
                Size videoSize = KS.GetVideoSize(NextMode);
                Size fitSize;
                if (Rotation % 2 == 0)
                {
                    fitSize = GetFitSize(KS.GetVideoSize(NextMode), XPanel.Size);
                }
                else
                {
                    fitSize = GetFitSize(KS.GetVideoSize(NextMode), new Size(XPanel.Size.Height, XPanel.Size.Width));
                }
                VideoScale = (float)fitSize.Width / videoSize.Width;
                VideoOutput = new RenderTarget2D(XDevice, videoSize.Width, videoSize.Height, false, SurfaceFormat.Color, DepthFormat.Depth16);
                VideoOutputRectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, videoSize.Width, videoSize.Height);
            }
        }

        private void DestroyXDevice()
        {
            if (XDevice != null)
            {
                if (Processing)
                {
                    SetStatus(LocalizedStrings.ReleasingGPUProcessingPipeline);
                    KS.DisableBufferUsage();
                    KS.ReleaseXNAResources();
                    SetStatus(LocalizedStrings.ProcessingHalted);
                }

                XTimer.Stop();
                XContent.Unload();
                XContent.Dispose();
                XContent = null;
                if (DepthCorrectionTexture != null)
                {
                    DepthCorrectionTexture.Dispose();
                    DepthCorrectionTexture = null;
                }
                if (VideoCorrectionTexture != null)
                {
                    VideoCorrectionTexture.Dispose();
                    VideoCorrectionTexture = null;
                }
                if (ModelVertexBuffer != null)
                {
                    ModelVertexBuffer.Dispose();
                    ModelVertexBuffer = null;
                }
                if (ModelIndexBuffer != null)
                {
                    ModelIndexBuffer.Dispose();
                    ModelIndexBuffer = null;
                }
                if (ScaleTexture != null)
                {
                    ScaleTexture.Dispose();
                    ScaleTexture = null;
                }
                if (SecondaryTarget != null)
                {
                    SecondaryTarget.Dispose();
                    SecondaryTarget = null;
                }

                XSprite.Dispose();
                XSprite = null;
                if (DepthTarget != null)
                {
                    DepthTarget.Dispose();
                    DepthTarget = null;
                }
                if (DepthVectorTarget1 != null)
                {
                    DepthVectorTarget1.Dispose();
                    DepthVectorTarget1 = null;
                }
                if (DepthVectorTarget2!=null)
                {
                    DepthVectorTarget2.Dispose();
                    DepthVectorTarget2 = null;
                }
                if (VideoOutput != null)
                {
                    VideoOutput.Dispose();
                    VideoOutput = null;
                }
                XDevice.Dispose();
                XDevice = null;
            }
        }

        void SetGauss()
        {
            if (XEffect != null && KS!=null)
            {
                XEffect.SetGaussCoeffs(KS.DepthWidth, KS.DepthHeight, GaussCoeffCount, GaussSigma, GaussSigma);
            }
        }

        Matrix LeftReprojection, RightReprojection;
        void SetReprojection()
        {
            if (XEffect != null)
            {
                Matrix T = Matrix.Transpose(Matrix.CreateTranslation(0, 0, -ReprojectionTranslationZ));
                Matrix T2 = Matrix.Transpose(Matrix.CreateTranslation(ReprojectionTranslationX, ReprojectionTranslationY, 0));
                Matrix Ti = Matrix.Invert(T);
                Matrix R =
                    Matrix.CreateRotationZ(ReprojectionRotationZ / 180 * MathHelper.Pi) *
                    Matrix.CreateRotationX(ReprojectionRotationX / 180 * MathHelper.Pi) *
                    Matrix.CreateRotationY(ReprojectionRotationY / 180 * MathHelper.Pi) *
                    Matrix.CreateRotationZ(-Rotation * MathHelper.PiOver2);
                Matrix reproj = DepthIntrinsics * Ti * R * T* T2 * DepthInverseIntrinsics;
                Matrix reprojModel = DepthIntrinsics * Ti * R * T;
                LeftReprojection = DepthIntrinsics * Ti * R * T * T2 * Matrix.Transpose(Matrix.CreateTranslation(CameraOffset, 0f, 0f)) * DepthInverseIntrinsics;
                RightReprojection = DepthIntrinsics * Ti * R * T * T2 * Matrix.Transpose(Matrix.CreateTranslation(-CameraOffset, 0f, 0f)) * DepthInverseIntrinsics;
                StereoFocus = new Vector2(FocusedDepth, 0);
                XEffect.Parameters["ReprojectionTransform"].SetValue(reproj);
                XEffect.Parameters["ReprojectionModelTransform"].SetValue(reprojModel);
                Vector2 ModelScale;
                float AspectRatio = (float)XDevice.PresentationParameters.BackBufferWidth / XDevice.PresentationParameters.BackBufferHeight;
                if (Rotation % 2 == 0)
                {
                    if (AspectRatio >= DefaultAspectRatio)
                    {
                        ModelScale.Y = 1f;
                        ModelScale.X = DefaultAspectRatio / AspectRatio;
                    }
                    else
                    {
                        ModelScale.X = 1f;
                        ModelScale.Y = AspectRatio / DefaultAspectRatio;
                    }
                }
                else
                {
                    if (AspectRatio >= DefaultAspectRatio)
                    {
                        ModelScale.Y = 1f / DefaultAspectRatio;
                        ModelScale.X = 1f / AspectRatio;
                    }
                    else
                    {
                        ModelScale.X = DefaultAspectRatio;
                        ModelScale.Y = AspectRatio;
                    }
                }
                XEffect.Parameters["ModelScale"].SetValue(ModelScale);

                ReprojectProbes(reprojModel, ModelScale);
                if (KSC != null) KSC.OnReprojectionChanged(SF.NBReprojectionTranslationX.Value, SF.NBReprojectionTranslationY.Value, SF.NBReprojectionTranslationZ.Value, SF.NBReprojectionRotationX.Value, SF.NBReprojectionRotationY.Value, SF.NBReprojectionRotationZ.Value);
            }
        }
        #endregion

        #region Kinect
        int SelectedKinectIndex = -1;
        MenuItem[] DeviceMenuItems;
        private void RefreshKinectList()
        {
            while (MIDevices.MenuItems.Count > DevicesMenuLength)
            {
                MIDevices.MenuItems[DevicesMenuLength].Dispose();
            }
            int deviceCount = 0;
            try
            {
                deviceCount = Kinect.DeviceCount;
            }
            catch
            {

            }
            DeviceMenuItems = new MenuItem[deviceCount];
            for (int i = 0; i < deviceCount; i++)
            {
                MenuItem MI = new MenuItem("Device " + i.ToString());
                MI.RadioCheck = true;
                MI.Tag = i;
                MI.Click += TSMIDevice_Click;
                MIDevices.MenuItems.Add(MI);
                DeviceMenuItems[i] = MI;
            }
            MIVirtualDevice.Checked = false;
            if (deviceCount > 0)
            {
                if (SelectedKinectIndex == -1 || SelectedKinectIndex - 1 > deviceCount)
                    SelectedKinectIndex = 0;
            }
            else SelectedKinectIndex = -1;
            MIStart.Enabled = TSBStartStop.Enabled= MIModes.Enabled= true;
            SetActiveDevice(SelectedKinectIndex);
        }

        bool Processing;
        Scanner KS;
        KinectScanner.Modes NextMode;
        private bool InitKinect(int index)
        {
            if (KS == null || (KS != null && KS.Index != index))
            {
                if (KS != null) Disconnect();
                SetStatus(LocalizedStrings.Connecting);
                try
                {
                    if (index == -1)
                    {
                        FileScanner FS = new FileScanner();
                        KS = FS;
                        FS.FileIndexChanged += new EventHandler(FS_FileIndexChanged);
                        TSSMainSeparatorFile.Visible = TSBFilePrevious.Visible = TSLFileCount.Visible = TSBFileNext.Visible = true;
                        TSLFileCount.Text = "-";
                        FS.LoadImage(RawFilePath);
                    }
                    else
                        KS = new KinectScanner(index, BufferedFrames);
                    KS.IOEvent += OnIOEvent;
                    InitSequenceScanner(KS);
                    KSC.OnScannerCreated(KS);
                }
                catch(Exception e)
                {
                    SetStatus(LocalizedStrings.ConnectingAborted);
                    MessageBox.Show(e.Message);     
                    return false;
                }
                MIDisconnect.Enabled = true;
                MITilt.Enabled = true;
                SetStatus(LocalizedStrings.ConnectionEstablished);
                return true;
            }
            return true;
        }

        void FS_FileIndexChanged(object sender, EventArgs e)
        {
            FileScanner FS=sender as FileScanner;
            if (FS.FileIndex == -1)
            {
                TSLFileCount.Text = "-";
                Text = Resources.AppTitle + " - Virtual Kinect";
            }
            else
            {
                TSLFileCount.Text = string.Format("{0}/{1}", FS.FileIndex+1, FS.FileCount);
                Text = Resources.AppTitle+" - " + Path.GetFileName(FS.FilePath);
            }
        }

        private void OnIOEvent(object o, IOEventArgs e)
        {
            if (e.Success)
            {
                SetStatus(string.Format(LocalizedStrings.SaveSuccessful, e.Path));
            }
            else
            {
                MessageBox.Show(string.Format(LocalizedStrings.SaveUnsuccessful, e.Path, e.Message), LocalizedStrings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Start()
        {
            if (!Processing)
            {
                if (InitKinect(SelectedKinectIndex))
                {
                    if (KS.IsModeSupported(NextMode))
                    {
                        CloseModel();
                        screenshotNext = false;

                        SetStatus(LocalizedStrings.CreatingGPUProcessingPipeline);
                        PrepareKinectGPUProcessingPipeline();
                        SetStatus(LocalizedStrings.OpeningVideoStream);
                        KS.Start(NextMode);
                        KSC.OnProcessingStarted();
                        Processing = true;
                        FrameID = 0; 
                        SetStatus(LocalizedStrings.ProcessingStarted);
                        SaveButtonsEnabled(true, NextMode);
                        MIStart.Enabled = false;
                        MIStop.Enabled = true;                                               
                        TSBStartStop.Text = LocalizedStrings.Stop;
                        TSBStartStop.Image = Resources.stop16;
                        TSBProbes.Enabled = true;
                        TSBSequenceSave.Enabled = NextMode == Scanner.Modes.Depth480;                        
                        if (KS is FileScanner) Text = Resources.AppTitle + " - Virtual Kinect";
                        else Text = Resources.AppTitle + " - Device "+SelectedKinectIndex.ToString();
                    }
                    else
                    {
                        MessageBox.Show(string.Format(LocalizedStrings.ModeUnsupported, NextMode.ToString()), LocalizedStrings.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void Stop()
        {
            if (Processing)
            {
                KSC.OnProcessingStopping();
                SaveButtonsEnabled(false);
                MIStart.Enabled = true;
                MIStop.Enabled = false;
                SetStatus(LocalizedStrings.PrepareToShutdown);
                KS.Stop();
                DisposeKinectGPUProcessingPipeline();
                SetStatus(LocalizedStrings.ProcessingStopped); GC.Collect();
                Processing = false;
                TSBStartStop.Text = LocalizedStrings.Start;
                TSBStartStop.Image = Resources.start16;
                Text = Resources.AppTitle;
                TSBProbes.Enabled = false;
                StopSequence();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnect();            
            SaveSettings();
            KSC.OnProgramClosing();
            ShutdownProbes();
            NotifyIcon.Dispose();
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

        #region Control
        bool screenshotNext = false;
        private void MIStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void MIStart_Click(object sender, EventArgs e)
        {
            Start();
        }

        private void MIGrabFrame_Click(object sender, EventArgs e)
        {
            if (Processing)
                screenshotNext = true;
        }

        private void MITiltHorizontal_Click(object sender, EventArgs e)
        {
            TiltHorizontal();
        }

        const double TiltStep = 0.2;
        public void TiltUp()
        {
            KS.Tilt += TiltStep;
        }

        public void TiltDown()
        {
            KS.Tilt -= TiltStep;
        }

        public void TiltHorizontal()
        {
            KS.Tilt = 0;
        }

        private void MITiltUp_Click(object sender, EventArgs e)
        {
            TiltUp();
        }

        private void MITiltDown_Click(object sender, EventArgs e)
        {
            TiltDown();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    TiltUp();
                    break;
                case Keys.Down:
                    TiltDown();
                    break;
                case Keys.Left:
                case Keys.Right:
                    KS.Tilt = 0;
                    break;
                case Keys.Space:
                    screenshotNext = true;
                    break;
                case Keys.F5:
                    Start();
                    break;
                case Keys.F6:
                    if (TSBSequenceSave.Visible)
                    {
                        SetSequenceMode(true);
                        RecordSequence();
                    }
                    break;
                case Keys.F7:
                    if (TSBSequenceSave.Visible)
                    {
                        PauseSequence();
                    }
                    break;
                case Keys.F8:
                    if (TSBSequenceSave.Visible)
                    {
                        StopSequence();
                    }
                    break;
                case Keys.Tab:
                    SetStereo(!StereoOn);
                    break;
            }
            if (StereoReady)
            {
                switch (e.KeyCode)
                {
                    case Keys.NumPad6:
                        CameraOffset += 0.001f;
                        SetReprojection();
                        break;
                    case Keys.NumPad4:
                        CameraOffset -= 0.001f;
                        if (CameraOffset < 0) CameraOffset = 0;
                        SetReprojection();
                        break;
                    case Keys.NumPad8:
                        FocusedDepth += 0.01f;
                        SetReprojection();
                        break;
                    case Keys.NumPad2:
                        FocusedDepth -= 0.01f;
                        SetReprojection();
                        break;
                }
                SetStatus("CameraOffset=" + CameraOffset + ", FocusedDepth=" + FocusedDepth);
            }
        }
        #endregion

        void RotateRight()
        {
            Rotation++;
            if (Rotation > 3) Rotation = 0;
            ResetVideoOutput();
            SetReprojection();
        }

        void RotateLeft()
        {
            Rotation--;
            if (Rotation < 0) Rotation = 3;
            ResetVideoOutput();
            SetReprojection();
        }

        private void MIRotateRight_Click(object sender, EventArgs e)
        {
            RotateRight();
        }

        private void TSBStart_Click(object sender, EventArgs e)
        {
            if (Processing)
            {
                Stop();
            }
            else
            {
                Start();
            }
        }

        private void MIDisconnect_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void Disconnect()
        {
            if (KS != null)
            {
                Stop();
                KSC.OnScannerDisposing();
                KS.Destroy();
                KS = null;
                MIDisconnect.Enabled = false;
                MITilt.Enabled = false;
                TSSMainSeparatorFile.Visible = TSBFilePrevious.Visible = TSLFileCount.Visible = TSBFileNext.Visible = false;
            }
        }

        private void MIDevicesRefresh_Click(object sender, EventArgs e)
        {
            RefreshKinectList();
        }

        private void MIRotateLeft_Click(object sender, EventArgs e)
        {
            RotateLeft();
        }

        private void TSBRotateLeft_Click(object sender, EventArgs e)
        {
            RotateLeft();
        }

        private void TSBRotateRight_Click(object sender, EventArgs e)
        {
            RotateRight();
        }

        private void MIWorkingDirectory_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog FBD = new FolderBrowserDialog())
            {
                FBD.SelectedPath = WorkingDirectory;
                FBD.Description = LocalizedStrings.SelectWorkingDirectory;
                if (FBD.ShowDialog() == DialogResult.OK)
                {
                    WorkingDirectory = FBD.SelectedPath;
                    KSC.WorkingDirectory = WorkingDirectory;
                }
            }
        }

        System.Drawing.Point BaseCursorPos;
        bool RotationMode;
        bool MoveMode;
        Vector2 BaseReprojectionMove;
        float BaseReprojectionRotationX, BaseReprojectionRotationY;
        private void XPanel_MouseDown(object sender, MouseEventArgs e)
        {
            BaseCursorPos = e.Location;
            switch (e.Button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    if (!ProbesOn)
                    {
                        MoveMode = true;
                        BaseReprojectionMove = ReprojectionMove;
                    }
                    break;
                case System.Windows.Forms.MouseButtons.Right:
                    RotationMode = true;
                    BaseReprojectionRotationX = ReprojectionRotationX;
                    BaseReprojectionRotationY = ReprojectionRotationY;
                    break;
            }
        }

        private void XPanel_MouseLeave(object sender, EventArgs e)
        {
            RotationMode = false;
            MoveMode = false;
        }

        private void XPanel_MouseUp(object sender, MouseEventArgs e)
        {
            RotationMode = false;
            MoveMode = false;
        }

        private void TSBResetView_Click(object sender, EventArgs e)
        {
            ResetView();
        }

        private void MIResetView_Click(object sender, EventArgs e)
        {
            ResetView();
        }

        void ResetView()
        {
            SF.NBReprojectionRotationX.Value = 0;
            SF.NBReprojectionRotationY.Value = 0;
            SF.NBReprojectionRotationZ.Value = 0;
            SF.NBReprojectionTranslationX.Value = 0;
            SF.NBReprojectionTranslationY.Value = 0;
            ReprojectionMove = Vector2.Zero;
            EFMove.SetValue(ReprojectionMove);
            ReprojectionScale = 1f;
            EFScale.SetValue(ReprojectionScale);
            SetReprojection();
        }

        private void MIOpenSavedModel_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog OFD=new OpenFileDialog())
            {
                OFD.Filter = LocalizedStrings.ExportedModels+" (*.vector4)|*.vector4";
                OFD.Title = LocalizedStrings.OpenModel;
                if (OFD.ShowDialog() == DialogResult.OK)
                {
                    Stop();
                    LoadModel(OFD.FileName);                    
                }
            }
        }

        VertexPositionTexture[] ModelVertices;
        int[] ModelIndicies;
        VertexBuffer ModelVertexBuffer;
        IndexBuffer ModelIndexBuffer;
        Texture2D ModelTexture;
        MemoryStream ModelTextureStream;
        private void LoadModel(string path)
        {
            CloseModel();
            try
            {
                SetStatus(LocalizedStrings.LoadingModel);

                byte[] data = File.ReadAllBytes(path);

                string texPath = Path.GetDirectoryName(path) + '\\' + Path.GetFileNameWithoutExtension(path) + "_tex.png";
                if (File.Exists(texPath))
                {
                    ModelTextureStream = new MemoryStream(File.ReadAllBytes(texPath));
                }

                ModelFloatToXNAData(data, out ModelVertices, out ModelIndicies);
                SendModelToGraphicsMemory();

                SetStatus(LocalizedStrings.ModelLoaded);
                Text = Resources.AppTitle + " - " + Path.GetFileName(path);
            }
            catch
            {
                SetStatus(LocalizedStrings.ModelLoadingFailed);
            }
        }

        unsafe private void ModelFloatToXNAData(byte[] data, out VertexPositionTexture[] vertices, out int[] indicies)
        {
            int vertexCount = data.Length / 4 / 4;
            vertices = new VertexPositionTexture[vertexCount];
            int j = 0;
            int w = 640;
            int h = 480;
            int wt = w - 1;
            int ht = h - 1;
            fixed (byte* pdata = &data[0])
            fixed (VertexPositionTexture* pvertices = &vertices[0])
            {
                float* fdata = (float*)pdata;
                VertexPositionTexture* v = pvertices;
                for (int i = 0; i < vertexCount; i++)
                {
                    v->Position.X = *fdata++;
                    v->Position.Y = *fdata++;
                    v->Position.Z = *fdata++;
                    v->TextureCoordinate.X = (float)i % w / wt;
                    v->TextureCoordinate.Y = (float)(i / w) / ht;
                    fdata++;
                    v++;
                }
            }

            int cellCount = wt * ht;
            indicies = new int[cellCount * 2 * 3];
            fixed (int* pindex = &indicies[0])
            {
                int* index = pindex;
                for (j = 0; j < ht; j++)
                {
                    for (int i = 0; i < wt; i++)
                    {
                        *index++ = j * w + i;
                        *index++ = j * w + i + 1;
                        *index++ = (j + 1) * w + i;
                        *index++ = j * w + i + 1;
                        *index++ = (j + 1) * w + i + 1;
                        *index++ = (j + 1) * w + i;
                    }
                }
            }
        }

        private void CloseModel()
        {
            Text = Resources.AppTitle;
            ModelIndicies = null;
            ModelVertices = null;
            if (ModelTexture != null)
            {
                ModelTexture.Dispose();
                ModelTexture = null;
            }
            if (ModelTextureStream != null)
            {
                ModelTextureStream.Dispose();
                ModelTextureStream = null;
            }
        }

        private void SaveButtonsEnabled(bool enabled, Scanner.Modes mode = Scanner.Modes.Depth480)
        {
            TSLLabel.Visible = TSTBLabel.Visible = TSBSave.Visible = TSSMainSeparatorSave.Visible = MISaveTo.Visible = TSBSequenceSave.Visible = enabled;
            if (!enabled) SetSequenceMode(false);

            if (enabled)
            {
                SaveModes modes = SaveModes.None;
                switch (mode)
                {
                    case Scanner.Modes.Depth480:
                        modes = DepthSupportedSaveModes;
                        break;
                    case Scanner.Modes.IR1024:
                    case Scanner.Modes.IR480:
                        modes = IRSupportedSaveModes;
                        break;
                    case Scanner.Modes.RGB1024:
                    case Scanner.Modes.RGB480:
                        modes = ColorSupportedSaveModes;
                        break;
                }

                if (!modes.HasFlag(SaveMode) || SaveMode == SaveModes.None)
                {
                    if (modes.HasFlag(SaveModes.Screenshot))
                    {
                        SetSaveMode(SaveModes.Screenshot);
                    }
                    else
                    {
                        TSBSave.Visible = MISaveTo.Visible = false;
                    }
                }

                foreach (MenuItem MI in MISaveTo.MenuItems)
                {
                    MI.Visible = modes.HasFlag((SaveModes)MI.Tag);
                }
                foreach (ToolStripItem TSI in TSBSave.DropDownItems)
                {
                    TSI.Visible = modes.HasFlag((SaveModes)TSI.Tag);
                }
            }
        }

        private void SendModelToGraphicsMemory()
        {
            if (ModelVertices != null && ModelIndicies != null)
            {
                ModelVertexBuffer.SetData<VertexPositionTexture>(ModelVertices);
                ModelIndexBuffer.SetData<int>(ModelIndicies);
                if (ModelTextureStream != null)
                {
                    if (ModelTexture != null)
                    {
                        ModelTexture.Dispose();
                        ModelTexture = null;
                    }
                    try
                    {
                        ModelTextureStream.Position = 0;
                        ModelTexture = Texture2D.FromStream(XDevice, ModelTextureStream);
                    }
                    catch
                    {
                        ModelTexture = null;
                    }
                }
            }
        }


        private void TSMISave_Click(object sender, EventArgs e)
        {
            SaveModes SM = SaveModes.None;
            if (sender != TSBSave)
            {
                if (sender is MenuItem) SM = (SaveModes)(sender as MenuItem).Tag;
                if (sender is ToolStripDropDownItem) SM = (SaveModes)(sender as ToolStripDropDownItem).Tag;
            }
            if (SM != SaveModes.None) SetSaveMode(SM);
            if (Processing)
            {
                switch (SaveMode)
                {
                    case SaveModes.Screenshot:
                    case SaveModes.STL:
                    case SaveModes.Vector4:
                        screenshotNext = true;
                        break;
                    case SaveModes.Raw:
                        KS.SaveRawData( WorkingDirectory + "\\" + (TSTBLabel.Text == "" ? "" : TSTBLabel.Text.ToFileName() + "_") + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
                        break;
                }
            }
        }

        private void SetSaveMode(SaveModes mode)
        {
            SaveMode = mode;
            TSBSave.Text = mode.ToString();
        }

        private void TSMIDevice_Click(object sender, EventArgs e)
        {
            Stop();
            SetActiveDevice((int)(sender as MenuItem).Tag);
            Start();
        }

        string RawFilePath;
        private void MIOpenRawData_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog OFD = new OpenFileDialog())
            {
                OFD.Filter = LocalizedStrings.ExportedModels + " (*.rwd)|*.rwd";
                OFD.Title = LocalizedStrings.OpenModel;
                if (OFD.ShowDialog() == DialogResult.OK)
                {
                    LoadRawModel(OFD.FileName);
                }
            }
        }

        private void LoadRawModel(string path)
        {
            RawFilePath = path.Substring(0, path.LastIndexOf('.'));
            SetStatus(LocalizedStrings.LoadingModel);
            if (KS is FileScanner)
                (KS as FileScanner).LoadImage(RawFilePath);
            else
            {
                Stop();
                SetActiveDevice(-1);
                Start();                
            }
            Text = Resources.AppTitle + " - " + Path.GetFileName(path);
            SetStatus(LocalizedStrings.ModelLoaded);
        }

        private void SetActiveDevice(int id)
        {
            if (SelectedKinectIndex == -1)
                MIVirtualDevice.Checked = false;                
            else
                DeviceMenuItems[SelectedKinectIndex].Checked = false;
            SelectedKinectIndex = id;
            if (SelectedKinectIndex == -1)
            {
                SetMode(Scanner.Modes.Depth480);
                MIVirtualDevice.Checked = true;
                MICalibration.Enabled = false;
            }
            else
            {
                MICalibration.Enabled = true;
                DeviceMenuItems[SelectedKinectIndex].Checked = true;
            }
        }

        private void TSBFileNext_Click(object sender, EventArgs e)
        {
            if(KS is FileScanner)
            {
                FileScanner FS = KS as FileScanner;
                if (FS.FileIndex == FS.FileCount - 1)
                    FS.FileIndex = 0;
                else
                    FS.FileIndex++;
            }
        }

        private void TSBFilePrevious_Click(object sender, EventArgs e)
        {
            if (KS is FileScanner)
            {
                FileScanner FS = KS as FileScanner;
                if (FS.FileIndex == 0)
                    FS.FileIndex = FS.FileCount-1;
                else
                    FS.FileIndex--;
            }
        }

        private void MICalibration_Click(object sender, EventArgs e)
        {
            new CalibrationForm(this).ShowDialog();
        }

        bool Calibration;
        string CalibrationSavePath;
        public void StartCalibration()
        {
            Disconnect();
            SF.CBReproject.Checked = false;
            SF.NBGaussSigma.Value = 1;
            SF.NBGaussFilterPasses.Int32Value = 8;
            SF.NBZLimit.Value = 10;
            SF.CBAntidistortTest.Checked = false;
            SF.NBAveragedFrames.Int32Value = 15;
            Calibration = true;
        }

        public void CalibrationSave(string path)
        {
            SetSaveMode(SaveModes.Screenshot);
            CalibrationSavePath = path;
            screenshotNext = true;
        }

        public event EventHandler CalibrationSaveCompleted;

        public void StopCalibration()
        {
            Calibration = false;
        }

        bool StereoOn, StereoReady;
        public void SetStereo(bool enabled)
        {
            StereoOn = enabled;            
            if (StereoOn)
            {
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                FormBorderStyle = FormBorderStyle.Sizable;
                WindowState = FormWindowState.Normal;
            }
        }
    }
}
