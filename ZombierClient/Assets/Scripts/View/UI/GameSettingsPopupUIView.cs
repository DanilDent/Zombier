using Prototype.Service;
using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Prototype.View
{
    public class GameSettingsPopupUIView : MonoBehaviour
    {
        [Inject]
        public void Construct(GameEventService eventService)
        {
            _eventService = eventService;
        }

        //
        [SerializeField] private RectTransform _viewRoot;
        [SerializeField] private Button _btnClose;

        // Injected
        private GameEventService _eventService;

        private void OnEnable()
        {
            _eventService.ShowSettings += HandleShowSettings;
            //
            _btnClose.onClick.AddListener(OnClose);
        }

        private void OnDisable()
        {
            _eventService.ShowSettings -= HandleShowSettings;
            //
            _btnClose.onClick.RemoveAllListeners();
        }

        private void OnClose()
        {
            _viewRoot.gameObject.SetActive(false);
            _eventService.OnGameUnpause();
        }

        private void HandleShowSettings(object sender, EventArgs e)
        {
            _viewRoot.gameObject.SetActive(true);
        }
    }
}
