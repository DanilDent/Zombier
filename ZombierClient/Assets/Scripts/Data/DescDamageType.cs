using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Prototype.Data
{
    [Serializable]
    public struct DescDamageType : IEquatable<DescDamageType>
    {
        public DescDamageType(DamageTypeType type)
        {
            Type = type;

            ValueRange = new DescRandomRange { Min = 0f, Max = 0f };
            Chance = 0;
        }

        public DamageTypeType Type;
        [JsonIgnore] public float Value => UnityEngine.Random.Range(ValueRange.Min, ValueRange.Max);
        public DescRandomRange ValueRange;
        public float Chance;

        public static DescDamageType operator +(DescDamageType a, DescDamageType b)
        {
            if (a.Type == b.Type)
            {
                return new DescDamageType
                {
                    Type = a.Type,
                    ValueRange = a.ValueRange + b.ValueRange,
                    Chance = Mathf.Clamp01(a.Chance + b.Chance)
                };
            }

            throw new Exception($"Trying to sum damage type {a.Type} with {b.Type}");
        }

        public bool Equals(DescDamageType other)
        {
            return
                this.Type == other.Type &&
                this.ValueRange == other.ValueRange &&
                this.Chance == other.Chance;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is DescDamageType cast)
                return Equals(cast);
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 10007;
            hash = hash * 15377 + (int)Type.GetHashCode();
            hash = hash * 15377 + (int)ValueRange.GetHashCode();
            hash = hash * 15377 + (int)Chance.GetHashCode();

            return hash;
        }

        public static bool operator ==(DescDamageType t1, DescDamageType t2)
        {
            return t1.Equals(t2);
        }

        public static bool operator !=(DescDamageType t1, DescDamageType t2)
        {
            return !t1.Equals(t2);
        }
    }
}
