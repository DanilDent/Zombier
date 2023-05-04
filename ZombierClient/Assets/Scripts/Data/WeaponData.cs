using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Weapon Data", menuName = "Data/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        public int FireRateRPM;
        public float AttackRange;
        public ProjectileData ProjectileData;
    }
}