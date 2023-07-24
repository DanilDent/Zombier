using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Enemy Data", menuName = "Data/Enemy Data")]
    public class EnemyData : SerializedScriptableObject
    {
        public string ModelPrefabAddress;
        public string ViewPrefabAddress;
        public float MaxSpeed;

        [OdinSerialize] public DescDamage Damage;
        public float CritChance;
        public float CritMultiplier;

        public float Health;
        public float MaxHealth;
        [OdinSerialize] public DescDamage Resists;

        public EnemyAttackData EnemyAttack;
        public List<DescAttackStrategy> AttackStrategies;
        public int ExpReward;
    }
}
