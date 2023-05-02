using System.Collections.Generic;
using UnityEngine;

namespace Prototype.SO
{
    [CreateAssetMenu(fileName = "New Enemy Spawner Data", menuName = "Enemy Spawner Data")]
    public class EnemySpawnSO : ScriptableObject
    {
        public List<EnemySO> Enemies;
        public int MinEnemyCount;
        public int MaxEnemyCount;
        public int MinEnemyLevel;
        public int MaxEnemyLevel;
    }

}