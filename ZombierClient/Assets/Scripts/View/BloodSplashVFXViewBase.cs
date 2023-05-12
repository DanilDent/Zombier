using Prototype.ObjectPool;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Prototype.View
{
    public abstract class BloodSplashVFXViewBase : PoolObject
    {
        // Public

        [Inject]
        public void Construct(List<ParticleSystem> particleSystems)
        {
            _particleSystems = particleSystems;
        }

        public float Duration => _particleSystems[0].main.duration;

        // Protected

        // Injected
        protected Transform _ground;
        protected List<ParticleSystem> _particleSystems;

        protected void Start()
        {
            _ground = FindObjectOfType<MarkerGround>().transform;

            foreach (var system in _particleSystems)
            {
                if (system.collision.enabled)
                {
                    system.collision.SetPlane(0, _ground);
                }
            }
        }

        protected virtual void OnEnable()
        {
            foreach (var system in _particleSystems)
            {
                system.Play();
            }
        }

        protected virtual void OnDisable()
        {
            foreach (var system in _particleSystems)
            {
                system.Stop();
            }
        }
    }
}
