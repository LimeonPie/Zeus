using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeus.Engine;
using Zeus.Helpers;

namespace Zeus.Engine
{

    public enum Elements
    {
        Nitrogen,
        Oxygen
    };

    public class DataLoader
    {

        private static DataLoader instance;

        public Element activeElement;
        public List<Element> neutralElements;

        private DataLoader() { 
            initForElement(Elements.Nitrogen);
        }

        public static DataLoader Instance {
            get {
                if (instance == null) {
                    instance = new DataLoader();
                }
                return instance;
            }
        }

        // Загружаем основной элемент
        public void initForElement(Elements el) {
            string path = Constants.appJsonPath + getFilenameForElement(el);
            List<Element> list = JsonWrapper.parseJsonForElements(path);
            activeElement = list.ElementAt(0);
            if (list.Count != 1) {
                LogManager.Session.logMessage("Well, there is multiple elements in " + path);
            }
        }

        // Загружаем газовый состав
        public void initWithNeutralParticles(string path) {
            neutralElements = JsonWrapper.parseJsonForElements(path);
        }

        private string getFilenameForElement(Elements el) {
            switch (el) {
                case Elements.Nitrogen:
                    return "nitrogen.json";
                case Elements.Oxygen:
                    return "oxygen.json";
                default:
                    return "nitrogen.json";
            }
        }
    }
}
