using DG.Tweening;
using Prototype.Service;
using System;
using UnityEngine;
using Zenject;

namespace Prototype.View
{
    public class LevelClearedPopupUIView : MonoBehaviour
    {
        [Inject]
        public void Construct(GameEventService eventService)
        {
            _eventService = eventService;
        }

        private void OnEnable()
        {
            _eventService.LevelCleared += HandleLevelCleared;
        }

        private void OnDisable()
        {
            _eventService.LevelCleared -= HandleLevelCleared;
        }

        // Injected
        private GameEventService _eventService;
        // From inspector
        [SerializeField] private RectTransform _viewRoot;

        private void HandleLevelCleared(object sender, EventArgs e)
        {
            _viewRoot.gameObject.SetActive(true);

            Sequence sequence = DOTween.Sequence();

            float duration = 2f;
            sequence.Append(_viewRoot.DOScale(0f, duration * 0.15f).From());
            sequence.Append(_viewRoot.DOScale(1.5f, duration * 0.15f));
            sequence.Append(_viewRoot.DOScale(1f, duration * 0.20f));
            sequence.AppendInterval(duration * 0.5f);
            sequence.OnComplete(() => _viewRoot.gameObject.SetActive(false));
        }
    }
}
