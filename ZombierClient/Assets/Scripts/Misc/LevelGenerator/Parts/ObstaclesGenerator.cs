using System.Collections.Generic;
using UnityEngine;

namespace Prototype.LevelGeneration
{
    public partial class LevelGenerator
    {
        private TileMap _obstaclesMap;

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
    }
}
