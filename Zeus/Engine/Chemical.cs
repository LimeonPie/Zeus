using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeus.Engine
{

    public enum Elements
    {
        Nitrogen = 0,
        Oxygen = 1
    };

    public class Chemical
    {

        private static Chemical instance;
        public static Elements currentElement;
        public Dictionary<string, double> concentrations;
        public Dictionary<string, double> crossSections;

        private Chemical() {
            concentrations = new Dictionary<string, double>();
            crossSections = new Dictionary<string, double>();  
        }

        public static Chemical Instance {
            get {
                if (instance == null) {
                    instance = new Chemical();
                }
                return instance;
            }
        }

        // Синхронизируем данные об элементах данного типа
        private void initForElement(Elements el) {
            
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
