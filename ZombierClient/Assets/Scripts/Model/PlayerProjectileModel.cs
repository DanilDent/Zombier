using Prototype.ObjectPool;
using Prototype.Service;
using UnityEngine;
using Zenject;

namespace Prototype.Model
{
    public class PlayerProjectileModel : ProjectileModelBase
    {
        // Public

        [Inject]
        public void Construct(MonoObjectPool<PlayerProjectileModel> pool)
        {
            _pool = pool;
        }

        // Private

        // Injected
        private MonoObjectPool<PlayerProjectileModel> _pool;
        //

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
            {
                _eventService.OnAttacked(new GameEventService.AttackedEventArgs { Attacker = Sender, Defender = damageable });

                if (damageable is EnemyModel cast)
                {
                    _eventService.OnEnemyHit(new GameEventService.EnemyHitEventArgs
                    {
                        EntityId = cast.Id,
                        HitDirection = _rigidbody.velocity,
                        HitPosition = transform.position
                    });
                }
            }
            _pool.Destroy(this);
        }
    }
}