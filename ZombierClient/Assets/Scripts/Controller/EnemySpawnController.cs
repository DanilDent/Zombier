using Prototype.Data;
using Prototype.Model;
using Prototype.Service;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

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
            GameplayEventService eventService)
        {
            _level = level;
            _enemyFactory = enemyFactory;
            _player = player;
            _enemies = enemies;
            _eventService = eventService;
        }

        // Private

        // Dependencies

        // Injected
        GameplayEventService _eventService;
        private LevelModel _level;
        private EnemyModel.Factory _enemyFactory;
        private PlayerModel _player;
        [SerializeField] private List<EnemyModel> _enemies;
        //

        // Minimal spawn enemy distance from player
        [SerializeField] private float _minDistanceFromPlayer = 10f;
        [SerializeField] private float _navMeshSampleRange = 10f;
        [SerializeField] private float _maxSampleDistance = 10f;
        // Spawn enemies only outside this range from player
        [SerializeField] private float _allowedCenterPointRange;

        private void Awake()
        {
            float distancePlayerToExit = Vector3.Distance(_player.transform.position, _level.ExitPoint.transform.position);
            _allowedCenterPointRange = distancePlayerToExit - _minDistanceFromPlayer - _maxSampleDistance;
        }

        private void OnEnable()
        {
            _eventService.Death += DespawnEnemy;

            SpawnEnemies();
        }

        private void OnDisable()
        {
            _eventService.Death -= DespawnEnemy;
        }

        private void Update()
        {
#if DEBUG
            DebugDrawSpawnPosition();
#endif
        }

        private void SpawnEnemies()
        {
            int countLeftToSpawn = Random.Range(_level.EnemySpawnData.MinEnemyCount, _level.EnemySpawnData.MaxEnemyCount + 1);

            while (countLeftToSpawn > 0)
            {
                if (GetRandomPointOnNavmeshDistantFromPlayer(out var newPosition))
                {
                    int randomIndex = Random.Range(0, _level.EnemySpawnData.Enemies.Count);
                    EnemyData enemyData = _level.EnemySpawnData.Enemies[randomIndex];
                    EnemyModel enemy = _enemyFactory.Create(enemyData);

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

        private void DespawnEnemy(object sender, GameplayEventService.DeathEventArgs e)
        {
            if (e.Entity is EnemyModel cast)
            {
                _enemies.Remove(cast);
                cast.Agent.enabled = false;
                float destroyDelay = 1f;
                Destroy(cast.gameObject, destroyDelay);
            }
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