using Prototype.Service;
using UnityEngine;
using Zenject;

namespace Prototype.Model
{
    public class ExitModel : MonoBehaviour
    {
        [Inject]
        public void Construct(GameEventService eventService)
        {
            _eventService = eventService;
        }

        private GameEventService _eventService;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<PlayerModel>(out var player))
            {
                _eventService.OnPlayerEnteredExit();
            }
        }
    }
}
