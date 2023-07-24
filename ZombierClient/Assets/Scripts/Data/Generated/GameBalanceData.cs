using System.Collections.Generic;

namespace Prototype.Data
{

    public class Damage
    {
        public int Index { get; set; }
        public string Id { get; set; }
        public IList<string> DamageTypes { get; set; }
    }

    public class DamageType
    {
        public int Index { get; set; }
        public string Id { get; set; }
        public string Damage { get; set; }
        public string Type { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
    }

    public class WeaponLevel
    {
        public int Index { get; set; }
        public string Id { get; set; }
        public string Weapon { get; set; }
        public int Level { get; set; }
        public int PriceSoft { get; set; }
        public string Damage { get; set; }
        public double Recoil { get; set; }
        public double FireRateAPS { get; set; }
        public double StoppingPower { get; set; }
    }

    public class PlayerConfig
    {
        public string Id { get; set; }
        public string AssetPath { get; set; }
        public double MaxSpeed { get; set; }
        public string Damage { get; set; }
        public double CritChance { get; set; }
        public int CritMultiplier { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public string Resists { get; set; }
        public string Weapon { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public IList<int> LevelExpThresholds { get; set; }
    }

    public class Player
    {
        public PlayerConfig PlayerConfig { get; set; }
    }

    public class Weapon
    {
        public int Index { get; set; }
        public string Id { get; set; }
        public string PrefabAddress { get; set; }
        public string IconAddress { get; set; }
        public string ProjectilePrefabAddress { get; set; }
        public string Level { get; set; }
    }

    public class Enemy
    {
        public int Index { get; set; }
        public string Id { get; set; }
        public string ModelPrefabAddress { get; set; }
        public string ViewPrefabAddress { get; set; }
        public string Level { get; set; }
    }

    public class Location
    {
        public int Index { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string LocationLevelPrefabAddress { get; set; }
        public string GroundPrefabAddress { get; set; }
        public string WallPrefabsLabel { get; set; }
        public string ObstaclePrefabsLabel { get; set; }
        public string ExitPrefabAddress { get; set; }
        public string EnvGroundPrefabAddress { get; set; }
        public string EnvObstaclePrefabsLabel { get; set; }
        public IList<string> Levels { get; set; }
    }

    public class Level
    {
        public int Index { get; set; }
        public string Id { get; set; }
        public string Locaiton { get; set; }
        public int LevelIndex { get; set; }
        public string EnemySpawnData { get; set; }
        public int LevelSize { get; set; }
    }

    public class EnemySpawnConfig
    {
        public int Index { get; set; }
        public string Id { get; set; }
        public int Level { get; set; }
        public IList<string> EnemySpawnTypes { get; set; }
        public int MinEnemyCount { get; set; }
        public int MaxEnemyCount { get; set; }
        public int MinEnemyLevel { get; set; }
        public int MaxEnemyLevel { get; set; }
    }

    public class EnemySpawnType
    {
        public int Index { get; set; }
        public string Id { get; set; }
        public int Level { get; set; }
        public string SpawnData { get; set; }
        public string Enemy { get; set; }
        public double Chance { get; set; }
    }

    public class EnemyLevel
    {
        public int Index { get; set; }
        public string Id { get; set; }
        public int Level { get; set; }
        public double MaxSpeed { get; set; }
        public string Damage { get; set; }
        public int CritChance { get; set; }
        public int CritMultipleir { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public string Resists { get; set; }
        public string Attacks { get; set; }
        public int ExpReward { get; set; }
    }

    public class EnemyAttack
    {
        public int Index { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
        public string Damage { get; set; }
        public int AttackRange { get; set; }
        public double AttackRateAPS { get; set; }
        public string ProjectilePrefabAddress { get; set; }
    }

    public class GameBalanceData
    {
        public IList<Damage> Damage { get; set; }
        public IList<DamageType> DamageTypes { get; set; }
        public IList<WeaponLevel> WeaponLevel { get; set; }
        public Player Player { get; set; }
        public IList<Weapon> Weapons { get; set; }
        public IList<Enemy> Enemies { get; set; }
        public IList<Location> Locations { get; set; }
        public IList<Level> Levels { get; set; }
        public IList<EnemySpawnConfig> EnemySpawnConfig { get; set; }
        public IList<EnemySpawnType> EnemySpawnTypes { get; set; }
        public IList<EnemyLevel> EnemyLevel { get; set; }
        public IList<EnemyAttack> EnemyAttacks { get; set; }
    }

}
