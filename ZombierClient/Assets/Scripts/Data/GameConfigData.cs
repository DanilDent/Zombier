using Prototype.LevelGeneration;
using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Game Config Data", menuName = "Data/Game Config Data")]
    public class GameConfigData : ScriptableObject
    {
        public int ProjectilesPoolSize = 10;
        public LevelGeneratorData LevelGeneratorConfig;
        public string EnemyPrefabAssetPath;
        public string EnemyProjectilePrefabAssetPath;
        public string HitVFXPrefabAssetPath;
        public string DeathVFXPrefabAssetPath;
        public string DamageTextUIPrefabAssetPath;
        public string ExpTextUIPrefabAssetPath;
    }
}
