using Firebase.Firestore;

namespace Prototype.Data
{
    [FirestoreData]
    public class UserData
    {
        [FirestoreProperty]
        public GameSessionData GameSession { get; set; }
    }
}
