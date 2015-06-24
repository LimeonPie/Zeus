using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeus.Helpers;

namespace Zeus.Engine
{

    // Основной класс, ответственен за
    // Взаимодействием с UI
    // Управление работой комплекса, куда входит
    // Загрузка входного файла
    // Создание вычислительного класса
    // Сохранение результата
    // и другие мелкие задачи
    // Дипломная работа
    // ИВТ(б)-411 Миняев Илья

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
        public bool error;
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

        public bool initSphereWithInputFile(string path) {
            Element active = loadActiveElement(ActiveElement.Nitrogen);
            inputData data = JsonWrapper.parseInputData(path);
            if (data.error == true) {
                return false;
            }
            if (!Validator.validateItemForType(data.delta, VALIDATION_TYPE.DELTA)) {
                LogManager.Session.logMessage("Critical error: delta is incorrect or not exist");
                return false;
            }
            if (!Validator.validateItemForType(data.longitude, VALIDATION_TYPE.LONGITUDE)) {
                LogManager.Session.logMessage("Critical error: longitude is incorrect or not exist");
                return false;
            }
            if (!Validator.validateItemForType(data.latitude, VALIDATION_TYPE.LATITUDE)) {
                LogManager.Session.logMessage("Critical error: latitude is incorrect or not exist");
                return false;
            }
            if (!Validator.validateItemForType(data.ne0, VALIDATION_TYPE.CONCENTRATION)) {
                LogManager.Session.logMessage("Critical error: electron conc is incorrect or not exist");
                return false;
            }
            if (!Validator.validateItemForType(data.nip0, VALIDATION_TYPE.CONCENTRATION)) {
                LogManager.Session.logMessage("Critical error: positive conc is incorrect or not exist");
                return false;
            }
            if (!Validator.validateItemForType(data.nin0, VALIDATION_TYPE.CONCENTRATION)) {
                LogManager.Session.logMessage("Critical error: negative conc is incorrect or not exist");
                return false;
            }

            if (data.time != null) Time.usedTime = DateTime.Parse(data.time);
            if (data.eternityFlux != 0) Constants.eternityFlux = data.eternityFlux;
            if (data.timeInterval != 0) Constants.timeInterval = data.timeInterval;
            else data.timeInterval = Constants.timeInterval;

            lowAtmosphere = new Sphere(data, active);
            return true;
        }

        public void preCaluculate() {
            lowAtmosphere.preCalculateData();
        }

        public void launchComputations() {
            lowAtmosphere.n();
        }

        public double getElectro() {
            return lowAtmosphere.electricity();
        }

        public Dictionary<int, double> convertArrayToDict(double[] array) {
            Dictionary<int, double> dict = array.Select((value, index) => new { value, index }).ToDictionary(pair => pair.index, pair => pair.value);
            return dict;
        }

        public void saveToFile(string filename) {
            outputData[] data = new outputData[lowAtmosphere.capacity];
            additionalData info = new additionalData();
            info.electricity = lowAtmosphere.electricity();
            info.date = DateTime.Now.ToString();
            for (int i = 0; i < lowAtmosphere.capacity; i++) {
                data[i].height = i * lowAtmosphere.delta;
                data[i].ne = lowAtmosphere.electronGrid[i].value;
                data[i].nip = lowAtmosphere.ionPositiveGrid[i].value;
                data[i].nin = lowAtmosphere.ionNegativeGrid[i].value;
                data[i].total = data[i].ne + data[i].nip + data[i].nin;
                data[i].neVel = lowAtmosphere.electronVelocityGrid[i].value;
                data[i].nipVel = lowAtmosphere.ionPosVelocityGrid[i].value;
                data[i].ninVel = lowAtmosphere.ionNegVelocityGrid[i].value;
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
