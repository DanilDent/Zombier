using UnityEngine;

namespace Prototype.LevelGeneration
{
    [CreateAssetMenu(fileName = "New Level Generator Data", menuName = "Data/Level Generator Data")]
    public class LevelGeneratorData : ScriptableObject
    {
        public int MaxLevelSize = 1000;
        public int MinRoomWidth = 10;
        public int MaxRoomWidth = 50;
        public int MinRoomHeight = 10;
        public int MaxRoomHeight = 50;
        public int MinRoomEntryWidth = 4;
        public int EnvSurroundSize = 10;
        public int SpawnPosY = 3;
        public int MinObstacleCountPerRoom = 2;
        public int MaxObstacleCountPerRoom = 4;
        public int MinEnvObstacleCountPerRoom = 3;
        public int MaxEnvObstacleCountPerRoom = 5;
    }
}
