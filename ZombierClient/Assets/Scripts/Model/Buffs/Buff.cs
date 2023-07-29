using Prototype.Data;
using Prototype.Service;

namespace Prototype.Model
{
    public abstract class Buff
    {
        public Buff(
            BuffConfig config,
            PlayerModel player,
            GameEventService eventService,
            GameBalanceData gameBalance)
        {
            Config = config;
            _player = player;
            _eventService = eventService;
            _gameBalance = gameBalance;
        }

        public BuffConfig Config { get; }
        //
        public abstract void Apply(bool updateSessionData = true);
        public abstract void Cancel(bool updateSessionData = true);
        //
        protected PlayerModel _player;
        protected GameEventService _eventService;
        protected GameBalanceData _gameBalance;
    }
}
