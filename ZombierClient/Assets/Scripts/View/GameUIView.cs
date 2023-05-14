using Prototype.Service;
using System;
using UnityEngine;
using Zenject;

namespace Prototype.View
{
    public class GameUIView : MonoBehaviour
    {
        [Inject]
        public void Construct(FloatingJoystick joystick, GameplayEventService eventService)
        {
            _joystick = joystick;
            _eventService = eventService;
        }

        // Injected
        private FloatingJoystick _joystick;
        private GameplayEventService _eventService;

        private void OnEnable()
        {
            _eventService.PlayerDeath += HandlePlayerDeath;
            _eventService.PlayerRevive += HandlePlayerRevive;
        }

        private void OnDisable()
        {
            _eventService.PlayerDeath -= HandlePlayerDeath;
            _eventService.PlayerRevive -= HandlePlayerRevive;
        }

        private void HandlePlayerDeath(object sender, EventArgs e)
        {
            _joystick.gameObject.SetActive(false);
        }

        private void HandlePlayerRevive(object sender, EventArgs e)
        {
            _joystick.gameObject.SetActive(true);
        }
    }
}
