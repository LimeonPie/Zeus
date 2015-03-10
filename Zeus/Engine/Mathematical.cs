using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeus.Engine
{
    public static class Mathematical
    {

        public static DateTime timeInPos(double latitude = 0, double longitude = 0) {
            // TODO!!!
            // Расчет времени в данной точки планеты
            return DateTime.Now;
        }

        public static double beta(double r) {
            return (2.35 * Math.Pow(10, -4) * Math.Pow(r, 1.457));
        }

        public static double n(int t) {
            return 0;
        }

        public static double degreesToRadians(double degrees) {
            return (degrees * Math.PI / 180);    
        }

        public static double radiansToDegrees(double radians) {
            return (radians * 180 / Math.PI); 
        }

        public static double timeAfternoon() {
            // Время отсчитываемое от местного полудня
            TimeSpan current = timeInPos().TimeOfDay;
            return current.TotalSeconds - 12*3600;
        }

        public static double hi(double latitude) {
            int day = timeInPos().DayOfYear;
            double delta = Math.Atan(Math.Tan(degreesToRadians(23.5)) * Math.Sin(2*Math.PI*(day-80)/365));
            double res = Math.Sin(delta) * Math.Sin(latitude) + Math.Cos(delta) * Math.Cos(latitude) * Math.Cos(Constants.earthRotVel * timeAfternoon());
            return Math.Acos(res);
        }
    }
}
