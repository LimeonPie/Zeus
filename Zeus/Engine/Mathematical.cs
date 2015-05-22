using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Zeus.Engine
{
    public static class Mathematical
    {

        public static double beta(double r) {
            //return(2.35E-4 * Math.Pow(r, 1.457));
            //return (0.436 * r - 9.2E-8);
            return (r * 0.57);
        }

        public static double celsiusToKelvins(double celsius) {
            return (celsius + 273);
        }

        public static double degreesToRadians(double degrees) {
            if (degrees != 0) return (degrees * Math.PI / 180);
            else return 0;    
        }

        public static double radiansToDegrees(double radians) {
            if (radians != 0) return (radians * 180 / Math.PI);
            else return 0; 
        }

        // Вычисляем hi зенитный угол Солнца
        // TODO!!!! Функция Чепмена!
        // При больших зенитных углах > 80 нужно заменить sec (hi) на функцию Чемпена
        public static double hi(double latitude, double longitude) {
            int day = Time.timeInPos(latitude, longitude).DayOfYear;
            double delta = Math.Atan(Math.Tan(degreesToRadians(23.5)) * Math.Sin(2*Math.PI*(day-80)/365));
            double res = Math.Sin(delta) * Math.Sin(degreesToRadians(latitude)) + Math.Cos(delta) * Math.Cos(degreesToRadians(latitude)) * Math.Cos(Constants.earthRotVel * Time.timeAfterNoon(latitude, longitude));
            return Math.Acos(res);
        }

        public static double sec(double anger) {
            return (1 / Math.Cos(anger));
        }
    }
}
