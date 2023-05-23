using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Weapon Data", menuName = "Data/Weapon Data")]
    public class WeaponData : SerializedScriptableObject
    {
        public int AttackRateRPM;
        public float AttackRange;
        public float Thrust = 3f;
        public float Recoil = 0.1f;
        [OdinSerialize] public DescDamage Damage;
        public ProjectileData ProjectileData;
    }
}