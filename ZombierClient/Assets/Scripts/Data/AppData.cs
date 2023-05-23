using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New App Data", menuName = "Data/App Data")]
    public class AppData : ScriptableObject
    {
        // Data
        public MetaData Meta;
        public UserData User;
    }
}
