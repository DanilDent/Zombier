using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Player Data", menuName = "Data/Player Data")]
    public class PlayerData : SerializedScriptableObject
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