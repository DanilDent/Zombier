using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Prototype.Controller
{
    public class LevelGenerator : MonoBehaviour
    {
        public GameObject GroundPrefab;
        public Transform GroundTransform;
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

        [SerializeField] private List<GameObject> _rooms;

        private TileType[,] _groundMap;

        private DescRoomGround _room;
        private DescRoomGround _prevRoom;

        private void OnEnable()
        {
            _rooms = new List<GameObject>();
            InitGroundMap();
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

        private void Update()
        {
            if (_roomCount == _maxRoomCount)
            {
                GenerateFirstRoom();
            }

            if (_roomCount > 0 && Input.GetMouseButtonDown(0))
            {
                _room = GenerateNextRoom(_prevRoom);
                _prevRoom = _room;
                _roomCount--;
            }
        }

        private DescRoomGround GenerateFirstRoom()
        {
            _room = new DescRoomGround(
                    Vector2Int.zero,
                    Random.Range(_minRoomWidth, _maxRoomWidth),
                    Random.Range(_minRoomHeight, _maxRoomHeight),
                    isBottomClosed: true);

            CreateRoomGround(_room);
            _prevRoom = _room;
            --_roomCount;

            return _room;
        }
        private DescRoomGround GenerateNextRoom(DescRoomGround prevRoom)
        {
            int width = Random.Range(_minRoomWidth, _maxRoomWidth);
            int height = Random.Range(_minRoomHeight, _maxRoomHeight);
            Vector2Int position = GetNewRoomPosition(prevRoom, width, height);
            var room = new DescRoomGround(position, width, height);
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
                    if (!IsGroundEmpty(position.x + w, position.y + h))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool IsGroundEmpty(int x, int y)
        {
            if (x > 0 && y > 0 && x < _maxLevelSize && y < _maxLevelSize && _groundMap[x, y] == TileType.Empty)
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
                }
            }
            room.transform.SetParent(GroundTransform);
            _rooms.Add(room);
        }

        private void OnDisable()
        {
            foreach (var go in _rooms)
            {
                DestroyImmediate(go);
            }
            _rooms = null;
            _groundMap = null;
            _lastRoomIndex = 0;
            _roomCount = 0;
        }
    }
}
