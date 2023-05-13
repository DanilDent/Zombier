using Prototype.Data;

namespace Prototype.Model
{
    public interface IDamageable
    {
        public float Health { get; set; }
        public float MaxHealth { get; set; }
        public DescDamage Resists { get; }
    }
}
