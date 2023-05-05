using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Weapon Data", menuName = "Data/Weapon Data")]
    public class WeaponData : SerializedScriptableObject
    {
        public int FireRateRPM;
        public float AttackRange;
        public float Thrust = 3f;
        public float Recoil = 0.1f;
        [NonSerialized] [OdinSerialize] public DescDamage Damage;
        public ProjectileData ProjectileData;
    }
}