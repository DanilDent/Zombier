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
        public void Construct(GameEventService gameEventService, AppEventService appEventService)
        {
            _gameEventService = gameEventService;
            _appEventService = appEventService;
        }

        //
        [SerializeField] private RectTransform _viewRoot;
        [SerializeField] private Button _btnClose;

        // Injected
        private GameEventService _gameEventService;
        private AppEventService _appEventService;

        private void OnEnable()
        {
            _gameEventService.ShowSettings += HandleShowSettings;
            //
            _btnClose.onClick.AddListener(OnClose);
        }

        private void OnDisable()
        {
            _gameEventService.ShowSettings -= HandleShowSettings;
            //
            _btnClose.onClick.RemoveAllListeners();
        }

        private void OnClose()
        {
            _viewRoot.gameObject.SetActive(false);
            _appEventService.OnGameUnpause();
        }

        private void HandleShowSettings(object sender, EventArgs e)
        {
            _viewRoot.gameObject.SetActive(true);
        }
    }
}
