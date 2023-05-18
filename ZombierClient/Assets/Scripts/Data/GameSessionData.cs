using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Gameplay Session Data", menuName = "Data/Gameplay Session Data")]
    public class GameSessionData : ScriptableObject
    {
        public PlayerData Player;

        public int CurrentLevelIndex;
        public LocationData Location;
    }
}
