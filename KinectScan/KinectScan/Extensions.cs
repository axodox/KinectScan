using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Xna.Framework;
using System.IO;
using System.Globalization;
using KinectScan.Properties;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;
using System.Diagnostics;

namespace KinectScan
{
    public class IOEventArgs : EventArgs
    {
        public string Path { get; private set; }
        public bool Success { get; private set; }
        public string Message { get; private set; }
        public IOEventArgs(string path, bool success, string message)
        {
            Path = path;
            Success = success;
            Message = message;
        }
    }
    public delegate void IOEventHandler(object o, IOEventArgs e);

    public class XNAGraph
    {
        public float Max { get; set; }
        public float Min { get; set; }
        public bool AutoRange { get; set; }
        private float Zero;
        private float[] Data;
        GraphicsDevice XDevice;
        TextureDoubleBuffer DataTexture;
        Effect XEffect;
        XPlane XPlane;
        Microsoft.Xna.Framework.Rectangle Rect;
        
        public XNAGraph()
        {
            XPlane = new XPlane(2, 2);
            AutoRange = true;
        }

        public void SetDevice(GraphicsDevice device, Effect effect)
        {
            XDevice = device;
            XEffect = effect;
            if (DataTexture != null)
            {
                DataTexture.Dispose();
                DataTexture = null;
            }
            RefreshTexture();
            Rect = new Microsoft.Xna.Framework.Rectangle(0, 0, XDevice.PresentationParameters.BackBufferWidth, XDevice.PresentationParameters.BackBufferHeight);
            XPlane.SetDevice(XDevice);
        }

        public void Update(float[] data)
        {
            if (AutoRange)
            {
                Max = float.MinValue;
                Min = float.MaxValue;
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] > Max) Max = data[i];
                    if (data[i] < Min) Min = data[i];
                }
            }
            
            float range = Max - Min;
            Zero = -Min / range;
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (data[i] - Min) / range;                
            }
            Data = data;
            RefreshTexture();
        }

        public unsafe void Update(int* data, int length)
        {
            if (AutoRange)
            {
                Max = float.MinValue;
                Min = float.MaxValue;
                for (int i = 0; i < length; i++)
                {
                    //if (data[i] != 0)
                    //{
                        if (data[i] > Max) Max = data[i];
                        if (data[i] < Min) Min = data[i];
                    //}
                }
            }

            float range = Max - Min;
            Zero = -Min / range;
            if (Data==null||Data.Length != length) Data = new float[length];
            for (int i = 0; i < length; i++)
            {
                
                Data[i] = (data[i] - Min) / range;
                if (Data[i] == 0) Data[i] = float.NaN;
            }
            RefreshTexture();
        }

        private void RefreshTexture()
        {
            if (XDevice != null && !XDevice.IsDisposed)
            {
                if ((DataTexture == null || (Data != null && DataTexture.Width != Data.Length)) && XDevice != null && Data != null)
                {
                    DataTexture = new TextureDoubleBuffer(XDevice, Data.Length, 1, SurfaceFormat.Single);
                }
                if (DataTexture != null)
                {
                    DataTexture.SetData<float>(Data);
                }
            }
        }

        public void Draw()
        {
            if (DataTexture != null)
            {
                XEffect.CurrentTechnique = XEffect.Techniques["Graph"];
                XEffect.CurrentTechnique.Passes[0].Apply();
                XDevice.Textures[0] = DataTexture.FrontTexture;
                XDevice.SamplerStates[0] = SamplerState.PointClamp;
                XDevice.BlendState = BlendState.AlphaBlend;
                XPlane.Draw();
            }
        }
    }

    public static class Extensions
    {
        private static CultureInfo Culture = new System.Globalization.CultureInfo("en-US");

        public static IOEventArgs Screenshot(this RenderTarget2D target, string path)
        {
            try
            {
                using (FileStream FS = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    target.SaveAsPng(FS, target.Width, target.Height);
                    return new IOEventArgs(path, true, null);
                }
            }
            catch (Exception e)
            {
                return new IOEventArgs(path, false, e.Message);
            }
        }

        public static IOEventArgs Vector4Screenshot(this RenderTarget2D target, string path)
        {
            try
            {
                int width = target.Width;
                int height = target.Height;
                float[] buffer = new float[width * height * 4];
                target.GetData<float>(buffer);
                using (FileStream FS = new FileStream(path, FileMode.Create, FileAccess.Write))
                using (BinaryWriter BW = new BinaryWriter(FS))
                {
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        BW.Write(buffer[i]);
                    }
                    BW.Close();
                    FS.Close();
                }
                return new IOEventArgs(path, true, null);
            }
            catch(Exception e)
            {
                return new IOEventArgs(path, false, e.Message);
            }
        }

        public unsafe static IOEventArgs ColorScreenshot(this RenderTarget2D target, string path)
        {
            try
            {
                target.GraphicsDevice.SetRenderTarget(null);
                int width = target.Width;
                int height = target.Height;
                Microsoft.Xna.Framework.Color[] buffer = new Microsoft.Xna.Framework.Color[width * height];
                target.GetData<Microsoft.Xna.Framework.Color>(buffer);
                Bitmap B = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                BitmapData BD = B.LockBits(new System.Drawing.Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                byte* sImage = (byte*)BD.Scan0;
                byte* pImage;
                fixed (Microsoft.Xna.Framework.Color* sBuffer = &buffer[0])
                {
                    Microsoft.Xna.Framework.Color* pBuffer = sBuffer, lBuffer;
                    for (int i = 0; i < height; i++)
                    {
                        pImage = sImage + i * BD.Stride;
                        for (lBuffer = pBuffer + width; pBuffer < lBuffer; pBuffer++)
                        {
                            *pImage++ = pBuffer->B;
                            *pImage++ = pBuffer->G;
                            *pImage++ = pBuffer->R;
                        }
                    }
                }
                B.UnlockBits(BD);
                B.Save(path, ImageFormat.Bmp);
                return new IOEventArgs(path, true, null);
            }
            catch (Exception e)
            {
                return new IOEventArgs(path, false, e.Message);
            }
        }

        public unsafe static IOEventArgs DepthScreenshot(this RenderTarget2D target, string path, float a, float b, float c)
        {
            try
            {
                target.GraphicsDevice.SetRenderTarget(null);
                int width = target.Width;
                int height = target.Height;
                Vector2[] buffer = new Vector2[width * height];
                target.GetData<Vector2>(buffer);
                byte[] fbuffer=new byte[width * height*4];                
                fixed (Vector2* sBuffer = &buffer[0])
                fixed (byte* sImage = &fbuffer[0])
                {
                    Vector2* pBuffer = sBuffer, eBuffer = sBuffer + buffer.Length;
                    float* pImage = (float*)sImage;
                    while (pBuffer < eBuffer)
                    {
                        if (pBuffer->X == 0f) *pImage++ = 0f;
                        else *pImage++ = (float)(c - b / (pBuffer->X - a));
                        pBuffer++;
                    }
                }
                File.WriteAllBytes(path, fbuffer);
                return new IOEventArgs(path, true, null);
            }
            catch (Exception e)
            {
                return new IOEventArgs(path, false, e.Message);
            }
        }

        public unsafe static IOEventArgs Screenshot(this GraphicsDevice device, string path)
        {
            try
            {
                int width = device.PresentationParameters.BackBufferWidth;
                int height = device.PresentationParameters.BackBufferHeight;
                Microsoft.Xna.Framework.Color[] buffer = new Microsoft.Xna.Framework.Color[width * height];
                device.GetBackBufferData<Microsoft.Xna.Framework.Color>(buffer);
                Bitmap B = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                BitmapData BD = B.LockBits(new System.Drawing.Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                byte* sImage = (byte*)BD.Scan0;
                byte* pImage;
                fixed (Microsoft.Xna.Framework.Color* sBuffer = &buffer[0])
                {
                    Microsoft.Xna.Framework.Color* pBuffer = sBuffer, lBuffer;
                    for (int i = 0; i < height; i++)
                    {
                        pImage = sImage + i * BD.Stride;
                        for (lBuffer = pBuffer + width; pBuffer < lBuffer; pBuffer++)
                        {
                            *pImage++ = pBuffer->B;
                            *pImage++ = pBuffer->G;
                            *pImage++ = pBuffer->R;
                        }
                    }
                }
                B.UnlockBits(BD);
                B.Save(path, ImageFormat.Bmp);
                return new IOEventArgs(path, true, null);
            }
            catch (Exception e)
            {
                return new IOEventArgs(path, false, e.Message);
            }
        }

        
        const int SaveBufferSize = 128;
        const float ExportScale = 1000;
        public unsafe static IOEventArgs STLScreenshot(this RenderTarget2D target, string path, string title)
        {
            try
            {
                int width = target.Width;
                int height = target.Height;
                Vector4[] vertices = new Vector4[width * height];
                target.GetData<Vector4>(vertices);

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

                    fixed (Vector4* sVertices = &vertices[0])
                    fixed (byte* sBuffer = &buffer[0])
                    {
                        Vector4* pVertices = sVertices, lVertices = sVertices + width - 1, eVertices = sVertices + (height - 1) * width;
                        Vector4* A, B, C;
                        byte* pBuffer = sBuffer;
                        byte* eBuffer = sBuffer + buffer.Length - 50;

                        while (pVertices < eVertices)
                        {
                            A = pVertices;
                            B = pVertices + 1;
                            C = pVertices + width;
                            i += CheckAndWriteTriangle(A, B, C, ref pBuffer);
                            C = pVertices + 1;
                            B = pVertices + width;
                            A = pVertices + width + 1;
                            i += CheckAndWriteTriangle(A, B, C, ref pBuffer);
                            pVertices++;
                            if (pVertices == lVertices)
                            {
                                pVertices++;
                                lVertices = pVertices + width - 1;
                            }
                            if (pBuffer >= eBuffer || pVertices == eVertices)
                            {
                                BW.Write(buffer, 0, i * 50);
                                pBuffer = sBuffer;
                                e += i;
                                i = 0;
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

        public unsafe static int CheckAndWriteTriangle(Vector4* a, Vector4* b, Vector4* c, ref byte* p)
        {
            Vector3 A, B, C;
            Vector3 N;
            Vector3* pVector3 = (Vector3*)p;
            if (float.IsNaN(a->W) || float.IsNaN(b->W) || float.IsNaN(c->W))
            {
                return 0;
            }
            else
            {
                A = *(Vector3*)a;
                B = *(Vector3*)b;
                C = *(Vector3*)c;
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

        public static bool Export(this SettingsPropertyValueCollection settings, string path, string application)
        {
            try
            {
                using (XmlWriter XW = XmlWriter.Create(path))
                {
                    XW.WriteStartElement("Settings");
                    XW.WriteAttributeString("Application", application);
                    foreach (SettingsPropertyValue v in settings)
                    {
                        XW.WriteStartElement("Setting");
                        XW.WriteAttributeString("Name", v.Name);
                        XW.WriteAttributeString("Type", v.Property.PropertyType.FullName);
                        XW.WriteValue(v.SerializedValue);
                        XW.WriteEndElement();
                    }
                    XW.WriteEndElement();
                    XW.Flush();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Import(this SettingsPropertyValueCollection settings, string path, string application)
        {
            try
            {
                using (XmlReader XR = XmlReader.Create(path))
                {
                    XR.Read();
                    XR.Read();
                    if (XR.Name != "Settings") return false;
                    XR.MoveToAttribute("Application");
                    CultureInfo CI = new CultureInfo("en-US");
                    string name, type, value;
                    SettingsPropertyValue v;
                    XR.Read();
                    while (XR.Name == "Setting")
                    {
                        name = "";
                        type = "";
                        while (XR.MoveToNextAttribute())
                        {
                            switch (XR.Name)
                            {
                                case "Name":
                                    name = XR.Value;
                                    break;
                                case "Type":
                                    type = XR.Value;
                                    break;
                            }
                        }
                        XR.MoveToContent();
                        value = XR.ReadElementContentAsString();
                        v = settings[name];
                        if (v != null)
                        {
                            try
                            {
                                switch (type)
                                {
                                    case "System.Byte": v.PropertyValue = Byte.Parse(value, CI); break;
                                    case "System.SByte": v.PropertyValue = SByte.Parse(value, CI); break;
                                    case "System.Decimal": v.PropertyValue = Decimal.Parse(value, CI); break;
                                    case "System.Boolean": v.PropertyValue = Boolean.Parse(value); break;
                                    case "System.Int16": v.PropertyValue = Int16.Parse(value, CI); break;
                                    case "System.Int32": v.PropertyValue = Int32.Parse(value, CI); break;
                                    case "System.Int64": v.PropertyValue = Int64.Parse(value, CI); break;
                                    case "System.UInt16": v.PropertyValue = UInt16.Parse(value, CI); break;
                                    case "System.UInt32": v.PropertyValue = UInt32.Parse(value, CI); break;
                                    case "System.UInt64": v.PropertyValue = UInt64.Parse(value, CI); break;
                                    case "System.Single": v.PropertyValue = Single.Parse(value, CI); break;
                                    case "System.Double": v.PropertyValue = Double.Parse(value, CI); break;
                                    case "System.Char": v.PropertyValue = char.Parse(value); break;
                                    case "System.String": v.PropertyValue = value; break;
                                    default:
                                        throw new NotImplementedException();
                                }
                            }
                            catch { }
                        }
                    }
                    XR.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static readonly char[] ColumnDelimiter = new char[] { ';' };
        private static readonly char[] RowDelimiter = new char[] { '|' };

        public static string ToMathematicaString(this Matrix m)
        {
            return string.Format(Culture, "<<{0}, {1}, {2}, {3}>, <{4}, {5}, {6}, {7}>, <{8}, {9}, {10}, {11}>, <{12}, {13}, {14}, {15}>>", new object[] { m.M11, m.M12, m.M13, m.M14, m.M21, m.M22, m.M23, m.M24, m.M31, m.M32, m.M33, m.M34, m.M41, m.M42, m.M43, m.M44 }).Replace('<', '{').Replace('>', '}');
        }

        public static Vector4 Vector4FromString(string s)
        {
            Vector4 vector;
            string[] items = s.Split(RowDelimiter);
            vector.X = float.Parse(items[0], Culture);
            vector.Y = float.Parse(items[1], Culture);
            vector.Z = float.Parse(items[2], Culture);
            vector.W = float.Parse(items[3], Culture);
            return vector;
        }

        public static Matrix MatrixFromString(string s)
        {
            Matrix matrix;
            string[] rows = s.Split(RowDelimiter);
            string[] columns;
            float[] items = new float[16];
            int j = 0;
            for (int i = 0; i < 4; i++)
            {
                columns = rows[i].Split(ColumnDelimiter);
                items[j++] = float.Parse(columns[0], Culture);
                items[j++] = float.Parse(columns[1], Culture);
                items[j++] = float.Parse(columns[2], Culture);
                items[j++] = float.Parse(columns[3], Culture);
            }
            matrix.M11 = items[0];
            matrix.M12 = items[1];
            matrix.M13 = items[2];
            matrix.M14 = items[3];
            matrix.M21 = items[4];
            matrix.M22 = items[5];
            matrix.M23 = items[6];
            matrix.M24 = items[7];
            matrix.M31 = items[8];
            matrix.M32 = items[9];
            matrix.M33 = items[10];
            matrix.M34 = items[11];
            matrix.M41 = items[12];
            matrix.M42 = items[13];
            matrix.M43 = items[14];
            matrix.M44 = items[15];
            return matrix;
        }

        public static string ToString(this Matrix matrix)
        {
            return
                matrix.M11.ToString() + ColumnDelimiter[0] + matrix.M12.ToString() + ColumnDelimiter[0] + matrix.M13.ToString() + ColumnDelimiter[0] + matrix.M14.ToString() + RowDelimiter[0] +
                matrix.M21.ToString() + ColumnDelimiter[0] + matrix.M22.ToString() + ColumnDelimiter[0] + matrix.M23.ToString() + ColumnDelimiter[0] + matrix.M24.ToString() + RowDelimiter[0] +
                matrix.M31.ToString() + ColumnDelimiter[0] + matrix.M32.ToString() + ColumnDelimiter[0] + matrix.M33.ToString() + ColumnDelimiter[0] + matrix.M34.ToString() + RowDelimiter[0] +
                matrix.M41.ToString() + ColumnDelimiter[0] + matrix.M42.ToString() + ColumnDelimiter[0] + matrix.M43.ToString() + ColumnDelimiter[0] + matrix.M44.ToString();
        }

        public static void GaussCoeffs(int count, double sigma, double spacing, int width, out float[] dispos, out float[] coeffs)
        {
            coeffs = new float[count];
            dispos = new float[count];
            double pos, coeff;
            double a = 1d / Math.Sqrt(2 * Math.PI * sigma * sigma);
            double b = -0.5 / sigma / sigma;
            int ibase = count / 2;
            for (int i = 0; i < count; i++)
            {
                pos = (i - ibase) * spacing;
                coeff = a * Math.Exp(pos * pos * b);
                coeffs[i] = (float)coeff;
                dispos[i] = (float)(pos / width);
            }
        }
        public static void SetGaussCoeffs(this Effect effect, int width, int  height,int count, double sigma, double spacing)
        {
            float[] HGaussWeights, HGaussPos, VGaussWeights, VGaussPos;
            Extensions.GaussCoeffs(count, sigma, spacing, width, out HGaussPos, out HGaussWeights);
            Extensions.GaussCoeffs(count, sigma, spacing, height, out VGaussPos, out VGaussWeights);
            effect.Parameters["GaussWeightsH"].SetValue(HGaussWeights);
            effect.Parameters["GaussPosH"].SetValue(HGaussPos);
            effect.Parameters["GaussWeightsV"].SetValue(VGaussWeights);
            effect.Parameters["GaussPosV"].SetValue(VGaussPos);

            //float[] NormalHGaussWeights, NormalHGaussPos, NormalVGaussWeights, NormalVGaussPos;
            //Extensions.GaussCoeffs(count, sigma, spacing, width, out NormalHGaussPos, out NormalHGaussWeights);
            //Extensions.GaussCoeffs(count, sigma, spacing, height, out NormalVGaussPos, out NormalVGaussWeights);
            //effect.Parameters["NormalHGaussWeights"].SetValue(NormalHGaussWeights);
            //effect.Parameters["NormalHGaussPos"].SetValue(NormalHGaussPos);
            //effect.Parameters["NormalVGaussWeights"].SetValue(NormalVGaussWeights);
            //effect.Parameters["NormalVGaussPos"].SetValue(NormalVGaussPos);
        }
        public static char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();
        unsafe public static string ToFileName(this string s)
        {
            if (s == null || s == "") return "";
            char[] source = s.ToCharArray();

            fixed (char* pSource = &source[0])
            fixed (char* pInvalid = &InvalidFileNameChars[0])
            {
                char* a = pSource;
                char* b;
                for (int i = 0; i < source.Length; i++)
                {
                    b = pInvalid;
                    for (int j = 0; j < InvalidFileNameChars.Length; j++)
                    {
                        if (*b == *a) *a = '_';
                        b++;
                    }
                    a++;
                }
            }
            return new string(source);
        }

        public static Texture2D ToTexture2D(this Bitmap bitmap, GraphicsDevice device)
        {
            Texture2D texture = new Texture2D(device, bitmap.Width, bitmap.Height, false, SurfaceFormat.Color);
            texture.Update(bitmap);
            return texture;
        }

        public struct Byte4
        {
            public byte X, Y, Z, W;
        };

        public unsafe static void Update(this Texture2D texture, Bitmap bitmap)
        {
            Byte4[] data=new Byte4[bitmap.Width * bitmap.Height];
            BitmapData BD = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int strideJump = BD.Stride/4 - BD.Width;
            Byte4* pSource = (Byte4*)BD.Scan0;
            fixed(Byte4* sTarget=&data[0])
            {
                Byte4* pTarget = sTarget;
                for (int y = 0; y < BD.Height; y++)
                {
                    for (int x = 0; x < BD.Width; x++)
                    {
                        *pTarget = *pSource;
                        pTarget++;
                        pSource++;
                    }
                    pSource += strideJump;
                }
            }
            bitmap.UnlockBits(BD);
            texture.SetData<Byte4>(data);
        }

        public static float ToRadians(this float angle)
        {
            return MathHelper.ToRadians(angle);
        }

        public static string[] EnumToStringArray<T>()
        {
            Array values = Enum.GetValues(typeof(T));
            string[] names = new string[values.Length];
            int i=0;
            foreach (T value in values)
            {
                names[i++] = value.ToString();
            }
            return names;
        }
    }
}
