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
        public UsersDbService(AppData appData, AuthenticationService authService)
        {
            _db = FirebaseFirestore.DefaultInstance;
            _appData = appData;
            _authService = authService;
        }

        public void SaveUser(UserData userData)
        {
            DocumentReference docRef = _db.Collection(USERS_COLLECTION_NAME).Document(_authService.CurrentUser.UserId.ToString());
            docRef.SetAsync(userData).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Added data to database.");
                }
                else
                {
                    Debug.LogError("Failed to add data to database: " + task.Exception);
                }
            });
        }

        public async Task LoadUserAsync()
        {
            DocumentReference docRef = _db.Collection(USERS_COLLECTION_NAME).Document(_authService.CurrentUser.UserId.ToString());
            await docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                DocumentSnapshot snapshot = task.Result;
                UserData data = snapshot.ConvertTo<UserData>();
                if (data == null)
                {
                    data = new UserData();
                    SaveUser(data);
                }
                _appData.User = data;
                _appData.User.IsInitComplete = true;
                Debug.Log($"User data: {JsonConvert.SerializeObject(data)}");
            });
        }

        private const string USERS_COLLECTION_NAME = "Users";
        private FirebaseFirestore _db;
        private AuthenticationService _authService;
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
