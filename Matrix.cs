using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMA3Charts
{
    class Matrix
    {
        public static double[] SolveEq(double[][] A, double[] B, int n, int m, int n2, int m2, bool print)
        {
            var Q = GetQ(A, n, m);
            var QTB = GetQTB(Q, B, n);
            var R = GetR(Q, A, n, m, n2, m2);
            var X = new double[n];

            if (print)
            {
                Console.WriteLine("A:");
                PrintMatrix(A);
                Console.WriteLine("B:");
                PrintArray(B);
                Console.WriteLine("Q:");
                PrintMatrix(Q);
                Console.WriteLine("QTB:");
                PrintArray(QTB);
                Console.WriteLine("R:");
                PrintMatrix(R);
            }

            X[n - 1] = QTB[n - 1] / R[n - 1][n - 1];
            for (int i = n - 2; i >= 0; i--)
            {
                X[i] = QTB[i];
                for (int j = n - 1; j > i; j--)
                {
                    X[i] -= R[j][i] * X[j];
                }
                X[i] /= R[i][i];
            }

            return X;
        }

        public static double[] GetQTB(double[][] Q, double[] B, int n)
        {
            return MultiplyVector(Q, B, n, 1);
        }

        public static double[][] GetR(double[][] Q, double[][] A, int n, int m, int n2, int m2)
        {
            return MultiplyMatrix(Q, A, n, m, n2, m2);
        }

        // Works
        public static double[][] GetQ(double[][] A, int n, int m)
        {
            double[][] B = new double[n][];
            double[] u = new double[n];
            B[0] = MultiplyConst(1 / GetVectorAbs(A[0]), A[0]);
            for (int i = 1; i < n; i++)
            {
                double[] b = new double[n];
                Array.Copy(A[i], b, n);
                for (int j = 0; j < i; j++)
                {
                    b = SubtractVectors(b, GetU(B[j], A[i]));
                }
                B[i] = b;
            }
            return B;
        }

        public static double[] GetU(double[] u1, double[] v2)
        {
            var u = MultiplyConst(GetVectorAbs(u1), u1);
            return MultiplyConst(MultiplyScalar(v2, u) / MultiplyScalar(u, u), u);
        }

        public static double MultiplyScalar(double[] u1, double[] u2)
        {
            double u = 0;

            for (int i = 0; i < u1.Length; i++)
            {
                u += u1[i] * u2[i];
            }

            return u;
        }

        public static double[] MultiplyConst(double c, double[] v)
        {
            double[] u = new double[v.Length];

            for (int i = 0; i < v.Length; i++)
            {
                u[i] = c * v[i];
            }

            return u;
        }

        public static double[][] MultiplyMatrix(double[][] TransA, double[][] B, int n, int m, int n2, int m2)
        {
            double[][] C = new double[n][];
            for (int i = 0; i < n; i++)
            {
                C[i] = new double[m2];
                for (int j = 0; j < m2; j++)
                {
                    C[i][j] = MultiplyScalar(TransA[j], B[i]);
                }
            }

            return C;
        }

        public static double[] MultiplyVector(double[][] TransA, double[] B, int n, int m)
        {
            var C = new double[n];

            for (int i = 0; i < n; i++)
            {
                C[i] = MultiplyScalar(TransA[i], B);
            }

            return C;
        }

        public static double[] GetMatrixRow(double[][] A, int rowNr, int n)
        {
            double[] row = new double[n];
            for (int i = 0; i < n; i++)
            {
                row[i] = A[i][rowNr];
            }

            return row;
        }

        public static double[] SubtractVectors(double[] u1, double[] u2)
        {
            double[] u = new double[u1.Length];
            for (int i = 0; i < u1.Length; i++)
            {
                u[i] = u1[i] - u2[i];
            }

            return u;
        }

        public static double[] AddVectors(double[] u1, double[] u2)
        {
            var u = new double[u1.Length];
            for (int i = 0; i < u1.Length; i++)
            {
                u[i] = u1[i] + u2[i];
            }

            return u;
        }

        public static double GetVectorAbs(double[] u)
        {
            double abs = 0;
            for (int i = 0; i < u.Length; i++)
            {
                abs += u[i] * u[i];
            }

            return Math.Sqrt(abs);
        }

        public static double[][] GetTranspose(double[][] matrix, int n, int m)
        {
            double[][] transpose = new double[m][];
            for (int i = 0; i < m; i++)
            {
                transpose[i] = new double[n];
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    transpose[j][i] = 
                        matrix[i][j];
                }
            }

            return transpose;
        }

        public static double L2Norm(double[] u)
        {
            double sum = 0;
            foreach (var a in u)
            {
                sum += a * a;
            }

            return Math.Sqrt(sum);
        }

        public static void PrintMatrix(double[][] A)
        {
            for (int i = 0; i < A.Length; i++)
            {
                for (int j = 0; j < A.Length; j++)
                {
                    Console.Write(String.Format("{0:0.000000} ", A[j][i]));
                }
                Console.WriteLine();
            }
        }

        public static void PrintArray(double[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Console.Write(String.Format("{0:0.000000} ", array[i]));
            }
            Console.WriteLine();
        }

        private static String FormatHeading(int n)
        {
            String formatted = "|";
            for (int i = 0; i < n; i++)
            {
                formatted += String.Format("{0, 10}|", "x" + i.ToString());
            }

            return formatted;
        }

        private static String FormatData(double[] X)
        {
            String formatted = "|";
            for (int i = 0; i < X.Length; i++)
            {
                formatted += String.Format("{0, 10}|", String.Format("{0:0.000000}", X[i]));
            }

            return formatted;
        }

        public static void PrintResult(double[] X)
        {
            Console.WriteLine();
            Console.WriteLine(new String('-', X.Length * 11 + 1));
            Console.WriteLine(FormatHeading(X.Length));
            Console.WriteLine(new String('-', X.Length * 11 + 1));
            Console.WriteLine(FormatData(X));
            Console.WriteLine(new String('-', X.Length * 11 + 1));
        }
    }
}
