using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMA3Charts
{
    class AkimaSpline
    {
        public static int CYCLE = 100;

        public static double[] HermiteX(double[] X, int i)
        {
            double steps = (X[i + 1] - X[i]) / CYCLE;
            double[] hermiteX = new double[CYCLE + 1];

            for (int j = 0; j < CYCLE + 1; j++)
            {
                hermiteX[j] = X[i] + j * steps;
            }

            return hermiteX;
        }
        public static double[] HermiteY(double[] X, double[] Y, double[] hermiteX, int i)
        {
            var DY = Akima(X, Y);

            double[] fy = new double[CYCLE + 1];
            for (int j = 0; j < CYCLE + 1; j++)
            {
                fy[j] = 0;
            }

            for (int j = 0; j < 2; j++)
            {
                double[] tempArray = new double[2];
                tempArray[0] = X[i];
                tempArray[1] = X[i + 1];
                double[] L = Lagrange(tempArray, j, hermiteX);
                double DL = D_Lagrange(tempArray, j, X[j]);

                double[] U = HermitU(tempArray, j, hermiteX, L, DL);
                double[] V = HermitV(tempArray, j, hermiteX, L, DL);


                for (int z = 0; z < U.Length; z++)
                {
                    var prev = fy[z];
                    fy[z] = fy[z] + U[z] * Y[i + j] + V[z] * DY[i + j];
                    if (double.IsNaN(fy[z]))
                    {
                        var a = prev + U[z] * Y[i + j] + V[z] * DY[i + j];
                    }
                }
            }

            return fy;
        }

        private static double[] HermitU(double[] X, int j, double[] x, double[] L, double DL)
        {
            double[] U = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                U[i] = (1 - 2 * DL * (x[i] - X[j])) * Math.Pow(L[i], 2);
            }

            return U;
        }

        private static double[] HermitV(double[] X, int j, double[] x, double[] L, double DL)
        {
            double[] V = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                V[i] = (x[i] - X[j]) * Math.Pow(L[i], 2);
            }
            return V;
        }


        private static double[] Lagrange(double[] X, int j, double[] x)
        {
            int n = X.Length;
            int m = x.Length;
            double[] L = new double[m];
            for (int i = 0; i < m; i++)
            {
                L[i] = 1;
            }
            for (int i = 0; i < n; i++)
            {
                if (i != j)
                {
                    for (int z = 0; z < m; z++)
                    {
                        double div = (X[j] - X[i]);
                        if (Math.Abs(div) <= 1e-7)
                        {
                            div = 1e-7;
                        }
                        L[z] = L[z] * (x[z] - X[i]) / div;
                    }
                }
            }
            return L;
        }

        private static double D_Lagrange(double[] X, int j, double x)
        {
            int n = X.Length;
            double DL = 0;
            for (int i = 0; i < n; i++)
            {
                if (i != j)
                {
                    double count = 1;
                    for (int k = 0; k < n; k++)
                    {
                        if (k != j && k != i)
                        {
                            count = count * (x - X[k]);
                        }
                    }
                    DL = DL + count;
                }
            }
            double denominator = 1;
            for (int i = 0; i < n; i++)
            {
                if (i != j)
                {
                    denominator = denominator * (X[j] - X[i]);
                }
            }
            if (Math.Abs(denominator) <= 1e-7)
            {
                denominator = 1e-7;
            }

            DL = DL / denominator;
            return DL;
        }

        private static double[] Akima(double[] X, double[] Y)
        {
            int n = X.Length;
            double[] DY = new double[n];
            for (int i = 0; i < n; i++)
            {
                if (i == 0)
                {
                    DY[i] = (2 * X[0] - X[1] - X[2]) / ((X[0] - X[1]) * (X[0] - X[2])) *
                        Y[0] + (2 * X[0] - X[0] - X[2]) / ((X[1] - X[0]) * (X[1] - X[2])) *
                        Y[1] + (2 * X[0] - X[0] - X[1]) / ((X[2] - X[0]) * (X[2] - X[1])) * Y[2];

                }
                else if (i == n - 1)
                {
                    DY[i] = (2 * X[n - 1] - X[n - 2] - X[n - 1]) / ((X[n - 3] - X[n - 2]) *
                        (X[n - 3] - X[n - 1])) * Y[n - 3] + (2 * X[n - 1] - X[n - 3] - X[n - 1]) /
                        ((X[n - 2] - X[n - 3]) * (X[n - 2] - X[n - 1])) * Y[n - 2] +
                        (2 * X[n - 1] - X[n - 3] - X[n - 2]) / ((X[n - 1] - X[n - 3]) * (X[n - 1] - X[n - 2])) * Y[n - 1];
                }
                else
                {
                    DY[i] = (2 * X[i] - X[i] - X[i + 1]) / ((X[i - 1] - X[i]) *
                        (X[i - 1] - X[i + 1])) * Y[i - 1] + (2 * X[i] - X[i - 1] - X[i + 1]) /
                        ((X[i] - X[i - 1]) * (X[i] - X[i + 1])) * Y[i] + (2 * X[i] - X[i - 1] - X[i]) /
                        ((X[i + 1] - X[i - 1]) * (X[i + 1] - X[i])) * Y[i + 1];
                }
            }
            return DY;
        }
    }
}
