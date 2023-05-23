﻿using Prototype.Data;
using Prototype.Extensions;
using Prototype.Service;
using System;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class AppController : MonoBehaviour
    {
        [Inject]
        public void Construct(AppEventService appEventService, AppData appData, SerializationService serializationService)
        {
            _appEventService = appEventService;
            _appData = appData;
            _serializationService = serializationService;
        }

        private void Start()
        {
            Application.targetFrameRate = 60;

            _appEventService.OnLoadScene(new LoadSceneEventArgs { To = Scene.MainMenu });

            // Events
            _appEventService.GamePause += HandleGamePause;
            _appEventService.GameUnpause += HandleGameUnpause;
            _appEventService.Play += HandlePlay;
            _appEventService.PlayerPassedLevel += HandlePlayerPassedLevel;
            _appEventService.ResumeGameSession += HandleResumeGameSession;
            _appEventService.DontResumeGameSession += HandleDontResumeGameSession;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                string json = _serializationService.SerializeAppData(_appData);
                Debug.Log("App data saved.");
                Debug.Log($"{json}");
            }
        }

        private void OnDestroy()
        {
            _appEventService.GamePause -= HandleGamePause;
            _appEventService.GameUnpause -= HandleGameUnpause;
            _appEventService.Play -= HandlePlay;
            _appEventService.PlayerPassedLevel -= HandlePlayerPassedLevel;
            _appEventService.ResumeGameSession -= HandleResumeGameSession;
            _appEventService.DontResumeGameSession -= HandleDontResumeGameSession;
        }

        private void OnApplicationPause()
        {
            _serializationService.SerializeAppData(_appData);
            Debug.Log($"Application paused, app data saved to {_serializationService.PersistentAppDataPath}");
        }

        private void OnApplicationQuit()
        {
            _serializationService.SerializeAppData(_appData);
            Debug.Log($"Application quitting, app data saved to {_serializationService.PersistentAppDataPath}");
        }

        // Private

        // Injected
        private AppEventService _appEventService;
        private AppData _appData;
        private SerializationService _serializationService;

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
            GameSessionData newGameSession = _appData.Meta.DefaultSession.Copy();
            newGameSession.Location = e.LocationData.Copy();
            _appData.User.GameSession = newGameSession.Copy();
            _appEventService.OnLoadScene(new LoadSceneEventArgs { To = Scene.Game });
        }

        private void HandleResumeGameSession(object sender, EventArgs e)
        {
            _appEventService.OnLoadScene(new LoadSceneEventArgs { To = Scene.Game });
        }

        private void HandleDontResumeGameSession(object sender, EventArgs e)
        {
            _appData.User.GameSession = null;
        }

        private void HandlePlayerPassedLevel(object sender, PlayerPassedLevelEventArgs e)
        {
            _appData.User.GameSession = e.GameSession.Copy();
        }


    }
}
