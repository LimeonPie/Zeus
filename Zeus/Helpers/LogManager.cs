using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Zeus.Engine;

namespace Zeus.Helpers
{
    public class LogManager
    {
        private static LogManager instance;
        private string currentPath;

        private LogManager() {
            currentPath = Constants.appLogsPath + "\\log.txt";
            if (!File.Exists(currentPath)) {
                FileStream stream = File.OpenWrite(currentPath);
                stream.Close();
            }
        }

        public static LogManager Session {
            get {
                if (instance == null) {
                    instance = new LogManager();
                }
                return instance;
            }
        }

        public bool log(string message) {
            try {
                StreamWriter sw = File.AppendText(currentPath);
                sw.WriteLine(DateTime.Now.ToUniversalTime().ToString() + ": " + message);
                sw.Close();
                return true;
            }
            catch (IOException e) {
                System.Diagnostics.Debug.WriteLine("Error " + e.Source + ": " + e.Message);
                return false;
            }
        }
    }
}
