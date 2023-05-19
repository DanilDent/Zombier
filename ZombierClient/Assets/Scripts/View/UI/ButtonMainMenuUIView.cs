using Prototype.Service;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Prototype.View
{
    public class ButtonMainMenuUIView : MonoBehaviour
    {
        [Inject]
        public void Construct(GameEventService eventService)
        {
            _eventService = eventService;
        }

        // Injected
        private GameEventService _eventService;
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
            _eventService.OnGameUnpause();
            SceneLoaderService.Load(SceneLoaderService.Scene.MainMenu);
        }
    }
}
