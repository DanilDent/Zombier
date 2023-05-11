using Prototype.View;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Enemy Data", menuName = "Data/Enemy Data")]
    public class EnemyData : SerializedScriptableObject
    {
        public EnemyView ViewPrefab;
        public float Health;
        public float MaxSpeed;
        [NonSerialized] [OdinSerialize] public DescDamage Damage;
        [NonSerialized] [OdinSerialize] public DescDamage Resists;
        public WeaponData Weapon;
    }
}
