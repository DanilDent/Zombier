using Prototype.Data;
using Prototype.MeshCombine;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.LevelGeneration
{
    // TODO: Make non MonoBehaviour, reorganize order, split private types in different files 
    public partial class LevelGenerator : MonoBehaviour
    {
        // TODO: Inject that
        public MeshCombiner MeshCombiner;
        //

        // DONE ----> TODO: external config, move to Scriptable object
        [SerializeField] private LocationData _locationData;
        [SerializeField] private LevelData _levelData;
        //

        // Private class variables, need this in order the class logic to work
        private Transform _tempTransform;
        private List<GameObject> _tempGameObjects;
        private int _minX;
        private int _maxX;
        private int _minY;
        private int _maxY;

        //

        // Internal config, move to LevelGenerator SO
        [SerializeField] private int _maxLevelSize = 1000;
        [SerializeField] private int _minRoomWidth = 10;
        [SerializeField] private int _maxRoomWidth = 50;
        [SerializeField] private int _minRoomHeight = 10;
        [SerializeField] private int _maxRoomHeight = 50;
        [SerializeField] private int _minRoomEntryWidth = 4;
        [SerializeField] private int _envSurroundSize = 10;
        [SerializeField] private int _playerPosY = 3;
        [SerializeField] private int _minObstacleCountPerRoom = 2;
        [SerializeField] private int _maxObstacleCountPerRoom = 4;
        [SerializeField] private int _minEnvObstacleCountPerRoom = 3;
        [SerializeField] private int _maxEnvObstacleCountPerRoom = 5;

        // TODO: move to constructor
        private void OnEnable()
        {
            _minObstacleCount = _minObstacleCountPerRoom * _levelData.LevelSize;
            _maxObstacleCount = _maxObstacleCountPerRoom * _levelData.LevelSize;
            _minEnvObstacleCount = _minEnvObstacleCountPerRoom * _levelData.LevelSize;
            _maxEnvObstacleCountPerRoom = _maxEnvObstacleCountPerRoom * _levelData.LevelSize;

            _minX = -_maxLevelSize / 2;
            _maxX = _maxLevelSize / 2 + _maxLevelSize % 2;
            _minY = -_maxLevelSize / 2;
            _maxY = _maxLevelSize / 2 + _maxLevelSize % 2;

            _minGroundCoordX = _maxX;
            _maxGroundCoordX = _minX;
            _minGroundCoordY = _maxY;
            _maxGroundCoordY = _minY;

            _groundMap = new TileMap(_maxLevelSize);
            _wallsMap = new TileMap(_maxLevelSize);
            _obstaclesMap = new TileMap(_maxLevelSize);

            GenerateLevel();
        }

        private void GenerateLevel()
        {
            Transform ground = GenerateGround().transform;
            Transform exit = GenerateExit().transform;
            Transform walls = GenerateWalls().transform;
            Transform obstacles = GenerateObstacles(
                "Obstacles",
                _locationData.ObstaclePrefabs,
                TileType.Obstacle,
                TileType.Ground,
                _minObstacleCount,
                _maxObstacleCount).transform;
            Transform envGround = GenerateEnvGround().transform;
            Transform envObstacles = GenerateObstacles(
                "EnvironmentObstacles",
                _locationData.EnvObstaclePrefabs,
                TileType.EnvironmentObstacle,
                TileType.EnvironmentGround,
                _minEnvObstacleCount,
                _maxEnvObstacleCount).transform;
        }

        private GameObject GenerateExit()
        {
            var exitRoom = _prevRoom;
            int posX = exitRoom.Position.x + exitRoom.Width / 2;
            int posY = exitRoom.Position.y + exitRoom.Height;

            Vector3 exitPosition = new Vector3(posX, 0f, posY);
            GameObject instance = Instantiate(_locationData.ExitPrefab, exitPosition, Quaternion.identity);
            instance.name = "Exit";
            var gfx = _locationData.ExitPrefab.transform.GetChild(0);
            Vector2Int size = new Vector2Int(Mathf.CeilToInt(gfx.transform.localScale.x), Mathf.CeilToInt(gfx.transform.localScale.z));
            for (int xOffset = -size.x / 2; xOffset < size.x / 2 + size.x % 2; ++xOffset)
            {
                for (int yOffset = -size.y / 2; yOffset < size.y / 2 + size.y % 2; ++yOffset)
                {
                    _wallsMap[posX + xOffset, posY + yOffset] = TileType.Exit;
                }
            }

            return instance;
        }

        // TODO: Move to destructor
        private void OnDisable()
        {
            _groundMap = null;
            _wallsMap = null;
            _obstaclesMap = null;
            _roomCount = 0;
        }
    }
}
