using Prototype.Data;
using Prototype.Service;

namespace Prototype.Model
{
    /// <summary>
    /// Player projectiles now bounce from walls and obstacles
    /// </summary>
    public class BouncingProjectilesBuff : Buff
    {
        public BouncingProjectilesBuff(BuffConfig config, PlayerModel player, GameEventService eventService, GameBalanceData gameBalance)
            : base(config, player, eventService, gameBalance)
        {
        }

        public override void Apply(bool updateSessionData = true)
        {
            if (_player.AppliedBuffs.Contains(Config.Id) && updateSessionData)
            {
                return;
            }

            if (updateSessionData)
            {
                _player.AppliedBuffs.Add(Config.Id);
            }

            _eventService.OnBounceProjectilesEnabled();
        }

        public override void Cancel(bool updateSessionData = true)
        { }
    }
}
