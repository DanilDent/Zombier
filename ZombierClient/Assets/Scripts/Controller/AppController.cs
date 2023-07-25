using Prototype.Data;
using Prototype.Extensions;
using Prototype.Service;
using System;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    // TODO: Move all game data saving logic inside a separate 
    // SaveAppDataController class
    public class AppController : MonoBehaviour
    {
        [Inject]
        public void Construct(
            AppEventService appEventService,
            AppData appData,
            SerializationService serializationService,
            GameplaySessionConfigurator sessionConfigurator,
            UsersDbService usersDb)
        {
            _appEventService = appEventService;
            _appData = appData;
            _serializationService = serializationService;
            _sessionConfigurator = sessionConfigurator;
            _usersDb = usersDb;
        }

        // Private

        // Injected
        private AppEventService _appEventService;
        private AppData _appData;
        private SerializationService _serializationService;
        private GameplaySessionConfigurator _sessionConfigurator;
        private UsersDbService _usersDb;

        private void Start()
        {
            Application.targetFrameRate = 60;

            _appEventService.OnLoadScene(new LoadSceneEventArgs { To = Scene.MainMenu });

            // Events
            _appEventService.GamePause += HandleGamePause;
            _appEventService.GameUnpause += HandleGameUnpause;
            _appEventService.Play += HandlePlay;
            _appEventService.SaveGameSession += HandleSaveGameSession;
            _appEventService.ResumeGameSession += HandleResumeGameSession;
            _appEventService.ResetGameSession += HandleResetGameSession;
        }

        private async void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                string json = _serializationService.SerializeAndSaveUserData(_appData.User);
                Debug.Log($"App data saved to {_serializationService.PersistentAppDataPath}");
                Debug.Log($"{json}");
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                _usersDb.SaveUser(_appData.User);
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                await _usersDb.LoadUserAsync();
            }
        }

        private void OnDestroy()
        {
            _appEventService.GamePause -= HandleGamePause;
            _appEventService.GameUnpause -= HandleGameUnpause;
            _appEventService.Play -= HandlePlay;
            _appEventService.SaveGameSession -= HandleSaveGameSession;
            _appEventService.ResumeGameSession -= HandleResumeGameSession;
            _appEventService.ResetGameSession -= HandleResetGameSession;
        }

        private void OnApplicationPause()
        {
            //_serializationService.SerializeAndSaveUserData(_appData.User);
            //Debug.Log($"Application paused, app data saved to {_serializationService.PersistentAppDataPath}");

            if (_appData.User == null || !_appData.User.IsInitComplete)
            {
                return;
            }

            _usersDb.SaveUser(_appData.User);
            Debug.Log($"Application paused, user data saved to database");
        }

        private void OnApplicationQuit()
        {
            //_serializationService.SerializeAndSaveUserData(_appData.User);
            //Debug.Log($"Application quitting, app data saved to {_serializationService.PersistentAppDataPath}");

            _usersDb.SaveUser(_appData.User);
            Debug.Log($"Application quitting, user data saved to database");
        }

        private void HandleGamePause(object sender, EventArgs e)
        {
            Time.timeScale = 0f;
        }

        private void HandleGameUnpause(object sender, EventArgs e)
        {
            Time.timeScale = 1f;
        }

        private void HandlePlay(object sender, PlayEventArgs e)
        {
            var newSession = _sessionConfigurator.CreateGameSession(e.LocationId);
            _appData.User.GameSession = newSession.Copy();
            _usersDb.SaveUser(_appData.User);
            _appEventService.OnLoadScene(new LoadSceneEventArgs { To = Scene.Game });
        }

        private void HandleResumeGameSession(object sender, EventArgs e)
        {
            _appEventService.OnLoadScene(new LoadSceneEventArgs { To = Scene.Game });
            Debug.Log("Game session resume.");
        }

        private void HandleResetGameSession(object sender, EventArgs e)
        {
            _appData.User.GameSession = null;
            _usersDb.SaveUser(_appData.User);
            Debug.Log("Game session reset.");
        }

        private void HandleSaveGameSession(object sender, PlayerPassedLevelEventArgs e)
        {
            _appData.User.GameSession = e.GameSession.Copy();
            //_serializationService.SerializeAndSaveUserData(_appData.User);
            //Debug.Log($"App data saved to {_serializationService.PersistentAppDataPath}");
            _usersDb.SaveUser(_appData.User);
            Debug.Log($"User data saved to database");
        }
    }
}
