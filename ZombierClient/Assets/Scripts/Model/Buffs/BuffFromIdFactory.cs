using Prototype.Data;
using Prototype.Extensions;
using Prototype.Service;
using System.Linq;
using Zenject;

namespace Prototype.Model
{
    public class BuffFromIdFactory : IFactory<string, Buff>
    {
        // Injected
        private readonly AppData _appData;
        private readonly GameEventService _eventService;
        private readonly PlayerModel _player;
        //
        private readonly GameBalanceData _gameBalance;

        public BuffFromIdFactory(AppData appData, GameEventService eventService, PlayerModel player)
        {
            _appData = appData;
            _gameBalance = _appData.GameBalance;
            _eventService = eventService;
            _player = player;
        }

        public Buff Create(string buffId)
        {
            BuffConfig buffCfg = _gameBalance.BuffConfigs.FirstOrDefault(_ => _.Id.Equals(buffId)).Copy();
            Buff result;

            switch (buffCfg.BuffType)
            {
                case BuffTypeEnum.Heal:
                    result = new HealBuff(buffCfg, _player, _eventService, _gameBalance);
                    break;
                case BuffTypeEnum.IncreaseMaxHealth:
                    result = new IncreaseMaxHealthBuff(buffCfg, _player, _eventService, _gameBalance);
                    break;
                case BuffTypeEnum.IncreaseDamage:
                    result = new IncreaseDamageBuff(buffCfg, _player, _eventService, _gameBalance);
                    break;
                case BuffTypeEnum.BouncingProjectiles:
                    result = new BouncingProjectilesBuff(buffCfg, _player, _eventService, _gameBalance);
                    break;
                case BuffTypeEnum.IncreaseMovementSpeed:
                    result = new IncreaseMovementSpeedBuff(buffCfg, _player, _eventService, _gameBalance);
                    break;
                default:
                    throw new System.Exception($"Unknown buff type: {buffCfg.BuffType}");
            }

            return result;
        }
    }
}
