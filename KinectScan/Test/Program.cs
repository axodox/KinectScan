using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Test
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

        }

        static void MatrixDotMatrix()
        {
            string s = "";
            for (int j = 1; j < 5; j++)
            {
                for (int i = 1; i < 5; i++)
                {
                    s += string.Format("m.M{0}{1} = ", j, i);
                    for (int k = 1; k < 5; k++)
                    {
                        s += string.Format("M{0}{1} *  b.M{2}{3}", new object[] { j, k, k, i });
                        if (k != 4) s += " + ";
                    }
                    s += ";\r\n";
                }
            }
            Clipboard.SetText(s);
        }
    }
}
