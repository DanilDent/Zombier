using Newtonsoft.Json;
using Prototype.Data;
using System.IO;
using UnityEngine;
using Zenject;

namespace Prototype.Service
{
    public class SerializationService
    {
        [Inject]
        public SerializationService(AppEventService appEventService)
        {
            _appEventService = appEventService;
        }

        private const string GAME_SESSION_PATH = "\\Config\\DataBase\\GAME_SESSION.json";
        // Injected
        private AppEventService _appEventService;

        public static string SerializeGameSession(GameSessionData session)
        {
            string json = JsonConvert.SerializeObject(session);
            string fullPath = Application.dataPath + GAME_SESSION_PATH;
            File.WriteAllText(fullPath, json);
            return json;
        }

        public static GameSessionData DeserializeGameSession()
        {
            string fullPath = Application.dataPath + GAME_SESSION_PATH;
            string json = File.ReadAllText(fullPath);
            GameSessionData session = JsonConvert.DeserializeObject<GameSessionData>(json);
            return session;
        }
    }
}
