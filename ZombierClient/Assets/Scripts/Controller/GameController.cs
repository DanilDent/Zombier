using Prototype.Data;
using Prototype.Model;
using Prototype.Service;
using System;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class GameController : MonoBehaviour
    {
        [Inject]
        public void Construct(
            GameplayEventService eventService,
            PlayerModel player,
            // Controllers
            DealDamageController dealDamageController,
            EnemyAttackController enemyAttackController,
            EnemyChaseController enemyChaseController,
            EnemyMovementController enemyMovementController,
            EnemySpawnController enemySpawnController,
            PlayerAimController playerAimController,
            PlayerMovementController playerMovementController,
            PlayerShootController playerShootController,
            SpawnDamageTextUIController spawnDamageTextUIController,
            VFXController vfxController)
        {
            _eventService = eventService;
            _player = player;
            // Controllers
            _dealDamageController = dealDamageController;
            _enemyAttackController = enemyAttackController;
            _enemyChaseController = enemyChaseController;
            _enemyMovementController = enemyMovementController;
            _enemySpawnController = enemySpawnController;
            _playerAimController = playerAimController;
            _playerMovementController = playerMovementController;
            _playerShootController = playerShootController;
            _spawnDamageTextUIController = spawnDamageTextUIController;
            _vfxController = vfxController;
        }

        private GameplayEventService _eventService;
        private PlayerModel _player;
        // Controllers
        private DealDamageController _dealDamageController;
        private EnemyAttackController _enemyAttackController;
        private EnemyChaseController _enemyChaseController;
        private EnemyMovementController _enemyMovementController;
        private EnemySpawnController _enemySpawnController;
        private PlayerAimController _playerAimController;
        private PlayerMovementController _playerMovementController;
        private PlayerShootController _playerShootController;
        private SpawnDamageTextUIController _spawnDamageTextUIController;
        private VFXController _vfxController;

        private void OnEnable()
        {
            _eventService.PlayerDeath += HandlePlayerDeath;
            _eventService.PlayerRevive += HandlePlayerRevive;
            _eventService.Reset += HandleReset;
        }

        private void Start()
        {
            Application.targetFrameRate = 60;

            InitGame();

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

        private void InitGame()
        {
            // Enemy spawn controller should be true before any other enemy related controllers
            _enemySpawnController.gameObject.SetActive(true);
            _enemySpawnController.SpawnEnemies();
            //
            _dealDamageController.gameObject.SetActive(true);
            _enemyAttackController.gameObject.SetActive(true);
            _enemyChaseController.gameObject.SetActive(true);
            _enemyMovementController.gameObject.SetActive(true);
            _playerAimController.gameObject.SetActive(true);
            _playerMovementController.gameObject.SetActive(true);
            _playerShootController.gameObject.SetActive(true);
            _spawnDamageTextUIController.gameObject.SetActive(true);
            _vfxController.gameObject.SetActive(true);
        }

        private void SetControllersState(bool enabled)
        {
            _enemySpawnController.gameObject.SetActive(enabled);
            _dealDamageController.gameObject.SetActive(enabled);
            _enemyAttackController.gameObject.SetActive(enabled);
            _enemyChaseController.gameObject.SetActive(enabled);
            _enemyMovementController.gameObject.SetActive(enabled);
            _playerAimController.gameObject.SetActive(enabled);
            _playerMovementController.gameObject.SetActive(enabled);
            _playerShootController.gameObject.SetActive(enabled);
            _spawnDamageTextUIController.gameObject.SetActive(enabled);
            _vfxController.gameObject.SetActive(enabled);
        }

        private void HandlePlayerDeath(object sender, EventArgs e)
        {
            SetControllersState(false);
        }

        private void HandlePlayerRevive(object sender, EventArgs e)
        {
            _player.Health = _player.MaxHealth;
            _eventService.OnDamaged(new GameplayEventService.DamagedEventArgs
            {
                DamagedEntity = _player,
                DamageValue = 0f,
                EntityId = IdData.Empty,
                IsCrit = false
            });
            SetControllersState(true);
        }

        private void HandleReset(object sender, EventArgs e)
        {
            SceneLoaderService.Load(SceneLoaderService.Scene.Game);
        }

        private void OnDisable()
        {
            _eventService.PlayerDeath -= HandlePlayerDeath;
            _eventService.PlayerRevive -= HandlePlayerRevive;
            _eventService.Reset -= HandleReset;
        }
    }
}
