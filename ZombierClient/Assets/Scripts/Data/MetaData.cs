using Prototype.LevelGeneration;
using Prototype.Model;
using Prototype.View;
using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Meta Data", menuName = "Data/Meta Data")]
    public class MetaData : ScriptableObject
    {
        public PlayerData Player;

        public LocationData Location;
        public LevelGeneratorData LevelGeneratorConfig;
        public EnemyModel EnemyPrefab;
        public EnemyProjectileModel EnemyProjectilePrefab;
        public HitBloodSplashVFXView HitVFXPrefab;
        public DeathBloodSplashVFXView DeathVFXPrefab;
        public DamageTextUIView DamageTextUIPrefab;
    }
}
