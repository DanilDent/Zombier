﻿using Prototype.Service;
using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Prototype.View
{
    public class DeathPopupUIView : MonoBehaviour
    {
        [Inject]
        public void Construct(GameUIEventService uiEventService, GameplayEventService eventService)
        {
            _uiEventService = uiEventService;
            _eventService = eventService;
        }

        // Injected
        private GameplayEventService _eventService;
        private GameUIEventService _uiEventService;
        // From inspector
        [SerializeField] private RectTransform _viewRoot;
        [SerializeField] private Button _btnRevive;
        [SerializeField] private Button _btnReset;
        [SerializeField] private Button _btnMainMenu;

        private void OnEnable()
        {
            _uiEventService.CameraOnDeadPlayer += HandleCameraOnDeadPlayer;
            //
            _btnRevive.onClick.AddListener(OnRevive);
            _btnReset.onClick.AddListener(OnReset);
            _btnMainMenu.onClick.AddListener(OnMainMenu);
        }

        private void OnDisable()
        {
            _uiEventService.CameraOnDeadPlayer -= HandleCameraOnDeadPlayer;
            //
            _btnRevive.onClick.RemoveAllListeners();
            _btnReset.onClick.RemoveAllListeners();
            _btnMainMenu.onClick.RemoveAllListeners();
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

        private void OnMainMenu()
        {
            _viewRoot.gameObject.SetActive(false);
            SceneLoaderService.Load(SceneLoaderService.Scene.App);
        }

        private void HandleCameraOnDeadPlayer(object sender, EventArgs e)
        {
            _viewRoot.gameObject.SetActive(true);
        }
    }
}