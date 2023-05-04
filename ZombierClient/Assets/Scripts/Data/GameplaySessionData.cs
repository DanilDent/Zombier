using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Gameplay Session Data", menuName = "Data/Gameplay Session Data")]
    public class GameplaySessionData : ScriptableObject
    {
        public int CurrentLevelIndex;

        public PlayerData Player;
        public LocationData Location;
    }
}
