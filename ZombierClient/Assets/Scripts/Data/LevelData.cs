using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Level Data", menuName = "Data/Level Data")]
    public class LevelData : ScriptableObject
    {
        public LevelModel LevelPrefab;
        public EnemySpawnData EnemySpawnData;
    }
}
