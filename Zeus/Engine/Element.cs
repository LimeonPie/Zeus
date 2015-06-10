using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeus.Helpers;
using System.Globalization;

namespace Zeus.Engine
{

	// Класс элемента, будь то аэрозоль или элемент атмосферы
	// Служит для хранения и получения данных

    public class Element
    {
        public string name;
        public short index;
        public double m;
        public double t;
        public double n0;
        public double maxWave;
        public Dictionary<string, double> atomCrossSections;
        public Dictionary<string, double> photonCrossSections;
        public double recombCoeff;
        public double radius;

        public Element() {

        }

        // Барометрическая формула
        // Температура должна быть постоянной
        public double getNForHeight(double height) {
            double result = n0;
            double temperature = t; //Sphere.temperatureForHeight(height); 
            double power = -this.m * (1E-3) * Constants.g * height / (Constants.R * temperature);
            double exp = Math.Exp(power);
            return result * exp;
        }

        public double getFullNFromHeight(double height) {
            double result = getNForHeight(height);
            for (double step = height + 1; step <= Constants.atmosphereLimit; step++) {
                double conc = getNForHeight(step);
                result += conc;
            }
            return result;
        }

        private double H(double height) {
            return (m * Constants.g / (Constants.R * Sphere.temperatureForHeight(height)));
        }
    }
}
