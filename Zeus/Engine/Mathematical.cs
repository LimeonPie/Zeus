using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoTimeZone;

namespace Zeus.Engine
{
    public static class Mathematical
    {

    	// ############
    	// ###PUBLIC###
    	// ############

        public static DateTime timeInPos(double latitude = 0, double longitude = 0) {
            GeoTimeZone.TimeZoneResult zone = GeoTimeZone.TimeZoneLookup.GetTimeZone(latitude, longitude);
            TimeZoneInfo info = TimeZoneInfo.CreateCustomTimeZone(zone.Result, TimeSpan.Zero, "Patience", "Time");
            DateTime timeHere = TimeZoneInfo.ConvertTime(DateTime.Now, info);
            return timeHere;
        }

        public static double beta(double r) {
            return (2.35 * Math.Pow(10, -4) * Math.Pow(r, 1.457));
        }

        public static double n(int t = 0, int h = 0, int Z = 1000) {
            return 0;
        }

        public static double q(double h) {
        	double q = 0;
            if (h == 0)
                return q;
            double n = Chemical.Instance.getConcentrationForHeight(h);
            double maxWave = Chemical.Instance.maxWave;
            double wave = 0;
            double sumFlux = 0;
            while (wave <= maxWave) {

            }
            return 0;
        }

        // #############
    	// ###PRIVATE###
    	// #############

        // Вычисляем поток фотонов с длиной волны wave на высоте height
        private static double photonFlux(double wave, double height) {
        	double eternityFlux = 1; // Поток бесконечности - должно быть какая-то константа
            return eternityFlux;
        }

        private static double degreesToRadians(double degrees) {
            if (degrees != 0) return (degrees * Math.PI / 180);
            else return 0;    
        }

        private static double radiansToDegrees(double radians) {
            if (radians != 0) return (radians * 180 / Math.PI);
            else return 0; 
        }

        // Время отсчитываемое от местного полудня
        private static double timeAfternoon() {
            TimeSpan current = timeInPos().TimeOfDay;
            return current.TotalSeconds - 12*3600;
        }

        // Вычисляем hi зенитный угол Солнца
        // TODO!!!! Функция Чепмена!
        // При больших зенитных углах > 80 нужно заменить sec (hi) на функцию Чемпена
        private static double hi(double latitude) {
            int day = timeInPos().DayOfYear;
            double delta = Math.Atan(Math.Tan(degreesToRadians(23.5)) * Math.Sin(2*Math.PI*(day-80)/365));
            double res = Math.Sin(delta) * Math.Sin(latitude) + Math.Cos(delta) * Math.Cos(latitude) * Math.Cos(Constants.earthRotVel * timeAfternoon());
            return Math.Acos(res);
        }
    }
}
