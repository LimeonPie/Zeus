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

        // Фоновая объемность радона
        public static double Q = 27.6;

        // Нормальное атмосферное давление
        public static double p = 1E+5;

        // Универсальная газовая постоянная
        public static double R = 8.31;

        // Коэффициент нейтрализации
        public static double gamma = 3E-12;

        // Поток фотонов на входе в атмосфер
        public static double eternityFlux = 3.8E+8;

        // Постоянная Авокадо
        public static double Na = 6.02E+23;

        // Молярный объем
        public static double Vm = 22.413;

        // Угловая скорость вращения Земли
        public static double earthRotVel = 7.92E-5;
 
        // Дельта t
        public static double dt = 1;

        // Ускорение свободное падения
        public static double g = 9.81;

        // Постоянная Больцмана
        public static double k = 1.38E-23;

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
