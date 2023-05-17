using DG.Tweening;
using Prototype.Model;
using Prototype.Service;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Prototype.View
{
    public class GameBackgroundUIView : MonoBehaviour
    {
        [Inject]
        public void Construct(GameplayEventService eventService)
        {
            _eventService = eventService;
        }

        // Injected
        private GameplayEventService _eventService;
        // From inspector
        [SerializeField] private Image _img;
        private Color _defaultColor;

        private void OnEnable()
        {
            _defaultColor = _img.color;

            // Events
            _eventService.Damaged += HandleDamaged;
        }

        private void OnDisable()
        {
            _eventService.Damaged -= HandleDamaged;
        }

        private void HandleDamaged(object sender, GameplayEventService.DamagedEventArgs e)
        {
            if (e.DamagedEntity is PlayerModel && e.DamageValue > 0f)
            {
                Color toColor = _defaultColor;
                toColor.a = .39f;
                float duration = .5f;
                Sequence sequence = DOTween.Sequence();
                sequence.Append(_img.DOColor(toColor, duration * .25f));
                sequence.Append(_img.DOColor(_defaultColor, duration * .75f));
            }
        }
    }
}
