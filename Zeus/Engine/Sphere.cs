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

        public double botBoundary;
        public double topBoundary;
        public double longitude;
        public double latitude;
        public double delta;
        private int capacity;
        public double ne0;
        public double[] neGrid;
        public double nip0;
        public double[] nipGrid;
        public double nin0;
        public double[] ninGrid;
        public List<Element> aerosolElements;
        public Element activeElement;

        public Sphere(InputData data, Element active) {
            this.ne0 = data.ne0;
            this.nip0 = data.nip0;
            this.nin0 = data.nin0;
            this.delta = data.delta;
            this.botBoundary = data.botBoundary;
            this.topBoundary = data.topBoundary;
            this.aerosolElements = data.aerosols;
            this.activeElement = active;
            this.latitude = data.latitude;
            this.longitude = data.longitude;
            this.capacity = (int)((topBoundary - botBoundary) / delta);
            neGrid = new double[this.capacity];
            neGrid[0] = ne0;
            nipGrid = new double[this.capacity];
            nipGrid[0] = nip0;
            ninGrid = new double[this.capacity];
            ninGrid[0] = nin0;
        }

        public void changeCoordinates(double longitude, double latitude) {
            this.latitude = latitude;
            this.longitude = longitude;
        }

        public double n() {
            double height = 0;
            double result = neGrid[0] + nipGrid[0] + ninGrid[0];
            for (int i = 1; i < capacity; i++) {
                height = i * delta;
                neGrid[i] = ne(i, neGrid[i - 1], height);
                nipGrid[i] = niPositive(i, nipGrid[i - 1], height);
                ninGrid[i] = niNegative(i, ninGrid[i - 1], height);
                result += neGrid[i] + nipGrid[i] + ninGrid[i];
                LogManager.Session.logMessage(String.Format("{0} stage is [{1}; {2}; {3}]", i, neGrid[i], nipGrid[i], ninGrid[i]));
            }
            return result;
        }

        // Концентрация электронов
        public double ne(int step, double nePrev, double height) {
            // Создание
            double creation = 0;
            double ionization = q(activeElement, height);
            creation += ionization;
            // Потери
            double loss = 0;
            double recombination;
            recombination = recombinate(activeElement, nePrev, neGrid[step - 1]);
            loss += recombination;
            double sticking = stickTo(neGrid[step - 1], height);
            loss += sticking;
            return (nePrev + (creation - loss) * Constants.dt);
        }

        // Концентрация положительных ионов
        public double niPositive(int step, double nipPrev, double height) {
            // Создание
            double creation = 0;
            double ionization = q(activeElement, height);
            creation += ionization;
            // Потери
            double loss = 0;
            double recombination;
            recombination = recombinate(activeElement, nipPrev, neGrid[step - 1]);
            loss += recombination;
            double neutralization = neutralize(nipGrid[step - 1], ninGrid[step - 1]);
            loss += neutralization;
            return (nipPrev + (creation - loss) * Constants.dt);
        }

        // Концентрация отрицательных ионов
        public double niNegative(int step, double ninPrev, double height) {
            // Создание
            double creation = 0;
            double sticking = stickTo(neGrid[step - 1], height);
            creation += sticking;
            // Потери
            double loss = 0;
            double neutralization = neutralize(nipGrid[step - 1], ninGrid[step - 1]);
            loss += neutralization;
            return (ninPrev + (creation - loss) * Constants.dt);
        }

        // Скорость фотоионизации
        private double q(Element el, double height) {
            double sum = 0;
            foreach (string key in el.atomCrossSections.Keys) {
                double flux = photonFlux(el, key, height);
                sum += flux * el.getAtomCSValueForKey(key) * (1E-4);
            }
            double conc = el.getNForHeight(height);
            double earthIonization = 7E+6 + Constants.Q * Math.Exp(-2.362 * height);
            double result = el.getNForHeight(height) * sum + earthIonization;
            return result;
        }

        // Вычисляем поток фотонов с длиной волны wave на высоте height
        private double photonFlux(Element el, string key, double height) {
            double eternityFlux = Constants.eternityFlux;
            double crossSection = el.getPhotonCSValueForKey(key);
            double flux = el.getPhotonCSValueForKey(key) * (1E-4) * el.getNForHeight(height);
            double hi = Mathematical.hi(latitude, longitude);
            double tay = Mathematical.sec(hi) * flux; // Hope that is not critical
            double exp = Math.Exp(-tay);
            double result = eternityFlux * exp;
            return result;
        }

        // Рекомбинация
        private double recombinate(Element el, double elN, double neN) {
            double result = 0;
            if (elN <= 0 || neN <= 0) return result;
            result = neN * el.recombCoeff * elN;
            System.Diagnostics.Debug.WriteLine("Recombination: " + elN + " * " + neN + " = " + result);
            return result;
        }

        // Прилипание
        private double stickTo(double neN, double height) {
            double result = 0;
            if (neN <= 0) return result;
            foreach (Element el in aerosolElements) {
                double beta = Mathematical.beta(el.radius);
                if (beta < 0) continue;
                double aerosol = el.getNForHeight(height);
                result += beta * (1E-6) * aerosol;
                System.Diagnostics.Debug.WriteLine("Z = Beta (" + beta + ") * aerosol[" + el.index + "]: " + result);
            }
            result *= neN;
            return result;
        }

        // Нейтрализация
        private double neutralize(double niP, double niN) {
            double result = 0;
            if (niP <= 0 || niN <= 0) return result;
            result = Constants.gamma * niP * niN;
            return result;
        }
    }
}
