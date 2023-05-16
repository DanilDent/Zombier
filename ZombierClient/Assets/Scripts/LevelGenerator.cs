using Prototype.MeshCombine;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Prototype.Controller
{
    public class LevelGenerator : MonoBehaviour
    {
        public MeshCombiner MeshCombiner;

        public GameObject GroundPrefab;
        public GameObject WallPrefab;
        public GameObject[] ObstaclePrefabs;
        public GameObject ExitPrefab;

        public Transform GroundTransform;
        public Transform WallsTransform;
        public Transform ObstaclesTransform;
        //
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

        [SerializeField] private int _maxRoomCount = 1;
        private int _roomCount;
        private int _lastRoomIndex = 0;
        //
        [SerializeField] private int _maxLevelSize = 1000;
        [SerializeField] private int _minRoomWidth = 10;
        [SerializeField] private int _maxRoomWidth = 50;
        [SerializeField] private int _minRoomHeight = 10;
        [SerializeField] private int _maxRoomHeight = 50;
        [SerializeField] private int _minObstaclesCount = 10;
        [SerializeField] private int _maxObstaclesCount = 40;
        [SerializeField] private int _obstaclesCount;
        [SerializeField] private GameObject _exitGO;

        private DescRoomGround _exitRoom;

        [SerializeField] private List<GameObject> _roomGrounds;
        [SerializeField] private List<GameObject> _groundQuadsGfx;
        [SerializeField] private List<GameObject> _walls;
        [SerializeField] private List<GameObject> _wallsGfx;
        [SerializeField] private List<GameObject> _obstaclesGfx;

        private Vector2Int _firstRoomPosition2D;

        private TileType[,] _groundMap;
        private TileType[,] _wallsMap;
        private TileType[,] _obstaclesMap;

        private DescRoomGround _room;
        private DescRoomGround _prevRoom;

        private void OnEnable()
        {
            _roomGrounds = new List<GameObject>();
            _walls = new List<GameObject>();
            _groundQuadsGfx = new List<GameObject>();
            _wallsGfx = new List<GameObject>();
            _obstaclesGfx = new List<GameObject>();
            InitGroundMap();
            InitWallsMap();
            InitObstaclesMap();
            _roomCount = _maxRoomCount;
        }

        private void Update()
        {
            if (_roomCount == _maxRoomCount)
            {
                GenerateFirstRoomGround();
            }

            if (_roomCount > 0 && Input.GetMouseButtonDown(0))
            {
                _room = GenerateNextRoomGround(_prevRoom);
                _prevRoom = _room;
                _roomCount--;
            }

            if (_roomCount == 0)
            {
                Vector3 firstRoomPosition = new Vector3(_firstRoomPosition2D.x, 0f, _firstRoomPosition2D.y);

                _exitRoom = _prevRoom;
                PlaceExit(_exitRoom);

                GenerateWalls();
                GenerateObstacles();

                GroundTransform.position -= firstRoomPosition;
                MeshCombiner.ObjectsToCombine = _groundQuadsGfx.ToArray();
                GameObject groundGO = MeshCombiner.Combine("GroundMesh");
                groundGO.gameObject.AddComponent<MeshCollider>();

                WallsTransform.position -= firstRoomPosition;
                MeshCombiner.ObjectsToCombine = _wallsGfx.ToArray();
                GameObject wallsGO = MeshCombiner.Combine("WallsMesh");
                wallsGO.gameObject.AddComponent<MeshCollider>();

                ObstaclesTransform.position -= firstRoomPosition;
                MeshCombiner.ObjectsToCombine = _obstaclesGfx.ToArray();
                GameObject obstaclesGO = MeshCombiner.Combine("ObstaclesMesh");
                obstaclesGO.gameObject.AddComponent<MeshCollider>();

                _exitGO.transform.position -= firstRoomPosition;

                _roomCount--;
            }
        }

        private void InitGroundMap()
        {
            _groundMap = new TileType[_maxLevelSize, _maxLevelSize];
            for (int w = 0; w < _maxLevelSize; ++w)
            {
                for (int h = 0; h < _maxLevelSize; ++h)
                {
                    _groundMap[w, h] = TileType.Empty;
                }
            }
        }

        private void InitWallsMap()
        {
            _wallsMap = new TileType[_maxLevelSize, _maxLevelSize];
            for (int w = 0; w < _maxLevelSize; ++w)
            {
                for (int h = 0; h < _maxLevelSize; ++h)
                {
                    _wallsMap[w, h] = TileType.Empty;
                }
            }
        }

        private void InitObstaclesMap()
        {
            _obstaclesMap = new TileType[_maxLevelSize, _maxLevelSize];
            for (int w = 0; w < _maxLevelSize; ++w)
            {
                for (int h = 0; h < _maxLevelSize; ++h)
                {
                    _obstaclesMap[w, h] = TileType.Empty;
                }
            }
        }

        private void PlaceExit(DescRoomGround exitRoom)
        {
            int posX = exitRoom.Position.x + exitRoom.Width / 2;
            int posY = exitRoom.Position.y + exitRoom.Height;

            Vector3 exitPosition = new Vector3(posX, 0f, posY);
            _exitGO = Instantiate(ExitPrefab, exitPosition, Quaternion.identity);
            var gfx = ExitPrefab.transform.GetChild(0);
            Vector2Int size = new Vector2Int(Mathf.CeilToInt(gfx.transform.localScale.x), Mathf.CeilToInt(gfx.transform.localScale.z));
            for (int w = -size.x / 2; w < size.x / 2 + size.x % 2; ++w)
            {
                for (int h = -size.y / 2; h < size.y / 2 + size.y % 2; ++h)
                {
                    _wallsMap[posX + w, posY + h] = TileType.Exit;
                }
            }
        }

        private void GenerateObstacles()
        {
            _obstaclesCount = Random.Range(_minObstaclesCount, _maxObstaclesCount);
            List<Vector2Int> groundCells = new List<Vector2Int>();
            for (int w = 0; w < _maxLevelSize; ++w)
            {
                for (int h = 0; h < _maxLevelSize; ++h)
                {
                    if (_groundMap[w, h] == TileType.Ground)
                    {
                        groundCells.Add(new Vector2Int(w, h));
                    }
                }
            }

            while (_obstaclesCount > 0)
            {
                PlaceNextObstacle(groundCells);
                --_obstaclesCount;
            }
        }

        private void PlaceNextObstacle(List<Vector2Int> groundCells)
        {
            int iterations = 100;
            GameObject prefab = ObstaclePrefabs[Random.Range(0, ObstaclePrefabs.Length)];
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
                if (CanPlaceObstacleAt(position.x, position.y, size))
                {
                    Quaternion rot = Quaternion.Euler(0f, rotY, 0f);
                    GameObject instance = Instantiate(prefab, new Vector3(position.x, 0f, position.y), rot, ObstaclesTransform);

                    for (int w = -size.x / 2; w < size.x / 2 + size.x % 2; ++w)
                    {
                        for (int h = -size.y / 2; h < size.y / 2 + size.y % 2; ++h)
                        {
                            _obstaclesMap[position.x + w, position.y + h] = TileType.Obstacle;
                        }
                    }

                    for (int i = 0; i < instance.transform.childCount; ++i)
                    {
                        var child = instance.transform.GetChild(i);
                        _obstaclesGfx.Add(child.gameObject);
                    }

                    return;
                }

                --iterations;
            }

            throw new System.Exception("Can't find a valid place for obstacle!");
        }

        private bool CanPlaceObstacleAt(int x, int y, Vector2Int size)
        {
            int xBlockOffset = 4;
            int yBlockOffset = 4;
            for (int w = -size.x / 2 - xBlockOffset; w < size.x / 2 + xBlockOffset; ++w)
            {
                for (int h = -size.y / 2 - yBlockOffset; h < size.y / 2 + yBlockOffset; ++h)
                {
                    if (!IsCellEquals(TileType.Ground, _groundMap, x + w, y + h) ||
                        IsCellEquals(TileType.Wall, _wallsMap, x + w, y + h) ||
                        !IsCellEmpty(_obstaclesMap, x + w, y + h))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void GenerateWalls()
        {
            for (int w = 0; w < _maxLevelSize; ++w)
            {
                for (int h = 0; h < _maxLevelSize; ++h)
                {
                    if (_groundMap[w, h] == TileType.Ground)
                    {
                        for (int x = -1; x <= 1; ++x)
                        {
                            for (int y = -1; y <= 1; ++y)
                            {
                                if (IsCellEmpty(_groundMap, w + x, h + y) &&
                                    IsCellEmpty(_wallsMap, w + x, h + y))
                                {
                                    _wallsMap[w + x, h + y] = TileType.Wall;
                                    Vector3 position = new Vector3(w + x, 0f, h + y);
                                    var instance = Instantiate(WallPrefab, position, Quaternion.identity, WallsTransform);
                                    _walls.Add(instance);
                                    var wallGfx = instance.transform.GetChild(0).gameObject;
                                    _wallsGfx.Add(wallGfx);
                                }
                            }
                        }
                    }
                }
            }
        }

        private DescRoomGround GenerateFirstRoomGround()
        {
            int width = Random.Range(_minRoomWidth, _maxRoomWidth);
            int height = Random.Range(_minRoomHeight, _maxRoomHeight);
            _firstRoomPosition2D = new Vector2Int(_maxLevelSize / 2 - width / 2, _maxLevelSize / 2 - height / 2);
            _room = new DescRoomGround(
                    _firstRoomPosition2D,
                    width,
                    height,
                    isBottomClosed: true,
                    isLeftClosed: true,
                    isRightClosed: true);

            CreateRoomGround(_room);
            _prevRoom = _room;
            --_roomCount;

            return _room;
        }

        private DescRoomGround GenerateNextRoomGround(DescRoomGround prevRoom)
        {
            int width = Random.Range(_minRoomWidth, _maxRoomWidth);
            int height = Random.Range(_minRoomHeight, _maxRoomHeight);
            Vector2Int position = GetNewRoomPosition(prevRoom, width, height);
            var room = new DescRoomGround(position, width, height, isBottomClosed: true);
            CreateRoomGround(room);
            return room;
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

                    for (int xOffset = -(newRoomWidth - 1); xOffset < prevRoom.Width; ++xOffset)
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

                    for (int yOffset = -(newRoomHeight - 1); yOffset < prevRoom.Height; ++yOffset)
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
            for (int w = 0; w < width; ++w)
            {
                for (int h = 0; h < height; ++h)
                {
                    if (!IsCellEmpty(_groundMap, position.x + w, position.y + h))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool IsCellEmpty(TileType[,] map, int x, int y)
        {
            return IsCellEquals(TileType.Empty, map, x, y);
        }

        private bool IsCellEquals(TileType type, TileType[,] map, int x, int y)
        {
            if (x >= 0 && y >= 0 && x < _maxLevelSize && y < _maxLevelSize && map[x, y] == type)
            {
                return true;
            }

            return false;
        }

        private void CreateRoomGround(DescRoomGround desc)
        {
            GameObject room = new GameObject();
            room.gameObject.name = $"Room#{_lastRoomIndex}";
            _lastRoomIndex++;

            for (int w = 0; w < desc.Width; ++w)
            {
                for (int h = 0; h < desc.Height; ++h)
                {
                    Vector2Int position = new Vector2Int(desc.Position.x + w, desc.Position.y + h);
                    _groundMap[position.x, position.y] = TileType.Ground;
                    var instance = Instantiate(
                        GroundPrefab,
                        new Vector3(position.x, 0f, position.y),
                        Quaternion.identity,
                        room.transform);
                    GameObject quadGfx = instance.transform.GetChild(0).gameObject;
                    _groundQuadsGfx.Add(quadGfx);
                }
            }
            room.transform.SetParent(GroundTransform);
            _roomGrounds.Add(room);
        }

        private void OnDisable()
        {
            foreach (var go in _roomGrounds)
            {
                Destroy(go);
            }
            foreach (var go in _walls)
            {
                Destroy(go);
            }
            _roomGrounds = null;
            _walls = null;
            _groundMap = null;
            _lastRoomIndex = 0;
            _roomCount = 0;
            _obstaclesGfx = null;
        }
    }
}
