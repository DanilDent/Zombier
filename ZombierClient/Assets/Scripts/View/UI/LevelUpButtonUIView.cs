using DG.Tweening;
using Prototype.Service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Prototype.View
{
    public class LevelUpButtonUIView : MonoBehaviour
    {
        [Inject]
        public void Construct(GameEventService eventService)
        {
            _eventService = eventService;
        }

        // Injected
        private GameEventService _eventService;
        // From inspector
        [SerializeField] private RectTransform _viewRoot;
        [SerializeField] private Button _btnLevelUp;
        [SerializeField] private TextMeshProUGUI _savedLevelUpCounterText;

        private void OnEnable()
        {
            _btnLevelUp.onClick.AddListener(OnLevelUpBtnClick);
            //
            _eventService.PlayerLevelChanged += HandlePlayerLevelChanged;
        }

        private void OnDisable()
        {
            _btnLevelUp.onClick.RemoveAllListeners();
            //
            _eventService.PlayerLevelChanged -= HandlePlayerLevelChanged;
        }

        private void HandlePlayerLevelChanged(object sender, GameEventService.PlayerLevelChangedEventArgs e)
        {
            if (e.Level > 1)
            {
                _viewRoot.gameObject.SetActive(true);
                Sequence sequence = DOTween.Sequence();
                float duration = 1f;
                sequence.Append(_viewRoot.transform.DOScale(1.5f, duration / 2f));
                sequence.Append(_viewRoot.transform.DOScale(1f, duration / 2f));

                if (e.SavedLevelUps > 1)
                {
                    _savedLevelUpCounterText.gameObject.SetActive(true);
                    _savedLevelUpCounterText.text = e.SavedLevelUps.ToString();
                }
            }
        }

        private void OnLevelUpBtnClick()
        {
            _savedLevelUpCounterText.gameObject.SetActive(false);
            _viewRoot.gameObject.SetActive(false);
        }
    }
}
