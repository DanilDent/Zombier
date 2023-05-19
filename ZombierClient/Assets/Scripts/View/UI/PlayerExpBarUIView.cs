using DG.Tweening;
using Prototype.Service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Prototype.View
{
    public class PlayerExpBarUIView : MonoBehaviour
    {
        // Public

        [Inject]
        public void Construct(GameEventService eventService)
        {
            _eventService = eventService;
        }

        // Private
        private const string EXP_TEXT = "EXP";
        // Injected
        private GameEventService _eventService;
        // From inspector
        [SerializeField] private Image _imgFiller;
        [SerializeField] private TextMeshProUGUI _text;

        private void OnEnable()
        {
            _eventService.PlayerCurrentExpChanged += HandlePlayerCurrentExpChanged;
        }

        private void OnDisable()
        {
            _eventService.PlayerCurrentExpChanged -= HandlePlayerCurrentExpChanged;
        }

        private void HandlePlayerCurrentExpChanged(object sender, GameEventService.PlayerCurrentExpChangedEventArgs e)
        {
            _text.SetText($"{e.CurrentExp} / {e.MaxExp} {EXP_TEXT}");
            DOTween.To(() => _imgFiller.fillAmount, x => _imgFiller.fillAmount = x, (float)e.CurrentExp / e.MaxExp, duration: 1f);
        }
    }
}
