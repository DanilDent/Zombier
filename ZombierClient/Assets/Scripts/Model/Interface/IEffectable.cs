using Prototype.Data;
using System.Collections.Generic;

namespace Prototype.Model
{
    public interface IEffectable
    {
        public List<EffectConfig> AppliableEffects { get; }

        public List<EffectConfig> AppliedEffects { get; }
    }
}
