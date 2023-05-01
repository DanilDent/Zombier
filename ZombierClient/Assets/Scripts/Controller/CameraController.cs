using Cinemachine;
using Prototype.Model;
using Zenject;

namespace Prototype.Controller
{
    public class CameraController : IInitializable
    {
        private CinemachineVirtualCamera _virtualCamera;
        private PlayerModel _player;

        public CameraController(CinemachineVirtualCamera virtualCamera, PlayerModel player)
        {
            _virtualCamera = virtualCamera;
            _player = player;
        }

        public void Initialize()
        {
            _virtualCamera.Follow = _player.transform;
            _virtualCamera.LookAt = _player.transform;
        }
    }
}
