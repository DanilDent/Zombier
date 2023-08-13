using Firebase.Firestore;
using System.Collections.Generic;

namespace Prototype.Data
{
    [FirestoreData]
    public class EnemyData
    {
        [FirestoreProperty]
        public string EnemyId { get; set; }

        [FirestoreProperty]
        public string ModelPrefabAddress { get; set; }

        [FirestoreProperty]
        public string ViewPrefabAddress { get; set; }

        [FirestoreProperty]
        public int Level { get; set; }

        public float MaxSpeed { get; set; }

        public DescDamage Damage { get; set; }

        public float CritChance { get; set; }

        public float CritMultiplier { get; set; }

        public float Health { get; set; }

        public float MaxHealth { get; set; }

        public DescDamage Resists { get; set; }

        public EnemyAttackData EnemyAttack { get; set; }

        public List<DescAttackStrategy> AttackStrategies { get; set; }

        public int ExpReward { get; set; }

        public float HitFromFront0Speed { get; set; }
    }
}
