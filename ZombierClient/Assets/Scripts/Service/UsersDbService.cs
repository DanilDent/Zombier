using Firebase.Extensions;
using Firebase.Firestore;
using Newtonsoft.Json;
using Prototype.Data;
using System.Threading.Tasks;
using UnityEngine;

namespace Prototype.Service
{
    public class UsersDbService
    {
        public UsersDbService(AppData appData)
        {
            _db = FirebaseFirestore.DefaultInstance;
            _appData = appData;
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

        public async Task LoadUserAsync()
        {
            DocumentReference docRef = _db.Collection("Users").Document("TestUser");
            await docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                DocumentSnapshot snapshot = task.Result;
                UserData data = snapshot.ConvertTo<UserData>();
                _appData.User = data;
                _appData.User.IsInitComplete = true;
                Debug.Log($"User data: {JsonConvert.SerializeObject(data)}");

                Debug.Log("Read all data from the users collection.");
            });
        }

        private FirebaseFirestore _db;
        private AppData _appData;

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
