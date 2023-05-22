using Prototype.Data;
using Prototype.MeshCombine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Prototype.LevelGeneration
{
    // TODO: Make non MonoBehaviour, reorganize order, split private types in different files 
    public partial class LevelGenerator
    {
        public LevelGenerator(
            LevelGeneratorData levelGeneratorData,
            LocationData locationData,
            LevelData levelData)
        {
            _levelGeneratorData = levelGeneratorData;
            _locationData = locationData;
            _levelData = levelData;

            _meshCombiner = new MeshCombiner();
            LoadResources();
            Init();
        }

        ~LevelGenerator()
        {
            _groundMap = null;
            _wallsMap = null;
            _obstaclesMap = null;
            _roomCount = 0;
        }

        public GameObject GenerateLevel()
        {
            GameObject ground = GenerateGround();
            GameObject exit = GenerateExit();
            GameObject walls = GenerateWalls();

            GameObject obstacles = GenerateObstacles(
                "Obstacles",
                _obstaclePrefabs,
                TileType.Obstacle,
                TileType.Ground,
                _minObstacleCount,
                _maxObstacleCount);

            GameObject environment = new GameObject("Environment");
            GameObject envGround = GenerateEnvGround();

            GameObject envObstacles = GenerateObstacles(
                "EnvironmentObstacles",
                _envObstaclePrefabs,
                TileType.EnvironmentObstacle,
                TileType.EnvironmentGround,
                _minEnvObstacleCount,
                _maxEnvObstacleCount);
            envGround.transform.SetParent(environment.transform);
            envObstacles.transform.SetParent(environment.transform);
            float environmentYOffset = 0f;
            environment.transform.position += Vector3.up * environmentYOffset;

            var locationLevelPrefab = Resources.Load<GameObject>(_locationData.LocationLevelPrefabAssetPath);
            GameObject levelInstance = Object.Instantiate(locationLevelPrefab);
            NavMeshSurface navMeshSurface = levelInstance.GetComponentInChildren<NavMeshSurface>();

            environment.transform.SetParent(levelInstance.transform);

            ground.transform.SetParent(navMeshSurface.transform);
            exit.transform.SetParent(navMeshSurface.transform);
            walls.transform.SetParent(navMeshSurface.transform);
            obstacles?.transform.SetParent(navMeshSurface.transform);

            Vector3 levelOffset = new Vector3(-_firstRoomWidth / 2, 0f, -_levelGeneratorData.SpawnPosY);
            levelInstance.transform.position += levelOffset;

            navMeshSurface.BuildNavMesh();

            return levelInstance;
        }

        #region Private

        private const string NOT_WALKABLE = "Not Walkable";
        // Injected
        private LocationData _locationData;
        private LevelData _levelData;
        private LevelGeneratorData _levelGeneratorData;
        //
        private MeshCombiner _meshCombiner;
        private Transform _tempTransform;
        private List<GameObject> _tempGameObjects;
        private int _minX;
        private int _maxX;
        private int _minY;
        private int _maxY;
        //
        private GameObject _groundPrefab;
        private GameObject _wallPrefab;
        private GameObject[] _obstaclePrefabs;
        private GameObject _exitPrefab;
        private GameObject _envGroundPrefab;
        private GameObject[] _envObstaclePrefabs;

        private void LoadResources()
        {
            _groundPrefab = Resources.Load<GameObject>(_locationData.GroundPrefabAssetPath);
            _wallPrefab = Resources.Load<GameObject>(_locationData.WallPrefabAssetPath);
            _obstaclePrefabs = Resources.LoadAll<GameObject>(_locationData.ObstaclePrefabsAssetPath);
            _exitPrefab = Resources.Load<GameObject>(_locationData.ExitPrefabAssetPath);
            _envGroundPrefab = Resources.Load<GameObject>(_locationData.EnvGroundPrefabAssetPath);
            _envObstaclePrefabs = Resources.LoadAll<GameObject>(_locationData.EnvObstaclePrefabsAssetPath);
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

            _groundMap = new TileMap(_levelGeneratorData.MaxLevelSize);
            _wallsMap = new TileMap(_levelGeneratorData.MaxLevelSize);
            _obstaclesMap = new TileMap(_levelGeneratorData.MaxLevelSize);
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
