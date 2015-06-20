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
        public double value { get; set; }
        public double height { get; set; }
        public PROCESS process { get; set; }
    }

    public enum PROCESS
    {
        PRECALCULATION_N,
        PRECALCULATION_Q,
        CALCULATION_MAIN,
    }

    public struct IterationUnit
    {
        public double value;
        public double creation;
        public double loss;
        public bool isSuits;
        public IterationUnit(double value, bool isSuits) {
            this.value = value;
            this.isSuits = isSuits;
            this.creation = 0;
            this.loss = 0;
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
        public double ionPositiveMass;
        public double ionNegativeMass;
        public int iterationLimit;
        public int capacity;
        public double ne0;
        public IterationUnit[] electronGrid;
        public double nip0;
        public IterationUnit[] ionPositiveGrid;
        public double nin0;
        public IterationUnit[] ionNegativeGrid;
        public double totalContent;
        public IterationUnit[] electronVelocityGrid;
        public double neVel0;
        public IterationUnit[] ionPosVelocityGrid;
        public double nipVel0;
        public IterationUnit[] ionNegVelocityGrid;
        public double ninVel0;
        public List<Element> aerosolElements;
        public Dictionary<double, double> fullNCalculated;
        public Dictionary<double, double> qCalculated;
        public Dictionary<double, double> differences;
        public static Dictionary<string, double> temperature;
        public Element activeElement;

        public event EventHandler<SphereEventArgs> stateCalculated;
        public event EventHandler<SphereEventArgs> calculationsDone;
        public event EventHandler<SphereEventArgs> preCalculateNProgessChanged;
        public event EventHandler<SphereEventArgs> preCalculateQProgessChanged;

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
            ionPositiveGrid = new IterationUnit[this.capacity];
            ionNegativeGrid = new IterationUnit[this.capacity];
            electronVelocityGrid = new IterationUnit[this.capacity];
            ionPosVelocityGrid = new IterationUnit[this.capacity];
            ionNegVelocityGrid = new IterationUnit[this.capacity];
            
            for (int i = 0; i < this.capacity; i++) {
                electronGrid[i] = new IterationUnit(ne0, false);
                ionPositiveGrid[i] = new IterationUnit(nip0, false);
                ionNegativeGrid[i] = new IterationUnit(nin0, false);
                electronVelocityGrid[i] = new IterationUnit(data.velocity, false);
                ionPosVelocityGrid[i] = new IterationUnit(data.velocity, false);
                ionNegVelocityGrid[i] = new IterationUnit(data.velocity, false);
            }
        }

        public void preCalculateData() {
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

        protected virtual void OnPreCalculateNProgressChanged(SphereEventArgs e) {
            EventHandler<SphereEventArgs> handler = preCalculateNProgessChanged;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnPreCalculateQProgressChanged(SphereEventArgs e) {
            EventHandler<SphereEventArgs> handler = preCalculateQProgessChanged;
            if (handler != null) {
                handler(this, e);
            }
        }

        // Рассчитываем полные концентрации на всех высотых
        private void preCalculateN() {
            for (int step = 0; step < capacity; step++) {
                double height = step * delta;
                double value = activeElement.getFullNFromHeight(height);
                fullNCalculated.Add(height, value);
                SphereEventArgs args = new SphereEventArgs();
                args.height = height;
                args.state = step;
                args.value = value;
                args.process = PROCESS.PRECALCULATION_N;
                OnPreCalculateNProgressChanged(args);
            }
        }

        // Рассчитываем ионизацию на всех высотах
        private void preCalculateQ() {
            for (int step = 0; step < capacity; step++) {
                double height = step * delta;
                double value = q3(height);
                qCalculated.Add(height, value);
                SphereEventArgs args = new SphereEventArgs();
                args.height = height;
                args.state = step;
                args.value = value;
                args.process = PROCESS.PRECALCULATION_Q;
                OnPreCalculateQProgressChanged(args);
            }
        }

        private void calculateMasses() {
            // Рассчитаем массы частиц
            electronMass = Constants.eMass;
            ionPositiveMass = (activeElement.m / Constants.Na) * (1E-3) - Constants.eMass;
            ionNegativeMass = 0;
            foreach (Element el in aerosolElements) {
                ionNegativeMass += el.m / Constants.Na;
            }
            ionNegativeMass *= (1E-3);
            ionNegativeMass += 300 * aerosolElements.Count * Constants.eMass;
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
                    // Новый расчет
                    // Поступление частицы
                    double ionization = qCalculated[height];
                    electronGrid[i].creation = ionization;
                    ionPositiveGrid[i].creation = ionization;
                    double sticking = stickTo(electronGrid[i].creation, height);
                    ionNegativeGrid[i].creation = sticking;

                    // Потери частиц
                    double recombination = recombinate(electronGrid[i].creation, ionPositiveGrid[i].creation);
                    double neutralization = neutralize(ionPositiveGrid[i].creation, ionNegativeGrid[i].creation);
                    electronGrid[i].loss = recombination + sticking;
                    ionPositiveGrid[i].loss = recombination + neutralization;
                    ionNegativeGrid[i].loss = neutralization;

                    // Наводим порядок
                    double electrons = electronGrid[i].value + Constants.dt * (electronGrid[i].creation - electronGrid[i].loss);
                    double ionsPlus = ionPositiveGrid[i].value + Constants.dt * (ionPositiveGrid[i].creation - ionPositiveGrid[i].loss);
                    double ionsMinus = ionNegativeGrid[i].value + Constants.dt * (ionNegativeGrid[i].creation - ionNegativeGrid[i].loss);

                    if (electronGrid[i].isSuits == false) {
                        if (Mathematical.compareWithFault(electrons, electronGrid[i].value, epsilum)) {
                            electronGrid[i].value = electrons;
                            electronGrid[i].isSuits = true;
                        }
                        else {
                            electronGrid[i].value = electrons;
                            electronGrid[i].isSuits = false;
                        }
                    }

                    if (ionPositiveGrid[i].isSuits == false) {
                        if (Mathematical.compareWithFault(ionsPlus, ionPositiveGrid[i].value, epsilum)) {
                            ionPositiveGrid[i].value = ionsPlus;
                            ionPositiveGrid[i].isSuits = true;
                        }
                        else {
                            ionPositiveGrid[i].value = ionsPlus;
                            ionPositiveGrid[i].isSuits = false;
                        }
                    }

                    if (ionNegativeGrid[i].isSuits == false) {
                        if (Mathematical.compareWithFault(ionsMinus, ionNegativeGrid[i].value, epsilum)) {
                            ionNegativeGrid[i].value = ionsMinus;
                            ionNegativeGrid[i].isSuits = true;
                        }
                        else {
                            ionNegativeGrid[i].value = ionsMinus;
                            ionNegativeGrid[i].isSuits = false;
                        }
                    }

                    iteration++;
                    everythingSuits = electronGrid[i].isSuits && ionPositiveGrid[i].isSuits && ionNegativeGrid[i].isSuits;
                }
                result += electronGrid[i].value + ionPositiveGrid[i].value + ionNegativeGrid[i].value;
                // Вызываем эвент о каждой стадии
                SphereEventArgs args = new SphereEventArgs();
                args.state = i;
                args.value = result;
                args.height = height;
                args.process = PROCESS.CALCULATION_MAIN;
                OnStateCalculated(args);
            }
            allVelocities();
            // Сообщаем что закончили вычисления
            SphereEventArgs finalArgs = new SphereEventArgs();
            finalArgs.state = capacity;
            finalArgs.value = result;
            finalArgs.height = height;
            finalArgs.process = PROCESS.CALCULATION_MAIN;
            OnCalculationsDone(finalArgs);
            totalContent = result;
            return result;
        }

        public void allVelocities() {
            double height = 0;
            for (int i = 1; i < (capacity-1); i++) {
                height = i * delta;
                electronVelocityGrid[i].value = velocity2(electronVelocityGrid[i].value, electronGrid[i - 1].value, electronGrid[i].value, electronGrid[i + 1].value, electronMass, height);
                ionPosVelocityGrid[i].value = velocity2(ionPosVelocityGrid[i].value, ionPositiveGrid[i - 1].value, ionPositiveGrid[i].value, ionPositiveGrid[i + 1].value, ionPositiveMass, height);
                ionNegVelocityGrid[i].value = velocity2(ionNegVelocityGrid[i].value, ionNegativeGrid[i - 1].value, ionNegativeGrid[i].value, ionNegativeGrid[i + 1].value, ionNegativeMass, height);
            }
            electronVelocityGrid[capacity - 1].value = electronVelocityGrid[capacity - 2].value;
            ionPosVelocityGrid[capacity - 1].value = ionPosVelocityGrid[capacity - 2].value;
            ionNegVelocityGrid[capacity - 1].value = ionNegVelocityGrid[capacity - 2].value;
        }

        // Находим силу тока
        public double electricity() {
            double result = 1;
            int last = capacity - 1;
            double electronCurrent = -Constants.qe * electronGrid[last].value * electronVelocityGrid[last].value;
            System.Diagnostics.Debug.WriteLine("Electron = " + electronCurrent);
            double ionPlusCurrent = Constants.qe * ionPositiveGrid[last].value * ionPosVelocityGrid[last].value;
            System.Diagnostics.Debug.WriteLine("Ion plus = " + ionPlusCurrent);
            double ionMinusCurrent = -Constants.qe * ionNegativeGrid[last].value * 300 * ionNegVelocityGrid[last].value;
            System.Diagnostics.Debug.WriteLine("Ion minus = " + ionMinusCurrent);
            result = electronCurrent + ionPlusCurrent + ionMinusCurrent;
            return result;
        }

        // Скорость от двух узлов, верхнего и нижнего
        public double velocity2(double velPrev, double nPrev, double nCur, double nNext, double mass, double height) {
            double result = 0;
            double tPrev = 273; // temperatureForHeight(height - delta);
            double tNext = 273; // temperatureForHeight(height + delta);
            double pressure = (p(nNext, tNext) - p(nPrev, tPrev)) / delta;
            double gradient = pressure/2;

            result += velPrev + Constants.dt * (-gradient / (mass * nCur) - Constants.g);
            return result;
        }

        // Нахождение скорости частицы от предыдущего узла
        public double velocity(double velPrev, double nPrev, double nCur, double mass, double height) {
            double result = 0;
            double tCur = 273; //temperatureForHeight(height);
            double tPrev = 273; //temperatureForHeight(height - delta);
            double p1 = p(nCur, tCur);
            double p0 = p(nPrev, tPrev);
            double gradient = (p(nCur, tCur) - p(nPrev, tPrev)) / delta;
            double tmp = -gradient / (mass * nCur);
            result += velPrev + Constants.dt * (-gradient / (mass * nCur) - Constants.g);
            return result;
        }

        public double pressureGradient(double nPrev, double nCur, double height) {
            double tCur = temperatureForHeight(height);
            double tPrev = temperatureForHeight(height - delta);
            double p1 = p(nCur, tCur);
            double p0 = p(nPrev, tPrev);
            double gradient = (p(nCur, tCur) - p(nPrev, tPrev)) / delta;
            return gradient;
        }

        // Сила давления на частицу
        public double p(double n, double t) {
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
            recombination = recombinate(nePrev, nePrev);
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
            recombination = recombinate(nipPrev, electronGrid[step].value);
            //recombination = recombinate(activeElement, creation, creation);
            loss += recombination;
            double neutralization; 
            neutralization = neutralize(nipPrev, ionNegativeGrid[step].value);
            //neutralization = neutralize(creation - loss, ionMinusGrid[step].value);
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
            neutralization = neutralize(ionPositiveGrid[step].value, ninPrev);
            //neutralization = neutralize(ionPlusGrid[step].value, creation);
            loss += neutralization;
            return (ninPrev + (creation - loss) * Constants.dt);
        }

        // Скорость фотоионизации
        // По Ермакову, Стожкову
        public double q3(double height) {
            double flux = photonFlux3(height);
            double result = activeElement.getNForHeight(height) * flux * Constants.sigma;
            return result;
        }

        // Вычисляем поток фотонов с длиной волны wave на высоте height
        // По Ермакову, Стожкову
        public double photonFlux3(double height) {
            double flux = Constants.absorption * fullNCalculated[height];
            double exp = Math.Exp(-flux);
            double result = Constants.eternityFlux * exp;
            return result;
        }

        // Рекомбинация
        private double recombinate(double electrons, double ions) {
            double result = 0;
            result = activeElement.recombCoeff * electrons * ions;
            return result;
        }

        // Нейтрализация
        private double neutralize(double ionsPlus, double ionsMinus) {
            double result = 0;
            result = Constants.gamma * ionsPlus * ionsMinus;
            return result;
        }

        // Прилипание
        private double stickTo(double electrons, double height) {
            double result = 0;
            foreach (Element el in aerosolElements) {
                double beta = Mathematical.beta(el.radius);
                if (beta < 0) continue;
                double aerosol = el.getNForHeight(height);
                result += beta * aerosol;
            }
            result *= electrons;
            if (result > electrons) result = electrons;
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
