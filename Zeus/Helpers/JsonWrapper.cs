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
    public struct ElementView
    {
        public string name;
        public Dictionary<string, double> prop;
    }

    public static class JsonWrapper
    {
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

        public static List<Element> parseJsonForElements(string filename) {
            List<Element> result = new List<Element>();
            JObject o = JObject.Parse(File.ReadAllText(filename));
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

        public static void writeJson(Dictionary<string, string> dict, string filename) {
            StreamWriter sw = new StreamWriter(filename);
            JsonWriter writer = new JsonTextWriter(sw);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartObject();
            foreach (string key in dict.Keys) {
                writer.WritePropertyName(key);
                writer.WriteValue(dict[key]);
            }
            writer.WriteEndObject();
            sw.Close();
        }
    }
}
