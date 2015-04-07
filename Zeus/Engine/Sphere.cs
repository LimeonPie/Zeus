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

        public int botBoundary;
        public int topBoundary;
        public double delta;
        private int capacity;
        public double ne0;
        public double[] neGrid;
        public double nip0;
        public double[] nipGrid;
        public double nin0;
        public double[] ninGrid;

        public Sphere(double ne0, double nip0, double nin0, double delta, int botBoundary = 0, int topBoundary = 60000) {
            this.ne0 = ne0;
            this.nip0 = nip0;
            this.nin0 = nin0;
            this.delta = delta;
            this.botBoundary = botBoundary;
            this.topBoundary = topBoundary;
            this.capacity = (int)((topBoundary - botBoundary) / delta);
            neGrid = new double[this.capacity];
            neGrid[0] = ne0;
            nipGrid = new double[this.capacity];
            nipGrid[0] = nip0;
            ninGrid = new double[this.capacity];
            ninGrid[0] = nin0;
        }

        public void n() {
            for (int i = 1; i < capacity; i++) {
                neGrid[i] = ne(neGrid[i - 1]);
                nipGrid[i] = niPositive(nipGrid[i - 1]);
                ninGrid[i] = niNegative(ninGrid[i - 1]);
            }
        }

        // Концентрация электронов
        public double ne(double nePrev) {
            double ne = nePrev;
            // Создание
            double creation;

            // Потери
            double loss;
            return (ne + (creation - loss)*Constants.dt);
        }

        // Концентрация положительных ионов
        public double niPositive(double nipPrev) {
            double nip = nipPrev;
            // Создание
            double creation;
            // Потери
            double loss;
            return (nip + (creation - loss)*Constants.dt);
        }

        // Концентрация отрицательных ионов
        public double niNegative(double ninPrev) {
            double nin = ninPrev;
            // Создание
            double creation;
            // Потери
            double loss;
            return (nin + (creation - loss)*Constants.dt);
        }
    }
}
