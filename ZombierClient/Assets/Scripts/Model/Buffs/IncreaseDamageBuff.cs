using Prototype.Data;
using Prototype.Service;
using System.Linq;
using UnityEngine;

namespace Prototype.Model
{
    /// <summary>
    /// Increases damage of type DamageType by value %
    /// </summary>
    public class IncreaseDamageBuff : Buff
    {
        public IncreaseDamageBuff(BuffConfig config, PlayerModel player, GameEventService eventService, GameBalanceData gameBalance)
            : base(config, player, eventService, gameBalance)
        {
        }

        public override void Apply(bool updateSessionData = true)
        {
            if (updateSessionData && Config.BuffLevel > 1)
            {
                BuffConfig prevLvlBuffCfg = _gameBalance.BuffConfigs
                    .FirstOrDefault(_ =>
                    _.BuffType == Config.BuffType
                    && _.BuffLevel == Config.BuffLevel - 1
                    && _.DamageType == Config.DamageType);

                if (!_player.AppliedBuffs.Contains(prevLvlBuffCfg.Id))
                {
                    throw new System.Exception($"Player should have prev level buff applied in order to upgrade");
                }
                _player.AppliedBuffs.Remove(prevLvlBuffCfg.Id);
            }

            if (updateSessionData && _player.AppliedBuffs.Contains(Config.Id))
            {
                Debug.LogWarning($"Trying to apply buff {Config.Id} twice, this is not allowed");
                return;
            }

            if (updateSessionData)
            {
                _player.AppliedBuffs.Add(Config.Id);
            }

            DescDamageType oldDmgType = _player.Weapon.Damage[DamageTypeEnum.Physical];
            DescDamageType newDmgType = new DescDamageType
            {
                Type = Config.DamageType,
                ValueRange = oldDmgType.ValueRange * ((Config.DamageType == DamageTypeEnum.Physical ? 1f : 0f) + (float)Config.Value),
                Chance = oldDmgType.Chance
            };
            _player.Damage[Config.DamageType] = newDmgType;
            Debug.Log($"old value range: {oldDmgType.ValueRange}");
            Debug.Log($"new value range: {newDmgType.ValueRange}");

            EffectConfig effectCfg = _gameBalance.EffectConfigs.FirstOrDefault(_ => _.Id.Equals(Config.Effects));
            if (effectCfg != null)
            {
                _player.AppliableEffects.Add(effectCfg);
            }
        }

        public override void Cancel(bool updateSessionData = true)
        {
            if (updateSessionData)
            {
                _player.AppliedBuffs.Remove(Config.Id);
            }

            DescDamageType oldDmgType = _player.Weapon.Damage[Config.DamageType];
            DescDamageType newDmgType = oldDmgType;
            _player.Damage[Config.DamageType] = newDmgType;

            int effectIndex = _player.AppliableEffects.FindIndex(_ => _.Id.Equals(Config.Effects));
            if (effectIndex != -1)
            {
                _player.AppliableEffects.RemoveAt(effectIndex);
            }
        }
    }
}
