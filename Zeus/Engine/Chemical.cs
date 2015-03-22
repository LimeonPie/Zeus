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
        public Dictionary<string, double> concentrations;
        public Dictionary<string, double> atomCrossSections;
        public Dictionary<string, double> photonCrossSections;
        public double maxWave = 0;

        private Chemical() {
            concentrations = new Dictionary<string, double>();
            atomCrossSections = new Dictionary<string, double>();
            photonCrossSections = new Dictionary<string, double>(); 
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
            atomCrossSections = JsonWrapper.readJson(path, "ionization-cs");
            photonCrossSections = JsonWrapper.readJson(path, "photon-cs");
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

        // Получение значения сечения фотоионизации по длине волны
        public double getAtomCSForWave(double wave) {
            string key = canGetKeyForValue(wave, atomCrossSections);
            if (key != null) {
                return atomCrossSections[key];
            }
            else return 0;
        }

        // Получение значения сечения поглощения фотона по длине волны
        public double getPhotonCSForWave(double wave) {
            string key = canGetKeyForValue(wave, photonCrossSections);
            if (key != null) {
                return photonCrossSections[key];
            }
            else return 0;
        }

        // #############
    	// ###PRIVATE###
    	// #############

        // Есть ли в словаре концентраций нужные нам границы
        // TODO!!!!
        // Пока что в словаре концентраций только одно значение
        // Наверно так и останется, придумать что-то получше
        private string canGetConcentrationForHeight(double height) {
            foreach (string key in concentrations.Keys) {
                if (isInBounds(key, height)) return key;
            }
            return null;
        }

        private string canGetKeyForValue(double value, Dictionary<string, double> dict) {
            foreach (string key in dict.Keys) {
                if (isInBounds(key, value)) return key;
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
