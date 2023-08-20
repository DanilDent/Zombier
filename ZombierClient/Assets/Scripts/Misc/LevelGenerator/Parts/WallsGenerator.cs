namespace Prototype.LevelGeneration
{
    public partial class LevelGenerator
    {
        private TileMap _wallsMap;

        private void GenerateWalls()
        {
            foreach ((int, int) keyPos in _groundMap.Keys)
            {
                int x = keyPos.Item1;
                int y = keyPos.Item2;

                if (_groundMap.IsCellEquals(TileType.Ground, x, y))
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
