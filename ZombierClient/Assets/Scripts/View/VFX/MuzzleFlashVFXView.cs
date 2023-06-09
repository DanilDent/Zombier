using Prototype.Service;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Prototype.View
{
    public class MuzzleFlashVFXView : MonoBehaviour
    {
        [Inject]
        public void Construct(GameEventService eventService, List<ParticleSystem> particleSystems)
        {
            _eventService = eventService;
            _particleSystems = particleSystems;
        }

        private void OnEnable()
        {
            _eventService.PlayerShoot += PlayVFX;
        }

        private void OnDisable()
        {
            _eventService.PlayerShoot -= PlayVFX;
        }

        private void PlayVFX(object sender, EventArgs e)
        {
            foreach (var system in _particleSystems)
            {
                if (!system.isPlaying)
                {
                    system.Play();
                }
            }
        }

        private GameEventService _eventService;
        private List<ParticleSystem> _particleSystems;
    }
}