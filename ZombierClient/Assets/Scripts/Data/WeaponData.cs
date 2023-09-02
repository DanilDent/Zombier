using Firebase.Firestore;

namespace Prototype.Data
{
    [FirestoreData]
    public class WeaponData
    {
        [FirestoreProperty]
        public string Id { get; set; }

        [FirestoreProperty]
        public string PrefabAddress { get; set; }

        [FirestoreProperty]
        public float AttackRateRPM { get; set; }

        [FirestoreProperty]
        public float AttackRange { get; set; }

        [FirestoreProperty]
        public float Thrust { get; set; }

        [FirestoreProperty]
        public float Recoil { get; set; }

        [FirestoreProperty]
        public DescDamage Damage { get; set; }

        [FirestoreProperty]
        public ProjectileData ProjectileData { get; set; }

        [FirestoreProperty]
        public ShootingTypeEnum ShootingType { get; set; }
    }
}