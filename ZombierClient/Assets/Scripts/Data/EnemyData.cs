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
        public EnemyView EnemyViewPrefab;
        public float Health;
        public float Speed;
        [NonSerialized] [OdinSerialize] public DescDamage Damage;
        [NonSerialized] [OdinSerialize] public DescDamage Resists;
    }
}
