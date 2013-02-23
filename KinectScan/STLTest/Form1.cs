using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace STLTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Text = ReadSTL(@"C:\Users\Major Péter\Desktop\2012-07-29_20-24-21.stl").Length.ToString();
        }

        struct Vector3
        {
            public float X, Y, Z;
        }

        struct Triangle
        {
            public Vector3 A,B,C,N;
        }

        Triangle[] ReadSTL(string path)
        {
            FileStream FS = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.ASCII);
            BR.ReadBytes(80);
            uint count = BR.ReadUInt32();
            Triangle[] triangles = new Triangle[count];
            Vector3 n, a, b, c;
            for (int i = 0; i < triangles.Length; i++)
            {
                n = new Vector3() { X = BR.ReadSingle(), Y = BR.ReadSingle(), Z = BR.ReadSingle() };
                a = new Vector3() { X = BR.ReadSingle(), Y = BR.ReadSingle(), Z = BR.ReadSingle() };
                b = new Vector3() { X = BR.ReadSingle(), Y = BR.ReadSingle(), Z = BR.ReadSingle() };
                c = new Vector3() { X = BR.ReadSingle(), Y = BR.ReadSingle(), Z = BR.ReadSingle() };
                triangles[i] = new Triangle() { N = n, A = a, B = b, C = c };
                BR.ReadUInt16();
            }
            BR.Close();
            BR.Dispose();
            FS.Dispose();
            return triangles;
        }
    }
}
