using Prototype.Model;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Player Data", menuName = "Data/Player Data")]
    public class PlayerData : SerializedScriptableObject
    {
        public PlayerModel PlayerPrefab;
        public float MaxSpeed;

        [NonSerialized] [OdinSerialize] public DescDamage Damage;
        public float CritChance;
        public float CritMultiplier;

        public float Health;
        public float MaxHealth;
        [NonSerialized] [OdinSerialize] public DescDamage Resists;

        public WeaponData Weapon;

        public int CurrentLevel;
        public int CurrentExp;
        public int[] LevelExpThresholds;
    }
}