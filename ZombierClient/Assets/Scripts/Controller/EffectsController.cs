using Prototype.Data;
using Prototype.Model;
using Prototype.Service;
using Prototype.Timer;
using System;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class EffectsController : MonoBehaviour
    {
        [Inject]
        public void Construct(
            TimerService timerSerivce,
            GameEventService eventService,
            EffectModel.Factory effectFactory)
        {
            _timerService = timerSerivce;
            _eventService = eventService;
            _effectFactory = effectFactory;
        }

        // Injected
        private TimerService _timerService;
        private GameEventService _eventService;
        private EffectModel.Factory _effectFactory;

        private void OnEnable()
        {
            _eventService.EffectApplied += HandleEffectApplied;
        }

        private void OnDisable()
        {
            _eventService.EffectApplied -= HandleEffectApplied;
        }

        private void HandleEffectApplied(object sender, GameEventService.EffectAppliedEventArgs e)
        {
            switch (e.EffectConfig.EffectType)
            {
                case EffectTypeEnum.Burn:
                    ApplyBurn(e.EffectConfig, e.Target);
                    break;
                default:
                    throw new NotImplementedException($"{e.EffectConfig.EffectType} is not yet implemented");
            }
        }

        private void ApplyBurn(EffectConfig config, object target)
        {
            if (target is EnemyModel enemy)
            {
                EffectModel effect = _effectFactory.Create(config);

                _timerService.AddTimer(new TimerConfig
                   (
                   duration: (float)config.Duration,
                   tickInterval: (float)config.Interval,
                   onInit: () =>
                   {
                       if (target is IEffectable effectable)
                       {
                           effectable.AppliedEffects.Add(config);
                           _eventService.OnVisualEffectApplied(new GameEventService.VisualEffectAppliedEventArgs { TargetId = enemy.Id, EffectType = config.EffectType });
                       }
                   },
                   onTick: () =>
                   {
                       if (target is IDamageable cast && cast != null)
                       {
                           _eventService.OnDamageDealt(new GameEventService.DamageDealtEventArgs { Attacker = effect, Defender = cast });
                       }
                   },
                   onDispose: () =>
                   {
                       if (target is IEffectable effectable)
                       {
                           effectable.AppliedEffects.Remove(config);
                           _eventService.OnVisualEffectCanceled(new GameEventService.VisualEffectCanceledEventArgs { TargetId = enemy.Id, EffectType = config.EffectType });
                       }
                   },
                   target: target
                   ));
            }
        }
    }
}
