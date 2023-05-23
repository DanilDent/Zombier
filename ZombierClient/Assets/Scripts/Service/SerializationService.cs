using Newtonsoft.Json;
using Prototype.Data;
using System.IO;
using UnityEngine;

namespace Prototype.Service
{
    public class SerializationService
    {
        private const string APP_DATA_PATH = "/APP_DATA.json";

        public string SerializeAppData(AppData appData)
        {
            string fullPath = Application.persistentDataPath + APP_DATA_PATH;
            string json = JsonConvert.SerializeObject(appData);
            File.WriteAllText(fullPath, json);
            return json;
        }

        public AppData DeserializeAppData()
        {
            string fullPath = Application.persistentDataPath + APP_DATA_PATH;
            string json = File.ReadAllText(fullPath);
            AppData appData = JsonConvert.DeserializeObject<AppData>(json);
            return appData;
        }

        public bool TryDeserializeAppData(out AppData appData)
        {
            string fullPath = Application.persistentDataPath + APP_DATA_PATH;

            if (!File.Exists(fullPath))
            {
                Debug.LogError("Cannot find app data.");
                appData = null;
                return false;
            }

            string json = File.ReadAllText(fullPath);
            appData = JsonConvert.DeserializeObject<AppData>(json);
            return true;
        }

        public bool AppDataExists()
        {
            string fullPath = Application.persistentDataPath + APP_DATA_PATH;
            return File.Exists(fullPath);
        }

        public string PersistentAppDataPath => Application.persistentDataPath + APP_DATA_PATH;
    }
}
