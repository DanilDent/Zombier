using Prototype.Model;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class GameController : MonoBehaviour
    {
        private LevelModel _level;
        private PlayerModel _player;
        private EnemySpawnController _enemySpawnController;

        [Inject]
        public void Construct(LevelModel level, PlayerModel player, EnemySpawnController positionEnemyController)
        {
            _level = level;
            _player = player;
            _enemySpawnController = positionEnemyController;
        }

        private void Start()
        {
            PositionPlayer();
            _enemySpawnController.SpawnEnemies();
        }

        private void PositionPlayer()
        {
            _player.transform.position = _level.PlayerSpawnPoint.transform.position;
        }
    }

}
