namespace Prototype.Data
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
    using System.Globalization;

    public partial class GameBalanceData
    {
        [JsonProperty("Damage")]
        public DamageElement[] Damage { get; set; }

        [JsonProperty("DamageTypes")]
        public DamageType[] DamageTypes { get; set; }

        [JsonProperty("WeaponLevel")]
        public WeaponLevel[] WeaponLevel { get; set; }

        [JsonProperty("Player")]
        public Player Player { get; set; }

        [JsonProperty("Weapons")]
        public Weapon[] Weapons { get; set; }

        [JsonProperty("Enemies")]
        public EnemyElement[] Enemies { get; set; }

        [JsonProperty("Locations")]
        public Location[] Locations { get; set; }

        [JsonProperty("Levels")]
        public Level[] Levels { get; set; }

        [JsonProperty("EnemySpawnConfig")]
        public EnemySpawnConfig[] EnemySpawnConfig { get; set; }

        [JsonProperty("EnemySpawnTypes")]
        public EnemySpawnType[] EnemySpawnTypes { get; set; }

        [JsonProperty("EnemyLevel")]
        public EnemyLevel[] EnemyLevel { get; set; }

        [JsonProperty("EnemyAttacks")]
        public EnemyAttack[] EnemyAttacks { get; set; }
    }

    public partial class DamageElement
    {
        [JsonProperty("Index")]
        public long Index { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("DamageTypes")]
        public string[] DamageTypes { get; set; }
    }

    public partial class DamageType
    {
        [JsonProperty("Index")]
        public long Index { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Damage")]
        public string Damage { get; set; }

        [JsonProperty("Type")]
        public DamageTypeType Type { get; set; }

        [JsonProperty("Min ")]
        public double Min { get; set; }

        [JsonProperty("Max")]
        public double Max { get; set; }
    }

    public partial class EnemyElement
    {
        [JsonProperty("Index")]
        public long Index { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("ModelPrefabAddress")]
        public string ModelPrefabAddress { get; set; }

        [JsonProperty("ViewPrefabAddress")]
        public string ViewPrefabAddress { get; set; }

        [JsonProperty("Level")]
        public string Level { get; set; }
    }

    public partial class EnemyAttack
    {
        [JsonProperty("Index")]
        public long Index { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Type")]
        public EnemyAttackType Type { get; set; }

        [JsonProperty("Damage")]
        public string Damage { get; set; }

        [JsonProperty("AttackRange")]
        public long AttackRange { get; set; }

        [JsonProperty("AttackRateAPS")]
        public double AttackRateAps { get; set; }

        [JsonProperty("ProjectilePrefabAddress")]
        public ProjectilePrefabAddress ProjectilePrefabAddress { get; set; }
    }

    public partial class EnemyLevel
    {
        [JsonProperty("Index")]
        public long Index { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Level")]
        public long Level { get; set; }

        [JsonProperty("MaxSpeed")]
        public double MaxSpeed { get; set; }

        [JsonProperty("Damage")]
        public string Damage { get; set; }

        [JsonProperty("CritChance")]
        public long CritChance { get; set; }

        [JsonProperty("CritMultipleir")]
        public long CritMultipleir { get; set; }

        [JsonProperty("Health")]
        public long Health { get; set; }

        [JsonProperty("MaxHealth")]
        public long MaxHealth { get; set; }

        [JsonProperty("Resists")]
        public string Resists { get; set; }

        [JsonProperty("Attacks")]
        public string Attacks { get; set; }

        [JsonProperty("ExpReward")]
        public long ExpReward { get; set; }
    }

    public partial class EnemySpawnConfig
    {
        [JsonProperty("Index")]
        public long Index { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Level")]
        public long Level { get; set; }

        [JsonProperty("EnemySpawnTypes")]
        public string[] EnemySpawnTypes { get; set; }

        [JsonProperty("MinEnemyCount")]
        public long MinEnemyCount { get; set; }

        [JsonProperty("MaxEnemyCount")]
        public long MaxEnemyCount { get; set; }

        [JsonProperty("MinEnemyLevel")]
        public long MinEnemyLevel { get; set; }

        [JsonProperty("MaxEnemyLevel")]
        public long MaxEnemyLevel { get; set; }
    }

    public partial class EnemySpawnType
    {
        [JsonProperty("Index")]
        public long Index { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Level")]
        public long Level { get; set; }

        [JsonProperty("SpawnData")]
        public string SpawnData { get; set; }

        [JsonProperty("Enemy")]
        public EnemyEnum Enemy { get; set; }

        [JsonProperty("Chance")]
        public double Chance { get; set; }
    }

    public partial class Level
    {
        [JsonProperty("Index")]
        public long Index { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Locaiton")]
        public Locaiton Locaiton { get; set; }

        [JsonProperty("LevelIndex")]
        public long LevelIndex { get; set; }

        [JsonProperty("EnemySpawnData")]
        public string EnemySpawnData { get; set; }

        [JsonProperty("LevelSize")]
        public long LevelSize { get; set; }
    }

    public partial class Location
    {
        [JsonProperty("Index")]
        public long Index { get; set; }

        [JsonProperty("Id")]
        public Locaiton Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("LocationLevelPrefabAddress")]
        public string LocationLevelPrefabAddress { get; set; }

        [JsonProperty("GroundPrefabAddress")]
        public string GroundPrefabAddress { get; set; }

        [JsonProperty("WallPrefabsLabel")]
        public string WallPrefabsLabel { get; set; }

        [JsonProperty("ObstaclePrefabsLabel")]
        public string ObstaclePrefabsLabel { get; set; }

        [JsonProperty("ExitPrefabAddress")]
        public string ExitPrefabAddress { get; set; }

        [JsonProperty("EnvGroundPrefabAddress")]
        public string EnvGroundPrefabAddress { get; set; }

        [JsonProperty("EnvObstaclePrefabsLabel")]
        public string EnvObstaclePrefabsLabel { get; set; }

        [JsonProperty("Levels")]
        public string[] Levels { get; set; }
    }

    public partial class Player
    {
        [JsonProperty("PlayerConfig")]
        public PlayerConfig PlayerConfig { get; set; }
    }

    public partial class PlayerConfig
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("AssetPath")]
        public string AssetPath { get; set; }

        [JsonProperty("MaxSpeed")]
        public double MaxSpeed { get; set; }

        [JsonProperty("Damage")]
        public string Damage { get; set; }

        [JsonProperty("CritChance")]
        public double CritChance { get; set; }

        [JsonProperty("CritMultiplier")]
        public long CritMultiplier { get; set; }

        [JsonProperty("Health")]
        public long Health { get; set; }

        [JsonProperty("MaxHealth")]
        public long MaxHealth { get; set; }

        [JsonProperty("Resists")]
        public string Resists { get; set; }

        [JsonProperty("Weapon")]
        public WeaponEnum Weapon { get; set; }

        [JsonProperty("Level")]
        public long Level { get; set; }

        [JsonProperty("Exp")]
        public long Exp { get; set; }

        [JsonProperty("LevelExpThresholds")]
        public long[] LevelExpThresholds { get; set; }
    }

    public partial class WeaponLevel
    {
        [JsonProperty("Index")]
        public long Index { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Weapon")]
        public WeaponEnum Weapon { get; set; }

        [JsonProperty("Level")]
        public long Level { get; set; }

        [JsonProperty("Price_Soft")]
        public long PriceSoft { get; set; }

        [JsonProperty("Damage")]
        public string Damage { get; set; }

        [JsonProperty("Recoil")]
        public double Recoil { get; set; }

        [JsonProperty("FireRateAPS")]
        public double FireRateAps { get; set; }

        [JsonProperty("StoppingPower")]
        public double StoppingPower { get; set; }
    }

    public partial class Weapon
    {
        [JsonProperty("Index")]
        public long Index { get; set; }

        [JsonProperty("Id")]
        public WeaponEnum Id { get; set; }

        [JsonProperty("PrefabAddress")]
        public string PrefabAddress { get; set; }

        [JsonProperty("IconAddress")]
        public string IconAddress { get; set; }

        [JsonProperty("ProjectilePrefabAddress")]
        public string ProjectilePrefabAddress { get; set; }

        [JsonProperty("Level")]
        public string Level { get; set; }
    }

    public enum DamageTypeType { Electric, Fire, Physical, Toxic };

    public enum ProjectilePrefabAddress { Rock };

    public enum EnemyAttackType { Range };

    public enum EnemyEnum { IdEnemyLocation0RangeYbot };

    public enum Locaiton { IdLocaiton0 };

    public enum WeaponEnum { IdPistol };

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
                EnemyAttackTypeConverter.Singleton,
                EnemyEnumConverter.Singleton,
                LocaitonConverter.Singleton,
                WeaponEnumConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
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

    internal class ProjectilePrefabAddressConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ProjectilePrefabAddress) || t == typeof(ProjectilePrefabAddress?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "Rock")
            {
                return ProjectilePrefabAddress.Rock;
            }
            throw new Exception("Cannot unmarshal type ProjectilePrefabAddress");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (ProjectilePrefabAddress)untypedValue;
            if (value == ProjectilePrefabAddress.Rock)
            {
                serializer.Serialize(writer, "Rock");
                return;
            }
            throw new Exception("Cannot marshal type ProjectilePrefabAddress");
        }

        public static readonly ProjectilePrefabAddressConverter Singleton = new ProjectilePrefabAddressConverter();
    }

    internal class EnemyAttackTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(EnemyAttackType) || t == typeof(EnemyAttackType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "Range")
            {
                return EnemyAttackType.Range;
            }
            throw new Exception("Cannot unmarshal type EnemyAttackType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (EnemyAttackType)untypedValue;
            if (value == EnemyAttackType.Range)
            {
                serializer.Serialize(writer, "Range");
                return;
            }
            throw new Exception("Cannot marshal type EnemyAttackType");
        }

        public static readonly EnemyAttackTypeConverter Singleton = new EnemyAttackTypeConverter();
    }

    internal class EnemyEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(EnemyEnum) || t == typeof(EnemyEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "Id_Enemy_Location0_Range_Ybot")
            {
                return EnemyEnum.IdEnemyLocation0RangeYbot;
            }
            throw new Exception("Cannot unmarshal type EnemyEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (EnemyEnum)untypedValue;
            if (value == EnemyEnum.IdEnemyLocation0RangeYbot)
            {
                serializer.Serialize(writer, "Id_Enemy_Location0_Range_Ybot");
                return;
            }
            throw new Exception("Cannot marshal type EnemyEnum");
        }

        public static readonly EnemyEnumConverter Singleton = new EnemyEnumConverter();
    }

    internal class LocaitonConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Locaiton) || t == typeof(Locaiton?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "Id_Locaiton0")
            {
                return Locaiton.IdLocaiton0;
            }
            throw new Exception("Cannot unmarshal type Locaiton");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Locaiton)untypedValue;
            if (value == Locaiton.IdLocaiton0)
            {
                serializer.Serialize(writer, "Id_Locaiton0");
                return;
            }
            throw new Exception("Cannot marshal type Locaiton");
        }

        public static readonly LocaitonConverter Singleton = new LocaitonConverter();
    }

    internal class WeaponEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(WeaponEnum) || t == typeof(WeaponEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "Id_Pistol")
            {
                return WeaponEnum.IdPistol;
            }
            throw new Exception("Cannot unmarshal type WeaponEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (WeaponEnum)untypedValue;
            if (value == WeaponEnum.IdPistol)
            {
                serializer.Serialize(writer, "Id_Pistol");
                return;
            }
            throw new Exception("Cannot marshal type WeaponEnum");
        }

        public static readonly WeaponEnumConverter Singleton = new WeaponEnumConverter();
    }
}
