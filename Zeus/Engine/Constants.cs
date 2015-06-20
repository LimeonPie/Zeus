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

        // Постоянное сечение ионизации
        public static double sigma = 2E-22;

        // Постоянное сечение поглощения протона воздухом
        public static double absorption = 350E-31;

        // Масса электрона
        public static double eMass = 9.10938281E-31;

        // Элементарный электрический заряд
        public static double qe = 1.60217657E-19;

        // Фоновая объемность радона
        public static double Q = 27.6;

        // Нормальное атмосферное давление
        public static double p = 1E+5;

        // Универсальная газовая постоянная
        public static double R = 8.31;

        // Коэффициент нейтрализации
        public static double gamma = 3.12E-12;

        // Лимит количества итераций по умолчанию
        public static int iterationLimitDefault = 10000;

        // Поток фотонов на входе в атмосфер
        // Раньше были значнения:
        // 3.8E+8
        // 0.6E+5
        // 1E-4
        public static double eternityFlux = 0.5E+5;

        // Лимит атмосферы
        public static double atmosphereLimit = 80000;

        // Постоянная Авокадо
        public static double Na = 6.02E+23;

        // Молярный объем
        public static double Vm = 22.413;

        // Угловая скорость вращения Земли
        public static double earthRotVel = 7.9221158553E-5;
 
        // Дельта t
        public static double dt = 1;

        // Период времени
        public static int timeInterval = 1;

        // Ускорение свободное падения
        public static double g = 9.81;

        // Постоянная Больцмана
        public static double k = 1.3806488E-23;

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
