using Prototype.ObjectPool;
using Prototype.Service;
using UnityEngine;
using Zenject;

namespace Prototype.Model
{
    public class PlayerProjectileModel : ProjectileModelBase
    {
        public void Init(bool isBouncing = false)
        {
            _isBouncing = isBouncing;
        }

        // Private

        // Injected
        [Inject(Id = "DefaultPlayerProjectileObjectPool")] private IMonoObjectPool<PlayerProjectileModel> _defaultPool;
        [Inject(Id = "BouncePlayerProjectileObjectPool")] private IMonoObjectPool<PlayerProjectileModel> _bouncingPool;
        //
        private bool _isBouncing;

        private void OnCollisionEnter(Collision collision)
        {
            if (!_isBouncing)
            {
                HandleCollisionDefault(collision);
            }
            else
            {
                HandleCollisionBouncing(collision);
            }
        }

        private void HandleCollisionDefault(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
            {
                _eventService.OnDamageDealt(new GameEventService.DamageDealtEventArgs { Attacker = Sender, Defender = damageable });

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
            _defaultPool.Destroy(this);
        }

        private void HandleCollisionBouncing(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
            {
                _eventService.OnDamageDealt(new GameEventService.DamageDealtEventArgs { Attacker = Sender, Defender = damageable });

                if (damageable is EnemyModel cast)
                {
                    _eventService.OnEnemyHit(new GameEventService.EnemyHitEventArgs
                    {
                        EntityId = cast.Id,
                        HitDirection = _rigidbody.velocity,
                        HitPosition = transform.position
                    });
                }

                _bouncingPool.Destroy(this);
            }
        }
    }
}