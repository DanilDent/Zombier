using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Meta Data", menuName = "Data/Meta Data")]
    public class MetaData : ScriptableObject
    {
        public PlayerData Player;
        public WeaponData[] PlayerWeapons;
        public LocationData[] Locations;
        public GameSessionData DefaultSession;
    }
}
