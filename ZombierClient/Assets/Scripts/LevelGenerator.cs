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
        public Transform GroundTransform;
        public Transform WallsTransform;
        //
        private enum TileType
        {
            Empty = 0,
            Ground,
            Wall,
            Obstacle,
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

        [SerializeField] private List<GameObject> _roomGrounds;
        [SerializeField] private List<GameObject> _groundQuadsGfx;
        [SerializeField] private List<GameObject> _walls;
        [SerializeField] private List<GameObject> _wallsGfx;

        private Vector2Int _firstRoomPosition2D;

        private TileType[,] _groundMap;
        private TileType[,] _wallsMap;

        private DescRoomGround _room;
        private DescRoomGround _prevRoom;

        private void OnEnable()
        {
            _roomGrounds = new List<GameObject>();
            _walls = new List<GameObject>();
            _groundQuadsGfx = new List<GameObject>();
            _wallsGfx = new List<GameObject>();
            InitGroundMap();
            InitWallsMap();
            _roomCount = _maxRoomCount;
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
                GenerateWalls();
                Vector3 firstRoomPosition = new Vector3(_firstRoomPosition2D.x, 0f, _firstRoomPosition2D.y);
                GroundTransform.position -= firstRoomPosition;
                MeshCombiner.ObjectsToCombine = _groundQuadsGfx.ToArray();
                GameObject groundGO = MeshCombiner.Combine("GroundMesh");
                groundGO.gameObject.AddComponent<MeshCollider>();
                WallsTransform.position -= firstRoomPosition;
                MeshCombiner.ObjectsToCombine = _wallsGfx.ToArray();
                GameObject wallsGO = MeshCombiner.Combine("WallsMesh");
                wallsGO.gameObject.AddComponent<MeshCollider>();
                _roomCount--;
            }
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
        }
    }
}
