using Prototype.Data;
using Prototype.Extensions;
using Prototype.Model;
using Prototype.Service;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class ApplyBuffController : MonoBehaviour
    {
        [Inject]
        public void Construct(
            PlayerModel player,
            GameEventService eventService,
            BuffFactory buffFactory)
        {
            _player = player;
            _eventService = eventService;
            _buffFactory = buffFactory;
        }

        private void OnEnable()
        {
            _eventService.PlayerBuffApplied += HandleApplyBuff;
        }

        private void Start()
        {
            ApplyAllBuffs();
        }

        private void OnDisable()
        {
            _eventService.PlayerBuffApplied -= HandleApplyBuff;
        }

        private void OnDestroy()
        {
            CancelAllBuffs();
        }

        private void ApplyAllBuffs()
        {
            foreach (var buffId in _player.AppliedBuffs)
            {
                Buff buff = _buffFactory.Create(buffId);
                buff.Apply(updateSessionData: false);
                Debug.Log($"Saved buff applied: {buff.Config.Id}");
            }
        }

        private void CancelAllBuffs()
        {
            foreach (var buffId in _player.AppliedBuffs)
            {
                Buff buff = _buffFactory.Create(buffId);
                buff.Cancel(updateSessionData: false);
                Debug.Log($"Buff canceled runtime");
            }
        }

        private void HandleApplyBuff(object sender, GameEventService.PlayerBuffAppliedEventArgs e)
        {
            Buff buff = _buffFactory.Create(e.BuffId);
            buff.Apply(_player);
            Debug.Log($"Buff applied: {buff.Config.Id}");
        }

        // Private

        // Dependencies

        // Injected
        private PlayerModel _player;
        private GameEventService _eventService;
        private BuffFactory _buffFactory;
    }

    public class BuffFactory : PlaceholderFactory<string, Buff> { }

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
                default:
                    throw new System.Exception($"Unknown buff type: {buffCfg.BuffType}");
            }

            return result;
        }
    }

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

    /// <summary>
    /// Increase player max health by value%
    /// </summary>
    public class IncreaseMaxHealthBuff : Buff
    {
        public IncreaseMaxHealthBuff(BuffConfig config, PlayerModel player, GameEventService eventService, GameBalanceData gameBalance)
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

            float baseMaxHealth = _gameBalance.Player.PlayerConfig.MaxHealth;
            _player.MaxHealth = baseMaxHealth * (1f + (float)Config.Value);

            _eventService.OnPlayerHealthChanged(
               new GameEventService.PlayerHealthChangedEventArgs { Health = _player.Health, MaxHealth = _player.MaxHealth });
        }

        public override void Cancel(bool updateSessionData = true)
        {
            if (updateSessionData)
            {
                _player.AppliedBuffs.Remove(Config.Id);
            }

            float baseMaxHealth = _gameBalance.Player.PlayerConfig.MaxHealth;
            _player.MaxHealth = baseMaxHealth;

            _eventService.OnPlayerHealthChanged(
               new GameEventService.PlayerHealthChangedEventArgs { Health = _player.Health, MaxHealth = _player.MaxHealth });
        }
    }

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

            DescDamageType oldDmgType = _player.Weapon.Damage[Config.DamageType];
            DescDamageType newDmgType = new DescDamageType
            {
                Type = Config.DamageType,
                ValueRange = oldDmgType.ValueRange * (1f + (float)Config.Value),
                Chance = oldDmgType.Chance
            };
            _player.Damage[Config.DamageType] = newDmgType;
            Debug.Log($"old value range: {oldDmgType.ValueRange}");
            Debug.Log($"new value range: {newDmgType.ValueRange}");


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
        }
    }

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
