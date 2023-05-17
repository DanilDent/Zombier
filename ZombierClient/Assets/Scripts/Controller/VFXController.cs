using Prototype.Model;
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
        public void Construct(
            GameEventService eventService,
            MonoObjectPool<HitBloodSplashVFXView> hitVFXPool,
            MonoObjectPool<DeathBloodSplashVFXView> deathVFXPool)
        {
            _eventService = eventService;
            _hitVFXPool = hitVFXPool;
            _deathVFXPool = deathVFXPool;
        }

        // Private

        // Injected
        private GameEventService _eventService;
        private MonoObjectPool<HitBloodSplashVFXView> _hitVFXPool;
        private MonoObjectPool<DeathBloodSplashVFXView> _deathVFXPool;

        private void OnEnable()
        {
            _eventService.EnemyHit += HandleHitVFX;
            _eventService.EnemyDeathInstant += HandleDeathVFX;
        }

        private void OnDisable()
        {
            _eventService.EnemyHit -= HandleHitVFX;
            _eventService.EnemyDeathInstant -= HandleDeathVFX;
        }

        private void HandleHitVFX(object sender, GameEventService.EnemyHitEventArgs e)
        {
            HitBloodSplashVFXView instance = _hitVFXPool.Create(e.HitPosition + Vector3.up * Random.Range(-0.2f, 0.2f), Quaternion.identity);
            StartCoroutine(_hitVFXPool.Destroy(instance, instance.Duration));
        }

        private void HandleDeathVFX(object sender, GameEventService.EnemyDeathEventArgs e)
        {
            if (e.Entity is EnemyModel cast)
            {
                float enemyHeight = 1f;
                DeathBloodSplashVFXView instance = _deathVFXPool.Create(cast.transform.position + Vector3.up * enemyHeight * 0.5f, Quaternion.identity);
                StartCoroutine(_deathVFXPool.Destroy(instance, instance.Duration));
            }
        }
    }
}
