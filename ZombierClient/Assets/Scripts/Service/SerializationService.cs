using Newtonsoft.Json;
using Prototype.Data;
using System.IO;
using UnityEngine;

namespace Prototype.Service
{
    public class SerializationService
    {
        private const string APP_DATA_PATH = "/APP_DATA.json";

        public string SerializeUserData(UserData userData)
        {
            string fullPath = Application.persistentDataPath + APP_DATA_PATH;
            string json = JsonConvert.SerializeObject(userData);
            File.WriteAllText(fullPath, json);
            return json;
        }

        public UserData DeserializeUserData()
        {
            string fullPath = Application.persistentDataPath + APP_DATA_PATH;
            string json = File.ReadAllText(fullPath);
            UserData userData = JsonConvert.DeserializeObject<UserData>(json);
            return userData;
        }

        public bool TryDeserializeUserData(out UserData userData)
        {
            string fullPath = Application.persistentDataPath + APP_DATA_PATH;

            if (!File.Exists(fullPath))
            {
                Debug.LogError("Cannot find app data.");
                userData = null;
                return false;
            }

            string json = File.ReadAllText(fullPath);
            userData = JsonConvert.DeserializeObject<UserData>(json);
            return true;
        }

        public bool AppDataFolderExists()
        {
            string fullPath = Application.persistentDataPath + APP_DATA_PATH;
            return File.Exists(fullPath);
        }

        public string PersistentAppDataPath => Application.persistentDataPath + APP_DATA_PATH;
    }
}
