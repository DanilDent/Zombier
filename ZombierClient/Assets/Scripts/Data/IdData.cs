using Newtonsoft.Json;
using System;

namespace Prototype.Data
{
    [Serializable]
    public struct IdData : IEquatable<IdData>
    {
        public string Value;

        [JsonIgnore]
        public static IdData Empty => new IdData("empty");

        [JsonConstructor]
        public IdData(string value)
        {
            Value = value;
        }

        public bool Equals(IdData other)
        {
            return other.Value.Equals(Value);
        }

        public override bool Equals(object @object)
        {
            return @object is IdData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return Value == null ? "empty" : Value;
        }

        public static explicit operator IdData(string value)
        {
            return new IdData(value);
        }
    }
}
