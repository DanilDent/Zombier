using Prototype.Service;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Prototype.View
{
    public class ButtonMainMenuUIView : MonoBehaviour
    {
        [Inject]
        public void Construct(AppEventService appEventService)
        {
            _appEventService = appEventService;
        }

        // Injected
        private AppEventService _appEventService;
        // From inspector
        [SerializeField] private Button _button;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveAllListeners();
        }

        private void OnClick()
        {
            _appEventService.OnGameUnpause();
            _appEventService.OnLoadScene(new LoadSceneEventArgs { To = Scene.MainMenu });
        }
    }
}
