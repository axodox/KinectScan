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
using KinectScan.Properties;
using AntiDistortion;
using System.Globalization;

namespace KinectScan
{
    public partial class MainForm
    {
        SettingsForm SF;
        string DepthDistortionSettings, VideoDistortionSettings;
        float DepthA, DepthB, DepthC, DepthMin;
        Vector2 DepthToIRDisp, DepthToIRScale;
        Vector4 DepthToIR, IR640To1280, Color640To1280;

        private void MISettings_Click(object sender, EventArgs e)
        {
            SF.Show();
        }

        private void LoadHardwareSettings(string path)
        {
            CultureInfo CI = new CultureInfo("en-US");
            XmlReader XR = XmlReader.Create(path);
            while (XR.Name != "KinectScan" && !XR.EOF)
                XR.Read();
            if (XR.Name == "KinectScan")
            {
                while (XR.Name != "KinectScan" || XR.NodeType != XmlNodeType.EndElement)
                {
                    XR.Read();
                    switch (XR.Name)
                    {
                        case "DepthCamera":
                            if (XR.HasAttributes)
                            {
                                while (XR.MoveToNextAttribute())
                                {
                                    switch (XR.Name)
                                    {
                                        case "Intrinsic":
                                            DepthIntrinsics = Extensions.MatrixFromString(XR.Value);
                                            DepthInverseIntrinsics = Matrix.Invert(DepthIntrinsics);
                                            break;
                                        case "Extrinsic":
                                            DepthInverseExtrinsics = Matrix.Invert(Extensions.MatrixFromString(XR.Value));
                                            break;
                                        case "Distortion":
                                            DepthDistortionSettings = XR.Value;
                                            break;
                                        case "A":
                                            DepthA = float.Parse(XR.Value, CI);
                                            break;
                                        case "B":
                                            DepthB = float.Parse(XR.Value, CI);
                                            break;
                                        case "C":
                                            DepthC = float.Parse(XR.Value, CI);
                                            break;
                                        case "DepthToIRScaleX":
                                            DepthToIRScale.X = float.Parse(XR.Value, CI);
                                            break;
                                        case "DepthToIRScaleY":
                                            DepthToIRScale.Y = float.Parse(XR.Value, CI);
                                            break;
                                        case "DepthToIRDispX":
                                            DepthToIRDisp.X = float.Parse(XR.Value, CI);
                                            break;
                                        case "DepthToIRDispY":
                                            DepthToIRDisp.Y = float.Parse(XR.Value, CI);
                                            break;
                                    }
                                }
                            }
                            break;
                        case "VideoCamera":
                            if (XR.HasAttributes)
                            {
                                while (XR.MoveToNextAttribute())
                                {
                                    switch (XR.Name)
                                    {
                                        case "Intrinsic":
                                            VideoIntrinsics = Extensions.MatrixFromString(XR.Value);
                                            break;
                                        case "Extrinsic":
                                            VideoExtrinsics = Extensions.MatrixFromString(XR.Value);
                                            break;
                                        case "Distortion":
                                            VideoDistortionSettings = XR.Value;
                                            break;
                                    }
                                }
                            }
                            break;
                        case "Mapping":
                            if (XR.HasAttributes)
                            {
                                while (XR.MoveToNextAttribute())
                                {
                                    switch (XR.Name)
                                    {
                                        case "DepthToIR":
                                            DepthToIR = Extensions.Vector4FromString(XR.Value);
                                            break;
                                        case "Color640To1280":
                                            Color640To1280 = Extensions.Vector4FromString(XR.Value);
                                            break;
                                        case "IR640To1280":
                                            IR640To1280 = Extensions.Vector4FromString(XR.Value);
                                            break;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            XR.Close();


            //Mapping
            Matrix MappingColor640To1280 = new Matrix(
                Color640To1280.X / 640f, 0, 0, Color640To1280.Z,
                0, Color640To1280.Y / 480f, 0, Color640To1280.W,
                0, 0, 1, 0,
                0, 0, 0, 1);
            VideoIntrinsics = Matrix.Invert(MappingColor640To1280) * VideoIntrinsics;
            Matrix MappingDepthToIR = new Matrix(
                DepthToIR.X / 640f, 0, 0, DepthToIR.Z,
                0, DepthToIR.Y / 480f, 0, DepthToIR.W,
                0, 0, 1, 0,
                0, 0, 0, 1);
            Matrix MappingIR640To1280 = new Matrix(
                IR640To1280.X / 640f, 0, 0, IR640To1280.Z,
                0, IR640To1280.Y / 480f, 0, IR640To1280.W,
                0, 0, 1, 0,
                0, 0, 0, 1);
            DepthIntrinsics = Matrix.Invert(MappingDepthToIR) * DepthIntrinsics;
            DepthInverseIntrinsics = Matrix.Invert(DepthIntrinsics);

            DepthMin = DepthA - DepthB / (-DepthC);
            if (!Directory.Exists("DistortionMaps")) Directory.CreateDirectory("DistortionMaps");
            if (Settings.Default.VideoDistortion != VideoDistortionSettings || !File.Exists("DistortionMaps/color.dismap"))
            {
                CreateDistortionMap(VideoDistortionSettings, "DistortionMaps/color.dismap");
                Settings.Default.VideoDistortion = VideoDistortionSettings;
            }
            if (Settings.Default.DepthDistortion != DepthDistortionSettings || !File.Exists("DistortionMaps/depth.dismap"))
            {
                CreateDistortionMap(DepthDistortionSettings, "DistortionMaps/depth.dismap");
                Settings.Default.DepthDistortion = DepthDistortionSettings;
            }
            VideoCorrection = LoadDistortionMap2("DistortionMaps/color.dismap", out VideoDismapWidth, out VideoDismapHeight);
            DepthCorrection = LoadDistortionMap2("DistortionMaps/depth.dismap", out DepthDismapWidth, out DepthDismapHeight);

            SF.NBDepthDispX.Value = DepthToIRDisp.X;
            SF.NBDepthDispY.Value = DepthToIRDisp.Y;
            SF.NBDepthScaleX.Value = DepthToIRScale.X;
            SF.NBDepthScaleY.Value = DepthToIRScale.Y;
        }

        void LoadSettings()
        {
            SF = new SettingsForm();
            SF.NBRainbowPeriod.ValueChanged += new EventHandler(NBRainbowPeriod_ValueChanged);
            SF.NBReprojectionRotationX.ValueChanged += new EventHandler(NBReprojectionRotation_ValueChanged);
            SF.NBReprojectionRotationY.ValueChanged += new EventHandler(NBReprojectionRotation_ValueChanged);
            SF.NBReprojectionRotationZ.ValueChanged += new EventHandler(NBReprojectionRotation_ValueChanged);
            SF.NBReprojectionTranslationX.ValueChanged += new EventHandler(NBReprojectionTranslation_ValueChanged);
            SF.NBReprojectionTranslationY.ValueChanged += new EventHandler(NBReprojectionTranslation_ValueChanged);
            SF.NBReprojectionTranslationZ.ValueChanged += new EventHandler(NBReprojectionTranslation_ValueChanged);
            SF.NBTriangleRemove.ValueChanged += new EventHandler(NBTriangleRemove_ValueChanged);
            SF.NBZLimit.ValueChanged += new EventHandler(NBZLimit_ValueChanged);
            SF.CBReproject.CheckedChanged += new EventHandler(CBReproject_CheckedChanged);
            SF.NBGaussFilterPasses.ValueChanged += new EventHandler(NBGaussFilterPasses_ValueChanged);
            SF.NBGaussSigma.ValueChanged += new EventHandler(NBGaussSigma_ValueChanged);
            SF.DDBShadingMode.SelectedIndexChanged += new EventHandler(DDBShadingMode_SelectedIndexChanged);
            SF.NBAveragedFrames.ValueChanged += new EventHandler(NBAveragedFrames_ValueChanged);
            SF.NBMinColoringDepth.ValueChanged += new EventHandler(NBMinColoringDepth_ValueChanged);
            SF.NBDepthDispX.ValueChanged += new EventHandler(NBDepthDisp_ValueChanged);
            SF.NBDepthDispY.ValueChanged += new EventHandler(NBDepthDisp_ValueChanged);
            SF.NBDepthScaleX.ValueChanged += new EventHandler(NBDepthScale_ValueChanged);
            SF.NBDepthScaleY.ValueChanged += new EventHandler(NBDepthScale_ValueChanged);

            SF.NBTriangleRemove.Value = Settings.Default.TriangleRemoveLevel;
            SF.NBRainbowPeriod.Value = Settings.Default.RainbowPeriod;
            SF.NBZLimit.Value = Settings.Default.ZLimit;
            SF.NBReprojectionTranslationX.Value = Settings.Default.ReprojectionTranslationX;
            SF.NBReprojectionTranslationY.Value = Settings.Default.ReprojectionTranslationY;
            SF.NBReprojectionTranslationZ.Value = Settings.Default.ReprojectionTranslationZ;
            SF.NBReprojectionRotationX.Value = Settings.Default.ReprojectionRotationX;
            SF.NBReprojectionRotationY.Value = Settings.Default.ReprojectionRotationY;
            SF.NBReprojectionRotationZ.Value = Settings.Default.ReprojectionRotationZ;
            SF.CBReproject.Checked = Settings.Default.Reprojection;
            SF.NBGaussFilterPasses.Int32Value = Settings.Default.GaussPasses;
            SF.NBGaussSigma.Value = Settings.Default.GaussSigma;
            SF.DDBShadingMode.SelectedIndex = Settings.Default.ShadingMode;
            SF.NBAveragedFrames.Int32Value = BufferedFrames = Settings.Default.BufferedFrames;
            SF.NBMinColoringDepth.Value = Settings.Default.MinColoringDepth;
            SetSaveMode((SaveModes)Settings.Default.SaveMode);
            SetSequenceInterval(Settings.Default.SequenceInterval);


            int i = 0;
            foreach (string shadingStyle in SF.DDBShadingMode.Items)
            {
                ToolStripMenuItem TSMI = new ToolStripMenuItem(shadingStyle);
                TSMI.Tag = i++;
                TSMI.Click += (object sender, EventArgs e) =>
                    {
                        SF.DDBShadingMode.SelectedIndex = (int)(sender as ToolStripMenuItem).Tag;
                    };
                TSDDBShading.DropDownItems.Add(TSMI);
            }

            Rotation = Settings.Default.Rotation;
            WorkingDirectory = Settings.Default.WorkingDirectory;
            if (string.IsNullOrEmpty(WorkingDirectory))
            {
                WorkingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
        }

        void NBDepthScale_ValueChanged(object sender, EventArgs e)
        {
            if (XEffect != null) EPDepthScale.SetValue(new Vector2(SF.NBDepthScaleX.Value, SF.NBDepthScaleY.Value));            
        }

        void NBDepthDisp_ValueChanged(object sender, EventArgs e)
        {
            if (XEffect != null) EPDepthDisp.SetValue(new Vector2(SF.NBDepthDispX.Value, SF.NBDepthDispY.Value));
        }

        void NBMinColoringDepth_ValueChanged(object sender, EventArgs e)
        {
            if (XEffect != null) XEffect.Parameters["MinColoringDepth"].SetValue(SF.NBMinColoringDepth.Value);
        }

        void NBAveragedFrames_ValueChanged(object sender, EventArgs e)
        {
            BufferedFrames = SF.NBAveragedFrames.Int32Value;
        }      

        void SaveSettings()
        {
            Settings.Default.TriangleRemoveLevel = SF.NBTriangleRemove.Value;
            Settings.Default.RainbowPeriod = SF.NBRainbowPeriod.Value;
            Settings.Default.ZLimit = SF.NBZLimit.Value;
            Settings.Default.ReprojectionTranslationX = ReprojectionTranslationX;
            Settings.Default.ReprojectionTranslationY = ReprojectionTranslationY;
            Settings.Default.ReprojectionTranslationZ = ReprojectionTranslationZ;
            Settings.Default.ReprojectionRotationX = ReprojectionRotationX;
            Settings.Default.ReprojectionRotationY = ReprojectionRotationY;
            Settings.Default.ReprojectionRotationZ = ReprojectionRotationZ;
            Settings.Default.Reprojection = Reprojection;
            Settings.Default.GaussSigma = GaussSigma;
            Settings.Default.GaussPasses = GaussPasses;
            Settings.Default.ShadingMode = ShadingMode;
            Settings.Default.Rotation = Rotation;
            Settings.Default.WorkingDirectory = WorkingDirectory;
            Settings.Default.BufferedFrames = BufferedFrames;
            Settings.Default.SaveMode = (byte)SaveMode;
            Settings.Default.SequenceInterval = (int)SequenceInterval;
            Settings.Default.Save();
        }

        void DDBShadingMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShadingMode = SF.DDBShadingMode.SelectedIndex;
        } 

        void NBTriangleRemove_ValueChanged(object sender, EventArgs e)
        {
            if (XEffect != null) XEffect.Parameters["TriangleRemoveLimit"].SetValue(SF.NBTriangleRemove.Value);
            if (KSC != null) KSC.OnTriangleRemoveLimitChanged(SF.NBTriangleRemove.Value);
        }

        void NBZLimit_ValueChanged(object sender, EventArgs e)
        {
            if (XEffect != null) XEffect.Parameters["DepthZLimit"].SetValue(SF.NBZLimit.Value);
            if (KSC != null) KSC.OnDepthZLimitChanged(SF.NBZLimit.Value);
        }

        void NBReprojectionTranslation_ValueChanged(object sender, EventArgs e)
        {
            ReprojectionTranslationX = SF.NBReprojectionTranslationX.Value;
            ReprojectionTranslationY = SF.NBReprojectionTranslationY.Value;
            ReprojectionTranslationZ = SF.NBReprojectionTranslationZ.Value;
            SetReprojection();            
        }

        void NBReprojectionRotation_ValueChanged(object sender, EventArgs e)
        {
            ReprojectionRotationX = SF.NBReprojectionRotationX.Value;
            ReprojectionRotationY = SF.NBReprojectionRotationY.Value;
            ReprojectionRotationZ = SF.NBReprojectionRotationZ.Value;
            SetReprojection();
        }

        void NBRainbowPeriod_ValueChanged(object sender, EventArgs e)
        {
            if (XEffect != null) XEffect.Parameters["DepthHSLColoringPeriod"].SetValue(SF.NBRainbowPeriod.Value);
        }

        void CBReproject_CheckedChanged(object sender, EventArgs e)
        {
            Reprojection = SF.CBReproject.Checked;
        }

        void NBGaussSigma_ValueChanged(object sender, EventArgs e)
        {
            GaussSigma = SF.NBGaussSigma.Value;
            SetGauss();
        }

        void NBGaussFilterPasses_ValueChanged(object sender, EventArgs e)
        {
            GaussPasses = SF.NBGaussFilterPasses.Int32Value;
        }

        unsafe void CreateDistortionMap(string settings, string path)
        {
            DistortionMap DM = new DistortionMap();
            DM.Deserialize(settings);
            float[] fData = DM.GenerateMap();
            byte[] bData = new byte[fData.Length * 4+8];
            using (FileStream FS = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                fixed (float* sfData = &fData[0])
                fixed (byte* sbData = &bData[0])
                {
                    float* pfData = sfData;
                    float* pbData = (float*)sbData + 2;
                    int* piData = (int*)sbData;
                    *piData++ = DM.Width;
                    *piData++ = DM.Height;
                    for (int i = 0; i < fData.Length; i++)
                    {
                        *pbData++ = *pfData++;
                    }
                }
            }
            File.WriteAllBytes(path, bData);
        }

        float[] LoadDistortionMap(string path, out int w, out int h)
        { 
            float[] data = null;

            using (FileStream FS = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (BinaryReader BR = new BinaryReader(FS))
            {
                w = BR.ReadInt32();
                h = BR.ReadInt32();
                data = new float[w * h * 2];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = BR.ReadSingle();
                }
            }
            return data;
        }

        unsafe float[] LoadDistortionMap2(string path, out int w, out int h)
        {
            byte[] bData = File.ReadAllBytes(path);
            float[] fData = null;            
            fixed (byte* sbData = &bData[0])
            {
                float* pbData = (float*)sbData + 2;
                w = *(int*)sbData;
                h = *(int*)(sbData + 4);
                fData = new float[w * h * 2];
                fixed (float* sfData = &fData[0])
                {
                    float* pfData = sfData;
                    for (int i = 0; i < fData.Length; i++)
                    {
                        *pfData++ = *pbData++;
                    }
                }
            }
            return fData;
        }
    }
}