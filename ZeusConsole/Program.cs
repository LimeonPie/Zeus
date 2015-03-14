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
            //display("Seconds = " + Mathematical.timeAfternoon());
            //display(Mathematical.timeInPos(50, 13).ToLongTimeString());
            /*Dictionary<string, double> dict = JsonWrapper.readJson(Constants.appJsonPath + "nitrogen.json", "cross-section");
            display("Writing dict data");
            foreach (string key in dict.Keys) {
                Console.WriteLine("Key = " + key + ", Value = " + dict[key]);
            }
            display("Reading key");*/
            Console.ReadKey();
        }
    }
}
