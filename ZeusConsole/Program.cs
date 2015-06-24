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

            if (Engine.Instance.initSphereWithInputFile(Constants.appJsonPath + "input.json")) {
                // Запуск вычислений
                Engine.Instance.launchComputations();
                double electro = Engine.Instance.getElectro();
                Engine.Instance.saveToFile(Constants.appJsonPath + "output.json");
                display("Electricity = " + electro);
            }
            Console.Write("Please enter a key...");
            Console.ReadKey();
        }
    }
}
