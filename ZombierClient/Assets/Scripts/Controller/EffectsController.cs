using Prototype.Service;
using Prototype.Timer;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class EffectsController : MonoBehaviour
    {
        [Inject]
        public void Construct(TimerService timerSerivce, GameEventService eventService)
        {
            _timerService = timerSerivce;
            _eventService = eventService;
        }

        private TimerService _timerService;
        private GameEventService _eventService;

        //private void HandleEffectApplied(EffectConfig effectCfg, IDamageable target)
        //{
        //    EffectModel effect = _effectFactory.Create(effectCfg);

        //    Action onInit;
        //    Action onTick;
        //    Action onDispose;

        //    switch(effectCfg.EffectType)
        //    {
        //        case EffectEnum.Burn:
        //            onInit = BurnOnInit;
        //            onTick = BurnOnTick;
        //            onDispose = BurnOnDispose;
        //            break;
        //        case EffectEnum.Frost:
        //            onInit = FrostOnInit;
        //            onTick = FrostOnTick;
        //            onDispose = FrostOnDispose;
        //            break;
        //        case EffectEnum.Poison:
        //            onInit = PoisonOnInit;
        //            onTick PoisonOnTick;
        //            onDispose = PoisonOnDispose;
        //            break;
        //    }

        //    _timerService.AddTimer(new TimerConfig
        //        (
        //        duration: effectCfg.Duration,
        //        tickInterval: effectCfg.Interval,
        //        onInit: onInit,
        //        onTick: onTick,
        //        onDispose: onDispose
        //        ));
        //}

        //private void BurnOnInit()
        //{
        //    _eventService.OnVisualEffectApplied(EffectEnum.Burn, target);
        //}

        //private void BurnOnTick()
        //{
        //    _eventService.OnDamageDealt(new GameEventService.DamageDealtEventArgs { Attacker = EffectEnum, Defender = target });
        //}

        //private void BurnOnDispose()
        //{
        //    _eventService.OnVisualEffectCanceled(EffectEnum.Burn, target);
        //}
    }
}
