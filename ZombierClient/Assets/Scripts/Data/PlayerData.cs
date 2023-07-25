using Sirenix.Serialization;

namespace Prototype.Data
{
    public class PlayerData
    {
        public string PlayerPrefabAddress;
        public float MaxSpeed;

        [OdinSerialize] public DescDamage Damage;
        public float CritChance;
        public float CritMultiplier;

        public float Health;
        public float MaxHealth;
        [OdinSerialize] public DescDamage Resists;

        public WeaponData Weapon;

        public int CurrentLevel;
        public int SavedLevelUpCounter;
        public int CurrentExp;
        public int[] LevelExpThresholds;
    }
}