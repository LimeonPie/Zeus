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
        Nitrogen = 0,
        Oxygen = 1
    };

    public class Chemical
    {

        private static Chemical instance;
        public static Elements currentElement;
        private Dictionary<string, double> concentrations;
        private Dictionary<string, double> crossSections;
        public double maxWave = 0;


        private Chemical() {
            concentrations = new Dictionary<string, double>();
            crossSections = new Dictionary<string, double>(); 
            initForElement(Elements.Nitrogen);
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
        public void initForElement(Elements el) {
            string path = Constants.appJsonPath + getFilenameForElement(el);
            concentrations = JsonWrapper.readJson(path, "concentration");
            crossSections = JsonWrapper.readJson(path, "cross-section");
            maxWave = JsonWrapper.readJson(path, "maxWave")["maxWave"];
            currentElement = el;
        }

        // Получение значения концентрации по высоте
        public double getConcentrationForHeight(double height) {
            string key = canGetConcentrationForHeight(height);
            if (key != null) {
                return concentrations[key];
            }
            else return 0;
        }

        // Получение значения сечения фотоионизации по высоте
        public double getCrossSectionsForWave(double length) {
            string key = canGetCrossSectionsForWave(length);
            if (key != null) {
                return crossSections[key];
            }
            else return 0;
        }

        // #############
    	// ###PRIVATE###
    	// #############

        // Есть ли в словаре концентраций нужные нам границы
        private string canGetConcentrationForHeight(double height) {
            foreach (string key in concentrations.Keys) {
                if (isInBounds(key, height)) return key;
            }
            return null;
        }

        // Есть ли в словаре сечений фотоионизации нужные нам значения
        private string canGetCrossSectionsForWave(double lenght) {
            foreach (string key in crossSections.Keys) {
                if (isInBounds(key, lenght)) return key;
            }
            return null;
        }

        private bool isInBounds(string bounds, double value) {
            string[] mass = bounds.Split('-');
            if (mass.Length != 0) {
                if (mass.Length == 1) {
                    double edge = Convert.ToDouble(mass[0]);
                    if (Math.Round(edge) == value) return true;
                    else return false;
                }
                else if (mass.Length == 2) {
                    double low = Convert.ToDouble(mass[0]);
                    double high = Convert.ToDouble(mass[1]);
                    if ((value >= Math.Round(low)) && (value <= Math.Round(high))) return true;
                    else return false;
                }
                return false;
            }
            else return false;
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
