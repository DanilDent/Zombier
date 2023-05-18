using Prototype.Model;
using Prototype.View;
using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Gameplay Session Data", menuName = "Data/Gameplay Session Data")]
    public class GameSessionData : ScriptableObject
    {
        public PlayerData Player;

        public int CurrentLevelIndex;
        public LocationData Location;
        public EnemyModel EnemyPrefab;
        public EnemyProjectileModel EnemyProjectilePrefab;
        public HitBloodSplashVFXView HitVFXPrefab;
        public DeathBloodSplashVFXView DeathVFXPrefab;
        public DamageTextUIView DamageTextUIPrefab;
    }
}
