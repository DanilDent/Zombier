using Prototype.View;
using UnityEngine;

namespace Prototype.SO
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy Data")]
    public class EnemySO : ScriptableObject
    {
        public EnemyView EnemyViewPrefab;
        public int Health;
        public int Speed;
    }
}
