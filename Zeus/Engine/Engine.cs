using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeus.Helpers;

namespace Zeus.Engine
{

    // Основной класс, ответственен за
    // Управление работой комплекса
    // Взаимодействием с UI
    // Да в принципе все

    public enum ActiveElement
    {
        Nitrogen,
        Oxygen
    };

    public struct inputData
    {
        public double longitude;
        public double latitude;
        public double ne0;
        public double nip0;
        public double nin0;
        public double delta;
        public double epsilum;
        public double eternityFlux;
        public int timeInterval;
        public string time;
        public double velocity;
        public double botBoundary;
        public double topBoundary;
        public List<Element> aerosols;
        public Dictionary<string, double> temperature;
    };

    public struct outputData 
    {
        public double height;
        public double ne;
        public double nip;
        public double nin;
        public double total;
        public double neVel;
        public double nipVel;
        public double ninVel;
    };

    public struct additionalData
    {
        public double electricity;
        public string date;
    }

    public class Engine
    {
        private static Engine instance;
        public Sphere lowAtmosphere;
        private string inputFilename;

        private Engine() {
            
        }

        public static Engine Instance {
            get {
                if (instance == null) {
                    instance = new Engine();
                }
                return instance;
            }
        }

        public void initSphereWithInputFile(string path) {
            inputFilename = path;
            Element active = loadActiveElement(ActiveElement.Nitrogen);
            inputData data = JsonWrapper.parseInputData(path);
            if (data.time != null) Time.usedTime = DateTime.Parse(data.time);
            if (data.eternityFlux != 0) Constants.eternityFlux = data.eternityFlux;
            if (data.timeInterval != 0) Constants.timeInterval = data.timeInterval;
            else data.timeInterval = Constants.timeInterval;

            lowAtmosphere = new Sphere(data, active);
        }

        public double launchComputations() {
            lowAtmosphere.n();
            double electricity = lowAtmosphere.electricity();
            return electricity;
        }

        public double getElectro() {
            return lowAtmosphere.electricity();
        }

        public Dictionary<int, double> convertArrayToDict(double[] array) {
            Dictionary<int, double> dict = array.Select((value, index) => new { value, index }).ToDictionary(pair => pair.index, pair => pair.value);
            return dict;
        }

        public void setCoordinates(double longitude, double latitude) {
            lowAtmosphere.changeCoordinates(longitude, latitude);
        }

        public void saveToFile(string filename) {
            outputData[] data = new outputData[lowAtmosphere.capacity];
            additionalData info = new additionalData();
            info.electricity = lowAtmosphere.electricity();
            info.date = DateTime.Now.ToString();
            for (int i = 0; i < lowAtmosphere.capacity; i++) {
                data[i].height = i * lowAtmosphere.delta;
                data[i].ne = lowAtmosphere.electronGrid[i].value;
                data[i].nip = lowAtmosphere.ionPlusGrid[i].value;
                data[i].nin = lowAtmosphere.ionMinusGrid[i].value;
                data[i].total = data[i].ne + data[i].nip + data[i].nin;
                data[i].neVel = lowAtmosphere.electronVelocityGrid[i].value;
                data[i].nipVel = lowAtmosphere.ionPlusVelocityGrid[i].value;
                data[i].ninVel = lowAtmosphere.ionMinusVelocityGrid[i].value;
            }
            if (filename != null && !filename.Equals(string.Empty)) {
                JsonWrapper.writeJsonOutputData(data, info, filename);
            }
            else {
                string path = Constants.appJsonPath + "\\output.json";
                JsonWrapper.writeJsonOutputData(data, info, path);
            }
        }

        // Загружаем основной элемент
        private Element loadActiveElement(ActiveElement el) {
            string path = Constants.appJsonPath + getFilenameForElement(el);
            Element activeElement = JsonWrapper.deserializeJsonToElement(path);
            return activeElement;
        }

        private string getFilenameForElement(ActiveElement el) {
            switch (el) {
                case ActiveElement.Nitrogen:
                    return "nitrogen.json";
                case ActiveElement.Oxygen:
                    return "oxygen.json";
                default:
                    return "nitrogen.json";
            }
        }
    }
}
