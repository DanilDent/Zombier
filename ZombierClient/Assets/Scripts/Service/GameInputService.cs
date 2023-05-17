using UnityEngine;
using Zenject;

namespace Prototype.Service
{
    public class GameInputService
    {
        private FloatingJoystick _joystick;

        [Inject]
        public GameInputService(FloatingJoystick joystick)
        {
            _joystick = joystick;
        }

        public Vector2 Direction => _joystick.Direction;
    }
}
