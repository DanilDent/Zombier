using Firebase.Extensions;
using Firebase.Firestore;
using Prototype.Data;
using UnityEngine;

namespace Prototype.Service
{
    public class UsersDbService
    {
        public UsersDbService()
        {
            _db = FirebaseFirestore.DefaultInstance;
        }

        public void SaveUser(UserData userData)
        {
            var city = new City { Name = "Saratov", State = "Saratov district", Country = "Russia", Capital = false, Population = 20003423 };
            DocumentReference docRef = _db.Collection("Users").Document("TestUser");
            docRef.SetAsync(userData).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Added data to the users document in the users collection.");
                }
                else
                {
                    Debug.LogError("Failed to add data to the aturing document: " + task.Exception);
                }
            });
        }

        private FirebaseFirestore _db;

        [FirestoreData]
        public class City
        {
            [FirestoreProperty]
            public string Name { get; set; }

            [FirestoreProperty]
            public string State { get; set; }

            [FirestoreProperty]
            public string Country { get; set; }

            [FirestoreProperty]
            public bool Capital { get; set; }

            [FirestoreProperty]
            public long Population { get; set; }
        }
    }
}
