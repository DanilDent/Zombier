using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Enemy Spawner Data", menuName = "Data/Enemy Spawner Data")]
    public class EnemySpawnData : ScriptableObject
    {
        public List<EnemyData> Enemies = new List<EnemyData>();
        public int MinEnemyCount;
        public int MaxEnemyCount;
        public int MinEnemyLevel;
        public int MaxEnemyLevel;
    }

}