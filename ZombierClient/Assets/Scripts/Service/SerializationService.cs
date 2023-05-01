using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Prototype.Service
{
    public class SerializationService
    {
        public SerializationService()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new IdDataConverter() },
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
        }

        public void SerializeObject<T>(T target, string path)
            where T : class
        {
            var output = JsonConvert.SerializeObject(target, Formatting.Indented);
            File.WriteAllText(path, output);
        }

        public void DeserializeObject<T>(string path, out T target)
            where T : class
        {
            var input = File.ReadAllText(path, Encoding.UTF8);
            target = JsonConvert.DeserializeObject<T>(input);
        }
    }
}
