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

        public CameraController(CinemachineVirtualCamera virtualCamera, PlayerModel player, GameplayEventService eventService)
        {
            _virtualCamera = virtualCamera;
            _player = player;
            _eventService = eventService;
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
        //
        private CinemachineBasicMultiChannelPerlin _noise;

        private void HandlePlayerDeath(object sender, EventArgs e)
        {
            Vector3 deathFollowOffset = new Vector3(0f, 10f, 0f);
            float transitionDuration = 1f;
            CinemachineTransposer transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            DOTween.To(() => transposer.m_FollowOffset, x => transposer.m_FollowOffset = x, deathFollowOffset, transitionDuration)
                .OnComplete(() => _noise.enabled = true);
        }

        public void Dispose()
        {
            _eventService.PlayerDeath -= HandlePlayerDeath;
        }
    }
}
