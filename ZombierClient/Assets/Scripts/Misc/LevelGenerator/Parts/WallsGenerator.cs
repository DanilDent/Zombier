using System.Collections.Generic;
using UnityEngine;

namespace Prototype.LevelGeneration
{
    public partial class LevelGenerator
    {
        private TileMap _wallsMap;

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
    }
}
