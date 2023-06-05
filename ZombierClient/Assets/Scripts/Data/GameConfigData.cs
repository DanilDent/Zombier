using Prototype.LevelGeneration;
using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Game Config Data", menuName = "Data/Game Config Data")]
    public class GameConfigData : ScriptableObject
    {
        public int ProjectilesPoolSize = 10;
        public LevelGeneratorData LevelGeneratorConfig;
        public string EnemyContextAddress;
        public string EnemyProjectilePrefabAddress;
        public string HitVFXPrefabAddress;
        public string DeathVFXPrefabAddress;
        public string DamageTextUIPrefabAddress;
        public string ExpTextUIPrefabAddress;
    }
}
