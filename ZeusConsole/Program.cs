using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeus.Engine;

namespace ZeusConsole
{
    class Program
    {

        private static void display(string message) {
            // Ламповый вывод
            Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + message);
        }

        static void Main(string[] args) {
            display("Process initilized");
            display("Seconds = " + Mathematical.timeAfternoon());
            Console.ReadKey();
        }
    }
}
