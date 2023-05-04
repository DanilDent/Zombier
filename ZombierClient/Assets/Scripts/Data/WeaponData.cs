using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Weapon Data", menuName = "Data/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        public int FireRateRPM;
        public float AttackRange;
        public float Thrust = 3f;
        public float Recoil = 0.1f;
        public ProjectileData ProjectileData;
    }
}