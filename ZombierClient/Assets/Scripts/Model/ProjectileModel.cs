using Prototype.ObjectPool;
using Prototype.Service;
using UnityEngine;
using Zenject;

namespace Prototype.Model
{
    public class ProjectileModel : PoolObject
    {
        // Public

        [Inject]
        public void Construct(GameplayEventService eventService, MonoObjectPool<ProjectileModel> pool, Rigidbody rigidbody)
        {
            _eventService = eventService;
            _pool = pool;
            _rigidbody = rigidbody;
        }

        public IDamaging Sender { get; set; }
        public Rigidbody Rigidbody => _rigidbody;

        // Private

        // Dependencies

        // Injected
        private GameplayEventService _eventService;
        private MonoObjectPool<ProjectileModel> _pool;
        private Rigidbody _rigidbody;
        //
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<IDamageable>(out var target))
            {
                _eventService.OnDamaged(new GameplayEventService.DamagedEventArgs { Attacker = Sender, Defender = target });
            }
            _pool.Destroy(this);
        }

        private void OnDisable()
        {
            _rigidbody.velocity = Vector3.zero;
        }
    }
}