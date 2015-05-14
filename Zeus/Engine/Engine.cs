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
        public double botBoundary;
        public double topBoundary;
        public List<Element> aerosols;
    };

    public struct outputData 
    {
        public double height;
        public double ne;
        public double nip;
        public double nin;
        public double total;
    };

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
            lowAtmosphere = new Sphere(data, active);
        }

        public void launchComputations() {
            lowAtmosphere.n();
        }

        public Dictionary<int, double> convertArrayToDict(double[] array) {
            Dictionary<int, double> dict = array.Select((value, index) => new { value, index }).ToDictionary(pair => pair.index, pair => pair.value);
            return dict;
        }

        public void setCoordinates(double longitude, double latitude) {
            lowAtmosphere.changeCoordinates(longitude, latitude);
        }

        public void saveToFile() {
            outputData[] data = new outputData[lowAtmosphere.capacity];
            for (int i = 0; i < lowAtmosphere.capacity; i++) {
                data[i].height = i * lowAtmosphere.delta;
                data[i].ne = lowAtmosphere.neGrid[i];
                data[i].nip = lowAtmosphere.nipGrid[i];
                data[i].nin = lowAtmosphere.ninGrid[i];
                data[i].total = data[i].ne + data[i].nip + data[i].nin;
            }
            JsonWrapper.writeJsonOutputData(data, "\\output.json");
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
