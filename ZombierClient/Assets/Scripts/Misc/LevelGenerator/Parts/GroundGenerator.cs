using System.Collections.Generic;
using UnityEngine;

namespace Prototype.LevelGeneration
{
    public partial class LevelGenerator
    {
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

        private int _roomCount;
        private int _minGroundCoordX;
        private int _maxGroundCoordX;
        private int _minGroundCoordY;
        private int _maxGroundCoordY;
        private int _firstRoomWidth;
        private Vector2Int _firstRoomPosition2D;
        private DescRoomGround _prevRoom;

        private TileMap _groundMap;

        private void GenerateGround()
        {
            _roomCount = _levelData.LevelSize;

            GenerateFirstRoomGround();

            while (_roomCount > 0)
            {
                GenerateNextRoomGround(_prevRoom);
            }
        }

        private void GenerateFirstRoomGround()
        {
            int width = Random.Range(_levelGeneratorData.MinRoomWidth, _levelGeneratorData.MaxRoomWidth);
            int height = Random.Range(_levelGeneratorData.MinRoomHeight, _levelGeneratorData.MaxRoomHeight);
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
            _obstaclesMap[_firstRoomWidth / 2, _levelGeneratorData.SpawnPosY] = TileType.Obstacle;
            _prevRoom = room;
            --_roomCount;
        }

        private void GenerateNextRoomGround(DescRoomGround prevRoom)
        {
            int width = Random.Range(_levelGeneratorData.MinRoomWidth, _levelGeneratorData.MaxRoomWidth);
            int height = Random.Range(_levelGeneratorData.MinRoomHeight, _levelGeneratorData.MaxRoomHeight);
            Vector2Int position = GetNewRoomPosition(prevRoom, width, height);
            var room = new DescRoomGround(position, width, height, isBottomClosed: true);
            CreateRoomGround(room);
            _prevRoom = room;
            --_roomCount;
        }

        private Vector2Int GetNewRoomPosition(DescRoomGround prevRoom, int newRoomWidth, int newRoomHeight)
        {
            List<Vector2Int> potentialPositions = new List<Vector2Int>();
            int minRoomEntryWidth = _levelGeneratorData.MinRoomEntryWidth;

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

                    for (int xOffset = -(newRoomWidth - 1) + minRoomEntryWidth; xOffset < Mathf.Max(prevRoom.Width - minRoomEntryWidth, 1); ++xOffset)
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

                    for (int yOffset = -(newRoomHeight - 1) + minRoomEntryWidth; yOffset < Mathf.Max(prevRoom.Height - minRoomEntryWidth, 1); ++yOffset)
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


                    _minGroundCoordX = Mathf.Min(_minGroundCoordX, position.x);
                    _maxGroundCoordX = Mathf.Max(_maxGroundCoordX, position.x);
                    _minGroundCoordY = Mathf.Min(_minGroundCoordY, position.y);
                    _maxGroundCoordY = Mathf.Max(_maxGroundCoordY, position.y);
                }
            }
        }

        private void GenerateEnvGround()
        {
            int envSurroundSize = _levelGeneratorData.EnvSurroundSize;

            foreach ((int, int) keyPos in _wallsMap.Keys)
            {
                int x = keyPos.Item1;
                int y = keyPos.Item2;

                if (!_wallsMap.IsCellEquals(TileType.Wall, x, y))
                {
                    continue;
                }

                for (int offsetX = -envSurroundSize; offsetX <= envSurroundSize; ++offsetX)
                {
                    for (int offsetY = -envSurroundSize; offsetY <= envSurroundSize; ++offsetY)
                    {
                        if (_groundMap.IsCellEmpty(x + offsetX, y + offsetY) && _wallsMap.IsCellEmpty(x + offsetX, y + offsetY))
                        {
                            _groundMap[x + offsetX, y + offsetY] = TileType.EnvironmentGround;
                        }
                    }
                }
            }
        }
    }
}
