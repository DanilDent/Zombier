namespace Prototype.LevelGeneration
{
    public partial class LevelGenerator
    {
        private TileMap _wallsMap;

        private void GenerateWalls()
        {
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
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
