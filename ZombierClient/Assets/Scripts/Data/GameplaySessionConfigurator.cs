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

        public GameSessionData CreateGameSession()
        {
            var session = ScriptableObject.CreateInstance<GameSessionData>();
            session.Player = CreatePlayerData();
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

            var weaponData = ScriptableObject.CreateInstance<WeaponData>();

            weaponData.AttackRateRPM = (float)weaponLvlCfg.FireRateAps * 60f;
            // TODO: add this variable to config
            weaponData.AttackRange = 12f;
            // TODO: add this variable to config
            weaponData.Thrust = (float)weaponLvlCfg.StoppingPower;
            weaponData.Recoil = (float)weaponLvlCfg.Recoil;
            weaponData.Damage = CreateDescDamage(weaponLvlCfg.Damage);

            var projectileData = ScriptableObject.CreateInstance<ProjectileData>();
            projectileData.PrefabAddress = weaponCfg.ProjectilePrefabAddress;
            weaponData.ProjectileData = projectileData;

            return weaponData;
        }

        private PlayerData CreatePlayerData()
        {
            var playerCfg = _gameBalance.Player.PlayerConfig;
            var playerData = ScriptableObject.CreateInstance<PlayerData>();

            playerData.PlayerPrefabAddress = playerCfg.AssetPath;
            playerData.MaxSpeed = (float)playerCfg.MaxSpeed;
            playerData.Damage = CreateDescDamage(playerCfg.Damage.ToString());
            playerData.CritChance = (float)playerCfg.CritChance;
            playerData.CritMultiplier = (float)playerCfg.CritMultiplier;
            playerData.MaxHealth = (float)playerCfg.MaxHealth;
            playerData.Health = playerData.MaxHealth;
            playerData.Resists = CreateDescDamage(playerCfg.Resists.ToString());
            playerData.Weapon = CreateWeaponData(playerCfg.Weapon.ToString());
            playerData.CurrentLevel = (int)playerCfg.Level;
            playerData.CurrentExp = (int)playerCfg.Exp;
            playerData.LevelExpThresholds = playerCfg.LevelExpThresholds.Select(_ => (int)_).ToArray();

            return playerData;
        }
    }
}
