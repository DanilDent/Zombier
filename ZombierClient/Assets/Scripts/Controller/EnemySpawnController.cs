using Prototype.Data;
using Prototype.Misc;
using Prototype.Model;
using Prototype.Service;
using Prototype.Timer;
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
            GameSessionData session,
            LevelModel level,
            EnemyModel.Factory enemyFactory,
            PlayerModel player,
            List<EnemyModel> enemies,
            GameEventService eventService,
            TimerService timerService,
            GameplaySessionConfigurator sessionConfigurator)
        {
            _session = session;
            _level = level;
            _enemyFactory = enemyFactory;
            _player = player;
            _enemies = enemies;
            _eventService = eventService;
            _timerService = timerService;
            _sessionConfigurator = sessionConfigurator;
        }

        public int EnemyCount => _enemies.Count;

        public void SpawnEnemies()
        {
            _countLeftToSpawn = Random.Range(_level.EnemySpawnData.MinEnemyCount, _level.EnemySpawnData.MaxEnemyCount + 1);

            while (_countLeftToSpawn > 0)
            {
                SpawnNextGroup();
            }
            Debug.Log($"Spawn complete");
        }

        // Private

        // Dependencies

        // Injected
        GameSessionData _session;
        GameEventService _eventService;
        private LevelModel _level;
        private EnemyModel.Factory _enemyFactory;
        private PlayerModel _player;
        [SerializeField] private List<EnemyModel> _enemies;
        private TimerService _timerService;
        private GameplaySessionConfigurator _sessionConfigurator;
        //
        // Minimal spawn enemy distance from player
        [SerializeField] private float _minDistanceFromPlayer = 10f;
        [SerializeField] private float _navMeshSampleRange = 10f;
        [SerializeField] private float _maxSampleDistance = 10f;
        // Groups config
        [SerializeField] private int _minGroupSize = 1;
        [SerializeField] private int _maxGroupSize = 20;
        [SerializeField] private float _groupSpawnRange = 10f;
        // Spawn enemies only outside this range from player
        [SerializeField] private float _allowedCenterPointRange;
        private List<EnemyModel> _enemiesToDestroy;
        // Internal variables
        NavMeshTriangulation _triangulation;
        private int _countLeftToSpawn;
        WeightedRandomSelector<EnemySpawnTypeData> _randomSelector;


        private void OnEnable()
        {
            _triangulation = NavMesh.CalculateTriangulation();
            _randomSelector = new WeightedRandomSelector<EnemySpawnTypeData>(_level.EnemySpawnData.Enemies);
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

        private void SpawnNextGroup()
        {
            const int MAX_ITERATIONS = 500;
            int iteration = 0;

            int groupSize = Random.Range(_minGroupSize, Mathf.Min(_maxGroupSize, _countLeftToSpawn) + 1);
            if (!GetRandomPointOnNavmeshTriangulationDistantFrom(_player.transform.position, _minDistanceFromPlayer, out Vector3 groupCenterPoint))
            {
                Debug.LogWarning($"Can't find center point for new spawn group");
                return;
            }
            int leftToSpawnInGroup = groupSize;
            while (leftToSpawnInGroup > 0 && iteration < MAX_ITERATIONS)
            {
                if (GetRandomPointOnNavmeshTriangulationDistantFrom(groupCenterPoint, _groupSpawnRange, out var newPosition))
                {
                    if (Vector3.Distance(newPosition, _player.transform.position) < _minDistanceFromPlayer)
                    {
                        continue;
                    }
                    EnemySpawnTypeData spawnTypeData = _randomSelector.GetRandomElement();
                    int enemyLevel = Random.Range(_level.EnemySpawnData.MinEnemyLevel, _level.EnemySpawnData.MaxEnemyLevel);
                    EnemyData enemyData = _sessionConfigurator.CreateEnemyData(spawnTypeData.EnemyId, enemyLevel);
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

                    --leftToSpawnInGroup;
                }
                else
                {
                    Debug.LogWarning("Can't find random position on navmesh surface");
                }

                ++iteration;
            }
            if (iteration >= MAX_ITERATIONS)
            {
                Debug.LogWarning($"Can't find a place to spawn next group of enemies");
            }
            _countLeftToSpawn -= groupSize;
        }

        private bool GetRandomPointOnNavmeshDistantFromPlayerRandomSphere(out Vector3 result)
        {
            Vector3 sampleCenter = _level.ExitPoint.transform.position + Random.insideUnitSphere * _allowedCenterPointRange;
            Vector3 point;
            if (GetRandomPointOnNavmeshRandomSpehere(sampleCenter, _navMeshSampleRange, out point))
            {
                result = point;
                return true;
            }
            result = Vector3.zero;
            return false;
        }

        private bool GetRandomPointOnNavmeshRandomSpehere(Vector3 center, float range, out Vector3 result)
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

        private bool GetRandomPointOnNavmeshTriangulationDistantFrom(Vector3 distantFrom, float range, out Vector3 result)
        {
            int maxIterations = 100;
            for (int i = 0; i < maxIterations; ++i)
            {
                Vector3 point;
                if (GetRandomPointOnNavmeshTriangulation(out point) && Vector3.Distance(distantFrom, point) > range)
                {
                    result = point;
                    return true;
                }
            }

            result = Vector3.zero;
            return false;
        }

        private bool GetRandomPointOnNavmeshTriangulation(out Vector3 result)
        {
            int maxIterations = 100;
            for (int i = 0; i < maxIterations; i++)
            {
                int vertexIndex = Random.Range(0, _triangulation.vertices.Length);

                NavMeshHit hit;
                if (NavMesh.SamplePosition(_triangulation.vertices[vertexIndex], out hit, _maxSampleDistance, NavMesh.AllAreas))
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
                cast.GetComponent<Collider>().enabled = false;
                _timerService.RemoveTimersWithTarget(cast);
                _enemiesToDestroy.Add(cast);

                if (_enemies.Count == 0)
                {
                    _eventService.OnLevelCleared(new GameEventService.LevelClearedEventArgs
                    {
                        IsLastLevel = _session.CurrentLevelIndex == _session.Location.Levels.Length - 1
                    });
                }
            }
        }

        private void HandleEnemyDeathInstant(object sender, GameEventService.EnemyDeathEventArgs e)
        {
            if (e.Entity is EnemyModel cast && cast != null)
            {
                _enemies.Remove(cast);
                if (cast.Agent != null)
                {
                    cast.Agent.enabled = false;
                }
                if (cast.GetComponent<Collider>() != null)
                {
                    cast.GetComponent<Collider>().enabled = false;
                }
                _timerService.RemoveTimersWithTarget(cast);
                _eventService.OnEnemyPreDestroyed();
                Destroy(cast.transform.parent.gameObject, 0.1f);

                if (_enemies.Count == 0)
                {
                    _eventService.OnLevelCleared(new GameEventService.LevelClearedEventArgs
                    {
                        IsLastLevel = _session.CurrentLevelIndex == _session.Location.Levels.Length - 1
                    });
                }
            }
        }

        private void HandleEnemyDeathAnimationEvent(object sender, GameEventService.EnemyDeathAnimationEventArgs e)
        {
            EnemyModel toDestroy = _enemiesToDestroy.FirstOrDefault(_ => _.Id == e.EntityId);
            float destroyDelay = 1f;
            _eventService.OnEnemyPreDestroyed();
            Destroy(toDestroy.transform.parent.gameObject, destroyDelay);
        }

        // Debug 
        private void DebugDrawSpawnPosition()
        {
            if (GetRandomPointOnNavmeshDistantFromPlayerRandomSphere(out var point))
            {
                Debug.DrawRay(point, Vector3.up, Color.red, 1.0f);
            }
        }
    }
}