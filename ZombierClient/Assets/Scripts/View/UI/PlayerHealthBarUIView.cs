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
        }

        private void OnDisable()
        {
            _eventService.Damaged -= HandleDamaged;
        }

        private void HandleDamaged(object sender, GameEventService.DamagedEventArgs e)
        {
            if (e.DamagedEntity is PlayerModel player)
            {
                _imgFiller.fillAmount = player.Health / player.MaxHealth;
                float followDuration = 1f;
                DOTween.To(() => _imgFillerFollower.fillAmount, x => _imgFillerFollower.fillAmount = x, _imgFiller.fillAmount, followDuration);

                _text.SetText($"{(int)player.Health}/{(int)player.MaxHealth}");
            }
        }
    }
}
