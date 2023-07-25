using Firebase.Firestore;

namespace Prototype.Data
{
    [FirestoreData]
    public class LevelData
    {
        [FirestoreProperty]
        public EnemySpawnData EnemySpawnData { get; set; }

        [FirestoreProperty]
        public int LevelSize { get; set; }
    }
}
