using System;

namespace Prototype.Data
{
    [Serializable]
    public struct IdData : IEquatable<IdData>
    {
        public static readonly IdData Empty = new IdData();

        public IdData(string value)
        {
            _value = value;
        }

        public IdData(IdData other)
        {
            _value = other._value;
        }

        public bool Equals(IdData other)
        {
            return _value.Equals(other._value);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is IdData cast)
                return Equals(cast);
            return false;
        }

        public override int GetHashCode()
        {
            // This check is for Zenject dry run validation
            if (_value == null)
            {
                _value = String.Empty;
            }

            int hash = 10007;
            hash = hash * 15377 + _value.GetHashCode();
            return hash;
        }

        public static bool operator ==(IdData t1, IdData t2)
        {
            return t1.Equals(t2);
        }

        public static bool operator !=(IdData t1, IdData t2)
        {
            return !t1.Equals(t2);
        }

        public override string ToString()
        {
            return _value;
        }

        private string _value;
    }
}
