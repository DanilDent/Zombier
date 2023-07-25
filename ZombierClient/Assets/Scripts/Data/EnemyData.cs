using Sirenix.Serialization;
using System.Collections.Generic;

namespace Prototype.Data
{
    public class EnemyData
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
