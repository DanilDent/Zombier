using UnityEngine;
using Zenject;

namespace Prototype.Service
{
    public class GameplayInputService
    {
        private FloatingJoystick _joystick;

        [Inject]
        public GameplayInputService(FloatingJoystick joystick)
        {
            _joystick = joystick;
        }

        public Vector2 Direction => _joystick.Direction;
    }
}
