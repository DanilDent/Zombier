namespace Prototype.Data
{
    using Newtonsoft.Json;
    using Prototype.Misc;

    public partial class GameBalanceData
    {
        [JsonProperty("Damage")]
        public Damage[] Damage { get; set; }

        [JsonProperty("DamageTypes")]
        public DamageType[] DamageTypes { get; set; }

        [JsonProperty("WeaponLevel")]
        public WeaponLevel[] WeaponLevel { get; set; }

        [JsonProperty("Player")]
        public Player Player { get; set; }

        [JsonProperty("Weapons")]
        public Weapon[] Weapons { get; set; }

        [JsonProperty("Enemies")]
        public Enemy[] Enemies { get; set; }

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

        [JsonProperty("BuffConfigs")]
        public BuffConfig[] BuffConfigs { get; set; }

        [JsonProperty("EffectConfigs")]
        public EffectConfig[] EffectConfigs { get; set; }
    }

    public partial class BuffConfig : IWeighted
    {
        [JsonProperty("Index")]
        public long Index { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("DescriptionText")]
        public string DescriptionText { get; set; }

        [JsonProperty("RarityText")]
        public string RarityText { get; set; }

        [JsonProperty("BuffType")]
        public BuffTypeEnum BuffType { get; set; }

        [JsonProperty("BuffLevel")]
        public long BuffLevel { get; set; }

        [JsonProperty("Value")]
        public double Value { get; set; }

        [JsonProperty("DamageType")]
        public DamageTypeEnum DamageType { get; set; }

        [JsonProperty("Effects")]
        public string Effects { get; set; }

        [JsonProperty("Rarity")]
        public RarityEnum Rarity { get; set; }

        [JsonProperty("Weight")]
        public float Weight { get; set; }
    }

    public partial class Damage
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
        public DamageTypeEnum Type { get; set; }

        [JsonProperty("Min ")]
        public double Min { get; set; }

        [JsonProperty("Max")]
        public double Max { get; set; }
    }

    public partial class EffectConfig
    {
        [JsonProperty("Index")]
        public long Index { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("EffectType")]
        public EffectTypeEnum EffectType { get; set; }

        [JsonProperty("Chance")]
        public double Chance { get; set; }

        [JsonProperty("Duration")]
        public double Duration { get; set; }

        [JsonProperty("Interval")]
        public double Interval { get; set; }

        [JsonProperty("ApplyEventType")]
        public ApplyEventTypeEnum ApplyEventType { get; set; }

        [JsonProperty("DamageType")]
        public DamageTypeEnum DamageType { get; set; }

        [JsonProperty("DamageValue")]
        public double DamageValue { get; set; }

        [JsonProperty("SpreadRadius")]
        public double SpreadRadius { get; set; }
    }

    public partial class Enemy
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
        public string Type { get; set; }

        [JsonProperty("Damage")]
        public string Damage { get; set; }

        [JsonProperty("AttackRange")]
        public double AttackRange { get; set; }

        [JsonProperty("AttackRateAPS")]
        public double AttackRateAps { get; set; }

        [JsonProperty("ProjectilePrefabAddress", NullValueHandling = NullValueHandling.Ignore)]
        public string ProjectilePrefabAddress { get; set; }
    }

    public partial class EnemyLevel
    {
        [JsonProperty("Index")]
        public long Index { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("EnemyId")]
        public string EnemyId { get; set; }

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
        public string Enemy { get; set; }

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
        public string Locaiton { get; set; }

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
        public string Id { get; set; }

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
        public string Weapon { get; set; }

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
        public string Weapon { get; set; }

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
        public string Id { get; set; }

        [JsonProperty("PrefabAddress")]
        public string PrefabAddress { get; set; }

        [JsonProperty("IconAddress")]
        public string IconAddress { get; set; }

        [JsonProperty("ProjectilePrefabAddress", NullValueHandling = NullValueHandling.Ignore)]
        public string ProjectilePrefabAddress { get; set; }

        [JsonProperty("Level")]
        public string Level { get; set; }
    }
}
