using Prototype.Data;
using Prototype.Service;
using UnityEngine;
using Zenject;

namespace Prototype.View
{
    public class BurnVFX : VFXViewBase
    {
        [Inject]
        public void Construct(IdData id, GameEventService eventService)
        {
            _id = id;
            _eventService = eventService;
        }

        // Injected
        private IdData _id;
        private GameEventService _eventService;
        //
        private GameObject _child;

        protected override void Awake()
        {
            base.Awake();
            _child = transform.GetChild(0).gameObject;
        }

        private void OnEnable()
        {
            _eventService.VisualEffectApplied += HandleVisualEffectApplied;
            _eventService.VisualEffectCanceled += HandleVisualEffectCanceled;
        }

        private void OnDisable()
        {
            _eventService.VisualEffectApplied -= HandleVisualEffectApplied;
            _eventService.VisualEffectCanceled -= HandleVisualEffectCanceled;
        }

        private void HandleVisualEffectApplied(object sender, GameEventService.VisualEffectAppliedEventArgs e)
        {
            if (e.EffectType == EffectTypeEnum.Burn && _id.Equals(e.TargetId))
            {
                _child.SetActive(true);
                Play();
            }
        }

        private void HandleVisualEffectCanceled(object sender, GameEventService.VisualEffectCanceledEventArgs e)
        {
            if (e.EffectType == EffectTypeEnum.Burn && _id.Equals(e.TargetId))
            {
                _child.SetActive(false);
                Stop();
            }
        }
    }
}
