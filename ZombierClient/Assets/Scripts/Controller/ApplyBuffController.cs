using Prototype.Data;
using Prototype.Model;
using Prototype.Service;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class ApplyBuffController : MonoBehaviour
    {
        [Inject]
        public void Construct(PlayerModel player, GameEventService eventService)
        {
            _player = player;
            _eventService = eventService;
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
            IBuff buff = e.Buff;
            buff.Subscribe(_eventService);
            buff.Apply(_player);
            Debug.Log($"Buff applied: {buff.Type}");
        }

        // Private

        // Dependencies

        // Injected
        private PlayerModel _player;
        private GameEventService _eventService;
    }

    public interface IBuff
    {
        public BuffType Type { get; }

        public void Subscribe(GameEventService eventService);
        public void Unsubscribe(GameEventService eventService);
        //
        public void Apply(PlayerModel player);
        public void Cancel(PlayerModel player);
    }

    public enum BuffType
    {
        Heal,
        IncreaseMaxHealth,
        IncreaseDamage,
        BouncingProjectiles,
        FireProjectiles
    }

    /// <summary>
    /// Restores value% of max health
    /// </summary>
    public class HealBuff : IBuff
    {
        public HealBuff(float value)
        {
            Type = BuffType.Heal;
            _value = value;
        }

        public BuffType Type { get; }
        private readonly float _value;
        //
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
        public IncreaseMaxHealthBuff(float value)
        {
            Type = BuffType.IncreaseMaxHealth;
            _value = value;
        }

        public BuffType Type { get; }
        //
        private readonly float _value;
        //
        private GameEventService _eventService;

        public void Apply(PlayerModel player)
        {
            player.AppliedBuffs.Add(this);

            float ratio = player.Health / player.MaxHealth;
            player.MaxHealth = player.MaxHealth * (1f + _value);
            player.Health = player.MaxHealth * ratio;

            _eventService.OnPlayerHealthChanged(
               new GameEventService.PlayerHealthChangedEventArgs { Health = player.Health, MaxHealth = player.MaxHealth });
        }

        public void Cancel(PlayerModel player)
        {
            player.AppliedBuffs.Remove(this);

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
        public IncreaseDamageBuff(DamageTypeEnum dmgType, float value)
        {
            Type = BuffType.IncreaseDamage;
            _damageType = dmgType;
            _value = value;
        }

        public BuffType Type { get; }
        // Dependencies
        private GameEventService _eventService;
        // Internal variables
        private DamageTypeEnum _damageType;
        private float _value;

        public void Apply(PlayerModel player)
        {
            player.AppliedBuffs.Add(this);
            DescDamageType oldDmgType = player.Damage[_damageType];
            DescDamageType newDmgType = new DescDamageType { Type = _damageType, ValueRange = oldDmgType.ValueRange * (1f + _value), Chance = oldDmgType.Chance };
            player.Damage[_damageType] = newDmgType;
            Debug.Log($"old value range: {oldDmgType.ValueRange}");
            Debug.Log($"new value range: {newDmgType.ValueRange}");
        }

        public void Cancel(PlayerModel player)
        {
            player.AppliedBuffs.Remove(this);
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
        public BuffType Type { get; }

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
