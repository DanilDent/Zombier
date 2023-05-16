﻿using Prototype.Data;
using Prototype.MeshCombine;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.LevelGeneration
{
    // TODO: Make non MonoBehaviour, reorganize order, split private types in different files 
    public partial class LevelGenerator : MonoBehaviour
    {
        // TODO: Inject that
        public MeshCombiner MeshCombiner;
        //

        // DONE ----> TODO: external config, move to Scriptable object
        [SerializeField] private LocationData _locationData;
        [SerializeField] private LevelData _levelData;
        [SerializeField] private LevelGeneratorData _levelGeneratorData;
        //

        // Private class variables, need this in order the class logic to work
        private Transform _tempTransform;
        private List<GameObject> _tempGameObjects;
        private int _minX;
        private int _maxX;
        private int _minY;
        private int _maxY;

        // TODO: move to constructor
        private void OnEnable()
        {
            _minObstacleCount = _levelGeneratorData.MinObstacleCountPerRoom * _levelData.LevelSize;
            _maxObstacleCount = _levelGeneratorData.MaxObstacleCountPerRoom * _levelData.LevelSize;
            _minEnvObstacleCount = _levelGeneratorData.MinEnvObstacleCountPerRoom * _levelData.LevelSize;
            _maxEnvObstacleCount = _levelGeneratorData.MaxEnvObstacleCountPerRoom * _levelData.LevelSize;

            _minX = -_levelGeneratorData.MaxLevelSize / 2;
            _maxX = _levelGeneratorData.MaxLevelSize / 2 + _levelGeneratorData.MaxLevelSize % 2;
            _minY = -_levelGeneratorData.MaxLevelSize / 2;
            _maxY = _levelGeneratorData.MaxLevelSize / 2 + _levelGeneratorData.MaxLevelSize % 2;

            _minGroundCoordX = _maxX;
            _maxGroundCoordX = _minX;
            _minGroundCoordY = _maxY;
            _maxGroundCoordY = _minY;

            _groundMap = new TileMap(_levelGeneratorData.MaxLevelSize);
            _wallsMap = new TileMap(_levelGeneratorData.MaxLevelSize);
            _obstaclesMap = new TileMap(_levelGeneratorData.MaxLevelSize);

            GenerateLevel();
        }

        private void GenerateLevel()
        {
            Transform ground = GenerateGround().transform;
            Transform exit = GenerateExit().transform;
            Transform walls = GenerateWalls().transform;
            Transform obstacles = GenerateObstacles(
                "Obstacles",
                _locationData.ObstaclePrefabs,
                TileType.Obstacle,
                TileType.Ground,
                _minObstacleCount,
                _maxObstacleCount).transform;
            Transform envGround = GenerateEnvGround().transform;
            Transform envObstacles = GenerateObstacles(
                "EnvironmentObstacles",
                _locationData.EnvObstaclePrefabs,
                TileType.EnvironmentObstacle,
                TileType.EnvironmentGround,
                _minEnvObstacleCount,
                _maxEnvObstacleCount).transform;
        }

        private GameObject GenerateExit()
        {
            var exitRoom = _prevRoom;
            int posX = exitRoom.Position.x + exitRoom.Width / 2;
            int posY = exitRoom.Position.y + exitRoom.Height;

            Vector3 exitPosition = new Vector3(posX, 0f, posY);
            GameObject instance = Instantiate(_locationData.ExitPrefab, exitPosition, Quaternion.identity);
            instance.name = "Exit";
            var gfx = _locationData.ExitPrefab.transform.GetChild(0);
            Vector2Int size = new Vector2Int(Mathf.CeilToInt(gfx.transform.localScale.x), Mathf.CeilToInt(gfx.transform.localScale.z));
            for (int xOffset = -size.x / 2; xOffset < size.x / 2 + size.x % 2; ++xOffset)
            {
                for (int yOffset = -size.y / 2; yOffset < size.y / 2 + size.y % 2; ++yOffset)
                {
                    _wallsMap[posX + xOffset, posY + yOffset] = TileType.Exit;
                }
            }

            return instance;
        }

        // TODO: Move to destructor
        private void OnDisable()
        {
            _groundMap = null;
            _wallsMap = null;
            _obstaclesMap = null;
            _roomCount = 0;
        }
    }
}
