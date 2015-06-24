using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Zeus.Engine;

namespace Zeus.Helpers
{

    // Логирование, и этим все сказано
    // Создает файл логов в папке программы/Logs/log.txt
    // Дипломная работа
    // ИВТ(б)-411 Миняев Илья

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
            clearLogs();
        }

        public static LogManager Session {
            get {
                if (instance == null) {
                    instance = new LogManager();
                }
                return instance;
            }
        }

        public bool logMessage(string message) {
            try {
                StreamWriter sw = File.AppendText(currentPath);
                sw.Write(DateTime.Now.ToLocalTime().ToString() + ": " + message);
                sw.WriteLine();
                sw.Close();
                return true;
            }
            catch (IOException e) {
                System.Diagnostics.Debug.WriteLine("Error " + e.Source + ": " + e.Message);
                return false;
            }
        }

        private bool clearLogs() {
            try {
                StreamWriter sw = new StreamWriter(File.OpenWrite(currentPath));
                sw.Write(String.Empty);
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
