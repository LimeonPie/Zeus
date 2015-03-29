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

        public Element(string name, Dictionary<string, double> dict) {
            this.name = name;
            if (dict != null) {
                if (isInBounds(dict, "index", 0, 119)) this.index = (short)dict["index"];
                else this.index = 0;

                if (isInBounds(dict, "m", 0, Double.MaxValue)) this.m = dict["m"];
                else this.m = 0;

                if (isInBounds(dict, "n0", 0, Double.MaxValue)) this.n0 = dict["n0"];
                else this.n0 = 0;
            }
        }

        public double getConcForHeight(double height) {
            double temperature = 23; // Температура, что-то с ней нужно сделать
            double power = -this.m * Constants.g * height / (Constants.k * temperature);
            return this.n0 * Math.Exp(power);
        }

        private bool isInBounds(Dictionary<string, double> data, string key, double min, double max) {
            double value;
            if (data.TryGetValue(key, out value)) {
                if (value > min && value < max) return true;
                else {
                    LogManager.Session.logMessage("Element with name <" + this.name + ">, " + key + " out of range");
                    return false;
                }
            }
            else {
                LogManager.Session.logMessage("Element with name <" + this.name + ">, no " + key + " field in dict");
                return false;
            }
        }
    }
}
