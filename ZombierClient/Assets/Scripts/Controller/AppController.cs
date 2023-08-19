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
            GameplaySessionConfigurator sessionConfigurator,
            UsersDbService usersDb,
            AuthenticationService authService)
        {
            _appEventService = appEventService;
            _appData = appData;
            _sessionConfigurator = sessionConfigurator;
            _usersDb = usersDb;
            _authService = authService;
        }

        // Private

        // Injected
        private AppEventService _appEventService;
        private AppData _appData;
        private GameplaySessionConfigurator _sessionConfigurator;
        private UsersDbService _usersDb;
        private AuthenticationService _authService;

        private async void Start()
        {
            Application.targetFrameRate = 60;

            await _authService.SingInAnonymouslyAsync();

            _appEventService.OnLoadScene(new LoadSceneEventArgs { To = Scene.MainMenu });

            // Events
            _appEventService.GamePause += HandleGamePause;
            _appEventService.GameUnpause += HandleGameUnpause;
            _appEventService.Play += HandlePlay;
            _appEventService.SaveGameSession += HandleSaveGameSession;
            _appEventService.ResumeGameSession += HandleResumeGameSession;
            _appEventService.ResetGameSession += HandleResetGameSession;
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
            if (_appData.User == null || !_appData.User.IsInitComplete)
            {
                return;
            }

            _usersDb.SaveUser(_appData.User);
            Debug.Log($"Application paused, user data saved to database");
        }

        private void OnApplicationQuit()
        {
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
            _appData.User.GameSession = newSession;
            _usersDb.SaveUser(_appData.User);
            _appEventService.OnLoadScene(new LoadSceneEventArgs { To = Scene.Game });
        }

        private void HandleResumeGameSession(object sender, EventArgs e)
        {
            _appData.User.GameSession.Location = _sessionConfigurator.CreateLocationData(_appData.User.GameSession.LocationId);
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
            _usersDb.SaveUser(_appData.User);
            Debug.Log($"User data saved to database");
        }
    }
}
