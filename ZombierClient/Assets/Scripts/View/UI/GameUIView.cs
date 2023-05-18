using Prototype.Service;
using System;
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

        // Injected
        private FloatingJoystick _joystick;
        private GameEventService _eventService;
        // From inspector
        [SerializeField] private Button _btnPause;

        private void OnEnable()
        {
            _eventService.PlayerDeath += HandlePlayerDeath;
            _eventService.PlayerRevive += HandlePlayerRevive;
            _eventService.GameUnpause += HandleGameUnpause;
            //
            _btnPause.onClick.AddListener(OnPause);
        }

        private void OnDisable()
        {
            _eventService.PlayerDeath -= HandlePlayerDeath;
            _eventService.PlayerRevive -= HandlePlayerRevive;
            _eventService.GameUnpause -= HandleGameUnpause;
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
    }
}
