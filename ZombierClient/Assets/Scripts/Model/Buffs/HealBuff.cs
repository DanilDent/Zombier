using Prototype.Data;
using Prototype.Service;
using UnityEngine;

namespace Prototype.Model
{
    /// <summary>
    /// Restores value% of max health
    /// </summary>
    public class HealBuff : Buff
    {
        public HealBuff(BuffConfig config, PlayerModel player, GameEventService eventService, GameBalanceData gameBalance)
            : base(config, player, eventService, gameBalance)
        {
        }

        public override void Apply(bool updateSessionData = true)
        {
            float newHealth = _player.Health + _player.MaxHealth * (float)Config.Value;
            newHealth = Mathf.Clamp(newHealth, 0f, _player.MaxHealth);
            _player.Health = newHealth;
            _eventService.OnPlayerHealthChanged(
                new GameEventService.PlayerHealthChangedEventArgs { Health = _player.Health, MaxHealth = _player.MaxHealth });
        }

        public override void Cancel(bool updateSessionData = true) { }
    }
}
