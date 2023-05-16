using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Location Data", menuName = "Data/Location Data")]
    public class LocationData : ScriptableObject
    {
        public LevelData[] Levels;
        public GameObject LocationLevelPrefab;
        // Visuals
        public GameObject GroundPrefab;
        public GameObject WallPrefab;
        public GameObject[] ObstaclePrefabs;
        public GameObject ExitPrefab;
        public GameObject EnvGroundPrefab;
        public GameObject[] EnvObstaclePrefabs;
    }
}
