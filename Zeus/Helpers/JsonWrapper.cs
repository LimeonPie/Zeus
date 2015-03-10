using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Zeus.Helpers
{
    static class JsonWrapper
    {
        public static void readJson(string filename) {
            JsonTextReader reader = new JsonTextReader(new StreamReader(filename));
            StreamWriter sw = new StreamWriter("..\\..\\Resources\\JSON\\nitro.txt");
            while (reader.Read()) {
                if (reader.Value != null)
                    sw.WriteLine("Token: {0}, Value: {1}", reader.TokenType, reader.Value);
                else
                    sw.WriteLine("Token: {0}", reader.TokenType);
            }
            sw.Close();
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
