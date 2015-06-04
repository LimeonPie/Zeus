using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeus.Engine;
using Zeus.Helpers;

namespace ZeusConsole
{
    class Program
    {

        static void display(string message) {
            // Ламповый вывод
            Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + message);
        }

        static void Main(string[] args) {
            display("Process initilized");

            Engine.Instance.initSphereWithInputFile(Constants.appJsonPath + "input.json");
            // Запуск вычислений
            double electro = Engine.Instance.launchComputations();
            Engine.Instance.saveToFile(Constants.appJsonPath + "output.json");
            display("Electricity = " + electro);
            display("Press any key...");
            Console.ReadKey();
        }
    }
}
