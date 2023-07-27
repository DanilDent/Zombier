using Prototype.Data;
using System.Collections.Generic;

namespace Prototype.Model
{
    public interface IDamaging
    {
        public DescDamage Damage { get; }

        public float CritChance { get; set; }

        public float CritMultiplier { get; set; }

        public List<EffectConfig> DamagingEffects { get; }
    }
}
