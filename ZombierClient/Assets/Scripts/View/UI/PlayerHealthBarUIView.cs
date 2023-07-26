using DG.Tweening;
using Prototype.Model;
using Prototype.Service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Prototype.View
{
    public class PlayerHealthBarUIView : MonoBehaviour
    {
        // Public

        [Inject]
        public void Construct(GameEventService eventService)
        {
            _eventService = eventService;
        }

        // Private

        // Injected
        private GameEventService _eventService;
        // From inspector
        [SerializeField] private Image _imgFiller;
        [SerializeField] private Image _imgFillerFollower;
        [SerializeField] private TextMeshProUGUI _text;

        private void OnEnable()
        {
            _eventService.Damaged += HandleDamaged;
            _eventService.PlayerHealthChanged += HandlePlayerHealthChanged;
        }

        private void OnDisable()
        {
            _eventService.Damaged -= HandleDamaged;
            _eventService.PlayerHealthChanged -= HandlePlayerHealthChanged;
        }

        private void HandlePlayerHealthChanged(object sender, GameEventService.PlayerHealthChangedEventArgs e)
        {
            UpdateHealth(e.Health, e.MaxHealth);
        }

        private void HandleDamaged(object sender, GameEventService.DamagedEventArgs e)
        {
            if (e.DamagedEntity is PlayerModel player)
            {
                UpdateHealth(player.Health, player.MaxHealth);
            }
        }

        private void UpdateHealth(float value, float maxValue)
        {
            float newRatio = value / maxValue;
            float difference = newRatio - _imgFiller.fillAmount;
            _imgFiller.fillAmount = value / maxValue;
            if (difference < 0)
            {
                float followDuration = 1f;
                DOTween.To(() => _imgFillerFollower.fillAmount, x => _imgFillerFollower.fillAmount = x, _imgFiller.fillAmount, followDuration);
            }
            else
            {
                _imgFillerFollower.fillAmount = _imgFiller.fillAmount;
            }


            _text.SetText($"{(int)value}/{(int)maxValue}");
        }
    }
}
