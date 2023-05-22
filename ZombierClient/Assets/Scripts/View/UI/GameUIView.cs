using Prototype.Service;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Prototype.View
{
    public class GameUIView : MonoBehaviour
    {
        [Inject]
        public void Construct(FloatingJoystick joystick, GameEventService eventService, AppEventService appEventService)
        {
            _joystick = joystick;
            _gameEventService = eventService;
            _appEventService = appEventService;
        }

        private const string CURRENT_LEVEL_TEXT = "Current level: ";
        // Injected
        private FloatingJoystick _joystick;
        private GameEventService _gameEventService;
        private AppEventService _appEventService;
        // From inspector
        [SerializeField] private Button _btnPause;
        [SerializeField] private TextMeshProUGUI _textCurrentLevel;

        private void OnEnable()
        {
            _gameEventService.PlayerDeath += HandlePlayerDeath;
            _gameEventService.PlayerRevive += HandlePlayerRevive;
            _appEventService.GameUnpause += HandleGameUnpause;
            _gameEventService.CurrentLevelChanged += HandleCurrentLevelChanged;
            //
            _btnPause.onClick.AddListener(OnPause);
        }

        private void OnDisable()
        {
            _gameEventService.PlayerDeath -= HandlePlayerDeath;
            _gameEventService.PlayerRevive -= HandlePlayerRevive;
            _appEventService.GameUnpause -= HandleGameUnpause;
            _gameEventService.CurrentLevelChanged -= HandleCurrentLevelChanged;
            //
            _btnPause.onClick.RemoveAllListeners();
        }

        private void HandlePlayerDeath(object sender, EventArgs e)
        {
            _joystick.gameObject.SetActive(false);
        }

        private void HandlePlayerRevive(object sender, EventArgs e)
        {
            _joystick.gameObject.SetActive(true);
        }

        private void OnPause()
        {
            _joystick.gameObject.SetActive(false);
            _appEventService.OnGamePause();
            _gameEventService.OnShowSettings();
        }

        private void HandleGameUnpause(object sender, EventArgs e)
        {
            _joystick.gameObject.SetActive(true);
        }

        private void HandleCurrentLevelChanged(object sender, GameEventService.CurrentLevelChangedEventArgs e)
        {
            _textCurrentLevel.text = CURRENT_LEVEL_TEXT + e.Value + " / " + e.MaxValue;
        }
    }
}
