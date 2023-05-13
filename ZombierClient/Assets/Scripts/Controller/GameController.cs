using Prototype.Data;
using Prototype.Model;
using Prototype.Service;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class GameController : MonoBehaviour
    {
        [Inject]
        public void Construct(GameplayEventService eventService, PlayerModel player)
        {
            _eventService = eventService;
            _player = player;
        }

        private void Start()
        {
            Application.targetFrameRate = 60;

            // Init player
            _player.Health = _player.MaxHealth;
            // Use this call to update player HealthBar
            _eventService.OnDamaged(new GameplayEventService.DamagedEventArgs
            {
                DamagedEntity = _player,
                DamageValue = 0f,
                EntityId = IdData.Empty,
                IsCrit = false
            });
        }

        private GameplayEventService _eventService;
        private PlayerModel _player;
    }
}
