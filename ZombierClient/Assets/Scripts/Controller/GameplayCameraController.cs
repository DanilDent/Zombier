using Prototype.Model;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class GameplayCameraController : MonoBehaviour
    {
        private PlayerModel _player;

        [SerializeField] private Vector3 offset = new Vector3(0.0f, 13.5f, -13.5f);

        [Inject]
        public void Construct(PlayerModel player)
        {
            _player = player;
        }

        private void LateUpdate()
        {
            transform.position = _player.transform.position + offset;
        }
    }
}
