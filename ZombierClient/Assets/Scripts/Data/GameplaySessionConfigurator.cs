using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Prototype.Data
{
    public class GameplaySessionConfigurator
    {
        public GameplaySessionConfigurator(AppData appData)
        {
            _appData = appData;
        }

        public GameSessionData CreateGameSession(string locationId)
        {
            var session = new GameSessionData();
            session.Player = CreatePlayerData();
            session.Location = CreateLocationData(locationId);
            return session;
        }

        private readonly AppData _appData;
        private GameBalanceData _gameBalance => _appData.GameBalance;

        private DescDamage CreateDescDamage(string damageId)
        {
            var dmgTypesCfg = _gameBalance.DamageTypes.Where(_ => _.Damage.Equals(damageId)).ToList();

            var descDmgTypes = dmgTypesCfg.Select(_ =>
            {
                var descDmgType = new DescDamageType(_.Type);
                descDmgType.ValueRange = new DescRandomRange { Min = (float)_.Min, Max = (float)_.Max };
                descDmgType.Chance = 1f;
                return descDmgType;
            }).ToList();

            var descDmg = new DescDamage();
            descDmg.AddRange(descDmgTypes);
            return descDmg;
        }

        private WeaponData CreateWeaponData(string weaponId)
        {
            var weaponCfg = _gameBalance.Weapons.FirstOrDefault(_ => _.Id.ToString().Equals(weaponId));
            var weaponLvlCfg = _gameBalance.WeaponLevel.FirstOrDefault(_ => _.Weapon.ToString().Equals(weaponCfg.Id.ToString()));

            var weaponData = new WeaponData();

            weaponData.AttackRateRPM = (float)weaponLvlCfg.FireRateAps * 60f;
            // TODO: add this variable to config
            weaponData.AttackRange = 12f;
            // TODO: add this variable to config
            weaponData.Thrust = (float)weaponLvlCfg.StoppingPower;
            weaponData.Recoil = (float)weaponLvlCfg.Recoil;
            weaponData.Damage = CreateDescDamage(weaponLvlCfg.Damage);

            var projectileData = new ProjectileData();
            projectileData.PrefabAddress = weaponCfg.ProjectilePrefabAddress;
            weaponData.ProjectileData = projectileData;

            return weaponData;
        }

        private PlayerData CreatePlayerData()
        {
            var playerCfg = _gameBalance.Player.PlayerConfig;
            var playerData = new PlayerData();

            playerData.PlayerPrefabAddress = playerCfg.AssetPath;
            playerData.MaxSpeed = (float)playerCfg.MaxSpeed;
            playerData.Damage = CreateDescDamage(playerCfg.Damage);
            playerData.CritChance = (float)playerCfg.CritChance;
            playerData.CritMultiplier = (float)playerCfg.CritMultiplier;
            playerData.MaxHealth = (float)playerCfg.MaxHealth;
            playerData.Health = playerData.MaxHealth;
            playerData.Resists = CreateDescDamage(playerCfg.Resists.ToString());
            playerData.Weapon = CreateWeaponData(playerCfg.Weapon.ToString());
            playerData.CurrentLevel = (int)playerCfg.Level;
            playerData.CurrentExp = (int)playerCfg.Exp;
            playerData.LevelExpThresholds = playerCfg.LevelExpThresholds.Select(_ => (int)_).ToArray();
            playerData.AppliedBuffs = new List<string>();

            return playerData;
        }

        private EnemyAttackData CreateEnemyAttackData(string enemyAttackId)
        {
            var enemyAttackCfg = _gameBalance.EnemyAttacks.FirstOrDefault(_ => _.Id.Equals(enemyAttackId));

            var enemyAttackData = new EnemyAttackData();
            enemyAttackData.AttackRateRPM = (float)enemyAttackCfg.AttackRateAps * 60f;
            enemyAttackData.AttackRange = (float)enemyAttackCfg.AttackRange;
            // TODO: add this parameter to remote config
            enemyAttackData.Thrust = 30f;
            // TODO: add this parameter to remote config
            enemyAttackData.Recoil = 0.1f;
            enemyAttackData.Damage = CreateDescDamage(enemyAttackCfg.Damage);
            enemyAttackData.ProjectileData = new ProjectileData { PrefabAddress = enemyAttackCfg.ProjectilePrefabAddress };

            return enemyAttackData;
        }

        private EnemyData CreateEnemyData(string enemyId, int enemyLevel)
        {
            var enemyCfg = _gameBalance.Enemies.FirstOrDefault(_ => _.Id.Equals(enemyId));
            var enemyLvlCfg = _gameBalance.EnemyLevel.FirstOrDefault(_ => _.Level == enemyLevel && _.EnemyId.Equals(enemyCfg.Id));

            var enemyData = new EnemyData();

            enemyData.ModelPrefabAddress = enemyCfg.ModelPrefabAddress;
            enemyData.ViewPrefabAddress = enemyCfg.ViewPrefabAddress;
            enemyData.MaxSpeed = (float)enemyLvlCfg.MaxSpeed;
            enemyData.Damage = CreateDescDamage(enemyLvlCfg.Damage);
            enemyData.CritChance = (float)enemyLvlCfg.CritChance;
            enemyData.CritMultiplier = (float)enemyLvlCfg.CritMultipleir;
            enemyData.MaxHealth = (float)enemyLvlCfg.MaxHealth;
            enemyData.Health = enemyData.MaxHealth;
            enemyData.Resists = CreateDescDamage(enemyLvlCfg.Resists);
            enemyData.EnemyAttack = CreateEnemyAttackData(enemyLvlCfg.Attacks);
            enemyData.ExpReward = (int)enemyLvlCfg.ExpReward;

            return enemyData;
        }

        private EnemySpawnData CreateEnemySpawnData(string enemySpawnConfigId)
        {
            var enemySpawnConfig = _gameBalance.EnemySpawnConfig.FirstOrDefault(_ => _.Id.Equals(enemySpawnConfigId));
            var enemySpawnData = ScriptableObject.CreateInstance<EnemySpawnData>();

            enemySpawnData.MinEnemyCount = (int)enemySpawnConfig.MinEnemyCount;
            enemySpawnData.MaxEnemyCount = (int)enemySpawnConfig.MaxEnemyCount;
            enemySpawnData.MinEnemyLevel = (int)enemySpawnConfig.MinEnemyLevel;
            enemySpawnData.MaxEnemyLevel = (int)enemySpawnConfig.MaxEnemyLevel;

            var enemySpawnTypesCfg = _gameBalance.EnemySpawnTypes
                .Where(_ => enemySpawnConfig.EnemySpawnTypes.Contains(_.Id)).ToArray(); ;
            var enemies = enemySpawnTypesCfg
                .Select(_ => CreateEnemyData(_.Enemy, 1)).ToList();
            enemySpawnData.Enemies = enemies;

            return enemySpawnData;
        }

        private LevelData CreateLevelData(string levelId)
        {
            var levelCfg = _gameBalance.Levels.FirstOrDefault(_ => _.Id.Equals(levelId));
            var levelData = new LevelData();

            levelData.EnemySpawnData = CreateEnemySpawnData(levelCfg.EnemySpawnData);
            levelData.LevelSize = (int)levelCfg.LevelSize;

            return levelData;
        }

        private LocationData CreateLocationData(string locationId)
        {
            var locationCfg = _gameBalance.Locations.FirstOrDefault(_ => _.Id.Equals(locationId));
            var locationData = new LocationData();

            var levelsCfg = _gameBalance.Levels
                .Where(_ => locationCfg.Levels.Contains(_.Id)).ToArray();
            locationData.Levels = levelsCfg
                .Select(_ => CreateLevelData(_.Id)).ToArray();

            locationData.LocationLevelPrefabAddress = locationCfg.LocationLevelPrefabAddress;
            locationData.GroundPrefabAddress = locationCfg.GroundPrefabAddress;
            locationData.WallPrefabsLabel = locationCfg.WallPrefabsLabel;
            locationData.ObstaclePrefabsLabel = locationCfg.ObstaclePrefabsLabel;
            locationData.ExitPrefabAddress = locationCfg.ExitPrefabAddress;
            locationData.EnvGroundPrefabAddress = locationCfg.EnvGroundPrefabAddress;
            locationData.EnvObstaclePrefabsLabel = locationCfg.EnvObstaclePrefabsLabel;

            return locationData;
        }
    }
}
