using Prototype.Data;
using Prototype.Misc;
using Prototype.Service;
using Prototype.StaticBatch;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

using Object = UnityEngine.Object;

namespace Prototype.LevelGeneration
{
    public partial class LevelGenerator
    {
        public event EventHandler<LevelGeneratedEventArgs> LevelGenerated;

        public class LevelGeneratedEventArgs
        {
            public GameObject LevelGameObject;
            public GameObject ExitGameObject;
        }

        public LevelGenerator(
            LevelGeneratorData levelGeneratorData,
            LocationData locationData,
            LevelData levelData)
        {
            _levelGeneratorData = levelGeneratorData;
            _locationData = locationData;
            _levelData = levelData;

            _combiner = new StaticBatcher();
            LoadPrefabs();
            Init();
        }

        ~LevelGenerator()
        {
            _groundMap = null;
            _wallsMap = null;
            _obstaclesMap = null;
            _roomCount = 0;
        }

        public IEnumerator GenerateLevel()
        {
            float startTime = Time.realtimeSinceStartup;

            float genGroundStartTime = Time.realtimeSinceStartup;
            GenerateGround();
            float genGroundEndTime = Time.realtimeSinceStartup;

            GameObject exit = GenerateExit();

            float genWallsStartTime = Time.realtimeSinceStartup;
            GenerateWalls();
            float genWallsEndTime = Time.realtimeSinceStartup;

            float genObsStartTime = Time.realtimeSinceStartup;
            GameObject obstacles = GenerateObstacles(
                "Obstacles",
                _obstaclePrefabs,
                TileType.Obstacle,
                TileType.Ground,
                _minObstacleCount,
                _maxObstacleCount);
            float genObsEndTime = Time.realtimeSinceStartup;

            GameObject environment = new GameObject("Environment");

            float genEnvGroundStartTime = Time.realtimeSinceStartup;
            GenerateEnvGround();
            float genEnvGroundEndTime = Time.realtimeSinceStartup;

            float instGroundAndWallsStartTime = Time.realtimeSinceStartup;
            List<GameObject> groundAndWalls = new List<GameObject>();
            yield return CoroutineRunner.Instance.StartCoroutine(InstantiateGroundsAndWalls(groundAndWalls));
            float instGroundAnddWallsEndTime = Time.realtimeSinceStartup;

            float genEnvObsStartTime = Time.realtimeSinceStartup;
            GameObject envObstacles = GenerateObstacles(
                "EnvironmentObstacles",
                _envObstaclePrefabs,
                TileType.EnvironmentObstacle,
                TileType.EnvironmentGround,
                _minEnvObstacleCount,
                _maxEnvObstacleCount);
            float genEnvObsEndTime = Time.realtimeSinceStartup;

            var envGround = groundAndWalls.ToList().FirstOrDefault(_ => _.name.Equals("EnvironmentGround"));
            envGround.transform.SetParent(environment.transform);
            envObstacles.transform.SetParent(environment.transform);
            float environmentYOffset = 0f;
            environment.transform.position += Vector3.up * environmentYOffset;

            GameObject levelInstance = Object.Instantiate(_locationLevelPrefab);
            NavMeshSurface navMeshSurface = levelInstance.GetComponentInChildren<NavMeshSurface>();

            environment.transform.SetParent(levelInstance.transform);

            var ground = groundAndWalls.ToList().FirstOrDefault(_ => _.name.Equals("Ground"));
            ground.transform.SetParent(navMeshSurface.transform);
            exit.transform.SetParent(navMeshSurface.transform);
            var walls = groundAndWalls.ToList().FirstOrDefault(_ => _.name.Equals("Walls"));
            walls.transform.SetParent(navMeshSurface.transform);
            obstacles?.transform.SetParent(navMeshSurface.transform);

            Vector3 levelOffset = new Vector3(-_firstRoomWidth / 2, 0f, -_levelGeneratorData.SpawnPosY);
            levelInstance.transform.position += levelOffset;
            navMeshSurface.BuildNavMesh();

            float endTime = Time.realtimeSinceStartup;

            Debug.Log($"Level generation took {(endTime - startTime) * 1000f} ms");
            Debug.Log($"Ground gen time: {(genGroundEndTime - genGroundStartTime) * 1000f} ms");
            //Debug.Log($"Ground instantiate time: {(instGroundEndTime - instGroundStartTime) * 1000f} ms");
            Debug.Log($"Walls gen time: {(genWallsEndTime - genWallsStartTime) * 1000f} ms");
            //Debug.Log($"Walls instantiate time: {(instWallsEndTime - instWallsStartTime) * 1000f} ms");
            Debug.Log($"Gen obstacles time: {(genObsEndTime - genObsStartTime) * 1000f} ms");
            Debug.Log($"Gen env ground time: {(genEnvGroundEndTime - genEnvGroundStartTime) * 1000f} ms");
            Debug.Log($"Instantiate ground and walls time: {(instGroundAnddWallsEndTime - instGroundAndWallsStartTime) * 1000f} ms");
            //Debug.Log($"Env ground instantiate time: {(instEnvGroundEndTime - instEnvGroundStartTime) * 1000f} ms");
            Debug.Log($"Gen env obstacles time: {(genEnvObsEndTime - genEnvObsStartTime) * 1000f} ms");

            LevelGenerated?.Invoke(this, new LevelGeneratedEventArgs
            {
                LevelGameObject = levelInstance,
                ExitGameObject = exit
            });
        }

        private IEnumerator InstantiateGroundsAndWalls(List<GameObject> results)
        {
            const int instantiatePerFrameCount = 600;
            int instantiatedCount = instantiatePerFrameCount;

            List<GameObject> groundGOs = new List<GameObject>();
            List<GameObject> wallsGOs = new List<GameObject>();
            List<GameObject> envGroundGOs = new List<GameObject>();

            foreach ((int, int) posKey in _groundMap.Keys)
            {
                int x = posKey.Item1;
                int y = posKey.Item2;

                if (_groundMap.IsCellEquals(TileType.Ground, x, y))
                {
                    GameObject instance = Object.Instantiate(
                    _groundPrefab,
                    new Vector3(x, 0f, y),
                    Quaternion.identity);
                    groundGOs.Add(instance);
                    instantiatedCount--;
                }
                if (_groundMap.IsCellEquals(TileType.EnvironmentGround, x, y))
                {
                    var instance = Object.Instantiate(
                          _envGroundPrefab,
                          new Vector3(x, 1f, y),
                          Quaternion.identity);
                    envGroundGOs.Add(instance);
                    instantiatedCount--;
                }

                if (instantiatedCount <= 0)
                {
                    instantiatedCount = instantiatePerFrameCount;
                    yield return null;
                }
            }

            foreach ((int, int) posKey in _wallsMap.Keys)
            {
                int x = posKey.Item1;
                int y = posKey.Item2;

                if (_wallsMap.IsCellEquals(TileType.Wall, x, y))
                {
                    Vector3 position = new Vector3(x, 0f, y);
                    var instance = Object
                        .Instantiate(_wallPrefab, position, Quaternion.identity);
                    wallsGOs.Add(instance);
                    instantiatedCount--;
                }

                if (instantiatedCount <= 0)
                {
                    instantiatedCount = instantiatePerFrameCount;
                    yield return null;
                }
            }

            _combiner.SetObjectsToCombine(groundGOs.ToArray());
            GameObject groundRes = _combiner.Combine("Ground");
            groundRes.AddComponent<MeshCollider>();
            groundRes.AddComponent<MarkerGround>();

            _combiner.SetObjectsToCombine(wallsGOs.ToArray());
            GameObject wallsRes = _combiner.Combine("Walls");
            wallsRes.AddComponent<MeshCollider>();
            NavMeshModifier navMeshModifier = wallsRes.AddComponent<NavMeshModifier>();
            navMeshModifier.overrideArea = true;
            navMeshModifier.area = NavMesh.GetAreaFromName(NOT_WALKABLE);

            _combiner.SetObjectsToCombine(envGroundGOs.ToArray());
            GameObject envGroundRes = _combiner.Combine("EnvironmentGround");

            results.Add(groundRes);
            results.Add(wallsRes);
            results.Add(envGroundRes);


            //groundGOs.ForEach(_ => Object.Destroy(_));
            //wallsGOs.ForEach(_ => Object.Destroy(_));
            //envGroundGOs.ForEach(_ => Object.Destroy(_));
        }

        private GameObject InstantiateGround()
        {
            _tempGameObjects = new List<GameObject>();

            for (int x = _minX; x < _maxX; ++x)
            {
                for (int y = _minY; y < _maxY; ++y)
                {
                    if (_groundMap.IsCellEquals(TileType.Ground, x, y))
                    {
                        GameObject instance = Object.Instantiate(
                        _groundPrefab,
                        new Vector3(x, 0f, y),
                        Quaternion.identity);
                        _tempGameObjects.Add(instance);
                    }
                }
            }

            _combiner.SetObjectsToCombine(_tempGameObjects.ToArray());
            GameObject result = _combiner.Combine("Ground");
            //result.AddComponent<MeshCollider>();
            result.AddComponent<MarkerGround>();

            _tempGameObjects.Clear();
            _tempGameObjects = null;

            return result;
        }

        private GameObject InstantiateWalls()
        {
            _tempGameObjects = new List<GameObject>();

            for (int x = _minX; x < _maxX; ++x)
            {
                for (int y = _minY; y < _maxY; ++y)
                {
                    if (_wallsMap.IsCellEquals(TileType.Wall, x, y))
                    {
                        Vector3 position = new Vector3(x, 0f, y);
                        var instance = Object
                            .Instantiate(_wallPrefab, position, Quaternion.identity);
                        _tempGameObjects.Add(instance);
                    }
                }
            }

            _combiner.SetObjectsToCombine(_tempGameObjects.ToArray());
            GameObject result = _combiner.Combine("Walls");
            //result.AddComponent<MeshCollider>();
            NavMeshModifier navMeshModifier = result.AddComponent<NavMeshModifier>();
            navMeshModifier.overrideArea = true;
            navMeshModifier.area = NavMesh.GetAreaFromName(NOT_WALKABLE);

            _tempGameObjects.Clear();
            _tempGameObjects = null;

            return result;
        }

        private GameObject InstantiateEnvGround()
        {
            _tempGameObjects = new List<GameObject>();

            for (int x = _minX; x < _maxX; ++x)
            {
                for (int y = _minY; y < _maxY; ++y)
                {
                    if (_groundMap.IsCellEquals(TileType.EnvironmentGround, x, y))
                    {
                        var instance = Object.Instantiate(
                              _envGroundPrefab,
                              new Vector3(x, 1f, y),
                              Quaternion.identity);
                        _tempGameObjects.Add(instance);
                    }
                }
            }

            _combiner.SetObjectsToCombine(_tempGameObjects.ToArray());
            GameObject result = _combiner.Combine("EnvironmentGround");

            _tempGameObjects.Clear();
            _tempGameObjects = null;

            return result;
        }

        #region Private

        private const string NOT_WALKABLE = "Not Walkable";
        // Injected
        private LocationData _locationData;
        private LevelData _levelData;
        private LevelGeneratorData _levelGeneratorData;
        //
        private ICombiner _combiner;
        private Transform _tempTransform;
        private List<GameObject> _tempGameObjects;
        private int _minX;
        private int _maxX;
        private int _minY;
        private int _maxY;
        //
        private ResourcesLoader _resourcesLoader;
        private GameObject _locationLevelPrefab;
        private GameObject _groundPrefab;
        private GameObject _wallPrefab;
        private GameObject[] _obstaclePrefabs;
        private GameObject _exitPrefab;
        private GameObject _envGroundPrefab;
        private GameObject[] _envObstaclePrefabs;

        // TODO: Make multiple wall types
        private void LoadPrefabs()
        {
            _resourcesLoader = new ResourcesLoader();

            _locationLevelPrefab = _resourcesLoader.Load<GameObject>(_locationData.LocationLevelPrefabAddress);
            _groundPrefab = _resourcesLoader.Load<GameObject>(_locationData.GroundPrefabAddress);
            _wallPrefab = _resourcesLoader.Load<GameObject>(_locationData.WallPrefabsLabel);
            _exitPrefab = _resourcesLoader.Load<GameObject>(_locationData.ExitPrefabAddress);
            _obstaclePrefabs = _resourcesLoader.LoadAll<GameObject>(_locationData.ObstaclePrefabsLabel);
            _envGroundPrefab = _resourcesLoader.Load<GameObject>(_locationData.EnvGroundPrefabAddress);
            _envObstaclePrefabs = _resourcesLoader.LoadAll<GameObject>(_locationData.EnvObstaclePrefabsLabel);
        }

        private void Init()
        {
            _minObstacleCount = _levelGeneratorData.MinObstacleCountPerRoom * _levelData.LevelSize;
            _maxObstacleCount = _levelGeneratorData.MaxObstacleCountPerRoom * _levelData.LevelSize;
            _minEnvObstacleCount = _levelGeneratorData.MinEnvObstacleCountPerRoom * _levelData.LevelSize;
            _maxEnvObstacleCount = _levelGeneratorData.MaxEnvObstacleCountPerRoom * _levelData.LevelSize;

            _minX = -_levelGeneratorData.MaxLevelSize / 2;
            _maxX = _levelGeneratorData.MaxLevelSize / 2 + _levelGeneratorData.MaxLevelSize % 2;
            _minY = -_levelGeneratorData.MaxLevelSize / 2;
            _maxY = _levelGeneratorData.MaxLevelSize / 2 + _levelGeneratorData.MaxLevelSize % 2;

            _minGroundCoordX = _maxX;
            _maxGroundCoordX = _minX;
            _minGroundCoordY = _maxY;
            _maxGroundCoordY = _minY;

            _groundMap = new TileMap();
            _wallsMap = new TileMap();
            _obstaclesMap = new TileMap();
        }

        private GameObject GenerateExit()
        {
            var exitRoom = _prevRoom;
            int posX = exitRoom.Position.x + exitRoom.Width / 2;
            int posY = exitRoom.Position.y + exitRoom.Height;

            Vector3 exitPosition = new Vector3(posX, 0f, posY);

            GameObject instance = Object.Instantiate(_exitPrefab, exitPosition, Quaternion.identity);
            instance.name = "Exit";
            var gfx = _exitPrefab.transform.GetChild(0);
            Vector2Int size = new Vector2Int(Mathf.CeilToInt(gfx.transform.localScale.x), Mathf.CeilToInt(gfx.transform.localScale.z));
            for (int xOffset = -size.x / 2; xOffset < size.x / 2 + size.x % 2; ++xOffset)
            {
                for (int yOffset = -size.y / 2; yOffset < size.y / 2 + size.y % 2; ++yOffset)
                {
                    _wallsMap[posX + xOffset, posY + yOffset] = TileType.Exit;
                }
            }

            instance.AddComponent<MarkerLevelExitPoint>();
            NavMeshModifier navMeshModifier = instance.AddComponent<NavMeshModifier>();
            navMeshModifier.overrideArea = true;
            navMeshModifier.area = NavMesh.GetAreaFromName(NOT_WALKABLE);

            return instance;
        }

        #endregion Private
    }
}
