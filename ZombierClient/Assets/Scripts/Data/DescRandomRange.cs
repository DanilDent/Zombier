using System;

namespace Prototype.Data
{
    [Serializable]
    public struct DescRandomRange : IEquatable<DescRandomRange>
    {
        public float Min;
        public float Max;

        public static DescRandomRange operator +(DescRandomRange a, DescRandomRange b)
        {
            return new DescRandomRange
            {
                Min = a.Min + b.Min,
                Max = a.Max + b.Max
            };
        }

        public bool Equals(DescRandomRange other)
        {
            return
               this.Min == other.Min &&
               this.Max == other.Max;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is DescRandomRange cast)
                return Equals(cast);
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 10007;
            hash = hash * 15377 + Min.GetHashCode();
            hash = hash * 15377 + Max.GetHashCode();

            return hash;
        }

        public static bool operator ==(DescRandomRange t1, DescRandomRange t2)
        {
            return t1.Equals(t2);
        }

        public static bool operator !=(DescRandomRange t1, DescRandomRange t2)
        {
            return !t1.Equals(t2);
        }
    }
}
