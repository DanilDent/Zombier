using Newtonsoft.Json;
using Prototype.Data;
using System;

namespace Prototype.Service
{
    public class IdDataConverter : JsonConverter<IdData>
    {
        public override void WriteJson(JsonWriter writer, IdData value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override IdData ReadJson(JsonReader reader, Type objectType, IdData existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string s = (string)reader.Value;

            return new IdData(s);
        }
    }
}
