using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;
using System.IO;

namespace SMA3Charts
{
    public partial class Form1 : Form
    {
        Series Fx1, Fx2, Fx3;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            PreparareForm(1, 11, -1, .5f);
            Fx1 = chart1.Series.Add("Duota funkcija");
            Fx1.ChartType = SeriesChartType.Line;
            Fx1.BorderWidth = 3;
            Fx1.Color = Color.Red;

            Fx2 = chart1.Series.Add("Interpoliuota funkcija");
            Fx2.ChartType = SeriesChartType.Line;
            Fx2.BorderWidth = 3;
            Fx2.Color = Color.Blue;

            Fx3 = chart1.Series.Add("Netiktis");
            Fx3.ChartType = SeriesChartType.Line;
            Fx3.BorderWidth = 3;
            Fx3.Color = Color.Green;

            var minX = 2;
            var maxX = 10;
            int pointNr = 30;

            Function F = new Function(Func, minX, maxX, pointNr);
            F.Points = F.GetPoints();
            F.NewtonsInterpolation();
            Console.WriteLine(F.GetNewtonsFunc());
            var interval = F.GetIntervals();

            for (double x = minX; x <= maxX; x += 0.01)
            {
                Fx1.Points.AddXY(x, Func(x));
                Fx2.Points.AddXY(x, F.InterpolatedFuncY(x));
                Fx3.Points.AddXY(x, Func(x) - F.InterpolatedFuncY(x));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            PreparareForm(1, 11, -1, .5f);
            Fx1 = chart1.Series.Add("Duota funkcija");
            Fx1.ChartType = SeriesChartType.Line;
            Fx2 = chart1.Series.Add("Interpoliuota funkcija");
            Fx2.ChartType = SeriesChartType.Line;
            Fx2.BorderWidth = 3;
            Fx2.Color = Color.Blue;
            Fx1.BorderWidth = 3;
            Fx1.Color = Color.Red;

            Fx3 = chart1.Series.Add("Netiktis");
            Fx3.ChartType = SeriesChartType.Line;
            Fx3.BorderWidth = 3;
            Fx3.Color = Color.Green;

            var minX = 2;
            var maxX = 10;
            int pointNr = 30;

            Function F = new Function(Func, minX, maxX, pointNr);
            F.Points = F.GetChebyshevNodes();
            F.NewtonsInterpolation();

            for (double x = 2; x <= 10; x += 0.01)
            {
                Fx1.Points.AddXY(x, Func(x));
                Fx2.Points.AddXY(x, F.InterpolatedFuncY(x));
                Fx3.Points.AddXY(x, Func(x) - F.InterpolatedFuncY(x));
            }
        }

        List<Point> temperatures = new List<Point>
        {
            new Point(1, 10.5617),
            new Point(2, 11.1504),
            new Point(3, 14.4338),
            new Point(4, 16.9788),
            new Point(5, 21.0081),
            new Point(6, 24.7199),
            new Point(7, 28.526),
            new Point(8, 27.9448),
            new Point(9, 25.7463),
            new Point(10, 23.1029),
            new Point(11, 16.7226),
            new Point(12, 11.5084),
        };

        private void button3_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            PreparareForm(1, 12, -5, 30);
            Fx1 = chart1.Series.Add("Temperatūra");
            Fx1.ChartType = SeriesChartType.Point;
            Fx1.MarkerSize = 7;
            Fx2 = chart1.Series.Add("t(x)");
            Fx2.ChartType = SeriesChartType.Line;
            Fx2.BorderWidth = 3;
            Fx2.Color = Color.Blue;
            Fx1.BorderWidth = 3;
            Fx1.Color = Color.Red;

            var minX = 0;
            var maxX = 12;

            Function F = new Function(Func, minX, maxX)
            {
                Points = temperatures
            };
            F.NewtonsInterpolation();
            var interval = F.GetIntervals();

            for (int x = minX; x < maxX; x++)
            {
                Fx1.Points.AddXY(temperatures[x].X, temperatures[x].Y);
            }

            for (double x = minX; x <= maxX; x += 0.01)
            {
                Fx2.Points.AddXY(x, F.InterpolatedFuncY(x));
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            PreparareForm(1, 12, -5, 30);
            Fx1 = chart1.Series.Add("Temperatūra");
            Fx1.ChartType = SeriesChartType.Point;
            Fx1.MarkerSize = 7;
            Fx1.BorderWidth = 3;
            Fx1.Color = Color.Red;

            var X = (from p in temperatures select p.X).ToArray();
            var Y = (from p in temperatures select p.Y).ToArray();

            for (int i = 0; i < 12; i++)
            {
                Fx1.Points.AddXY(X[i], Y[i]);
            }

            InterpolationErmit(X, Y);
        }

        private const string CYPRUS_X = "../../../CyprusX.txt";
        private const string CYPRUS_Y = "../../../CyprusY.txt";
        private void button5_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            PreparareForm(32, 34, 34, 36);
            Fx1 = chart1.Series.Add("Duoti taškai");
            Fx1.ChartType = SeriesChartType.Point;
            Fx1.MarkerSize = 7;
            Fx1.BorderWidth = 3;
            Fx1.Color = Color.Red;

            int amount = 100;
            var X = GetCountryValues(CYPRUS_X);
            var Y = GetCountryValues(CYPRUS_Y);
            var selectedX = SelectValues(X, amount);
            var selectedY = SelectValues(Y, amount);

            for (int i = 0; i < X.Count(); i++)
            {
                Fx1.Points.AddXY(X[i], Y[i]);
            }

           InterpolationErmit(selectedX, selectedY);
        }

        private double[] GetCountryValues(string path)
        {
            var line = File.ReadAllText(path);
            var data = line.Split(',');
            double[] values = new double[data.Count()];

            for (int i = 0; i < data.Count(); i++)
            {
                values[i] = double.Parse(data[i]);
            }

            return values;
        }

        private double[] SelectValues(double[] data, int amount)
        {
            double[] values = new double[amount];
            int d = data.Count() / amount;

            for (int i = 0; i < amount; i++)
            {
                if (i + 1 == amount)
                {
                    values[i] = data[data.Count() - 1];
                    break;
                }
                values[i] = data[i  * data.Count() / amount];
            }
            return values;
        }

        void InterpolationErmit(double[] X, double[] Y)
        {
            Fx2 = chart1.Series.Add("t(x)");
            Fx2.ChartType = SeriesChartType.Line;
            Fx2.BorderWidth = 3;
            Fx2.Color = Color.Blue;
            int m = X.Length;
            for (int i = 0; i < m- 1; i++)
            {
                var xHermite = AkimaSpline.HermiteX(X, i);
                var yHermite = AkimaSpline.HermiteY(X, Y, xHermite, i);

                for (int j = 0; j < AkimaSpline.CYCLE + 1; j++)
                {
                    if (!double.IsNaN(yHermite[j]) && !double.IsInfinity(yHermite[j]))
                    {
                        Fx2.Points.AddXY(xHermite[j], yHermite[j]);
                    }
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            PreparareForm(1, 12, -5, 30);
            Fx1 = chart1.Series.Add("Temperatūra");
            Fx1.ChartType = SeriesChartType.Point;
            Fx1.MarkerSize = 7;
            Fx2 = chart1.Series.Add("t(x)");
            Fx2.ChartType = SeriesChartType.Line;
            Fx2.BorderWidth = 3;
            Fx2.Color = Color.Blue;
            Fx1.BorderWidth = 3;
            Fx1.Color = Color.Red;

            var minX = 0;
            var maxX = 12;
            for (int x = minX; x < maxX; x++)
            {
                Fx1.Points.AddXY(temperatures[x].X, temperatures[x].Y);
            }

            var polyCount = 2;
            var matrix = MakeLeastSquareMatrix(temperatures, polyCount);
            var yVector = GetYVector(temperatures);
            var A = Matrix.MultiplyMatrix(matrix, matrix, polyCount, temperatures.Count, temperatures.Count, polyCount);
            var B = Matrix.MultiplyVector(matrix, yVector, polyCount, temperatures.Count);
            var X = Matrix.SolveEq(A, B, polyCount, temperatures.Count, temperatures.Count, polyCount, false);


            for (double x = 1; x <= temperatures.Count; x += 0.01)
            {
                Fx2.Points.AddXY(x, CalcPoly(X, x));
            }
        }



        private double CalcPoly(double[] X, double x)
        {
            double res = 0;
            for (int i = 0; i < X.Count(); i++)
            {
                res += X[i] * Math.Pow(x, i);
            }

            return res;
        }

        private double[][] MakeLeastSquareMatrix(List<Point> points, int polyCount)
        {
            var matrix = new double[polyCount][];
            for (int i = 0; i < polyCount; i++)
            {
                matrix[i] = new double[points.Count];
                for (int j = 0; j < points.Count; j++)
                {
                    matrix[i][j] = Math.Pow(points[j].X, i);
                }
            }

            return matrix;
        }

        private double[] GetYVector(List<Point> points)
        {
            var vector = new double[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                vector[i] = points[i].Y;
            }

            return vector;
        }

        private static double Func(double x)
        {
            return Math.Log(x) / (Math.Sin(2 * x) + 2.5) - x / 7;
        }
    }
}
