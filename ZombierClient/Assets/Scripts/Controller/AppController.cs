using Prototype.Service;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class AppController : IInitializable
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
        }
    }
}
