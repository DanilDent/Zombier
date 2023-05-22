using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Location Data", menuName = "Data/Location Data")]
    public class LocationData : ScriptableObject
    {
        public LevelData[] Levels;
        public string LocationLevelPrefabAssetPath;
        // Visuals
        public string GroundPrefabAssetPath;
        public string WallPrefabAssetPath;
        public string ObstaclePrefabsAssetPath;
        public string ExitPrefabAssetPath;
        public string EnvGroundPrefabAssetPath;
        public string EnvObstaclePrefabsAssetPath;
    }
}
