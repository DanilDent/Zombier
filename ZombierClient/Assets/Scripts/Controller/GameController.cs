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
        // WIP: Adding all scene transitions, refactoring scene loader service so it now non static class 
        // which loads scenes by events
        [Inject]
        public void Construct(
            GameSessionData session,
            GameEventService eventService,
            AppEventService appEventService,
            PlayerModel player,
            // Controllers
            DealDamageController dealDamageController,
            EnemyAIController enemyAIController,
            EnemySpawnController enemySpawnController,
            PlayerAimController playerAimController,
            PlayerMovementController playerMovementController,
            PlayerShootController playerShootController,
            PlayerLevelUpController playerLevelUpController,
            SpawnWorldCanvasUIText spawnDamageTextUIController,
            VFXController vfxController)
        {
            _session = session;
            _gameEventService = eventService;
            _appEventService = appEventService;
            _player = player;
            // Controllers
            _dealDamageController = dealDamageController;
            _enemyAIController = enemyAIController;
            _enemySpawnController = enemySpawnController;
            _playerAimController = playerAimController;
            _playerMovementController = playerMovementController;
            _playerShootController = playerShootController;
            _playerLevelUpController = playerLevelUpController;
            _spawnDamageTextUIController = spawnDamageTextUIController;
            _vfxController = vfxController;
        }

        private GameSessionData _session;
        private GameEventService _gameEventService;
        private AppEventService _appEventService;
        private PlayerModel _player;
        // Controllers
        private DealDamageController _dealDamageController;
        private EnemyAIController _enemyAIController;
        private EnemySpawnController _enemySpawnController;
        private PlayerAimController _playerAimController;
        private PlayerMovementController _playerMovementController;
        private PlayerShootController _playerShootController;
        private PlayerLevelUpController _playerLevelUpController;
        private SpawnWorldCanvasUIText _spawnDamageTextUIController;
        private VFXController _vfxController;

        private void OnEnable()
        {
            _gameEventService.PlayerDeath += HandlePlayerDeath;
            _gameEventService.PlayerRevive += HandlePlayerRevive;
            _gameEventService.Reset += HandleReset;
            _gameEventService.PlayerEnteredExit += HandlePlayerEnteredExit;
        }

        private void Start()
        {
            InitGame();
            FireInitUIEvents();
        }

        private void FireInitUIEvents()
        {
            _gameEventService.OnDamaged(new GameEventService.DamagedEventArgs
            {
                DamagedEntity = _player,
                DamageValue = 0f,
                EntityId = IdData.Empty,
                IsCrit = false
            });

            _gameEventService.OnCurrentLevelChanged(new GameEventService.CurrentLevelChangedEventArgs
            {
                Value = _session.CurrentLevelIndex + 1,
                MaxValue = _session.Location.Levels.Length
            });

            _gameEventService.OnPlayerCurrentExpChanged(new GameEventService.PlayerCurrentExpChangedEventArgs
            {
                CurrentExp = _player.CurrentExp,
                MaxExp = _player.CurrentLevelExpThreshold
            });
        }

        private void InitGame()
        {
            // Enemy spawn controller should be true before any other enemy related controllers
            _enemySpawnController.gameObject.SetActive(true);
            _enemySpawnController.SpawnEnemies();
            //
            _dealDamageController.gameObject.SetActive(true);
            _enemyAIController.gameObject.SetActive(true);
            _playerAimController.gameObject.SetActive(true);
            _playerMovementController.gameObject.SetActive(true);
            _playerShootController.gameObject.SetActive(true);
            _playerLevelUpController.gameObject.SetActive(true);
            _spawnDamageTextUIController.gameObject.SetActive(true);
            _vfxController.gameObject.SetActive(true);
        }

        private void SetControllersState(bool enabled)
        {
            _enemySpawnController.gameObject.SetActive(enabled);
            _dealDamageController.gameObject.SetActive(enabled);
            _enemyAIController.gameObject.SetActive(enabled);
            _playerAimController.gameObject.SetActive(enabled);
            _playerMovementController.gameObject.SetActive(enabled);
            _playerShootController.gameObject.SetActive(enabled);
            _playerLevelUpController.gameObject.SetActive(enabled);
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
            _gameEventService.OnDamaged(new GameEventService.DamagedEventArgs
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
            _appEventService.OnLoadScene(new LoadSceneEventArgs { To = Scene.Game });
        }

        private void HandlePlayerEnteredExit(object sender, EventArgs e)
        {
            if (_enemySpawnController.EnemyCount == 0)
            {
                _session.CurrentLevelIndex++;
                if (_session.CurrentLevelIndex < _session.Location.Levels.Length)
                {
                    _appEventService.OnSaveGameSession(new PlayerPassedLevelEventArgs { GameSession = _session });
                    _appEventService.OnLoadScene(new LoadSceneEventArgs { To = Scene.Game });
                }
                else
                {
                    _appEventService.OnResetGameSession();
                    _appEventService.OnLoadScene(new LoadSceneEventArgs { To = Scene.Results });
                }
            }
        }

        private void OnDisable()
        {
            _gameEventService.PlayerDeath -= HandlePlayerDeath;
            _gameEventService.PlayerRevive -= HandlePlayerRevive;
            _gameEventService.Reset -= HandleReset;
            _gameEventService.PlayerEnteredExit -= HandlePlayerEnteredExit;
        }
    }
}
