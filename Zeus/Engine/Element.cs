using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeus.Helpers;

namespace Zeus.Engine
{
    public class Element
    {

        public string name;
        public short index;
        public double m;
        public double n0;
        public double maxWave;
        public Dictionary<string, double> atomCrossSections;
        public Dictionary<string, double> photonCrossSections;
        public double recombCoeff;
        public double radius;

        public Element() {

        }

        public Element(Dictionary<string, double> dict) {
            if (dict != null) {
                if (isDictCorrectForKey(dict, "index", 0, 119)) this.index = (short)dict["index"];
                else this.index = 0;

                if (isDictCorrectForKey(dict, "m", 0, Double.MaxValue)) this.m = dict["m"];
                else this.m = 0;

                if (isDictCorrectForKey(dict, "n0", 0, Double.MaxValue)) this.n0 = dict["n0"];
                else this.n0 = 0;
            }
        }

        public double getNForHeight(double height) {
            double temperature = 23; // Температура, что-то с ней нужно сделать
            double power = -this.m * Constants.g * height / (Constants.k * temperature);
            return this.n0 * Math.Exp(power);
        }

        // Получение значения сечения фотоионизации по длине волны
        public double getAtomCSForWave(double wave) {
            string key = canGetKeyForValue(wave, atomCrossSections);
            if (key != null) {
                return atomCrossSections[key];
            }
            else return 0;
        }

        // Получение значения сечения поглощения фотона по длине волны
        public double getPhotonCSForWave(double wave) {
            string key = canGetKeyForValue(wave, photonCrossSections);
            if (key != null) {
                return photonCrossSections[key];
            }
            else return 0;
        }

        private string canGetKeyForValue(double value, Dictionary<string, double> dict) {
            foreach (string key in dict.Keys) {
                if (isInBounds(key, value)) return key;
            }
            return null;
        }

        private bool isInBounds(string bounds, double value) {
            string[] mass = bounds.Split('-');
            if (mass.Length != 0) {
                if (mass.Length == 1) {
                    double edge = Convert.ToDouble(mass[0]);
                    if (Math.Round(edge) == value) return true;
                    else return false;
                }
                else if (mass.Length == 2) {
                    double low = Convert.ToDouble(mass[0]);
                    double high = Convert.ToDouble(mass[1]);
                    if ((value >= Math.Round(low)) && (value <= Math.Round(high))) return true;
                    else return false;
                }
                return false;
            }
            else return false;
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
    }
}
