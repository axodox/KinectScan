using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace KinectScan
{
    public static class LinearEquations
    {
        const double Tiny = 1e-40;
        public class InavlidArgumentException : Exception
        {
            private string text;
            public override string Message { get { return text; } }
            public InavlidArgumentException(string message)
            {
                text = message;
            }
        }
        public static void LUDecomposition(double[,] matrix, out int[] rowIndicies)
        {
            if (matrix.GetLength(0) != matrix.GetLength(1)) throw new Exception("Rectangular matrix expected.");
            int i, j, k, imax = -1, itemp, n = matrix.GetLength(0);
            rowIndicies = new int[n];
            double[] scaling = new double[n];
            double big, temp;

            //Search for maximum in each row and set scaling accordingly
            for (i = 0; i < n; i++)
            {
                big = 0d;
                for (j = 0; j < n; j++)
                {
                    temp = Math.Abs(matrix[i, j]);
                    if (temp > big) big = temp;
                }
                if (big == 0d) throw new InavlidArgumentException("Singular matrix");
                scaling[i] = 1d / big;
                rowIndicies[i] = i;
            }

            for (k = 0; k < n; k++)
            {
                //Search for scaled maximum in each row
                big = 0d;
                for (i = k; i < n; i++)
                {
                    temp = scaling[i] * Math.Abs(matrix[i, k]);
                    if (temp > big)
                    {
                        big = temp;
                        imax = i;
                    }
                }
                //Interchange rows
                if (k != imax)
                {
                    for (j = 0; j < n; j++)
                    {
                        temp = matrix[imax, j];
                        matrix[imax, j] = matrix[k, j];
                        matrix[k, j] = temp;
                    }
                    temp = scaling[imax];
                    scaling[imax] = scaling[k];
                    scaling[k] = temp;
                    itemp = rowIndicies[imax];
                    rowIndicies[imax] = rowIndicies[k];
                    rowIndicies[k] = itemp;
                }
                //Reduce matrix
                if (matrix[k, k] == 0d) matrix[k, k] = Tiny;
                for (i = k + 1; i < n; i++)
                {
                    temp = matrix[i, k] /= matrix[k, k];
                    for (j = k + 1; j < n; j++)
                    {
                        matrix[i, j] -= temp * matrix[k, j];
                    }
                }
            }
        }

        public static double[] LUSolve(double[,] matrix, double[] b, int[] indicies)
        {
            int n = matrix.GetLength(0);
            if (b.Length != n || n != matrix.GetLength(1) || indicies.Length != n)
                throw new InavlidArgumentException("Wrong argument size.");
            int i, j;

            double[] y = new double[n];
            double[] x = new double[n];

            for (i = 0; i < n; i++)
            {
                y[i] = b[indicies[i]];
            }
            for (i = 1; i < n; i++)
            {
                for (j = 0; j < i; j++)
                {
                    y[i] -= matrix[i, j] * y[j];
                }
            }
            for (i = n - 1; i >= 0; i--)
            {
                for (j = i + 1; j < n; j++)
                {
                    y[i] -= matrix[i, j] * y[j];
                }
                y[i] /= matrix[i, i];
            }
            for (i = 0; i < n; i++)
            {
                x[indicies[i]] = y[i];
            }
            return y;
        }

        public static double[] Solve(double[,] a, double[] b)
        {
            int[] indicies;
            LUDecomposition(a, out indicies);
            return LUSolve(a, b, indicies);
        }
    }

    public static class CurveFitting
    {
        public struct Point
        {
            public double X, Y;
            public Point(double x, double y)
            {
                X = x;
                Y = y;
            }
            private static readonly char[] Separator = new char[] { ';' };
            public override string ToString()
            {
                return X.ToString() + Separator[0] + Y.ToString();
            }
        }
        public static double SubstituteToPolynomial(this double[] poly, double x)
        {
            double sum = 0;
            double temp = 1;
            for (int i = 0; i < poly.Length; i++)
            {
                sum += poly[i] * temp;
                temp *= x;
            }
            return sum;
        }

        public static double SubstituteToDerivatedPolynomial(this double[] poly, double x)
        {
            double sum = 0;
            double temp = 1;
            for (int i = 1; i < poly.Length; i++)
            {
                sum += poly[i] * temp * i;
                temp *= x;
            }
            return sum;
        }

        public static double[] PolynomialFit(Point[] points, int degree)
        {
            int maxPower = degree * 2 + 2;
            double[] b = new double[degree + 1];
            double[,] A = new double[degree + 1, degree + 1];
            double[,] tiPowerk = new double[points.Length, maxPower];
            double[] tPowerSum = new double[maxPower];

            double sum, xi;
            int i, j;

            for (i = 0; i < points.Length; i++)
            {
                xi = points[i].X;
                tiPowerk[i, 0] = sum = 1d;
                for (j = 1; j < maxPower; j++)
                {
                    tiPowerk[i, j] = sum *= xi;
                }
            }

            for (i = 0; i < maxPower; i++)
            {
                sum = 0;
                for (j = 0; j < points.Length; j++)
                {
                    sum += tiPowerk[j, i];
                }
                tPowerSum[i] = sum;
            }

            for (i = 0; i <= degree; i++)
            {
                sum = 0;
                for (j = 0; j < points.Length; j++)
                {
                    sum += points[j].Y * tiPowerk[j, i];
                }
                b[i] = sum;

                for (j = 0; j <= degree; j++)
                {
                    A[i, j] = tPowerSum[i + j];
                }
            }
            int[] indicies;
            LinearEquations.LUDecomposition(A, out indicies);
            return LinearEquations.LUSolve(A, b, indicies);
        }

        public static double[] PolynomialClosedFit(Point[] points, int degree, int joinDegree)
        {
            int maxPower = degree * 2 + 2;
            double[] b = new double[degree + 1];
            double[,] A = new double[degree + 1, degree + 1];
            double[,] xiPowerj = new double[points.Length, maxPower];
            double[] xPowerSum = new double[maxPower];

            double temp, xi;
            int i, j;

            for (i = 0; i < points.Length; i++)
            {
                xi = points[i].X;
                xiPowerj[i, 0] = temp = 1d;
                for (j = 1; j < maxPower; j++)
                {
                    xiPowerj[i, j] = temp *= xi;
                }
            }

            for (i = 0; i < maxPower; i++)
            {
                temp = 0;
                for (j = 0; j < points.Length; j++)
                {
                    temp += xiPowerj[j, i];
                }
                xPowerSum[i] = temp;
            }

            for (i = joinDegree; i <= degree; i++)
            {
                temp = 0;
                for (j = 0; j < points.Length; j++)
                {
                    temp += points[j].Y * xiPowerj[j, i];
                }
                b[i] = temp;

                for (j = 0; j <= degree; j++)
                {
                    A[i, j] = xPowerSum[i + j];
                }
            }

            
            int last = points.Length - 1;
            int k;
            for (i = 0; i < joinDegree; i++)
            {
                for (j = 0; j < i; j++)
                {
                    A[i, j] = 0;
                }
                
                for (j = i; j <= degree; j++)
                {
                    temp = 1;
                    for (k = 0; k < i; k++) temp *= (j - k);
                    A[i, j] = temp * (xiPowerj[0, j - i] - xiPowerj[last, j - i]);
                }

                b[i] = 0;
            }
            int[] indicies;
            LinearEquations.LUDecomposition(A, out indicies);
            return LinearEquations.LUSolve(A, b, indicies);
        }

        private static CultureInfo CI = new CultureInfo("en-US");
        public static string ToMathematicaFunction(this double[] poly, string name = "f")
        {
            string text = name + "[t_]:=" + poly[0].ToString(CI);
            for (int i = 1; i < poly.Length; i++)
            {
                text += "+" + (poly[i] / Math.Pow(10, Math.Truncate(Math.Log10(Math.Abs(poly[i]))))).ToString("F10", CI) + " 10^" + Math.Truncate(Math.Log10(Math.Abs(poly[i]))).ToString(CI) + " t^" + i.ToString(CI);
            }
            return text+";";
        }

        public static string ToMathematicaString(this Point[] varray)
        {
            string text = "points={";
            int imax = varray.Length - 1;
            for (int i = 0; i < varray.Length; i++)
            {
                text += "{" + varray[i].X.ToString() + ", " + varray[i].Y.ToString() + "}";
                if (i != imax) text += ",";
            }
            text += "};";
            return text;
        }
    }
}
