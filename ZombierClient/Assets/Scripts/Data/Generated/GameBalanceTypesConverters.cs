using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace Prototype.Data
{
    public enum DamageTypeEnum { Electric, Fire, Physical, Toxic, Frost, None };
    public enum BuffTypeEnum { Heal, IncreaseMaxHealth, IncreaseDamage, BouncingProjectiles, None };
    public enum EffectTypeEnum { Burn, Poison, Freeze, Smite, Dodge, Explode, Hypnotize, Scare, None };
    public enum ApplyEventTypeEnum { DamageTarget, DamagedByTarget, DestroyTarget, None };

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
                EffectTypeEnumConverter.Singleton,
                ApplyEventTypEnumConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ApplyEventTypEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ApplyEventTypeEnum) || t == typeof(ApplyEventTypeEnum?);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "":
                    return ApplyEventTypeEnum.None;
                case "DamageTarget":
                    return ApplyEventTypeEnum.DamageTarget;
                case "DamagedByTarget":
                    return ApplyEventTypeEnum.DamagedByTarget;
                case "DestroyTarget":
                    return ApplyEventTypeEnum.DestroyTarget;
            }
            throw new Exception("Cannot unmarshal type ApplyEventTypeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (ApplyEventTypeEnum)untypedValue;
            switch (value)
            {
                case ApplyEventTypeEnum.None:
                    serializer.Serialize(writer, "");
                    return;
                case ApplyEventTypeEnum.DamageTarget:
                    serializer.Serialize(writer, "DamageTarget");
                    return;
                case ApplyEventTypeEnum.DamagedByTarget:
                    serializer.Serialize(writer, "DamagedByTarget");
                    return;
                case ApplyEventTypeEnum.DestroyTarget:
                    serializer.Serialize(writer, "DestroyTarget");
                    return;
            }
            throw new Exception("Cannot marshal type ApplyEventTypeEnum");
        }

        public static readonly ApplyEventTypEnumConverter Singleton = new ApplyEventTypEnumConverter();
    }

    internal class EffectTypeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(EffectTypeEnum) || t == typeof(EffectTypeEnum?);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "":
                    return EffectTypeEnum.None;
                case "Burn":
                    return EffectTypeEnum.Burn;
                case "Poison":
                    return EffectTypeEnum.Poison;
                case "Freeze":
                    return EffectTypeEnum.Freeze;
                case "Smite":
                    return EffectTypeEnum.Smite;
                case "Dodge":
                    return EffectTypeEnum.Dodge;
                case "Explode":
                    return EffectTypeEnum.Explode;
                case "Hypnotize":
                    return EffectTypeEnum.Hypnotize;
                case "Scare":
                    return EffectTypeEnum.Scare;

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
            var value = (EffectTypeEnum)untypedValue;
            switch (value)
            {
                case EffectTypeEnum.None:
                    serializer.Serialize(writer, "");
                    return;
                case EffectTypeEnum.Burn:
                    serializer.Serialize(writer, "Burn");
                    return;
                case EffectTypeEnum.Poison:
                    serializer.Serialize(writer, "Poison");
                    return;
                case EffectTypeEnum.Freeze:
                    serializer.Serialize(writer, "Freeze");
                    return;
                case EffectTypeEnum.Smite:
                    serializer.Serialize(writer, "Smite");
                    return;
                case EffectTypeEnum.Dodge:
                    serializer.Serialize(writer, "Dodge");
                    return;
                case EffectTypeEnum.Explode:
                    serializer.Serialize(writer, "Explode");
                    return;
                case EffectTypeEnum.Hypnotize:
                    serializer.Serialize(writer, "Hypnotize");
                    return;
                case EffectTypeEnum.Scare:
                    serializer.Serialize(writer, "Scare");
                    return;
            }
            throw new Exception("Cannot marshal type EffectEnum");
        }

        public static readonly EffectTypeEnumConverter Singleton = new EffectTypeEnumConverter();
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
                case "Frost":
                    return DamageTypeEnum.Frost;
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
                case DamageTypeEnum.Frost:
                    serializer.Serialize(writer, "Frost");
                    return;
            }
            throw new Exception("Cannot marshal type DamageTypeType");
        }

        public static readonly DamageTypeEnumConverter Singleton = new DamageTypeEnumConverter();
    }
}
