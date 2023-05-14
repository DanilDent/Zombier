using Cinemachine;
using DG.Tweening;
using Prototype.Model;
using Prototype.Service;
using System;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class CameraController : IInitializable, IDisposable
    {
        // Public

        public CameraController(
            CinemachineVirtualCamera virtualCamera,
            PlayerModel player,
            GameplayEventService eventService,
            GameUIEventService uiEventService)
        {
            _virtualCamera = virtualCamera;
            _player = player;
            _eventService = eventService;
            _uiEventService = uiEventService;
        }

        public void Initialize()
        {
            _virtualCamera.Follow = _player.transform;
            _virtualCamera.LookAt = _player.transform;
            _noise = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _noise.enabled = false;
            // Events
            _eventService.PlayerDeath += HandlePlayerDeath;
        }

        // Private

        // Injected
        private CinemachineVirtualCamera _virtualCamera;
        private PlayerModel _player;
        private GameplayEventService _eventService;
        private GameUIEventService _uiEventService;
        //
        private CinemachineBasicMultiChannelPerlin _noise;

        private void HandlePlayerDeath(object sender, EventArgs e)
        {
            Vector3 deathFollowOffset = new Vector3(0f, 10f, 0f);
            float transitionDuration = 1f;
            CinemachineTransposer transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            DOTween.To(() => transposer.m_FollowOffset, x => transposer.m_FollowOffset = x, deathFollowOffset, transitionDuration)
                .OnComplete(EnableNoise);
        }

        private void EnableNoise()
        {
            _noise.enabled = true;
            _uiEventService.OnCameraOnDeadPlayer();
        }

        public void Dispose()
        {
            _eventService.PlayerDeath -= HandlePlayerDeath;
        }
    }
}
