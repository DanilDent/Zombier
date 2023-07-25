using Firebase.Firestore;

namespace Prototype.Data
{
    [FirestoreData]
    public class LocationData
    {
        [FirestoreProperty]
        public LevelData[] Levels { get; set; }

        [FirestoreProperty]
        public string LocationLevelPrefabAddress { get; set; }

        // Visuals
        [FirestoreProperty]
        public string GroundPrefabAddress { get; set; }

        [FirestoreProperty]
        public string WallPrefabsLabel { get; set; }

        [FirestoreProperty]
        public string ObstaclePrefabsLabel { get; set; }

        [FirestoreProperty]
        public string ExitPrefabAddress { get; set; }

        [FirestoreProperty]
        public string EnvGroundPrefabAddress { get; set; }

        [FirestoreProperty]
        public string EnvObstaclePrefabsLabel { get; set; }
    }
}
