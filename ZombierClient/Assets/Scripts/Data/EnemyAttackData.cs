using Firebase.Firestore;

namespace Prototype.Data
{
    [FirestoreData]
    public class EnemyAttackData
    {
        public float AttackRateRPM { get; set; }

        public float Attack0SpeedMultiplier { get; set; }

        public float Attack1SpeedMultiplier { get; set; }

        public float AttackRange { get; set; }

        public float Thrust { get; set; }

        public float Recoil { get; set; }

        public DescDamage Damage { get; set; }

        public ProjectileData ProjectileData { get; set; }
    }
}
