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
            /*Engine.Instance.initSphereWithInputFile(Constants.appJsonPath + "input.json");
            display("Bot = " + Engine.Instance.lowAtmosphere.botBoundary.ToString());
            display("Top = " + Engine.Instance.lowAtmosphere.topBoundary.ToString());
            display("Delta = " + Engine.Instance.lowAtmosphere.delta.ToString());
            display("Active element conc = " + Engine.Instance.lowAtmosphere.activeElement.n0.ToString());*/
            Console.ReadKey();
        }
    }
}
