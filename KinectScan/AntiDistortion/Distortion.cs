using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace AntiDistortion
{
    public class DistortionMap
    {
        static CultureInfo CI = new CultureInfo("en-US");

        [CategoryAttribute("Radial distortion"), Description("First coefficient of radial distortion (1+K1*r^2+K2*r^4+...)")]
        public double K1 { get; set; }
        [CategoryAttribute("Radial distortion"), Description("Second coefficient of radial distortion (1+K1*r^2+K2*r^4+...)")]
        public double K2 { get; set; }
        [CategoryAttribute("Tangential distortion"), Description("First coefficient of tangential distortion")]
        public double P1 { get; set; }
        [CategoryAttribute("Tangential distortion"), Description("Second coefficient of tangential distortion")]
        public double P2 { get; set; }
        [CategoryAttribute("Principal point"), Description("X coordinate of the principal point")]
        public double CX { get; set; }
        [CategoryAttribute("Principal point"), Description("Y coordinate of the principal point")]
        public double CY { get; set; }
        [CategoryAttribute("Focal length"), Description("Focal length along X axis")]
        public double FX { get; set; }
        [CategoryAttribute("Focal length"), Description("Focal length along Y axis")]
        public double FY { get; set; }
        [CategoryAttribute("Calibration resolution"), Description("Calibration horizontal resolution")]
        public int OriginalWidth { get; set; }
        [CategoryAttribute("Calibration resolution"), Description("Calibration vertical resolution")]
        public int OriginalHeight { get; set; }
        [CategoryAttribute("Output resolution"), Description("Width of distortion map")]
        public int Width { get; set; }
        [CategoryAttribute("Output resolution"), Description("Height of distortion map")]
        public int Height { get; set; }
        [CategoryAttribute("Mapping"), Description("Mapping left")]
        public double MappingLeft { get; set; }
        [CategoryAttribute("Mapping"), Description("Mapping top")]
        public double MappingTop { get; set; }
        [CategoryAttribute("Mapping"), Description("Mapping width")]
        public double MappingWidth { get; set; }
        [CategoryAttribute("Mapping"), Description("Mapping height")]
        public double MappingHeight { get; set; }

        public float[] GenerateMap()
        {
            float[] map = new float[Width * Height * 2];
            double x, xd, y, yd, r2, r4;
            for (int j = 0; j < Height; j++)
            {
                y = (j * MappingHeight / Height + MappingTop - CY) / FY;
                for (int i = 0; i < Width; i++)
                {
                    x = (i * MappingWidth / Width + MappingLeft - CX) / FX;
                    r2 = x * x + y * y;
                    r4 = r2 * r2;
                    xd = x * (1 + K1 * r2 + K2 * r4) + P2 * (r2 + 2 * x * x) + 2 * P1 * x * y;
                    yd = y * (1 + K1 * r2 + K2 * r4) + P1 * (r2 + 2 * y * y) + 2 * P2 * x * y;
                    map[(j * Width + i) * 2] = (float)((xd * FX + CX - MappingLeft) / MappingWidth * Width);
                    map[(j * Width + i) * 2 + 1] = (float)((yd * FY + CY - MappingTop) / MappingHeight * Height);
                }
            }
            return map;
        }

        public string Serialize()
        {
            object[] d = new object[] { FX, FY, CX, CY, K1, K2, P1, P2, OriginalWidth, OriginalHeight, Width, Height, MappingWidth, MappingHeight, MappingLeft, MappingTop };
            string s = "";
            for (int i = 0; i < d.Length; i++)
            {
                if (d[i] is double)
                {
                    s += ((double)d[i]).ToString(CI) + '|';
                }
                if (d[i] is int)
                {
                    s += ((int)d[i]).ToString(CI) + '|';
                }
            }
            return s.Remove(s.Length - 1);
        }

        public void Deserialize(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                string[] settings = text.Split(new char[] { '|' });
                FX = double.Parse(settings[0], CI);
                FY = double.Parse(settings[1], CI);
                CX = double.Parse(settings[2], CI);
                CY = double.Parse(settings[3], CI);
                K1 = double.Parse(settings[4], CI);
                K2 = double.Parse(settings[5], CI);
                P1 = double.Parse(settings[6], CI);
                P2 = double.Parse(settings[7], CI);
                OriginalWidth = int.Parse(settings[8], CI);
                OriginalHeight = int.Parse(settings[9], CI);
                Width = int.Parse(settings[10], CI);
                Height = int.Parse(settings[11], CI);
                MappingWidth = double.Parse(settings[12], CI);
                MappingHeight = double.Parse(settings[13], CI);
                MappingLeft = double.Parse(settings[14], CI);
                MappingTop = double.Parse(settings[15], CI);
            }
        }
    }
}
