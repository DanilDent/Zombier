using Firebase.Firestore;
using System.Collections.Generic;

namespace Prototype.Data
{
    [FirestoreData]
    public class EnemySpawnData
    {
        [FirestoreProperty]
        public List<EnemySpawnTypeData> Enemies { get; set; }

        [FirestoreProperty]
        public int MinEnemyCount { get; set; }

        [FirestoreProperty]
        public int MaxEnemyCount { get; set; }

        [FirestoreProperty]
        public int MinEnemyLevel { get; set; }

        [FirestoreProperty]
        public int MaxEnemyLevel { get; set; }
    }

}