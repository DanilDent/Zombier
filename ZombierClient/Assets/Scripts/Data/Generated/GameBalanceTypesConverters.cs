using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace Prototype.Data
{
    public enum DamageTypeEnum { Electric, Fire, Physical, Toxic, None };
    public enum BuffTypeEnum { Heal, IncreaseMaxHealth, IncreaseDamage, BouncingProjectiles, None };
    public enum EffectEnum { Burn, None };

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                DamageTypeEnumConverter.Singleton,
                BuffTypeEnumConverter.Singleton,
                EffectEnumConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class EffectEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(EffectEnum) || t == typeof(EffectEnum?);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "":
                    return EffectEnum.None;
                case "Burn":
                    return EffectEnum.Burn;
            }
            throw new Exception("Cannot unmarshal type EffectEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (EffectEnum)untypedValue;
            switch (value)
            {
                case EffectEnum.None:
                    serializer.Serialize(writer, "");
                    return;
                case EffectEnum.Burn:
                    serializer.Serialize(writer, "Burn");
                    return;
            }
            throw new Exception("Cannot marshal type EffectEnum");
        }

        public static readonly EffectEnumConverter Singleton = new EffectEnumConverter();
    }

    internal class BuffTypeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(BuffTypeEnum) || t == typeof(BuffTypeEnum?);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "":
                    return BuffTypeEnum.None;
                case "Heal":
                    return BuffTypeEnum.Heal;
                case "IncreaseMaxHealth":
                    return BuffTypeEnum.IncreaseMaxHealth;
                case "IncreaseDamage":
                    return BuffTypeEnum.IncreaseDamage;
                case "BouncingProjectiles":
                    return BuffTypeEnum.BouncingProjectiles;
            }
            throw new Exception("Cannot unmarshal type BuffTypeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (BuffTypeEnum)untypedValue;
            switch (value)
            {
                case BuffTypeEnum.None:
                    serializer.Serialize(writer, "");
                    return;
                case BuffTypeEnum.Heal:
                    serializer.Serialize(writer, "Heal");
                    return;
                case BuffTypeEnum.IncreaseMaxHealth:
                    serializer.Serialize(writer, "IncreaseMaxHealth");
                    return;
                case BuffTypeEnum.IncreaseDamage:
                    serializer.Serialize(writer, "IncreaseDamage");
                    return;
                case BuffTypeEnum.BouncingProjectiles:
                    serializer.Serialize(writer, "BouncingProjectiles");
                    return;
            }
            throw new Exception("Cannot marshal type BuffTypeEnum");
        }

        public static readonly BuffTypeEnumConverter Singleton = new BuffTypeEnumConverter();
    }

    internal class DamageTypeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(DamageTypeEnum) || t == typeof(DamageTypeEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "":
                    return DamageTypeEnum.None;
                case "Electric":
                    return DamageTypeEnum.Electric;
                case "Fire":
                    return DamageTypeEnum.Fire;
                case "Physical":
                    return DamageTypeEnum.Physical;
                case "Toxic":
                    return DamageTypeEnum.Toxic;
            }
            throw new Exception("Cannot unmarshal type DamageTypeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (DamageTypeEnum)untypedValue;
            switch (value)
            {
                case DamageTypeEnum.Electric:
                    serializer.Serialize(writer, "Electric");
                    return;
                case DamageTypeEnum.Fire:
                    serializer.Serialize(writer, "Fire");
                    return;
                case DamageTypeEnum.Physical:
                    serializer.Serialize(writer, "Physical");
                    return;
                case DamageTypeEnum.Toxic:
                    serializer.Serialize(writer, "Toxic");
                    return;
            }
            throw new Exception("Cannot marshal type DamageTypeType");
        }

        public static readonly DamageTypeEnumConverter Singleton = new DamageTypeEnumConverter();
    }
}
