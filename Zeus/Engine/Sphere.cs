using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Zeus.Helpers;

namespace Zeus.Engine
{
	
	// Класс сферы, черт знает почему такое название
	// Ассоциация со слоем атмофсферы Земли, там все процессы происходят
	// Так или иначе, здесь происходят вычисления концентраций частиц и тока

    public class SphereEventArgs : EventArgs
    {
        public int state { get; set; }
        public double result { get; set; }
    }

    public class Sphere
    {

        public double botBoundary;
        public double topBoundary;
        public double longitude;
        public double latitude;
        public double delta;
        public int capacity;
        public double ne0;
        public double[] neGrid;
        public double nip0;
        public double[] nipGrid;
        public double nin0;
        public double[] ninGrid;
        public double totalConcentration;
        public double[] neVelGrid;
        public double neVel0;
        public double[] nipVelGrid;
        public double nipVel0;
        public double[] ninVelGrid;
        public double ninVel0;
        public List<Element> aerosolElements;
        public Dictionary<string, double> temperature;
        public Element activeElement;

        public event EventHandler<SphereEventArgs> stateCalculated;
        public event EventHandler<SphereEventArgs> calculationsDone;

        public Sphere(inputData data, Element active) {
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
            this.capacity = (int)((topBoundary - botBoundary) / delta) + 1;
            neGrid = new double[this.capacity];
            neGrid[0] = ne0;
            nipGrid = new double[this.capacity];
            nipGrid[0] = nip0;
            ninGrid = new double[this.capacity];
            ninGrid[0] = nin0;
            neVelGrid = new double[this.capacity];
            neVelGrid[0] = data.velocity;
            nipVelGrid = new double[this.capacity];
            nipVelGrid[0] = data.velocity;
            ninVelGrid = new double[this.capacity];
            ninVelGrid[0] = data.velocity;
            this.temperature = data.temperature;
        }

        protected virtual void OnStateCalculated(SphereEventArgs e) {
            EventHandler<SphereEventArgs> handler = stateCalculated;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnCalculationsDone(SphereEventArgs e) {
            EventHandler<SphereEventArgs> handler = calculationsDone;
            if (handler != null) {
                handler(this, e);
            }
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

                // Концентрации
                neGrid[i] = ne(i, neGrid[i - 1], height);
                nipGrid[i] = niPositive(i, nipGrid[i - 1], height);
                ninGrid[i] = niNegative(i, ninGrid[i - 1], height);

                // Скорость
                neVelGrid[i] = velocity(neVelGrid[i - 1], neGrid[i], neGrid[i - 1], Constants.eMass, height);
                nipVelGrid[i] = velocity(nipVelGrid[i - 1], nipGrid[i], nipGrid[i - 1], Constants.protonMass, height);
                ninVelGrid[i] = velocity(ninVelGrid[i - 1], ninGrid[i], ninGrid[i - 1], Constants.protonMass, height);

                result += neGrid[i] + nipGrid[i] + ninGrid[i];
                // Вызываем эвент о каждой стадии
                SphereEventArgs args = new SphereEventArgs();
                args.state = i;
                args.result = result;
                OnStateCalculated(args);
            }
            // Сообщаем что закончили вычисления
            SphereEventArgs finalArgs = new SphereEventArgs();
            finalArgs.state = capacity;
            finalArgs.result = result;
            OnCalculationsDone(finalArgs);
            totalConcentration = result;
            return result;
        }

        // Находим силу тока
        public double electricity() {
            double result = 1;
            int last = capacity - 1;
            result *= Constants.qe * ninGrid[last] * ninVelGrid[last] + Constants.qe * nipGrid[last] * nipVelGrid[last] - Constants.qe * neGrid[last] * neVelGrid[last];
            return result;
        }

        // Нахождение скорости частицы
        public double velocity(double velPrev, double nCur, double nPrev, double mass, double height) {
            double result = 0;
            if (nCur == 0 && nPrev == 0) return result;
            double t1 = temperatureForHeight(height);
            double t0 = temperatureForHeight(height - delta);
            double deltaMass = delta * mass;
            double chislitel = Constants.k * (nCur * t1 - nPrev * t0);
            result += velPrev - (Constants.k * (nCur * t1 - nPrev * t0)) / (delta * mass * nCur) + Constants.g;
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
            double flux = el.getPhotonCSValueForKey(key) * (1E-4) * el.getNForHeight(height);
            double hi = Mathematical.hi(latitude, longitude);
            double tay = Mathematical.sec(hi) * flux; 
            double exp = Math.Exp(-tay);
            double result = eternityFlux * exp;
            return result;
        }

        // Рекомбинация
        private double recombinate(Element el, double elN, double neN) {
            double result = 0;
            if (elN <= 0 || neN <= 0) return result;
            result = neN * el.recombCoeff * elN;
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

        // Температура по высоте
        private double temperatureForHeight(double height) {
            double result = temperature.Values.ElementAt(0);
            string tempKey = temperature.Keys.ElementAt(0);
            foreach (string key in temperature.Keys) {
                if (height > Convert.ToDouble(key)) {
                    tempKey = key;
                }
                else {
                    result = temperature[tempKey];
                    if (height == Convert.ToDouble(key)) {
                        result = temperature[key];
                    }
                    return result;
                }
            }
            return result;
        }
    }
}
