using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New User Data", menuName = "Data/User Data")]
    public class UserData : ScriptableObject
    {
        public GameSessionData GameSession;
    }
}
