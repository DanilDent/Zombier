using Prototype.Data;

namespace Prototype.Model
{
    public interface IDamaging
    {
        public DescDamage Damage { get; }
        public float CritChance { get; set; }
        public float CritMultiplier { get; set; }
    }
}
