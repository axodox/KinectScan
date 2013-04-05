using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Windows.Forms;
namespace KinectScan
{
    class ExCoreModeller:Modeller
    {
        public ExCoreModeller(KinectScanContext context)
            : base(context)
        {
            NextExCoreClear = new bool[2];
            ExCoreTick = new bool[2];
        }

        #region Visualization
        public enum SpecialViews { CorePosition, CoreReprojection, CorePolarDepth, CoreModel }
        public SpecialViews SpecialView { get; set; }
        public override string[] SpecialViewModes
        {
            get { return Extensions.EnumToStringArray<SpecialViews>(); }
        }
        public override int SpecialViewMode
        {
            get
            {
                return (int)SpecialView;
            }
            set
            {
                SpecialView = (SpecialViews)value;
            }
        }

        const float AngleStep = MathHelper.Pi / 36;
        const float DistanceStep = 0.01f;
        private float HorizantalAngle = 0f, VerticalAngle = 0f, Distance = 0.3f;
        void control_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (VerticalAngle + AngleStep < MathHelper.PiOver2) VerticalAngle += AngleStep;
                    break;
                case Keys.Down:
                    if (VerticalAngle - AngleStep > -MathHelper.PiOver2) VerticalAngle -= AngleStep;
                    break;
                case Keys.Right:
                    HorizantalAngle += AngleStep;
                    break;
                case Keys.Left:
                    HorizantalAngle -= AngleStep;
                    break;
                case Keys.NumPad1:
                    Distance += DistanceStep;
                    break;
                case Keys.NumPad0:
                    if (Distance - DistanceStep > 0) Distance -= DistanceStep;
                    break;
            }
        }
        #endregion

        #region Hardware acceleration
        RenderTarget2D[] ExCoreTargetA, ExCoreTargetB;
        EffectTechnique ColorModelTechnique, SimpleColorModelTechnique, PolarAddTechnique, PolarAvgTechnique, PolarOutputTechnique, PolarAvgDisplayTechnique, PolarDisplayTechnique, PolarToCartesianTechnique, PolarModelTechnique;
        EffectParameter ModelTransform, NormalTransform, WorldTransform, CorePos, CameraPosition, LightPosition, FiShift;
        VertexBuffer LinesVertexBuffer;
        Rectangle[] ExCoreTargetClippingRectangles, MainTargetClippingRectangles;
        
        public override void CreateDevice(Control control)
        {
            base.CreateDevice(control);
            control.KeyDown += control_KeyDown;

            //Render targets
            ExCoreTargetA = new RenderTarget2D[2];
            ExCoreTargetB = new RenderTarget2D[2];
            for (int i = 0; i < 2; i++)
            {
                ExCoreTargetA[i] = new RenderTarget2D(XDevice, Context.DepthWidth, Context.DepthHeight, false, SurfaceFormat.Vector2, DepthFormat.None);
                ExCoreTargetB[i] = new RenderTarget2D(XDevice, Context.DepthWidth, Context.DepthHeight, false, SurfaceFormat.Vector2, DepthFormat.None);
            }
            //Transforms
            ModelTransform = XEffect.Parameters["ModelTransform"];
            NormalTransform = XEffect.Parameters["NormalTransform"];
            WorldTransform = XEffect.Parameters["WorldTransform"];

            //Parameters
            CorePos = XEffect.Parameters["CorePos"];
            CameraPosition = XEffect.Parameters["CameraPosition"];
            LightPosition = XEffect.Parameters["LightPosition"];
            FiShift = XEffect.Parameters["FiShift"];

            //Techniques
            ColorModelTechnique = XEffect.Techniques["ColorModel"];
            PolarAddTechnique = XEffect.Techniques["PolarAdd"];
            PolarAvgTechnique = XEffect.Techniques["PolarAvg"];
            PolarOutputTechnique = XEffect.Techniques["PolarOutput"];
            PolarAvgDisplayTechnique = XEffect.Techniques["PolarAvgDisplay"];
            PolarDisplayTechnique = XEffect.Techniques["PolarDisplay"];
            PolarToCartesianTechnique = XEffect.Techniques["PolarToCartesian"];
            PolarModelTechnique = XEffect.Techniques["PolarModel"];
            SimpleColorModelTechnique = XEffect.Techniques["SimpleColorModel"];

            //Vertex buffers
            LinesVertexBuffer = new VertexBuffer(XDevice, VertexPositionColor.VertexDeclaration, DisplayLines, BufferUsage.WriteOnly);

            //Clip rectangles
            ExCoreTargetClippingRectangles = new Rectangle[2];
            ExCoreTargetClippingRectangles[0] = new Rectangle(0, 0, Context.DepthWidth / 2, Context.DepthHeight);
            ExCoreTargetClippingRectangles[1] = new Rectangle(Context.DepthWidth / 2, 0, Context.DepthWidth / 2, Context.DepthHeight);

            MainTargetClippingRectangles = new Rectangle[2];
            MainTargetClippingRectangles[0] = new Rectangle(0, 0, XDevice.PresentationParameters.BackBufferWidth / 2, XDevice.PresentationParameters.BackBufferHeight);
            MainTargetClippingRectangles[1] = new Rectangle(XDevice.PresentationParameters.BackBufferWidth / 2, 0, XDevice.PresentationParameters.BackBufferWidth / 2, XDevice.PresentationParameters.BackBufferHeight);
        }

        public override void DestroyDevice()
        {
            base.DestroyDevice();
            for (int i = 0; i < 2; i++)
            {
                ExCoreTargetA[i].Dispose(); ExCoreTargetA[i] = null;
                ExCoreTargetB[i].Dispose(); ExCoreTargetB[i] = null;
            }
            LinesVertexBuffer.Dispose(); LinesVertexBuffer = null;
        }
        #endregion

        #region Transforms
        private void SetModelProjection(float rotation)
        {
            Matrix T = Matrix.Transpose(Matrix.CreateTranslation(0, 0, TranslationZ));
            Matrix R = Matrix.CreateRotationY(rotation);
            Matrix reproj = Context.DepthIntrinsics * T * R;
            ModelTransform.SetValue(reproj);
        }
        private Matrix SetModelVisualizationProjection(int leg)
        {
            Vector3 cameraPosition = Distance * new Vector3(
                    (float)(Math.Cos(HorizantalAngle) * Math.Cos(VerticalAngle)), 
                    (float)(Math.Sin(HorizantalAngle) * Math.Cos(VerticalAngle)), 
                    (float)(Math.Sin(VerticalAngle)));
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, XDevice.DisplayMode.AspectRatio, 0.001f, 5);
            Matrix View = Matrix.CreateLookAt(
                cameraPosition,
                Vector3.Zero,
                Vector3.UnitZ);
            Matrix World = Matrix.CreateRotationX(-MathHelper.PiOver2)*Matrix.CreateRotationZ(-MathHelper.PiOver2)*Matrix.CreateScale(-1f,1f,1f);
            Matrix MainWorld = World;
            World *= (leg == 2 ? Matrix.CreateTranslation(0f, 0f, -(ProjectionYMax + ProjectionYMin) / 2f) : Matrix.CreateTranslation(CoreX * (leg == 0 ? -1f : 1f), CoreY, -(ProjectionYMax + ProjectionYMin) / 2f));

            LightPosition.SetValue(cameraPosition);
            CameraPosition.SetValue(cameraPosition);
            WorldTransform.SetValue(World);
            ModelTransform.SetValue(World*View*Projection);
            NormalTransform.SetValue(Matrix.Transpose(Matrix.Invert(World)));
            return MainWorld * View * Projection;
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
            FusionTransform.SetValue(Matrix.CreateRotationY(-SplitPlaneAngle + MathHelper.PiOver2) * space);
        }
        #endregion

        #region Processing
        public float CoreX { get; set; }
        public float CoreY { get; set; }

        const int DisplayLines = 12;
        bool[] NextExCoreClear, ExCoreTick;

        protected override void ProcessLeg(int leg, bool visualize, Viewport viewport, bool present, bool clear)
        {
            //Visualizing axes
            if (ViewMode == Views.Special && SpecialView == SpecialViews.CorePosition && leg == VisualizedLeg)
            {
                Visualize(SingleTargetTick ? SingleTargetA : SingleTargetB, SignedDepthVisualizationTechnique, MainViewport, false);
                SetModelProjection(-mainRotation + SplitPlaneAngle + MathHelper.PiOver2);
                VisualizeAxes(ColorModelTechnique);
                XDevice.Present();
            }

            //Polar reprojection
            XDevice.RasterizerState = new RasterizerState() { ScissorTestEnable = true, CullMode = CullMode.CullCounterClockwiseFace };
            XDevice.SetRenderTarget(SingleTargetTick ? SingleTargetB : SingleTargetA);
            XDevice.Clear(Color.Black);
            DepthSampler.SetValue(SingleTargetTick ? SingleTargetA : SingleTargetB);
            CorePos.SetValue(new Vector3(CoreY, 0, leg == 0 ? -CoreX : CoreX));
            SetExCoreReprojection(mainRotation);
            XEffect.CurrentTechnique = PolarOutputTechnique;
            for (int half = 0; half < 2; half++)
            {
                XDevice.ScissorRectangle = ExCoreTargetClippingRectangles[half];
                FiShift.SetValue(half == 0 ? 0f : 1f);
                XEffect.CurrentTechnique.Passes[0].Apply();
                MaxiPlane.Draw();
            }

            if (ViewMode == Views.Special && SpecialView == SpecialViews.CoreReprojection && visualize)
            {
                XDevice.SetRenderTarget(null);
                XDevice.Viewport = viewport;
                if (clear) XDevice.Clear(Color.White);
                XEffect.CurrentTechnique = PolarDisplayTechnique;
                for (int half = 0; half < 2; half++)
                {
                    XDevice.ScissorRectangle = MainTargetClippingRectangles[half];
                    FiShift.SetValue(half == 0 ? 0f : 1f);
                    XEffect.CurrentTechnique.Passes[0].Apply();
                    MaxiPlane.Draw();
                }
                if (present) XDevice.Present();
                XDevice.Viewport = MainViewport;
            }

            XDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            //Polar sum
            XDevice.SetRenderTarget(ExCoreTick[leg] ? ExCoreTargetA[leg] : ExCoreTargetB[leg]);
            if (NextExCoreClear[leg])
            {
                XDevice.Clear(ClearOptions.Target, Color.Black, 1, 0);
                NextExCoreClear[leg] = false;
            }
            else
            {
                DepthSamplerA.SetValue(SingleTargetTick ? SingleTargetB : SingleTargetA);
                DepthSamplerB.SetValue(ExCoreTick[leg] ? ExCoreTargetB[leg] : ExCoreTargetA[leg]);
                XEffect.CurrentTechnique = PolarAddTechnique;
                XEffect.CurrentTechnique.Passes[0].Apply();
                MiniPlane.Draw();
            }

            //Averaged visualization
            if (ViewMode == Views.Special && SpecialView == SpecialViews.CorePolarDepth && visualize)
            {
                XDevice.SetRenderTarget(null);
                XDevice.Viewport = viewport;
                if (clear) XDevice.Clear(Color.White);
                DepthSampler.SetValue(ExCoreTick[leg] ? ExCoreTargetA[leg] : ExCoreTargetB[leg]);
                XEffect.CurrentTechnique = PolarAvgDisplayTechnique;
                XEffect.CurrentTechnique.Passes[0].Apply();
                MiniPlane.Draw();
                if (present) XDevice.Present();
                XDevice.Viewport = MainViewport;
            }

            VisualizeModel(leg);
            ExCoreTick[leg] = !ExCoreTick[leg];
        }

        private void VisualizeAxes(EffectTechnique technique)
        {
            VertexPositionColor[] VPC = new VertexPositionColor[10];
            VPC[0] = new VertexPositionColor(new Vector3(0, -0.1f, 0), Color.Yellow);
            VPC[1] = new VertexPositionColor(new Vector3(0, 0.1f, 0), Color.Yellow);
            VPC[2] = new VertexPositionColor(new Vector3(0.1f, 0f, 0), Color.Yellow);
            VPC[3] = new VertexPositionColor(new Vector3(-0.1f, 0f, 0), Color.Yellow);
            VPC[4] = new VertexPositionColor(new Vector3(0, 0f, 0.1f), Color.Yellow);
            VPC[5] = new VertexPositionColor(new Vector3(0, 0f, -0.1f), Color.Yellow);
            VPC[6] = new VertexPositionColor(new Vector3(-CoreY, -1, CoreX), Color.Green);
            VPC[7] = new VertexPositionColor(new Vector3(-CoreY, 1, CoreX), Color.Green);
            VPC[8] = new VertexPositionColor(new Vector3(-CoreY, -1, -CoreX), Color.Red);
            VPC[9] = new VertexPositionColor(new Vector3(-CoreY, 1, -CoreX), Color.Red);            
            LinesVertexBuffer.SetData<VertexPositionColor>(VPC);
            XDevice.SetVertexBuffer(LinesVertexBuffer);
            XEffect.CurrentTechnique = technique;
            XEffect.CurrentTechnique.Passes[0].Apply();
            XDevice.DrawPrimitives(PrimitiveType.LineList, 0, 5);
        }

        protected override void OnRefresh()
        {
            VisualizeModel(0);
            VisualizeModel(1);
        }
     
        private void VisualizeModel(int leg)
        {
            //Model visualization
            if (ViewMode == Views.Special && SpecialView == SpecialViews.CoreModel &&(VisualizedLeg==2||VisualizedLeg==leg))
            {
                //Averaging
                XDevice.SetRenderTarget(SingleTargetTick ? SingleTargetB : SingleTargetA);
                XDevice.Clear(Color.Black);
                DepthSampler.SetValue(ExCoreTick[leg] ? ExCoreTargetA[leg] : ExCoreTargetB[leg]);
                XEffect.CurrentTechnique = PolarAvgTechnique;
                XEffect.CurrentTechnique.Passes[0].Apply();
                MiniPlane.Draw();

                //Gauss-filtering
                for (int i = 0; i < 0; i++)
                {
                    XDevice.SetRenderTarget(SingleTargetTick ? SingleTargetA : SingleTargetB);
                    DepthSampler.SetValue(SingleTargetTick ? SingleTargetB : SingleTargetA);
                    XEffect.CurrentTechnique = VGaussTechnique;
                    XEffect.CurrentTechnique.Passes[0].Apply();
                    MiniPlane.Draw();
                    SingleTargetTick = !SingleTargetTick;

                    XDevice.SetRenderTarget(SingleTargetTick ? SingleTargetA : SingleTargetB);
                    DepthSampler.SetValue(SingleTargetTick ? SingleTargetB : SingleTargetA);
                    XEffect.CurrentTechnique = HGaussTechnique;
                    XEffect.CurrentTechnique.Passes[0].Apply();
                    MiniPlane.Draw();
                    SingleTargetTick = !SingleTargetTick;
                }

                //Polar to Cartesian
                XDevice.SetRenderTarget(Vector4Target);
                XDevice.Clear(Color.Black);
                DepthSampler.SetValue(SingleTargetTick ? SingleTargetB : SingleTargetA);
                XEffect.CurrentTechnique = PolarToCartesianTechnique;
                XEffect.CurrentTechnique.Passes[0].Apply();
                MiniPlane.Draw();

                //Draw model
                XDevice.SetRenderTarget(null);
                if (leg == 0 || VisualizedLeg!=2) XDevice.Clear(Color.White);
                XDevice.DepthStencilState = DepthStencilState.Default;
                XDevice.RasterizerState = RasterizerState.CullNone;
                MainSampler.SetValue(Vector4Target);
                Matrix gridWorldViewProj = SetModelVisualizationProjection(VisualizedLeg == 2 ? leg : 2);
                XEffect.CurrentTechnique = PolarModelTechnique;
                XEffect.CurrentTechnique.Passes[0].Apply();
                
                MaxiPlane.Draw();
                ModelTransform.SetValue(gridWorldViewProj);
                VisualizeAxes(SimpleColorModelTechnique);
                if (leg == 1 || VisualizedLeg != 2)
                    XDevice.Present();
            }
        }

        public override void Clear()
        {
            base.Clear();
            for (int i = 0; i < 2; i++)
                NextExCoreClear[i] = true;
        }
        #endregion

        #region Save model
        public override void DebugSave()
        {
            for (int leg = 0; leg < 2; leg++)
            {
                RenderTarget2D tex = ExCoreTick[leg] ? ExCoreTargetB[leg] : ExCoreTargetA[leg];
                tex.VectorScreenshot("leg" + leg + ".excore", 2);
            }
        }

        public override void DebugLoad()
        {
            for (int leg = 0; leg < 2; leg++)
            {
                RenderTarget2D tex = ExCoreTick[leg] ? ExCoreTargetA[leg] : ExCoreTargetB[leg];
                tex.LoadFromFile("leg" + leg + ".excore");
            }
        }

        const float ExportScale = 1000f;
        public override void Save(string fileName)
        {
            for (int leg = 0; leg < 2; leg++)
            {
                //Averaging
                XDevice.SetRenderTarget(SingleTargetTick ? SingleTargetB : SingleTargetA);
                XDevice.Clear(Color.Black);
                DepthSampler.SetValue(ExCoreTick[leg] ? ExCoreTargetA[leg] : ExCoreTargetB[leg]);
                XEffect.CurrentTechnique = PolarAvgTechnique;
                XEffect.CurrentTechnique.Passes[0].Apply();
                MiniPlane.Draw();

                //Gauss-filtering :D
                for (int i = 0; i < 16; i++)
                {
                    XDevice.SetRenderTarget(SingleTargetTick ? SingleTargetA : SingleTargetB);
                    DepthSampler.SetValue(SingleTargetTick ? SingleTargetB : SingleTargetA);
                    XEffect.CurrentTechnique = VGaussTechnique;
                    XEffect.CurrentTechnique.Passes[0].Apply();
                    MiniPlane.Draw();
                    SingleTargetTick = !SingleTargetTick;

                    XDevice.SetRenderTarget(SingleTargetTick ? SingleTargetA : SingleTargetB);
                    DepthSampler.SetValue(SingleTargetTick ? SingleTargetB : SingleTargetA);
                    XEffect.CurrentTechnique = HGaussTechnique;
                    XEffect.CurrentTechnique.Passes[0].Apply();
                    MiniPlane.Draw();
                    SingleTargetTick = !SingleTargetTick;
                }

                XDevice.SetRenderTarget(null);
                STLScreenshot(SingleTargetTick ? SingleTargetB : SingleTargetA, Path.ChangeExtension(fileName,leg+".stl"), "Created by KinectScan.", MinY.GetValueSingle(), MaxHeight.GetValueSingle());
            }
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
                return new Vector3((float)Math.Cos(fi) * r, h, (float)Math.Sin(fi) * r);
            }
        }
        
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

        int SaveBufferSize = 128;
        public unsafe IOEventArgs STLScreenshot(RenderTarget2D target, string path, string title, float minH, float maxHeight)
        {
            try
            {
                int width = target.Width;
                int height = target.Height;

                FromPolarSettings FPS;
                FPS.width = width;
                FPS.height = height;
                FPS.minH = minH;
                FPS.stepH = maxHeight / height;
                FPS.stepFi = MathHelper.TwoPi / width;
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
                                else if (firstAHeight != null && firstBHeight != null && lastAHeight != null && lastBHeight != null && lastAHeight - firstAHeight > width * 0.97f && lastBHeight - firstBHeight > width * 0.97f)
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
            catch (Exception e)
            {
                return new IOEventArgs(path, true, null);
            }
        }
        #endregion
    }
}