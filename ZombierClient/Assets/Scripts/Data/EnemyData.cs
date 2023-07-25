using Firebase.Firestore;
using System.Collections.Generic;

namespace Prototype.Data
{
    [FirestoreData]
    public class EnemyData
    {
        [FirestoreProperty]
        public string ModelPrefabAddress { get; set; }

        [FirestoreProperty]
        public string ViewPrefabAddress { get; set; }

        [FirestoreProperty]
        public float MaxSpeed { get; set; }

        [FirestoreProperty]
        public DescDamage Damage { get; set; }

        [FirestoreProperty]
        public float CritChance { get; set; }

        [FirestoreProperty]
        public float CritMultiplier { get; set; }

        [FirestoreProperty]
        public float Health { get; set; }

        [FirestoreProperty]
        public float MaxHealth { get; set; }

        [FirestoreProperty]
        public DescDamage Resists { get; set; }

        [FirestoreProperty]
        public EnemyAttackData EnemyAttack { get; set; }

        [FirestoreProperty]
        public List<DescAttackStrategy> AttackStrategies { get; set; }

        [FirestoreProperty]
        public int ExpReward { get; set; }
    }
}
