using Prototype.Data;
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

        private void OnDisable()
        {
            _eventService.PlayerBuffApplied -= HandleApplyBuff;
        }

        private void HandleApplyBuff(object sender, GameEventService.PlayerBuffAppliedEventArgs e)
        {
            IBuff buff = _buffFactory.Create(e.BuffId);
            buff.Apply(_player);
            Debug.Log($"Buff applied: {buff.Id}");
        }

        // Private

        // Dependencies

        // Injected
        private PlayerModel _player;
        private GameEventService _eventService;
        private BuffFactory _buffFactory;
    }

    public class BuffFactory : PlaceholderFactory<string, IBuff> { }

    public class BuffFromIdFactory : IFactory<string, IBuff>
    {
        // Injected
        private readonly AppData _appData;
        private readonly GameEventService _eventService;
        //
        private readonly GameBalanceData _gameBalance;

        public BuffFromIdFactory(AppData appData, GameEventService eventService)
        {
            _appData = appData;
            _gameBalance = _appData.GameBalance;
            _eventService = eventService;
        }

        public IBuff Create(string buffId)
        {
            var buffCfg = _gameBalance.BuffConfigs.FirstOrDefault(_ => _.Id.Equals(buffId));
            IBuff result;

            switch (buffCfg.BuffType)
            {
                case BuffTypeEnum.Heal:
                    result = new HealBuff(buffId, (float)buffCfg.Value, (int)buffCfg.BuffLevel);
                    break;
                case BuffTypeEnum.IncreaseMaxHealth:
                    result = new IncreaseMaxHealthBuff(buffId, (float)buffCfg.Value, (int)buffCfg.BuffLevel);
                    break;
                case BuffTypeEnum.IncreaseDamage:
                    result = new IncreaseDamageBuff(buffId, buffCfg.DamageType, (float)buffCfg.Value, (int)buffCfg.BuffLevel);
                    break;
                case BuffTypeEnum.BouncingProjectiles:
                    result = new BouncingProjectilesBuff(buffId, (int)buffCfg.BuffLevel);
                    break;
                default:
                    throw new System.Exception($"Unknown buff type: {buffCfg.BuffType}");
            }

            result.Subscribe(_eventService);

            return result;
        }
    }

    public interface IBuff
    {
        public string Id { get; }
        public BuffTypeEnum Type { get; }
        public int Level { get; }

        public void Subscribe(GameEventService eventService);
        public void Unsubscribe(GameEventService eventService);
        //
        public void Apply(PlayerModel player);
        public void Cancel(PlayerModel player);
    }

    /// <summary>
    /// Restores value% of max health
    /// </summary>
    public class HealBuff : IBuff
    {
        public HealBuff(string id, float value, int level)
        {
            Id = id;
            Type = BuffTypeEnum.Heal;
            _value = value;
            Level = level;
        }

        public string Id { get; }

        public BuffTypeEnum Type { get; }

        public int Level { get; }

        // Internal variables
        private readonly float _value;
        private GameEventService _eventService;

        public void Apply(PlayerModel player)
        {
            float newHealth = player.Health + player.MaxHealth * _value;
            newHealth = Mathf.Clamp(newHealth, 0f, player.MaxHealth);
            player.Health = newHealth;
            _eventService.OnPlayerHealthChanged(
                new GameEventService.PlayerHealthChangedEventArgs { Health = player.Health, MaxHealth = player.MaxHealth });
        }

        public void Cancel(PlayerModel player) { }

        public void Subscribe(GameEventService eventService)
        {
            _eventService = eventService;
        }

        public void Unsubscribe(GameEventService eventService)
        { }
    }

    /// <summary>
    /// Increase player max health by value%
    /// </summary>
    public class IncreaseMaxHealthBuff : IBuff
    {
        public IncreaseMaxHealthBuff(string id, float value, int level)
        {
            Id = id;
            Type = BuffTypeEnum.IncreaseMaxHealth;
            _value = value;
            Level = level;
        }

        public string Id { get; }

        public BuffTypeEnum Type { get; }

        public int Level { get; }
        //
        private readonly float _value;
        //
        private GameEventService _eventService;

        public void Apply(PlayerModel player)
        {
            player.AppliedBuffs.Add(Id);

            float ratio = player.Health / player.MaxHealth;
            player.MaxHealth = player.MaxHealth * (1f + _value);
            player.Health = player.MaxHealth * ratio;

            _eventService.OnPlayerHealthChanged(
               new GameEventService.PlayerHealthChangedEventArgs { Health = player.Health, MaxHealth = player.MaxHealth });
        }

        public void Cancel(PlayerModel player)
        {
            player.AppliedBuffs.Remove(Id);

            float ratio = player.Health / player.MaxHealth;
            player.MaxHealth = player.MaxHealth / (1f + _value);
            player.Health = player.MaxHealth * ratio;

            _eventService.OnPlayerHealthChanged(
               new GameEventService.PlayerHealthChangedEventArgs { Health = player.Health, MaxHealth = player.MaxHealth });
        }

        public void Subscribe(GameEventService eventService)
        {
            _eventService = eventService;
        }

        public void Unsubscribe(GameEventService eventService)
        { }
    }

    /// <summary>
    /// Increases damage of type DamageType by value %
    /// </summary>
    public class IncreaseDamageBuff : IBuff
    {
        public IncreaseDamageBuff(string id, DamageTypeEnum dmgType, float value, int level)
        {
            Id = id;
            Type = BuffTypeEnum.IncreaseDamage;
            _damageType = dmgType;
            _value = value;
            Level = level;
        }

        public string Id { get; }

        public BuffTypeEnum Type { get; }

        public int Level { get; }

        // Dependencies
        private GameEventService _eventService;
        // Internal variables
        private DamageTypeEnum _damageType;
        private float _value;

        public void Apply(PlayerModel player)
        {
            player.AppliedBuffs.Add(Id);
            DescDamageType oldDmgType = player.Damage[_damageType];
            DescDamageType newDmgType = new DescDamageType { Type = _damageType, ValueRange = oldDmgType.ValueRange * (1f + _value), Chance = oldDmgType.Chance };
            player.Damage[_damageType] = newDmgType;
            Debug.Log($"old value range: {oldDmgType.ValueRange}");
            Debug.Log($"new value range: {newDmgType.ValueRange}");
        }

        public void Cancel(PlayerModel player)
        {
            player.AppliedBuffs.Remove(Id);
            DescDamageType oldDmgType = player.Damage[_damageType];
            DescDamageType newDmgType = new DescDamageType { ValueRange = oldDmgType.ValueRange / (1f + _value), Chance = oldDmgType.Chance };
            player.Damage[_damageType] = newDmgType;
        }

        public void Subscribe(GameEventService eventService)
        {
            _eventService = eventService;
        }

        public void Unsubscribe(GameEventService eventService)
        { }
    }

    /// <summary>
    /// Player projectiles now bounce from walls and obstacles
    /// </summary>
    public class BouncingProjectilesBuff : IBuff
    {
        public BouncingProjectilesBuff(string id, int level)
        {
            Id = id;
            Type = BuffTypeEnum.BouncingProjectiles;
            Level = level;
        }

        public string Id { get; }

        public BuffTypeEnum Type { get; }

        public int Level { get; }

        public void Apply(PlayerModel player)
        {
            throw new System.NotImplementedException();
        }

        public void Cancel(PlayerModel player)
        {
            throw new System.NotImplementedException();
        }

        public void Subscribe(GameEventService eventService)
        {
            throw new System.NotImplementedException();
        }

        public void Unsubscribe(GameEventService eventService)
        {
            throw new System.NotImplementedException();
        }
    }
}
