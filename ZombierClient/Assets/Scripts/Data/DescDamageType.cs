using System;
using UnityEngine;

namespace Prototype.Data
{
    [Serializable]
    public struct DescDamageType
    {
        public enum DamageType
        {
            Physical = 0,
            Toxic,
            Fire
        }

        public DescDamageType(DamageType type)
        {
            Type = type;
            Value = 0;
            Chance = 0;
        }

        public DamageType Type;
        public float Value;
        public float Chance;

        public static DescDamageType operator +(DescDamageType a, DescDamageType b)
        {
            if (a.Type == b.Type)
            {
                return new DescDamageType
                {
                    Type = a.Type,
                    Value = a.Value + b.Value,
                    Chance = Mathf.Clamp01(a.Chance + b.Chance)
                };
            }

            throw new Exception($"Trying to sum damage type {a.Type} with {b.Type}");
        }
    }
}
