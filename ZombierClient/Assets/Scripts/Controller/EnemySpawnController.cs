using Prototype.Data;
using Prototype.Model;
using Prototype.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

using Random = UnityEngine.Random;

namespace Prototype.Controller
{
    public class EnemySpawnController : MonoBehaviour
    {
        // Public

        [Inject]
        public void Construct(
            LevelModel level,
            EnemyModel.Factory enemyFactory,
            PlayerModel player,
            List<EnemyModel> enemies,
            GameEventService eventService)
        {
            _level = level;
            _enemyFactory = enemyFactory;
            _player = player;
            _enemies = enemies;
            _eventService = eventService;
        }

        public void SpawnEnemies()
        {
            int countLeftToSpawn = Random.Range(_level.EnemySpawnData.MinEnemyCount, _level.EnemySpawnData.MaxEnemyCount + 1);

            while (countLeftToSpawn > 0)
            {
                if (GetRandomPointOnNavmeshDistantFromPlayer(out var newPosition))
                {
                    int randomIndex = Random.Range(0, _level.EnemySpawnData.Enemies.Count);
                    EnemyData enemyData = _level.EnemySpawnData.Enemies[randomIndex];
                    IdData id = IdProviderService.GetNewId();
                    EnemyModel enemy = _enemyFactory.Create(id, enemyData);
                    string idStr = id.ToString();
                    int nCharsToDisplay = 8;
                    enemy.gameObject.name = $"Enemy#{idStr.Substring(0, Math.Min(idStr.Length, nCharsToDisplay))}";

                    newPosition.y = 0f;
                    enemy.transform.position = newPosition;
                    enemy.transform.rotation = Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f);
                    enemy.Agent.Warp(newPosition);
                    enemy.Agent.SetDestination(enemy.transform.position);
                    enemy.Agent.enabled = true;
                    _enemies.Add(enemy);

                    --countLeftToSpawn;
                }
                else
                {
                    Debug.LogWarning("Can't find random position on navmesh surface");
                }
            }
        }

        // Private

        // Dependencies

        // Injected
        GameEventService _eventService;
        private LevelModel _level;
        private EnemyModel.Factory _enemyFactory;
        private PlayerModel _player;
        [SerializeField] private List<EnemyModel> _enemies;
        private List<EnemyModel> _enemiesToDestroy;
        //
        // Minimal spawn enemy distance from player
        [SerializeField] private float _minDistanceFromPlayer = 10f;
        [SerializeField] private float _navMeshSampleRange = 10f;
        [SerializeField] private float _maxSampleDistance = 10f;
        // Spawn enemies only outside this range from player
        [SerializeField] private float _allowedCenterPointRange;

        private void OnEnable()
        {
            // Events
            _eventService.EnemyDeath += HandleEnemyDeath;
            _eventService.EnemyDeathAnimationEvent += HandleEnemyDeathAnimationEvent;
            _eventService.EnemyDeathInstant += HandleEnemyDeathInstant;
            //
            float distancePlayerToExit = Vector3.Distance(_player.transform.position, _level.ExitPoint.transform.position);
            _allowedCenterPointRange = distancePlayerToExit - _minDistanceFromPlayer - _maxSampleDistance;
            _enemiesToDestroy = new List<EnemyModel>();
        }

        private void OnDisable()
        {
            _eventService.EnemyDeath -= HandleEnemyDeath;
            _eventService.EnemyDeathAnimationEvent -= HandleEnemyDeathAnimationEvent;
            _eventService.EnemyDeathInstant -= HandleEnemyDeathInstant;
        }

        private void Update()
        {
#if DEBUG
            //DebugDrawSpawnPosition();
#endif
        }

        private bool GetRandomPointOnNavmeshDistantFromPlayer(out Vector3 result)
        {
            Vector3 sampleCenter = _level.ExitPoint.transform.position + Random.insideUnitSphere * _allowedCenterPointRange;
            Vector3 point;
            if (GetRandomPointOnNavmesh(sampleCenter, _navMeshSampleRange, out point))
            {
                result = point;
                return true;
            }
            result = Vector3.zero;
            return false;
        }

        private bool GetRandomPointOnNavmesh(Vector3 center, float range, out Vector3 result)
        {
            int maxIterations = 100;
            for (int i = 0; i < maxIterations; i++)
            {
                Vector3 randomPoint = center + Random.insideUnitSphere * range;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, _maxSampleDistance, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }
            result = Vector3.zero;
            return false;
        }

        private void HandleEnemyDeath(object sender, GameEventService.EnemyDeathEventArgs e)
        {
            if (e.Entity is EnemyModel cast)
            {
                _enemies.Remove(cast);
                cast.Agent.enabled = false;
                cast.Rigidbody.velocity = Vector3.zero;
                cast.Rigidbody.useGravity = false;
                cast.GetComponent<Collider>().enabled = false;
                _enemiesToDestroy.Add(cast);

                if (_enemies.Count == 0)
                {
                    _eventService.OnLevelCleared();
                }
            }
        }

        private void HandleEnemyDeathInstant(object sender, GameEventService.EnemyDeathEventArgs e)
        {
            if (e.Entity is EnemyModel cast)
            {
                _enemies.Remove(cast);
                cast.Agent.enabled = false;
                cast.Rigidbody.velocity = Vector3.zero;
                cast.Rigidbody.useGravity = false;
                cast.GetComponent<Collider>().enabled = false;
                _eventService.OnEnemyPreDestroyed();
                Destroy(cast.gameObject, 0.1f);

                if (_enemies.Count == 0)
                {
                    _eventService.OnLevelCleared();
                }
            }
        }

        private void HandleEnemyDeathAnimationEvent(object sender, GameEventService.EnemyDeathAnimationEventArgs e)
        {
            EnemyModel toDestroy = _enemiesToDestroy.FirstOrDefault(_ => _.Id == e.EntityId);
            float destroyDelay = 1f;
            _eventService.OnEnemyPreDestroyed();
            Destroy(toDestroy.gameObject, destroyDelay);
        }

        // Debug 
        private void DebugDrawSpawnPosition()
        {
            if (GetRandomPointOnNavmeshDistantFromPlayer(out var point))
            {
                Debug.DrawRay(point, Vector3.up, Color.red, 1.0f);
            }
        }
    }
}