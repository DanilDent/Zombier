using Prototype.Model;
using Prototype.SO;
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
            List<EnemyModel> enemies)
        {
            _level = level;
            _enemyFactory = enemyFactory;
            _player = player;
            _enemies = enemies;
        }

        // Private

        // Dependencies

        // Injected
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

        private void Start()
        {
            float distancePlayerToExit = Vector3.Distance(_player.transform.position, _level.ExitPoint.transform.position);
            _allowedCenterPointRange = distancePlayerToExit - _minDistanceFromPlayer - _maxSampleDistance;

            SpawnEnemies();
        }

        private void Update()
        {
#if DEBUG
            DebugDrawSpawnPosition();
#endif
        }

        private void SpawnEnemies()
        {
            int countLeftToSpawn = Random.Range(_level.EnemySpawnSO.MinEnemyCount, _level.EnemySpawnSO.MaxEnemyCount + 1);

            while (countLeftToSpawn > 0)
            {
                if (GetRandomPointOnNavmeshDistantFromPlayer(out var newPosition))
                {
                    int randomIndex = Random.Range(0, _level.EnemySpawnSO.Enemies.Count);
                    EnemySO enemySO = _level.EnemySpawnSO.Enemies[randomIndex];
                    EnemyModel enemy = _enemyFactory.Create(enemySO);

                    newPosition.y = 0f;
                    enemy.transform.position = newPosition;
                    enemy.Agent.Warp(newPosition);
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