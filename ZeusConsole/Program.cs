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
            display("Timezone is = " + Time.timeInPos(Engine.Instance.lowAtmosphere.latitude, Engine.Instance.lowAtmosphere.longitude).ToShortTimeString());
            display("Time in seconds is = " + Time.timeAfterNoon(Engine.Instance.lowAtmosphere.latitude, Engine.Instance.lowAtmosphere.longitude));
            double electro = Engine.Instance.launchComputations();
            Engine.Instance.saveToFile(string.Empty);
            display("Electro = " + electro.ToString());
            display("Enter key...");
            Console.ReadKey();
        }
    }
}
