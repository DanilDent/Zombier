using Prototype.Data;
using Prototype.MeshCombine;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Prototype.Controller
{
    // TODO: Make non MonoBehaviour, reorganize order, split private types in different files 
    public class LevelGenerator : MonoBehaviour
    {
        // Private types
        private enum TileType
        {
            Empty = 0,
            Ground,
            Wall,
            Obstacle,
            Exit,
            EnvironmentGround,
            EnvironmentObstacle,
        }

        private class TileMap
        {
            public TileMap(int maxSize)
            {
                _maxSize = maxSize;
                _map = new TileType[_maxSize, _maxSize];

                for (int x = 0; x < _maxSize; ++x)
                {
                    for (int y = 0; y < _maxSize; ++y)
                    {
                        _map[x, y] = TileType.Empty;
                    }
                }
            }

            public TileType this[int x, int y]
            {
                get
                {
                    return _map[IndexInternal(x), IndexInternal(y)];
                }
                set
                {
                    _map[IndexInternal(x), IndexInternal(y)] = value;
                }
            }

            public bool IsCellEmpty(int x, int y)
            {
                return IsCellEquals(TileType.Empty, x, y);
            }

            public bool IsCellEquals(TileType type, int x, int y)
            {
                x = IndexInternal(x);
                y = IndexInternal(y);

                if (x >= 0 && x < _maxSize &&
                    y >= 0 && y < _maxSize &&
                    _map[x, y] == type)
                {
                    return true;
                }

                return false;
            }

            private TileType[,] _map;
            private int _maxSize;
            private int IndexInternal(int index)
            {
                return index + _maxSize / 2;
            }
        }

        private struct DescRoomGround
        {
            public DescRoomGround(
                Vector2Int position,
                int width,
                int height,
                bool isBottomClosed = false,
                bool isLeftClosed = false,
                bool isTopClosed = false,
                bool isRightClosed = false)
            {
                Position = position;
                Width = width;
                Height = height;
                IsClosed = new bool[4];
                IsClosed[0] = isBottomClosed;
                IsClosed[1] = isLeftClosed;
                IsClosed[2] = isTopClosed;
                IsClosed[3] = isRightClosed;
            }

            public Vector2Int Position;
            public int Width;
            public int Height;
            // 0 - Bottom, 1 - Left, 2 - Top, 3 - Right
            public bool[] IsClosed;
        }
        //

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

        private int _roomCount;
        private int _minGroundCoordX;
        private int _maxGroundCoordX;
        private int _minGroundCoordY;
        private int _maxGroundCoordY;
        private DescRoomGround _prevRoom;
        private int _firstRoomWidth;
        private Vector2Int _firstRoomPosition2D;
        private int _minX;
        private int _maxX;
        private int _minY;
        private int _maxY;

        private TileMap _groundMap;
        private TileMap _wallsMap;
        private TileMap _obstaclesMap;
        //

        // Internal config, assign from Inspector
        [SerializeField] private int _maxLevelSize = 1000;
        [SerializeField] private int _minRoomWidth = 10;
        [SerializeField] private int _maxRoomWidth = 50;
        [SerializeField] private int _minRoomHeight = 10;
        [SerializeField] private int _maxRoomHeight = 50;
        [SerializeField] private int _minRoomEntryWidth = 4;
        [SerializeField] private int _envSurroundSize = 10;
        [SerializeField] private int _playerPosY = 3;
        // TODO: Recalculate that based on SO data
        [SerializeField] private int _minObstaclesCount = 10;
        [SerializeField] private int _maxObstaclesCount = 40;
        [SerializeField] private int _minEnvObstacleCount = 10;
        [SerializeField] private int _maxEnvObstacleCount = 30;
        //

        // TODO: move to constructor
        private void OnEnable()
        {
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
                _minObstaclesCount,
                _maxObstaclesCount).transform;
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

        private GameObject GenerateObstacles(
            string gameObjectName,
            GameObject[] obstaclesPrefabs,
            TileType obstacleType,
            TileType groundType,
            int minCnt,
            int maxCnt)
        {
            _tempTransform = new GameObject("TempTransform").transform;
            _tempGameObjects = new List<GameObject>();

            int counter = Random.Range(minCnt, maxCnt);
            List<Vector2Int> groundCells = new List<Vector2Int>();
            for (int x = _minX; x < _maxX; ++x)
            {
                for (int y = _minY; y < _maxY; ++y)
                {
                    if (_groundMap[x, y] == groundType)
                    {
                        groundCells.Add(new Vector2Int(x, y));
                    }
                }
            }

            while (counter > 0)
            {
                PlaceNextObstacle(groundCells, obstaclesPrefabs, obstacleType, groundType);
                --counter;
            }

            MeshCombiner.SetObjectsToCombine(_tempGameObjects.ToArray());
            GameObject result = MeshCombiner.Combine(gameObjectName);
            result.AddComponent<MeshCollider>();

            _tempGameObjects.Clear();
            _tempGameObjects = null;
            DestroyImmediate(_tempTransform.gameObject);
            _tempTransform = null;

            return result;
        }

        private void PlaceNextObstacle(List<Vector2Int> groundCells, GameObject[] obstaclesPrefabs, TileType obstacleType, TileType groundType)
        {
            int iterations = 100;
            GameObject prefab = obstaclesPrefabs[Random.Range(0, obstaclesPrefabs.Length)];
            ObstacleComp obstacleComp = prefab.GetComponent<ObstacleComp>();
            Vector2Int size = new Vector2Int(Mathf.CeilToInt(obstacleComp.Bounds.size.x), Mathf.CeilToInt(obstacleComp.Bounds.size.y));
            int[] rotationsY = new int[4] { 0, 90, 180, 270 };
            int rotY = rotationsY[Random.Range(0, rotationsY.Length)];
            if (rotY == 90 || rotY == 270)
            {
                size = new Vector2Int(size.y, size.x);
            }

            while (iterations > 0)
            {
                Vector2Int position = groundCells[Random.Range(0, groundCells.Count)];
                if (CanPlaceObstacleOn(groundType, position.x, position.y, size))
                {
                    Quaternion rot = Quaternion.Euler(0f, rotY, 0f);
                    GameObject instance = Instantiate(prefab, new Vector3(position.x, 0f, position.y), rot, _tempTransform);
                    _tempGameObjects.Add(instance);

                    for (int xOffset = -size.x / 2; xOffset < size.x / 2 + size.x % 2; ++xOffset)
                    {
                        for (int yOffset = -size.y / 2; yOffset < size.y / 2 + size.y % 2; ++yOffset)
                        {
                            _obstaclesMap[position.x + xOffset, position.y + yOffset] = obstacleType;
                        }
                    }

                    return;
                }

                --iterations;
            }

            Debug.LogWarning("Can't find a valid place for obstacle");
        }

        private bool CanPlaceObstacleOn(TileType tileType, int x, int y, Vector2Int size)
        {
            int xBlockOffset = 4;
            int yBlockOffset = 4;
            for (int xOffset = -size.x / 2 - xBlockOffset; xOffset < size.x / 2 + xBlockOffset; ++xOffset)
            {
                for (int yOffset = -size.y / 2 - yBlockOffset; yOffset < size.y / 2 + yBlockOffset; ++yOffset)
                {
                    if (!_groundMap.IsCellEquals(tileType, x + xOffset, y + yOffset) ||
                        _wallsMap.IsCellEquals(TileType.Wall, x + xOffset, y + yOffset) ||
                        !_obstaclesMap.IsCellEmpty(x + xOffset, y + yOffset))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private GameObject GenerateWalls()
        {
            _tempTransform = new GameObject("TempTransforms").transform;
            _tempGameObjects = new List<GameObject>();

            for (int x = _minX; x < _maxX; ++x)
            {
                for (int y = _minY; y < _maxY; ++y)
                {
                    if (_groundMap[x, y] == TileType.Ground)
                    {
                        for (int xOffset = -1; xOffset <= 1; ++xOffset)
                        {
                            for (int yOffset = -1; yOffset <= 1; ++yOffset)
                            {
                                if (_groundMap.IsCellEmpty(x + xOffset, y + yOffset) &&
                                    _wallsMap.IsCellEmpty(x + xOffset, y + yOffset))
                                {
                                    _wallsMap[x + xOffset, y + yOffset] = TileType.Wall;
                                    Vector3 position = new Vector3(x + xOffset, 0f, y + yOffset);
                                    var instance = Instantiate(_locationData.WallPrefab, position, Quaternion.identity, _tempTransform);
                                    _tempGameObjects.Add(instance);
                                }
                            }
                        }
                    }
                }
            }

            MeshCombiner.SetObjectsToCombine(_tempGameObjects.ToArray());
            GameObject result = MeshCombiner.Combine("Walls");
            result.AddComponent<MeshCollider>();

            _tempGameObjects.Clear();
            _tempGameObjects = null;
            DestroyImmediate(_tempTransform.gameObject);
            _tempTransform = null;

            return result;
        }

        private GameObject GenerateGround()
        {
            _tempTransform = new GameObject("TempTransform").transform;
            _tempGameObjects = new List<GameObject>();

            _roomCount = _levelData.LevelSize;

            GenerateFirstRoomGround();

            while (_roomCount > 0)
            {
                GenerateNextRoomGround(_prevRoom);
            }

            MeshCombiner.SetObjectsToCombine(_tempGameObjects.ToArray());
            GameObject result = MeshCombiner.Combine("Ground");
            result.AddComponent<MeshCollider>();

            _tempGameObjects.Clear();
            _tempGameObjects = null;
            DestroyImmediate(_tempTransform.gameObject);
            _tempTransform = null;

            return result;
        }

        private void GenerateFirstRoomGround()
        {
            int width = Random.Range(_minRoomWidth, _maxRoomWidth);
            int height = Random.Range(_minRoomHeight, _maxRoomHeight);
            _firstRoomPosition2D = Vector2Int.zero;
            _firstRoomWidth = width;

            var room = new DescRoomGround(
                    _firstRoomPosition2D,
                    width,
                    height,
                    isBottomClosed: true,
                    isLeftClosed: true,
                    isRightClosed: true);

            CreateRoomGround(room);
            _obstaclesMap[_firstRoomWidth / 2, _playerPosY] = TileType.Obstacle;
            _prevRoom = room;
            --_roomCount;
        }

        private void GenerateNextRoomGround(DescRoomGround prevRoom)
        {
            int width = Random.Range(_minRoomWidth, _maxRoomWidth);
            int height = Random.Range(_minRoomHeight, _maxRoomHeight);
            Vector2Int position = GetNewRoomPosition(prevRoom, width, height);
            var room = new DescRoomGround(position, width, height, isBottomClosed: true);
            CreateRoomGround(room);
            _prevRoom = room;
            --_roomCount;
        }

        private Vector2Int GetNewRoomPosition(DescRoomGround prevRoom, int newRoomWidth, int newRoomHeight)
        {
            List<Vector2Int> potentialPositions = new List<Vector2Int>();

            for (int side = 0; side < prevRoom.IsClosed.Length; ++side)
            {
                if (prevRoom.IsClosed[side])
                {
                    continue;
                }

                bool isHorizontal = (side % 2 == 0);
                if (isHorizontal)
                {
                    int yOffset = 0;

                    if (side == 0)
                    {
                        // Bottom
                        yOffset = -newRoomHeight;
                    }
                    else if (side == 2)
                    {
                        // Top
                        yOffset = prevRoom.Height;
                    }

                    for (int xOffset = -(newRoomWidth - 1) + _minRoomEntryWidth; xOffset < Mathf.Max(prevRoom.Width - _minRoomEntryWidth, 1); ++xOffset)
                    {
                        int x = prevRoom.Position.x + xOffset;
                        int y = prevRoom.Position.y + yOffset;
                        Vector2Int position = new Vector2Int(x, y);
                        if (CanRoomBePlaced(position, newRoomWidth, newRoomHeight))
                        {
                            Vector2Int cell = new Vector2Int(x, y);
                            potentialPositions.Add(cell);
                        }
                        else
                        {
                        }
                    }
                }
                else
                {
                    int xOffset = 0;
                    if (side == 1)
                    {
                        // Left
                        xOffset = -newRoomWidth;
                    }
                    else if (side == 3)
                    {
                        // Right
                        xOffset = prevRoom.Width;
                    }

                    for (int yOffset = -(newRoomHeight - 1) + _minRoomEntryWidth; yOffset < Mathf.Max(prevRoom.Height - _minRoomEntryWidth, 1); ++yOffset)
                    {
                        int x = prevRoom.Position.x + xOffset;
                        int y = prevRoom.Position.y + yOffset;
                        Vector2Int position = new Vector2Int(x, y);
                        if (CanRoomBePlaced(position, newRoomWidth, newRoomHeight))
                        {
                            Vector2Int cell = new Vector2Int(x, y);
                            potentialPositions.Add(cell);
                        }
                        else
                        {
                        }
                    }
                }
            }

            if (potentialPositions.Count > 0)
            {
                Vector2Int position = potentialPositions[Random.Range(0, potentialPositions.Count)];
                return position;
            }

            throw new System.Exception("Can't find a free cell for the new room!");
        }

        private bool CanRoomBePlaced(Vector2Int position, int width, int height)
        {
            for (int xOffset = 0; xOffset < width; ++xOffset)
            {
                for (int yOffset = 0; yOffset < height; ++yOffset)
                {
                    if (!_groundMap.IsCellEmpty(position.x + xOffset, position.y + yOffset))
                    {
                        return false;
                    }
                }
            }

            return true;
        }



        private void CreateRoomGround(DescRoomGround desc)
        {
            for (int xOffset = 0; xOffset < desc.Width; ++xOffset)
            {
                for (int yOffset = 0; yOffset < desc.Height; ++yOffset)
                {
                    Vector2Int position = new Vector2Int(desc.Position.x + xOffset, desc.Position.y + yOffset);
                    _groundMap[position.x, position.y] = TileType.Ground;
                    GameObject instance = Instantiate(
                        _locationData.GroundPrefab,
                        new Vector3(position.x, 0f, position.y),
                        Quaternion.identity,
                        _tempTransform);
                    _tempGameObjects.Add(instance);

                    _minGroundCoordX = Mathf.Min(_minGroundCoordX, position.x);
                    _maxGroundCoordX = Mathf.Max(_maxGroundCoordX, position.x);
                    _minGroundCoordY = Mathf.Min(_minGroundCoordY, position.y);
                    _maxGroundCoordY = Mathf.Max(_maxGroundCoordY, position.y);
                }
            }
        }

        private GameObject GenerateEnvGround()
        {
            _tempTransform = new GameObject("TempTransform").transform;
            _tempGameObjects = new List<GameObject>();

            int minX = Mathf.Max(_minX, _minGroundCoordX - _envSurroundSize);
            int maxX = Mathf.Min(_maxX, _maxGroundCoordX + _envSurroundSize);
            int minY = Mathf.Max(_minY, _minGroundCoordY - _envSurroundSize);
            int maxY = Mathf.Min(_maxY, _maxGroundCoordY + _envSurroundSize);

            for (int x = minX; x < maxX; ++x)
            {
                for (int y = minY; y < maxY; ++y)
                {
                    if (_groundMap.IsCellEmpty(x, y))
                    {
                        _groundMap[x, y] = TileType.EnvironmentGround;
                        var instance = Instantiate(
                            _locationData.EnvGroundPrefab,
                            new Vector3(x, 1f, y),
                            Quaternion.identity,
                            _tempTransform);
                        _tempGameObjects.Add(instance);
                    }
                }
            }

            MeshCombiner.SetObjectsToCombine(_tempGameObjects.ToArray());
            GameObject result = MeshCombiner.Combine("EnvironmentGround");

            _tempGameObjects.Clear();
            _tempGameObjects = null;
            DestroyImmediate(_tempTransform.gameObject);
            _tempTransform = null;

            return result;
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
