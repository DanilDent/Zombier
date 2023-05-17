using Prototype.ObjectPool;
using Prototype.Service;
using UnityEngine;
using Zenject;

namespace Prototype.Model
{
    public class EnemyProjectileModel : ProjectileModelBase
    {
        // Public

        [Inject]
        public void Construct(MonoObjectPool<EnemyProjectileModel> pool)
        {
            _pool = pool;
        }

        // Private

        // Injected
        private MonoObjectPool<EnemyProjectileModel> _pool;
        //
        private float _lifeTime = 6f;

        private void Start()
        {
            StartCoroutine(_pool.Destroy(this, _lifeTime));
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
            {
                _eventService.OnAttacked(new GameEventService.AttackedEventArgs { Attacker = Sender, Defender = damageable });
            }
            _pool.Destroy(this);
        }
    }
}