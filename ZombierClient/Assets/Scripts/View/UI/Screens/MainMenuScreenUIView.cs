using Prototype.Service;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Prototype.View
{
    public class MainMenuScreenUIView : ScreenUIViewBase
    {

        [Inject]
        public void Construct(AppEventService appEventService)
        {
            _appEventService = appEventService;
        }

        // Injected
        private AppEventService _appEventService;
        // From inspector
        [SerializeField] private Button _playButton;

        private void OnEnable()
        {
            _playButton.onClick.AddListener(OnPlay);
        }

        private void OnPlay()
        {
            _appEventService.OnLoadScene(new LoadSceneEventArgs { To = Scene.Game });
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveAllListeners();
        }
    }
}
