using Firebase.Firestore;

namespace Prototype.Data
{
    [FirestoreData]
    public class ProjectileData
    {
        [FirestoreProperty]
        public string PrefabAddress { get; set; }
    }
}
