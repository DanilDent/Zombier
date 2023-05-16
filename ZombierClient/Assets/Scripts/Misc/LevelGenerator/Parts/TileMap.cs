namespace Prototype.LevelGeneration
{
    public partial class LevelGenerator
    {
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
    }
}
