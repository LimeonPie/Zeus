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

    public struct IterationUnit
    {
        public double value;
        public bool isSuits;
        public IterationUnit(double value, bool isSuits) {
            this.value = value;
            this.isSuits = isSuits;
        }
    }

    public class Sphere
    {

        public double botBoundary;
        public double topBoundary;
        public double longitude;
        public double latitude;
        public double delta;
        public double epsilum;
        public double electronMass;
        public double ionPlusMass;
        public double ionMinusMass;
        public int iterationLimit;
        public int capacity;
        public double ne0;
        public IterationUnit[] electronGrid;
        public double nip0;
        public IterationUnit[] ionPlusGrid;
        public double nin0;
        public IterationUnit[] ionMinusGrid;
        public double totalContent;
        public IterationUnit[] electronVelocityGrid;
        public double neVel0;
        public IterationUnit[] ionPlusVelocityGrid;
        public double nipVel0;
        public IterationUnit[] ionMinusVelocityGrid;
        public double ninVel0;
        public List<Element> aerosolElements;
        public Dictionary<double, double> fullNCalculated;
        public Dictionary<double, double> qCalculated;
        public static Dictionary<string, double> temperature;
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
            this.epsilum = data.epsilum;
            this.iterationLimit = data.timeInterval;
            this.activeElement = active;
            this.latitude = data.latitude;
            this.longitude = data.longitude;
            temperature = data.temperature;
            this.capacity = (int)((topBoundary - botBoundary) / delta) + 1;
            electronGrid = new IterationUnit[this.capacity];
            ionPlusGrid = new IterationUnit[this.capacity];
            ionMinusGrid = new IterationUnit[this.capacity];
            electronVelocityGrid = new IterationUnit[this.capacity];
            ionPlusVelocityGrid = new IterationUnit[this.capacity];
            ionMinusVelocityGrid = new IterationUnit[this.capacity];
            
            for (int i = 0; i < this.capacity; i++) {
                electronGrid[i] = new IterationUnit(ne0, false);
                ionPlusGrid[i] = new IterationUnit(nip0, false);
                ionMinusGrid[i] = new IterationUnit(nin0, false);
                electronVelocityGrid[i] = new IterationUnit(data.velocity, false);
                ionPlusVelocityGrid[i] = new IterationUnit(data.velocity, false);
                ionMinusVelocityGrid[i] = new IterationUnit(data.velocity, false);
            }

            // Предвычисление полных концентраций и
            // потока частицы на разной высоте
            fullNCalculated = new Dictionary<double, double>();
            preCalculateN();
            qCalculated = new Dictionary<double, double>();
            preCalculateQ();
            calculateMasses();
        }

        // Эвенты
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

        private void preCalculateN() {
            for (int step = 0; step < capacity; step++) {
                double level = step * delta;
                fullNCalculated.Add(level, activeElement.getFullNFromHeight(level));
            }
        }

        private void preCalculateQ() {
            for (int step = 0; step < capacity; step++) {
                double level = step * delta;
                qCalculated.Add(level, q3(activeElement, level));
            }
        }

        private void calculateMasses() {
            // Рассчитаем массы частиц
            electronMass = Constants.eMass;
            ionPlusMass = (activeElement.m / Constants.Na) * (1E-3) - Constants.eMass;
            ionMinusMass = 0;
            foreach (Element el in aerosolElements) {
                ionMinusMass += el.m / Constants.Na;
            }
            ionMinusMass *= (1E-3);
            ionMinusMass -= aerosolElements.Count * Constants.eMass;
        }

        public double n() {
            double height = 0;
            double result = ne0 + nip0 + nin0;

            for (int i = 1; i < capacity; i++) {
                height = i * delta;

                int iteration = 1;
                bool everythingSuits = false;
                // Итерация по концентрациям
                while ((iteration <= iterationLimit) && !everythingSuits) {
                    double electrons = ne(i, electronGrid[i].value, height);
                    if (Mathematical.compareWithFault(electrons, electronGrid[i].value, epsilum)) {
                        electronGrid[i] = new IterationUnit(electrons, true);
                    }
                    else {
                        electronGrid[i] = new IterationUnit(electrons, false);
                    }

                    double ionsPlus = niPositive(i, ionPlusGrid[i].value, height);
                    if (Mathematical.compareWithFault(ionsPlus, ionPlusGrid[i].value, epsilum)) {
                        ionPlusGrid[i] = new IterationUnit(ionsPlus, true);
                    }
                    else {
                        ionPlusGrid[i] = new IterationUnit(ionsPlus, false);
                    }

                    double ionsMinus = niNegative(i, ionMinusGrid[i].value, height);
                    if (Mathematical.compareWithFault(ionsMinus, ionMinusGrid[i].value, epsilum)) {
                        ionMinusGrid[i] = new IterationUnit(ionsMinus, true);
                    }
                    else {
                        ionMinusGrid[i] = new IterationUnit(ionsMinus, false);
                    }

                    iteration++;
                    everythingSuits = electronGrid[i].isSuits && ionPlusGrid[i].isSuits && ionMinusGrid[i].isSuits;
                }

                // Концентрации как было без цикла
                /*electronGrid[i].value = ne(i, electronGrid[i].value, height);
                ionPlusGrid[i].value = niPositive(i, ionPlusGrid[i].value, height);
                ionMinusGrid[i].value = niNegative(i, ionMinusGrid[i].value, height);*/

                // Итерация по скорости посередине
                // Скорость
                iteration = 1;
                everythingSuits = false;
                while ((iteration <= iterationLimit) && !everythingSuits) {
                    double electronV = velocity(electronVelocityGrid[i].value, electronGrid[i - 1].value, electronGrid[i].value, electronMass, height);
                    if (Mathematical.compareWithFault(electronV, electronVelocityGrid[i].value, epsilum)) {
                        electronVelocityGrid[i] = new IterationUnit(electronV, true);
                    }
                    else {
                        electronVelocityGrid[i] = new IterationUnit(electronV, false);
                    }

                    double ionPlusV = velocity(ionPlusVelocityGrid[i].value, ionPlusGrid[i - 1].value, ionPlusGrid[i].value, ionPlusMass, height);
                    if (Mathematical.compareWithFault(ionPlusV, ionPlusVelocityGrid[i].value, epsilum)) {
                        ionPlusVelocityGrid[i] = new IterationUnit(ionPlusV, true);
                    }
                    else {
                        ionPlusVelocityGrid[i] = new IterationUnit(ionPlusV, false);
                    }

                    double ionMinusV = velocity(ionMinusVelocityGrid[i].value, ionMinusGrid[i - 1].value, ionMinusGrid[i].value, ionMinusMass, height);
                    if (Mathematical.compareWithFault(ionMinusV, ionMinusVelocityGrid[i].value, epsilum)) {
                        ionMinusVelocityGrid[i] = new IterationUnit(ionMinusV, true);
                    }
                    else {
                        ionMinusVelocityGrid[i] = new IterationUnit(ionMinusV, false);
                    }

                    iteration++;
                    everythingSuits = electronVelocityGrid[i].isSuits && ionPlusVelocityGrid[i].isSuits && ionMinusVelocityGrid[i].isSuits;
                }

                // Скорость как было, без цикла
                /*
                if (i > 1) {
                    electronVelocityGrid[i - 1].value = velocity2(electronVelocityGrid[i - 1].value, electronGrid[i - 2].value, electronGrid[i - 1].value, electronGrid[i].value, electronMass, height);
                    ionPlusVelocityGrid[i - 1].value = velocity2(ionPlusVelocityGrid[i - 1].value, ionPlusGrid[i - 2].value, ionPlusGrid[i - 1].value, ionPlusGrid[i].value, positiveIonMass, height);
                    ionMinusVelocityGrid[i - 1].value = velocity2(ionMinusVelocityGrid[i - 1].value, ionMinusGrid[i - 2].value, ionMinusGrid[i - 1].value, ionMinusGrid[i].value, aerosolMass, height);
                }
                if (i == (capacity - 1)) {
                    electronVelocityGrid[i].value = velocity(electronVelocityGrid[i].value, electronGrid[i - 1].value, electronGrid[i].value, electronMass, height);
                    ionPlusVelocityGrid[i].value = velocity(ionPlusVelocityGrid[i].value, ionPlusGrid[i - 1].value, ionPlusGrid[i].value, positiveIonMass, height);
                    ionMinusVelocityGrid[i].value = velocity(ionMinusVelocityGrid[i].value, ionMinusGrid[i - 1].value, ionMinusGrid[i].value, aerosolMass, height);
                }
                */
                result += electronGrid[i].value + ionPlusGrid[i].value + ionMinusGrid[i].value;
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
            totalContent = result;
            return result;
        }

        // Находим силу тока
        public double electricity() {
            double result = 1;
            int last = capacity - 1;
            result = Constants.qe * ionPlusGrid[last].value * ionPlusVelocityGrid[last].value - Constants.qe * ionMinusGrid[last].value * ionMinusVelocityGrid[last].value - Constants.qe * electronGrid[last].value * electronVelocityGrid[last].value;
            return result;
        }

        // Скорость от двух узлов, верхнего и нижнего
        public double velocity2(double velPrev, double nPrev, double nCur, double nNext, double mass, double height) {
            double result = 0;
            double tCur = temperatureForHeight(height);
            double tPrev = temperatureForHeight(height - delta);
            double tNext = temperatureForHeight(height + delta);

            double gradient = (p(nCur, tCur) - p(nPrev, tPrev))/delta + (p(nNext, nNext) - p(nCur, tCur))/delta;

            result += velPrev + Constants.dt * (-Constants.g - (1/(2*mass*nCur))*gradient);
            return result;
        }

        // Нахождение скорости частицы от предыдущего узла
        public double velocity(double velPrev, double nPrev, double nCur, double mass, double height) {
            double result = 0;
            double tCur = temperatureForHeight(height);
            double tPrev = temperatureForHeight(height - delta);
            double gradient = (p(nCur, tCur) - p(nPrev, tPrev)) / delta;
            result += velPrev + Constants.dt * (-gradient / (mass * nCur) - Constants.g);
            return result;
        }

        // Сила давления на частицу
        private double p(double n, double t) {
            double result = 1;
            result = Constants.k * n * t;
            return result;
        }

        // Концентрация электронов
        public double ne(int step, double nePrev, double height) {
            // Создание
            double creation = 0;
            //double ionization = q(activeElement, height);
            double ionization = qCalculated[height];
            creation += ionization;
            // Потери
            double loss = 0;
            double recombination;
            recombination = recombinate(activeElement, nePrev, nePrev);
            //recombination = recombinate(activeElement, creation, creation);
            loss += recombination;
            double sticking;
            sticking = stickTo(nePrev, height);
            //sticking = stickTo(creation - loss, height);
            loss += sticking;
            return (nePrev + (creation - loss) * Constants.dt);
        }

        // Концентрация положительных ионов
        public double niPositive(int step, double nipPrev, double height) {
            // Создание
            double creation = 0;
            //double ionization = q(activeElement, height);
            double ionization = qCalculated[height];
            creation += ionization;
            // Потери
            double loss = 0;
            double recombination;
            recombination = recombinate(activeElement, nipPrev, electronGrid[step].value);
            //recombination = recombinate(activeElement, creation, electronGrid[step].value);
            loss += recombination;
            double neutralization; 
            neutralization = neutralize(nipPrev, ionMinusGrid[step].value);
            loss += neutralization;
            return (nipPrev + (creation - loss) * Constants.dt);
        }

        // Концентрация отрицательных ионов
        public double niNegative(int step, double ninPrev, double height) {
            // Создание
            double creation = 0;
            double sticking = stickTo(electronGrid[step].value, height);
            creation += sticking;
            // Потери
            double loss = 0;
            double neutralization;
            neutralization = neutralize(ionPlusGrid[step].value, ninPrev);
            //neutralization = neutralize(ionPlusGrid[step].value, creation);
            loss += neutralization;
            return (ninPrev + (creation - loss) * Constants.dt);
        }

        // Скорость фотоионизации
        // По Намгаладзе
        // Старый метод, для солнечных лучей
        public double q(Element el, double height) {
            double sum = 0;
            foreach (string key in el.atomCrossSections.Keys) {
                double flux = photonFlux(el, key, height);
                sum += flux * el.getAtomCSValueForKey(key) * (1E-4);
            }
            double conc = el.getNForHeight(height);
            // Включаем ионизациюс Земли
            //double earthIonization = 7E+6 + Constants.Q * Math.Exp(-2.362 * height);
            double result = el.getNForHeight(height) * sum; //+ earthIonization;
            return result;
        }

        // Вычисляем поток фотонов с длиной волны wave на высоте height
        // По Намгаладзе
        // Старый метод, для сонечных лучей
        public double photonFlux(Element el, string key, double height) {
            double eternityFlux = Constants.eternityFlux;
            // Включаем расширенный поток
            // flux = el.getPhotonCSValueForKey(key) * (1E-4) * el.getNForHeight(height);
            // Так все работало, но долго
            //double flux = el.getPhotonCSValueForKey(key) * (1E-4) * el.getFullNFromHeight(height);
            double diff = topBoundary - height;
            double flux = el.getPhotonCSValueForKey(key) * (1E-4) * diff * fullNCalculated[height];
            double hi = Mathematical.hi(latitude, longitude);
            double sec = Mathematical.sec(hi);
            double tay = Mathematical.sec(hi) * flux; 
            double exp = Math.Exp(-tay);
            double result = eternityFlux * exp;
            return result;
        }

        // Скорость фотоионизации
        // По Ермакову, Стожкову
        public double q3(Element el, double height) {
            double flux = photonFlux3(el, height);
            double result = el.getNForHeight(height) * flux * Constants.sigma;
            return result;
        }

        // Вычисляем поток фотонов с длиной волны wave на высоте height
        // По Ермакову, Стожкову
        public double photonFlux3(Element el, double height) {
            double flux = Constants.absorption * fullNCalculated[height];
            double hi = Mathematical.hi(latitude, longitude);
            double tay = Mathematical.sec(hi) * flux;
            double exp = Math.Exp(-tay);
            double result = Constants.eternityFlux * exp;
            return result;
        }

        // Рекомбинация
        private double recombinate(Element el, double elN, double neN) {
            double result = 0;
            // с самого начала было так:
            result = neN * el.recombCoeff * elN;
            //result = elN * el.recombCoeff;
            return result;
        }

        // Прилипание
        private double stickTo(double neN, double height) {
            double result = 0;
            foreach (Element el in aerosolElements) {
                double beta = Mathematical.beta(el.radius);
                if (beta < 0) continue;
                double aerosol = el.getNForHeight(height);
                result += beta * aerosol;
            }
            result *= neN;
            return result;
        }

        // Нейтрализация
        private double neutralize(double niP, double niN) {
            double result = 0;
            result = Constants.gamma * niP * niN;
            return result;
        }

        // Температура по высоте
        public static double temperatureForHeight(double height) {
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
