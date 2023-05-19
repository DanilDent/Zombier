using Prototype.Model;
using Prototype.Service;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class PlayerLevelUpController : MonoBehaviour
    {
        // Public

        [Inject]
        public void Construct(GameEventService eventService, PlayerModel player)
        {
            _eventService = eventService;
            _player = player;
        }

        // Private

        // Injected
        private GameEventService _eventService;
        private PlayerModel _player;

        private void Start()
        {
            _eventService.OnPlayerCurrentExpChanged(new GameEventService.PlayerCurrentExpChangedEventArgs
            {
                CurrentExp = _player.CurrentExp,
                MaxExp = _player.CurrentLevelExpThreshold
            });

            _eventService.OnPlayerLevelChanged(new GameEventService.PlayerLevelChangedEventArgs
            {
                Level = _player.CurrentLevel + 1,
                SavedLevelUps = _player.SavedLevelUpCounter
            });
        }

        private void OnEnable()
        {
            _eventService.EnemyDeath += HandleEnemyDeath;
            _eventService.EnemyDeathInstant += HandleEnemyDeath;
        }

        private void OnDisable()
        {
            _eventService.EnemyDeath -= HandleEnemyDeath;
            _eventService.EnemyDeathInstant -= HandleEnemyDeath;
        }

        private void HandleEnemyDeath(object sender, GameEventService.EnemyDeathEventArgs e)
        {
            _player.CurrentExp += e.Entity.ExpReward;
            Debug.Log($"Player current exp: {_player.CurrentExp}");
            if (_player.CurrentExp >= _player.CurrentLevelExpThreshold)
            {
                _player.CurrentExp -= _player.CurrentLevelExpThreshold;
                _player.CurrentLevel++;
                _player.SavedLevelUpCounter++;
                Debug.Log($"Player Level Up: {_player.CurrentLevel}");
                _eventService.OnPlayerLevelChanged(new GameEventService.PlayerLevelChangedEventArgs
                {
                    Level = _player.CurrentLevel + 1,
                    SavedLevelUps = _player.SavedLevelUpCounter
                });
            }

            _eventService.OnPlayerCurrentExpChanged(new GameEventService.PlayerCurrentExpChangedEventArgs
            {
                CurrentExp = _player.CurrentExp,
                MaxExp = _player.CurrentLevelExpThreshold
            });
        }
    }
}
