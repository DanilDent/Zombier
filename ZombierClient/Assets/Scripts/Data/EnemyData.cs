using Prototype.View;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Enemy Data", menuName = "Data/Enemy Data")]
    public class EnemyData : SerializedScriptableObject
    {
        public EnemyView ViewPrefab;
        public float MaxSpeed;

        [NonSerialized] [OdinSerialize] public DescDamage Damage;
        public float CritChance;
        public float CritMultiplier;

        public float Health;
        public float MaxHealth;
        [NonSerialized] [OdinSerialize] public DescDamage Resists;

        public WeaponData Weapon;
        public List<DescAttackStrategy> AttackStrategies;
        public int ExpReward;
    }
}
