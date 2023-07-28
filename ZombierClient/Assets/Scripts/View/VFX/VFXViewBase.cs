using UnityEngine;

namespace Prototype.View
{
    public class VFXViewBase : MonoBehaviour
    {
        public void Play()
        {
            foreach (var ps in _particleSystems)
            {
                ps.Play();
            }
        }

        public void Stop()
        {
            foreach (var ps in _particleSystems)
            {
                ps.Stop();
            }
        }

        protected ParticleSystem[] _particleSystems;

        protected virtual void Awake()
        {
            _particleSystems = GetComponentsInChildren<ParticleSystem>();
        }
    }

}