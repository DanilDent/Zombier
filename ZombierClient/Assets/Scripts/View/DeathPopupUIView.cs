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
        public void Construct(GameUIEventService uiEventService)
        {
            _uiEventService = uiEventService;
        }

        // Injected
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

        }

        private void OnReset()
        {

        }

        private void HandleCameraOnDeadPlayer(object sender, EventArgs e)
        {
            _viewRoot.gameObject.SetActive(true);
        }
    }
}
