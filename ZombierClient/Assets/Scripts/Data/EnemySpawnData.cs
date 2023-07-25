using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Data
{
    [FirestoreData]
    public class EnemySpawnData : ScriptableObject
    {
        [FirestoreProperty]
        public List<EnemyData> Enemies { get; set; }

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