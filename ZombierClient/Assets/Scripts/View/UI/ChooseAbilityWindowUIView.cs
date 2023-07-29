using Prototype.Data;
using Prototype.Service;
using System;
using UnityEngine;
using Zenject;

namespace Prototype.View
{
    public class ChooseAbilityWindowUIView : MonoBehaviour
    {
        [Inject]
        public void Construct(GameEventService gameEventService, AppEventService appEventService)
        {
            _eventService = gameEventService;
            _appEventService = appEventService;
        }

        // From inspector
        [SerializeField] private Transform _uiRoot;
        [SerializeField] private AbilityUIView _buffPrefab;
        [SerializeField] private Transform _buffsContainer;
        // Injected
        private GameEventService _eventService;
        private AppEventService _appEventService;

        private void OnEnable()
        {
            _eventService.ChooseBuffWindowOpen += HandleBuffsWindowOpen;
            _eventService.ChooseBuffWindowClose += HandleBuffsWindowClose;
        }

        private void OnDisable()
        {
            _eventService.ChooseBuffWindowOpen -= HandleBuffsWindowOpen;
            _eventService.ChooseBuffWindowClose -= HandleBuffsWindowClose;
        }

        private void HandleBuffsWindowOpen(object sender, GameEventService.ChooseBuffWindowOpenEventArgs e)
        {
            Fill(e.AvailableBuffs);
            _uiRoot.gameObject.SetActive(true);
        }

        private void HandleBuffsWindowClose(object sender, EventArgs e)
        {
            _uiRoot.gameObject.SetActive(false);
            for (int i = 0; i < _buffsContainer.childCount; ++i)
            {
                Transform child = _buffsContainer.GetChild(i);
                Destroy(child.gameObject);
            }
        }

        private void Fill(BuffConfig[] chosenBuffs)
        {
            for (int i = 0; i < chosenBuffs.Length; ++i)
            {
                AbilityUIView buffCardInstance = Instantiate(_buffPrefab, _buffsContainer);
                buffCardInstance.Init(chosenBuffs[i], _eventService, _appEventService);
            }
        }
    }
}
