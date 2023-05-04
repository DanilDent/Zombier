using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Location Data", menuName = "Data/Location Data")]
    public class LocationData : ScriptableObject
    {
        public LevelData[] Levels;
    }
}
