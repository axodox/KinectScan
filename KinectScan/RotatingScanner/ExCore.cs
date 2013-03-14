using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
namespace KinectScan
{
    class ExCoreModeller:Modeller
    {
        public ExCoreModeller(KinectScanContext context)
            : base(context)
        {

        }

        #region Visualization
        public enum SpecialViews { CorePosition, CoreReprojection, CorePolarDepth }
        public SpecialViews SpecialView { get; set; }
        public override string[] SpecialViewModes
        {
            get { return Extensions.EnumToStringArray<SpecialViews>(); }
        }
        public override void SetSpecialViewMode(int index)
        {
            SpecialView = (SpecialViews)index;
        }
        #endregion

        #region Hardware acceleration
        RenderTarget2D ExCoreTargetA, ExCoreTargetB;
        EffectTechnique ColorModelTechnique, PolarAddTechnique, PolarAvgTechnique, PolarOutputTechnique, PolarAvgDisplayTechnique, PolarDisplayTechnique;
        EffectParameter ModelTransform, CorePos;
        VertexBuffer LinesVertexBuffer;
        
        public override void CreateDevice(System.IntPtr windowHandle)
        {
            base.CreateDevice(windowHandle);

            //Render targets
            ExCoreTargetA = new RenderTarget2D(XDevice, Context.DepthWidth, Context.DepthHeight, false, SurfaceFormat.Vector2, DepthFormat.None);
            ExCoreTargetB = new RenderTarget2D(XDevice, Context.DepthWidth, Context.DepthHeight, false, SurfaceFormat.Vector2, DepthFormat.None); 

            //Transforms
            ModelTransform = XEffect.Parameters["ModelTransform"];

            //Parameters
            CorePos = XEffect.Parameters["CorePos"];

            //Techniques
            ColorModelTechnique = XEffect.Techniques["ColorModel"];
            PolarAddTechnique = XEffect.Techniques["PolarAdd"];
            PolarAvgTechnique = XEffect.Techniques["PolarAvg"];
            PolarOutputTechnique = XEffect.Techniques["PolarOutput"];
            PolarAvgDisplayTechnique = XEffect.Techniques["PolarAvgDisplay"];
            PolarDisplayTechnique = XEffect.Techniques["PolarDisplay"];

            //Vertex buffers
            LinesVertexBuffer = new VertexBuffer(XDevice, VertexPositionColor.VertexDeclaration, DisplayLines, BufferUsage.WriteOnly);
        }

        public override void DestroyDevice()
        {
            base.DestroyDevice();
            ExCoreTargetA.Dispose(); ExCoreTargetA = null;
            ExCoreTargetB.Dispose(); ExCoreTargetB = null;
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
            FusionTranform.SetValue(Matrix.CreateRotationY(-SplitPlaneAngle + MathHelper.PiOver2) * space);
        }
        #endregion

        #region Processing
        public float CoreX { get; set; }
        public float CoreY { get; set; }

        const int DisplayLines = 12;
        bool ExCoreTick, NextExCoreClear;
        
        public override void ProcessFrame()
        {
            base.ProcessFrame();

            if (ViewMode == Views.Special && SpecialView == SpecialViews.CorePosition)
            {
                Visualize(FusionTick ? SingleTargetA : SingleTargetB, SignedDepthVisualizationTechnique, false);
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
                SetModelProjection(-mainRotation + SplitPlaneAngle + MathHelper.PiOver2);
                LinesVertexBuffer.SetData<VertexPositionColor>(VPC);
                XDevice.SetVertexBuffer(LinesVertexBuffer);
                XEffect.CurrentTechnique = ColorModelTechnique;
                XEffect.CurrentTechnique.Passes[0].Apply();
                XDevice.DrawPrimitives(PrimitiveType.LineList, 0, 5);
                XDevice.Present();
            }

            XDevice.SetRenderTarget(FusionTick ? SingleTargetB : SingleTargetA);
            XDevice.Clear(Color.Black);
            DepthSampler.SetValue(FusionTick ? SingleTargetA : SingleTargetB);
            XDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            CorePos.SetValue(new Vector3(CoreY, 0, -CoreX));
            SetExCoreReprojection(mainRotation);
            XEffect.CurrentTechnique = PolarOutputTechnique;
            XEffect.CurrentTechnique.Passes[0].Apply();
            MaxiPlane.Draw();

            if (ViewMode == Views.Special && SpecialView==SpecialViews.CoreReprojection)
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
            if (ViewMode == Views.Special && SpecialView==SpecialViews.CorePolarDepth)
            {
                XDevice.SetRenderTarget(null);
                XDevice.Clear(Color.White);
                DepthSampler.SetValue(ExCoreTick ? ExCoreTargetA : ExCoreTargetB);
                XEffect.CurrentTechnique = PolarAvgDisplayTechnique;
                XEffect.CurrentTechnique.Passes[0].Apply();
                MiniPlane.Draw();
                XDevice.Present();
            }

            if (ExCoreSaveTo != null)
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
                STLScreenshot(FusionTick ? SingleTargetB : SingleTargetA, ExCoreSaveTo, "Created by KinectScan.", MinY.GetValueSingle(), MaxHeight.GetValueSingle());                
                ExCoreSaveTo = null;
            }
            ExCoreTick = !ExCoreTick;
        }        

        public override void Clear()
        {
            base.Clear();
            NextExCoreClear = true;
        }
        #endregion

        #region Save model        
        private string ExCoreSaveTo;
        const float ExportScale = 1000f;
        public override void Save(string fileName)
        {
            ExCoreSaveTo = fileName;
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