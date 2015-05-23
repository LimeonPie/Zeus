using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoTimeZone;

namespace Zeus.Engine
{
    // Класс, отвечающий за операциями с временем
    public static class Time
    {

        public static DateTime usedTime = DateTime.Now;

        // Местное время по широте-долготе
        public static DateTime timeInPos(double latitude, double longitude) {
            GeoTimeZone.TimeZoneResult zone = GeoTimeZone.TimeZoneLookup.GetTimeZone(latitude, longitude);
            TimeZoneInfo info = TimeZoneInfo.CreateCustomTimeZone(zone.Result, TimeSpan.Zero, "Patience", "Time");
            DateTime timeHere = TimeZoneInfo.ConvertTime(usedTime, info);
            timeHere = timeHere.AddHours(2);
            return timeHere;
        }

        // Время отсчитываемое от местного полудня
        public static double timeAfterNoon(double latitude, double longitude) {
            TimeSpan current = timeInPos(latitude, longitude).TimeOfDay;
            if (current.Hours >= 12) {
                return current.TotalSeconds - 12 * 3600;
            }
            else {
                return current.TotalSeconds + 12 * 3600;
            }
        }
    }
}
