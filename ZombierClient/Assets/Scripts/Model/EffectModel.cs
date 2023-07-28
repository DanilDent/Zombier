using Prototype.Data;
using System;
using System.Collections.Generic;
using Zenject;

namespace Prototype.Model
{
    public class EffectModel : IDamaging, IEffectable
    {
        public class Factory : PlaceholderFactory<EffectConfig, EffectModel> { }

        public EffectModel(EffectConfig config)
        {
            Config = config;
            DamagingEffects = new List<EffectConfig>();
            CritChance = 0f;
            CritMultiplier = 1f;
            Damage = new DescDamage();
            foreach (DamageTypeEnum dmgType in Enum.GetValues(typeof(DamageTypeEnum)))
            {
                Damage.Add(new DescDamageType(dmgType));
            }

            if (Config.DamageType != DamageTypeEnum.None)
            {
                DescDamageType descDmgType = new DescDamageType
                {
                    Type = Config.DamageType,
                    ValueRange = new DescRandomRange { Min = (float)config.DamageValue, Max = (float)config.DamageValue },
                    Chance = 1f

                };
                Damage[descDmgType.Type] = descDmgType;
            }
            AppliableEffects = new List<EffectConfig>();
            AppliedEffects = new List<EffectConfig>();
        }

        public EffectConfig Config { get; }
        public DescDamage Damage { get; set; }
        public float CritChance { get; set; }
        public float CritMultiplier { get; set; }
        public List<EffectConfig> DamagingEffects { get; }
        public List<EffectConfig> AppliableEffects { get; set; }
        public List<EffectConfig> AppliedEffects { get; set; }
    }
}
