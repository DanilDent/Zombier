using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Location Data", menuName = "Data/Location Data")]
    public class LocationData : ScriptableObject
    {
        public LevelData[] Levels;
        public string LocationLevelPrefabAddress;
        // Visuals
        public string GroundPrefabAddress;
        public string WallPrefabsLabel;
        public string ObstaclePrefabsLabel;
        public string ExitPrefabAddress;
        public string EnvGroundPrefabAddress;
        public string EnvObstaclePrefabsLabel;
    }
}
