using Prototype.Model;
using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Gameplay Session Data", menuName = "Data/Gameplay Session Data")]
    public class GameplaySessionData : ScriptableObject
    {
        public PlayerData Player;

        public int CurrentLevelIndex;
        public LocationData Location;
        public EnemyModel EnemyPrefab;
    }
}
