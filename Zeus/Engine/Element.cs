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
        public double nPercent;
        public double t;
        public double n0; /*{
            get {
                return (Constants.p * (0.72E+23))/t;
                //return Convert.ToDecimal((this.nPercent * Constants.Na) / (100 * Constants.Vm));
                // Убираем умножение на постоянную Авогардо, потому что слишком много
                //return Convert.ToDecimal((this.nPercent) / (100 * Constants.Vm));
                // дичь какая-то
            }
        }*/
        public double maxWave;
        public Dictionary<string, double> atomCrossSections;
        public Dictionary<string, double> photonCrossSections;
        public double recombCoeff;
        public double radius;

        public Element() {

        }

        public double getNForHeight(double height) {
            double result = n0;
            double temperature = Sphere.temperatureForHeight(height);
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

        public double tryGetValueForKey(string key, Dictionary<string, double> dict) {
            if (String.IsNullOrEmpty(key)) return 0;
            double value = dict[key];
            return value;
        }

        // Получение значения сечения фотоионизации по ключу словаря
        public double getAtomCSValueForKey(string key) {
            if (String.IsNullOrEmpty(key)) return 0;
            double value = atomCrossSections[key];
            return value;
        }

        // Получение значения сечения поглощения фотона по ключу словаря
        public double getPhotonCSValueForKey(string key) {
            if (String.IsNullOrEmpty(key)) return 0;
            double value = photonCrossSections[key];
            return value;
        }

        // Получение значения сечения фотоионизации по длине волны
        public double getAtomCSForWave(double wave) {
            string key = tryGetKeyForValue(wave, atomCrossSections);
            if (key != null) {
                return atomCrossSections[key];
            }
            else return 0;
        }

        // Получение значения сечения поглощения фотона по длине волны
        public double getPhotonCSForWave(double wave) {
            string key = tryGetKeyForValue(wave, photonCrossSections);
            if (key != null) {
                return photonCrossSections[key];
            }
            else return 0;
        }

        private string tryGetKeyForValue(double value, Dictionary<string, double> dict) {
            foreach (string key in dict.Keys) {
                string[] mass = key.Split('-');
                if (mass.Length != 0) {
                    try {
                        if (mass.Length == 1) {
                            double edge;
                            if (Double.TryParse(mass[0], System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out edge)) {
                                if (value <= edge) {
                                    return key;
                                } 
                            }
                        }
                        else if (mass.Length == 2) {
                            double low = -1;
                            double high = -1;
                            bool convertSuccessful = Double.TryParse(mass[0], System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out low) && Double.TryParse(mass[1], System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out high);
                            if (convertSuccessful && ((value >= low && value <= high) || value <= low)) {
                                return key;
                            }
                        }
                    }
                    catch (Exception error) {
                        LogManager.Session.logMessage(error.Message + " In " + key);
                        Environment.Exit(-1);
                    }
                }
            }
            return null;
        }

        private bool isDictCorrectForKey(Dictionary<string, double> data, string key, double min, double max) {
            double value;
            if (data.TryGetValue(key, out value)) {
                if (value > min && value < max) return true;
                else {
                    LogManager.Session.logMessage("Element  <" + name + "> key[" + key + "] out of range");
                    return false;
                }
            }
            else {
                LogManager.Session.logMessage("Element <" + name + "> has no key [" + key + "] field in dict");
                return false;
            }
        }

        public double convertProcentToConc(double value) {
            return ((value * Constants.Na)/(100 * Constants.Vm));
        }
    }
}
