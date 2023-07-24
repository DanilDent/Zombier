using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace Prototype.Data
{
    public enum DamageTypeType { Electric, Fire, Physical, Toxic };

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                DamageTypeTypeConverter.Singleton,
                ProjectilePrefabAddressConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ProjectilePrefabAddressConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ProjectilePrefabAddress) || t == typeof(ProjectilePrefabAddress?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new ProjectilePrefabAddress { String = stringValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<long[]>(reader);
                    return new ProjectilePrefabAddress { IntegerArray = arrayValue };
            }
            throw new Exception("Cannot unmarshal type ProjectilePrefabAddress");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (ProjectilePrefabAddress)untypedValue;
            if (value.String != null)
            {
                serializer.Serialize(writer, value.String);
                return;
            }
            if (value.IntegerArray != null)
            {
                serializer.Serialize(writer, value.IntegerArray);
                return;
            }
            throw new Exception("Cannot marshal type ProjectilePrefabAddress");
        }

        public static readonly ProjectilePrefabAddressConverter Singleton = new ProjectilePrefabAddressConverter();
    }

    internal class DamageTypeTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(DamageTypeType) || t == typeof(DamageTypeType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Electric":
                    return DamageTypeType.Electric;
                case "Fire":
                    return DamageTypeType.Fire;
                case "Physical":
                    return DamageTypeType.Physical;
                case "Toxic":
                    return DamageTypeType.Toxic;
            }
            throw new Exception("Cannot unmarshal type DamageTypeType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (DamageTypeType)untypedValue;
            switch (value)
            {
                case DamageTypeType.Electric:
                    serializer.Serialize(writer, "Electric");
                    return;
                case DamageTypeType.Fire:
                    serializer.Serialize(writer, "Fire");
                    return;
                case DamageTypeType.Physical:
                    serializer.Serialize(writer, "Physical");
                    return;
                case DamageTypeType.Toxic:
                    serializer.Serialize(writer, "Toxic");
                    return;
            }
            throw new Exception("Cannot marshal type DamageTypeType");
        }

        public static readonly DamageTypeTypeConverter Singleton = new DamageTypeTypeConverter();
    }
}
