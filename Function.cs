using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMA3Charts
{
    class Function
    {
        private int pointNr;
        private double minX;
        private readonly double maxX;
        public List<Point> Points { get; set; }
        private Func<double, double> func;

        public Function(Func<double, double> func, double minX, double maxX, int pointNr = 30)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.pointNr = pointNr;
            this.func = func;
        }

        public double InterpolatedFuncY(double x)
        {
            double y = Points[0].Y;

            double totalT = 1;
            for (int i = 1; i < Points.Count; i++)
            {
                double root = Points[i].A;
                totalT *= x - Points[i - 1].X;
                root *= totalT;
                y += root;
            }

            return y;
        }

        public string GetNewtonsFunc()
        {
            StringBuilder s = new StringBuilder();
            StringBuilder t = new StringBuilder();
            s.Append(Points[0].A);
            for (int i = 1; i < Points.Count; i++)
            {
                t.Append("(x - x" + (i - 1) + ")");
                double root = Points[i].A;
                s.Append(" + " + Points[i].A + t);
            }

            return s.ToString();
        }

        public double DifFuncY(double x)
        {
            double x0 = 1e-7;

            return  (InterpolatedFuncY(x + x0) - InterpolatedFuncY(x)) / x0;
        }

        public void NewtonsInterpolation()
        {
            var tempA = new double[Points.Count];
            Points[0].A = Points[0].Y;
            for (int i = 0; i < Points.Count - 1; i++)
            {
                tempA[i] = (Points[i + 1].Y - Points[i].Y) / (Points[i + 1].X - Points[i].X);
            }
            Points[1].A = tempA[0];
            for (int i = 2; i < Points.Count; i++)
            {
                for (int j = 0; j < Points.Count - i; j++)
                {
                    tempA[j] = (tempA[j + 1] - tempA[j]) / (Points[i + j].X - Points[j].X);
                }
                Points[i].A = tempA[0];
            }
        }

        public List<Point> GetPoints()
        {
            List<Point> Points = new List<Point>();

            var intervals = GetIntervals();

            for (double x = minX; x <= maxX + intervals; x += intervals)
            {
                Points.Add(new Point(x, func(x)));
            }

            return Points;
        }

        public List<Point> GetChebyshevNodes()
        {
            List<Point> Points = new List<Point>();

            for (int i = 1; i <= pointNr; i++)
            {
                double x = GetChebyshevInterval(i);
                Points.Add(new Point(x, func(x)));
            }

            return Points;
        }

        private double GetChebyshevInterval(double k) => 0.5 * (minX + maxX) + 0.5 * (maxX - minX) * Math.Cos((2 * k - 1) / (2 * pointNr) * Math.PI);


        public double GetIntervals() => (maxX - minX) / pointNr;
    }
}
