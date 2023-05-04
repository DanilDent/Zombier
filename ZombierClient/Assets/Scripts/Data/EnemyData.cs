using Prototype.View;
using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Enemy Data", menuName = "Data/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public EnemyView EnemyViewPrefab;
        public int Health;
        public int Speed;
    }
}
