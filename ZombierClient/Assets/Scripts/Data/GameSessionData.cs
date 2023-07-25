using Firebase.Firestore;

namespace Prototype.Data
{
    [FirestoreData]
    public class GameSessionData
    {
        [FirestoreProperty]
        public PlayerData Player { get; set; }

        [FirestoreProperty]
        public int CurrentLevelIndex { get; set; }

        [FirestoreProperty]
        public LocationData Location { get; set; }
    }
}
