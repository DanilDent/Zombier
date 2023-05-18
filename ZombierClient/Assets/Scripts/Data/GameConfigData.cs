using Prototype.LevelGeneration;
using Prototype.Model;
using Prototype.View;
using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Game Config Data", menuName = "Data/Game Config Data")]
    public class GameConfigData : ScriptableObject
    {
        public int ProjectilesPoolSize = 10;
        public LevelGeneratorData LevelGeneratorConfig;
        public EnemyModel EnemyPrefab;
        public EnemyProjectileModel EnemyProjectilePrefab;
        public HitBloodSplashVFXView HitVFXPrefab;
        public DeathBloodSplashVFXView DeathVFXPrefab;
        public DamageTextUIView DamageTextUIPrefab;
    }
}
