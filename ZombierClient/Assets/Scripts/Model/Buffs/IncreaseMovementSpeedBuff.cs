using Prototype.Data;
using Prototype.Service;
using System.Linq;
using UnityEngine;

namespace Prototype.Model
{
    /// <summary>
    /// Increase player max health by value%
    /// </summary>
    public class IncreaseMovementSpeedBuff : Buff
    {
        public IncreaseMovementSpeedBuff(BuffConfig config, PlayerModel player, GameEventService eventService, GameBalanceData gameBalance)
            : base(config, player, eventService, gameBalance)
        {
        }

        public override void Apply(bool updateSessionData = true)
        {
            if (updateSessionData && Config.BuffLevel > 1)
            {
                BuffConfig prevLvlBuffCfg = _gameBalance.BuffConfigs.FirstOrDefault(_ => _.BuffType == Config.BuffType && _.BuffLevel == Config.BuffLevel - 1);
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

            float baseMaxSpeed = (float)_gameBalance.Player.PlayerConfig.MaxSpeed;
            _player.Speed = baseMaxSpeed * (1f + (float)Config.Value);
        }

        public override void Cancel(bool updateSessionData = true)
        {
            if (updateSessionData)
            {
                _player.AppliedBuffs.Remove(Config.Id);
            }

            float baseMaxSpeed = (float)_gameBalance.Player.PlayerConfig.MaxSpeed;
            _player.Speed = baseMaxSpeed;
        }
    }
}
