using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Zeus.Engine
{
    public static class Constants
    {

        // Угловая скорость вращения Земли
        public static double earthRotVel = 7.9221158553 * Math.Pow(10, -5);
 
        // Дельта t
        public static double dt = 1;

        // Ускорение свободное падения
        public static double g = 9.81;

        // Постоянная Больцмана
        public static double k = 1.3806488 * Math.Pow(10, -23);

        // Главная директория
        public static string appRootPath {
            get {
                // Очень упорото, но работает
                string debug = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                string bin = Path.GetDirectoryName(debug);
                string root = Path.GetDirectoryName(bin);
                return new Uri(root).LocalPath;
            }
        }

        // Путь к логам
        public static string appLogsPath {
            get {
                return (appRootPath + "\\Logs\\"); 
            }
        }

        // Путь к ресурсам
        public static string appResourcesPath {
            get {
                return (appRootPath + "\\Resources\\");
            }
        }

        // Путь к джейсонам
        public static string appJsonPath {
            get {
                return (appRootPath + "\\Resources\\JSON\\");
            }
        }
    }
}
