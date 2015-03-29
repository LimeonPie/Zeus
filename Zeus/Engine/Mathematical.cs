using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Zeus.Engine
{
    public static class Mathematical
    {

    	// ############
    	// ###PUBLIC###
    	// ############

        public static double beta(double r) {
            return (2.35 * Math.Pow(10, -4) * Math.Pow(r, 1.457));
        }

        public static double n(double t, double latitude, double longitude, int height, int Z) {
            double b = beta(0);
            double a = 0;
            double root = Math.Sqrt(b*b*Z*Z + 4*a*q(latitude, longitude, height));
            double first = (root - b*Z)/(2*a);
            double second = (1 - Math.Exp(-root * t)) / (1 + Math.Exp(-root * t));
            return first*second;
        }

        public static double q(double latitude, double longitude, double height) {
            if (height == 0)
                return 0;
            double n = Chemical.Instance.getConcentrationForHeight(height);
            double maxWave = Chemical.Instance.maxWave;
            double wave = 0;
            double sumFlux = 0;
            while (wave <= maxWave) {
                sumFlux += photonFlux(wave, latitude, longitude, height) * Chemical.Instance.getAtomCSForWave(wave);
                wave++;
            }
            return Chemical.Instance.getConcentrationForHeight(height) * sumFlux;
        }

        // #############
    	// ###PRIVATE###
    	// #############

        // Вычисляем поток фотонов с длиной волны wave на высоте height
        private static double photonFlux(double wave, double latitude, double longitude, double height) {
        	double eternityFlux = 1; // Поток бесконечности - должно быть какая-то константа
            /*double sum = 0;
            foreach (double pcs in Chemical.Instance.photonCrossSections.Values) {
                sum += pcs * Chemical.Instance.getConcentrationForHeight(height);  
            }*/
            double flux = Chemical.Instance.getPhotonCSForWave(wave) * Chemical.Instance.getConcentrationForHeight(height);
            double tay = -sec(hi(latitude, longitude)) * flux;
            return eternityFlux * Math.Exp(-tay);
        }

        private static double degreesToRadians(double degrees) {
            if (degrees != 0) return (degrees * Math.PI / 180);
            else return 0;    
        }

        private static double radiansToDegrees(double radians) {
            if (radians != 0) return (radians * 180 / Math.PI);
            else return 0; 
        }

        // Вычисляем hi зенитный угол Солнца
        // TODO!!!! Функция Чепмена!
        // При больших зенитных углах > 80 нужно заменить sec (hi) на функцию Чемпена
        private static double hi(double latitude, double longitude) {
            int day = Time.timeInPos(latitude, longitude).DayOfYear;
            double delta = Math.Atan(Math.Tan(degreesToRadians(23.5)) * Math.Sin(2*Math.PI*(day-80)/365));
            double res = Math.Sin(delta) * Math.Sin(latitude) + Math.Cos(delta) * Math.Cos(latitude) * Math.Cos(Constants.earthRotVel * Time.timeAfterNoon(latitude, longitude));
            return Math.Acos(res);
        }

        private static double sec(double anger) {
            return (1 / Math.Cos(anger));
        }
    }
}
