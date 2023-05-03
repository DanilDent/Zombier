using Prototype.Model;
using UnityEngine;

namespace Prototype.SO
{
    [CreateAssetMenu(fileName = "New Projectile Data", menuName = "Projectile Data")]
    public class ProjectileSO : ScriptableObject
    {
        public ProjectileModel Prefab;
        public int Speed;
        public int Damage;
    }
}
