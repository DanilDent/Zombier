using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
            public TileMap()
            {
                _map = new Dictionary<(int, int), TileType>();
            }

            public TileType this[int x, int y]
            {
                get
                {
                    _map.TryGetValue((x, y), out TileType value);
                    return value;
                }
                set
                {
                    if (value == TileType.Empty)
                    {
                        _map.Remove((x, y));
                    }
                    else
                    {
                        _map[(x, y)] = value;
                    }
                }
            }

            public Dictionary<(int, int), TileType>.KeyCollection Keys => _map.Keys;

            public bool IsCellEmpty(int x, int y)
            {
                return IsCellEquals(TileType.Empty, x, y);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsCellEquals(TileType type, int x, int y)
            {
                if (this[x, y] == type)
                {
                    return true;
                }

                return false;
            }

            private Dictionary<(int, int), TileType> _map;
        }
    }
}
