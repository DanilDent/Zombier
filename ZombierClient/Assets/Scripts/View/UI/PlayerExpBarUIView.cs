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
        [SerializeField] private TextMeshProUGUI _expText;
        [SerializeField] private TextMeshProUGUI _playerLevelText;

        private void OnEnable()
        {
            _eventService.PlayerCurrentExpChanged += HandlePlayerCurrentExpChanged;
            _eventService.PlayerLevelChanged += HandlePlayerLevelChanged;
        }

        private void OnDisable()
        {
            _eventService.PlayerCurrentExpChanged -= HandlePlayerCurrentExpChanged;
            _eventService.PlayerLevelChanged -= HandlePlayerLevelChanged;
        }

        private void HandlePlayerCurrentExpChanged(object sender, GameEventService.PlayerCurrentExpChangedEventArgs e)
        {
            _expText.SetText($"{e.CurrentExp} / {e.MaxExp} {EXP_TEXT}");
            DOTween.To(() => _imgFiller.fillAmount, x => _imgFiller.fillAmount = x, (float)e.CurrentExp / e.MaxExp, duration: 1f);
        }

        private void HandlePlayerLevelChanged(object sender, GameEventService.PlayerLevelChangedEventArgs e)
        {
            _playerLevelText.text = e.Level.ToString();
        }
    }
}
