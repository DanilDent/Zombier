using Prototype;
using Prototype.Data;
using Prototype.Model;
using Prototype.Service;
using UnityEngine;
using Zenject;

public class DealDamageController : MonoBehaviour
{
    [Inject]
    public void Construct(GameEventService eventService)
    {
        _eventService = eventService;
    }

    private void OnEnable()
    {
        _eventService.Attacked += HandleDamagedEvent;
    }

    private void OnDisable()
    {
        _eventService.Attacked -= HandleDamagedEvent;
    }

    private GameEventService _eventService;

    private void HandleDamagedEvent(object sender, GameEventService.AttackedEventArgs e)
    {
        IDamaging attacker = e.Attacker;
        IDamageable defender = e.Defender;

        float sumDmg = 0f;
        foreach (DescDamageType dmg in attacker.Damage)
        {
            bool isApplied = Helpers.TryRandom(dmg.Chance);
            sumDmg += isApplied ? dmg.Value * (1f - defender.Resists[dmg.Type].Value) : 0f;
        }

        bool isCritApplied = Helpers.TryRandom(attacker.CritChance);
        float critMultiplier = isCritApplied ? attacker.CritMultiplier : 1f;
        sumDmg *= critMultiplier;

        defender.Health -= sumDmg;
        defender.Health = Mathf.Clamp(defender.Health, 0f, defender.MaxHealth);

        IdData entityId = defender is EnemyModel cast ? cast.Id : IdData.Empty;

        _eventService.OnDamaged(new GameEventService.DamagedEventArgs
        {
            EntityId = entityId,
            DamagedEntity = defender,
            DamageValue = sumDmg,
            IsCrit = isCritApplied
        });

        if (defender.Health < 0f || Mathf.Approximately(defender.Health, 0f))
        {
            if (defender is EnemyModel enemy)
            {
                float noAnimDeathChance = 0.5f;
                if (Helpers.TryRandom(noAnimDeathChance))
                {
                    _eventService.OnEnemyDeathInstant(new GameEventService.EnemyDeathEventArgs { Entity = enemy });
                }
                else
                {
                    _eventService.OnEnemyDeath(new GameEventService.EnemyDeathEventArgs { Entity = enemy });
                }
            }
            else
            {
                _eventService.OnPlayerDeath();
            }
        }
    }
}
