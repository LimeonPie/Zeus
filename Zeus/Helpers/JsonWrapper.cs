using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Zeus.Engine;

namespace Zeus.Helpers
{

    // Обертка для джейсона
    // Парс, считывание, запись
    // Все здесь и работает
    // P.S. вроде как

    public static class JsonWrapper
    {

        public static inputData parseInputData(string filename) {
            inputData data = new inputData();
            JObject o = JObject.Parse(File.ReadAllText(filename));
            LogManager.Session.logMessage("Reading json file " + filename);
            foreach (JProperty prop in o.Properties()) {
                switch (prop.Name) {
                    case "longitude":
                        double longitude = prop.Value.ToObject<double>();
                        if (Validator.validateItemForType(longitude, VALIDATION_TYPE.LONGITUDE)) {
                           data.longitude = longitude; 
                        }
                        else data.longitude = 45;
                        break;
                    case "latitude":
                        double latitude = prop.Value.ToObject<double>();
                        if (Validator.validateItemForType(latitude, VALIDATION_TYPE.LATITUDE)) {
                           data.latitude = latitude; 
                        }
                        else data.latitude = 45;
                        break;
                    case "ne0":
                        data.ne0 = prop.Value.ToObject<double>();
                        break;
                    case "nin0":
                        data.nin0 = prop.Value.ToObject<double>();
                        break;
                    case "nip0":
                        data.nip0 = prop.Value.ToObject<double>();
                        break;
                    case "delta":
                        data.delta = prop.Value.ToObject<double>();
                        break;
                    case "epsilum":
                        data.epsilum = prop.Value.ToObject<double>();
                        break;
                    case "timeInterval":
                        data.timeInterval = prop.Value.ToObject<int>();
                        break;
                    case "eternityFlux":
                        data.eternityFlux = prop.Value.ToObject<int>();
                        break;
                    case "velocity":
                        data.velocity = prop.Value.ToObject<double>();
                        break;
                    case "time":
                        data.time = prop.Value.ToObject<string>();
                        break;
                    case "botBoundary":
                        data.botBoundary = prop.Value.ToObject<double>();
                        break;
                    case "topBoundary":
                        data.topBoundary = prop.Value.ToObject<double>();
                        break;
                    case "aerosols":
                        data.aerosols = parseJsonForElements(prop.Value.ToString());
                        break;
                    case "temperature":
                        data.temperature = deserializeJsonToDict(prop.Value.ToString());
                        break;
                    default:
                        LogManager.Session.logMessage("Unknown key " + prop.Name);
                        break;
                }
            }
            return data;
        }

        public static Dictionary<string, double> readJson(string filename, string key) {
            Dictionary<string, double> result = new Dictionary<string, double>();
            JObject o = JObject.Parse(File.ReadAllText(filename));
            LogManager.Session.logMessage("Reading json file " + filename + " for key " + key);
            foreach (JProperty prop in o.Properties()) {
                if (prop.Name.Equals(key)) {
                    if (prop.Value.Type == JTokenType.Object) {
                        foreach (JProperty token in prop.Values()) {
                            result.Add(token.Name, token.Value.ToObject<double>());
                        }
                        LogManager.Session.logMessage("Reading file " + filename + " completed");
                        return result;
                    }
                    else if (prop.Value.Type != JTokenType.Array) {
                        result.Add(prop.Name, prop.Value.ToObject<double>());
                        LogManager.Session.logMessage("Reading file " + filename + " completed");
                        return result;
                    }
                }
            }
            LogManager.Session.logMessage("Key " + key + " in " + filename + " was not found");
            return result;
        }

        public static List<Element> parseJsonForElements(string json) {
            List<Element> result = new List<Element>();
            JObject o = JObject.Parse(json);
            foreach (JProperty prop in o.Properties()) {
                Element el = JsonConvert.DeserializeObject<Element>(prop.Value.ToString());
                result.Add(el);
            }
            return result;
        }

        public static Element deserializeJsonToElement(string filename) {
            JObject o = JObject.Parse(File.ReadAllText(filename));
            JProperty prop = o.Properties().ElementAt(0);
            Element result = JsonConvert.DeserializeObject<Element>(prop.Value.ToString());
            return result;
        }

        public static Dictionary<string, double> deserializeJsonToDict(string json) {
            JObject o = JObject.Parse(json);
            Dictionary<string, double> result = JsonConvert.DeserializeObject<Dictionary<string, double>>(o.ToString());
            return result;
        }

        public static void writeJson(Dictionary<int, double> dict, string filename) {
            string path = Constants.appJsonPath + filename;
            StreamWriter sw = new StreamWriter(path);
            JsonWriter writer = new JsonTextWriter(sw);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartObject();
            foreach (int key in dict.Keys) {
                writer.WritePropertyName(key.ToString());
                writer.WriteValue(dict[key].ToString());
            }
            writer.WriteEndObject();
            sw.Close();
            LogManager.Session.logMessage("Writing " + path + " has been completed");
        }

        public static void writeJsonOutputData(outputData[] data, additionalData info, string path) {
            StreamWriter sw = new StreamWriter(path);
            JsonWriter writer = new JsonTextWriter(sw);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartObject();
            writer.WritePropertyName("electricity");
            writer.WriteValue(info.electricity.ToString("#.###E0"));
            writer.WritePropertyName("date");
            writer.WriteValue(info.date);
            writer.WritePropertyName("concentrations");
            writer.WriteStartObject();
            foreach (outputData part in data) {
                writer.WritePropertyName(part.height.ToString());
                writer.WriteStartObject();
                writer.WritePropertyName("electronConc");
                writer.WriteValue(part.ne.ToString("#.###E0"));
                writer.WritePropertyName("ionPlusConc");
                writer.WriteValue(part.nip.ToString("#.###E0"));
                writer.WritePropertyName("ionMinusConc");
                writer.WriteValue(part.nin.ToString("#.###E0"));
                writer.WritePropertyName("totalConc");
                writer.WriteValue(part.total.ToString("#.###E0"));
                writer.WritePropertyName("electronVelocity");
                writer.WriteValue(part.neVel.ToString("#.###E0"));
                writer.WritePropertyName("ionPlusVelocity");
                writer.WriteValue(part.nipVel.ToString("#.###E0"));
                writer.WritePropertyName("ionMinusVelocity");
                writer.WriteValue(part.ninVel.ToString("#.###E0"));
                writer.WriteEndObject();
            }
            writer.WriteEndObject();
            writer.WriteEndObject();
            sw.Close();
            LogManager.Session.logMessage("Writing " + path + " has been completed");
        }
    }
}
