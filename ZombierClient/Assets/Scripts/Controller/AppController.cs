using Prototype.Service;
using System;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class AppController : IInitializable, IDisposable
    {
        public AppController(AppEventService appEventService)
        {
            _appEventService = appEventService;
        }

        // Injected
        private AppEventService _appEventService;

        public void Initialize()
        {
            Application.targetFrameRate = 60;

            _appEventService.OnLoadScene(new LoadSceneEventArgs { To = Scene.MainMenu });

            // Events
            _appEventService.GamePause += HandleGamePause;
            _appEventService.GameUnpause += HandleGameUnpause;
        }

        private void HandleGamePause(object sender, EventArgs e)
        {
            Time.timeScale = 0f;
        }

        private void HandleGameUnpause(object sender, EventArgs e)
        {
            Time.timeScale = 1f;
        }

        public void Dispose()
        {
            _appEventService.GamePause -= HandleGamePause;
            _appEventService.GameUnpause -= HandleGameUnpause;
        }
    }
}
