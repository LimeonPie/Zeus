using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeus.Helpers;

namespace Zeus.Engine
{
    public class Sphere
    {

        public uint botBoundary;
        public uint topBoundary;
        public double delta;
        public double ne0;
        public double nip0;
        public double nin0;

        public Sphere(double delta) {
            this.delta = delta;
        }

        // Концентрация электронов
        public double ne() {
            double sum = 0;
            return sum;
        }

        // Концентрация положительных ионов
        public double niPositive() {
            double sum = 0;
            return sum;
        }

        // Концентрация отрицательных ионов
        public double niNegative() {
            double sum = 0;
            return sum;
        }
    }
}
