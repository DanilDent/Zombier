using Prototype.Model;
using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Projectile Data", menuName = "Data/Projectile Data")]
    public class ProjectileData : ScriptableObject
    {
        public ProjectileModelBase Prefab;
    }
}
