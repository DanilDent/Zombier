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
        public int Health;
        public int MaxHealth;
        public float Speed;
        [NonSerialized] [OdinSerialize] public DescDamage Damage;
        public float CritChance;
        public float CritMultiplier;
        public WeaponData Weapon;
    }
}