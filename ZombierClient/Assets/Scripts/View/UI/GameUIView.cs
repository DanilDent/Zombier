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
        public void Construct(FloatingJoystick joystick, GameEventService eventService)
        {
            _joystick = joystick;
            _eventService = eventService;
        }

        private const string CURRENT_LEVEL_TEXT = "Current level: ";
        // Injected
        private FloatingJoystick _joystick;
        private GameEventService _eventService;
        // From inspector
        [SerializeField] private Button _btnPause;
        [SerializeField] private TextMeshProUGUI _textCurrentLevel;

        private void OnEnable()
        {
            _eventService.PlayerDeath += HandlePlayerDeath;
            _eventService.PlayerRevive += HandlePlayerRevive;
            _eventService.GameUnpause += HandleGameUnpause;
            _eventService.CurrentLevelChanged += HandleCurrentLevelChanged;
            //
            _btnPause.onClick.AddListener(OnPause);
        }

        private void OnDisable()
        {
            _eventService.PlayerDeath -= HandlePlayerDeath;
            _eventService.PlayerRevive -= HandlePlayerRevive;
            _eventService.GameUnpause -= HandleGameUnpause;
            _eventService.CurrentLevelChanged -= HandleCurrentLevelChanged;
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
            _eventService.OnGamePause();
            _eventService.OnShowSettings();
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
