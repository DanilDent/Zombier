using Prototype.ObjectPool;
using Prototype.Service;
using Prototype.View;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class VFXController : MonoBehaviour
    {
        // Public

        [Inject]
        public void Construct(GameplayEventService eventService, MonoObjectPool<HitBloodSplashVFXView> hitVFXPool)
        {
            _eventService = eventService;
            _hitVFXPool = hitVFXPool;
        }

        // Private

        // Injected
        private GameplayEventService _eventService;
        private MonoObjectPool<HitBloodSplashVFXView> _hitVFXPool;

        private void OnEnable()
        {
            _eventService.EnemyHit += HandleHitVFX;
        }

        private void OnDisable()
        {
            _eventService.EnemyHit -= HandleHitVFX;
        }

        private void HandleHitVFX(object sender, GameplayEventService.EnemyHitEventArgs e)
        {
            HitBloodSplashVFXView instance = _hitVFXPool.Create(e.HitPosition + Vector3.up * Random.Range(-0.5f, 0.5f), Quaternion.identity);
            StartCoroutine(_hitVFXPool.Destroy(instance, instance.Duration));
        }
    }
}
