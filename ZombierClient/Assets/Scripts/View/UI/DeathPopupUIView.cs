using Prototype.Service;
using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Prototype.View
{
    public class DeathPopupUIView : MonoBehaviour
    {
        [Inject]
        public void Construct(GameUIEventService uiEventService, GameEventService eventService)
        {
            _uiEventService = uiEventService;
            _eventService = eventService;
        }

        // Injected
        private GameEventService _eventService;
        private GameUIEventService _uiEventService;
        // From inspector
        [SerializeField] private RectTransform _viewRoot;
        [SerializeField] private Button _btnRevive;
        [SerializeField] private Button _btnReset;

        private void OnEnable()
        {
            _uiEventService.CameraOnDeadPlayer += HandleCameraOnDeadPlayer;
            //
            _btnRevive.onClick.AddListener(OnRevive);
            _btnReset.onClick.AddListener(OnReset);
        }

        private void OnDisable()
        {
            _uiEventService.CameraOnDeadPlayer -= HandleCameraOnDeadPlayer;
            //
            _btnRevive.onClick.RemoveAllListeners();
            _btnReset.onClick.RemoveAllListeners();
        }

        private void OnRevive()
        {
            _eventService.OnPlayerRevive();
            _viewRoot.gameObject.SetActive(false);
        }

        private void OnReset()
        {
            _eventService.OnReset();
            _viewRoot.gameObject.SetActive(false);
        }

        private void HandleCameraOnDeadPlayer(object sender, EventArgs e)
        {
            _viewRoot.gameObject.SetActive(true);
        }
    }
}
