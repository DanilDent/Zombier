using Firebase.Firestore;
using System.Collections.Generic;

namespace Prototype.Data
{
    [FirestoreData]
    public class PlayerData
    {
        [FirestoreProperty]
        public string PlayerPrefabAddress { get; set; }

        [FirestoreProperty]
        public float MaxSpeed { get; set; }

        [FirestoreProperty]
        public float CritChance { get; set; }

        [FirestoreProperty]
        public float CritMultiplier { get; set; }

        [FirestoreProperty]
        public float HealthRatio { get; set; }

        [FirestoreProperty]
        public DescDamage Resists { get; set; }

        [FirestoreProperty]
        public WeaponData Weapon { get; set; }

        [FirestoreProperty]
        public int CurrentLevel { get; set; }

        [FirestoreProperty]
        public int SavedLevelUpCounter { get; set; }

        [FirestoreProperty]
        public int CurrentExp { get; set; }

        [FirestoreProperty]
        public int[] LevelExpThresholds { get; set; }

        [FirestoreProperty]
        public List<string> AppliedBuffs { get; set; }
    }
}