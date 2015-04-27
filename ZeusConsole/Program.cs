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
            Engine.Instance.launchComputations();
            Dictionary<int, double> neDict = Engine.Instance.convertArrayToDict(Engine.Instance.lowAtmosphere.neGrid);
            JsonWrapper.writeJson(neDict, "\\neGrid.json");
            Dictionary<int, double> nipDict = Engine.Instance.convertArrayToDict(Engine.Instance.lowAtmosphere.nipGrid);
            JsonWrapper.writeJson(nipDict, "\\nipGrid.json");
            Dictionary<int, double> ninDict = Engine.Instance.convertArrayToDict(Engine.Instance.lowAtmosphere.ninGrid);
            JsonWrapper.writeJson(ninDict, "\\ninGrid.json");
            Console.ReadKey();
        }
    }
}
