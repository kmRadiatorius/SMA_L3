﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMA3Charts
{
    class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double A { get; set; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}