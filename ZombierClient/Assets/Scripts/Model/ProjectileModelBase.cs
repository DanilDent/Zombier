using Prototype.ObjectPool;
using Prototype.Service;
using UnityEngine;
using Zenject;

namespace Prototype.Model
{
    public abstract class ProjectileModelBase : PoolObject
    {
        // Public

        [Inject]
        public void Construct(GameEventService eventService, Rigidbody rigidbody)
        {
            _eventService = eventService;
            _rigidbody = rigidbody;
        }

        public IDamaging Sender { get; set; }
        public Rigidbody Rigidbody => _rigidbody;

        // Private

        // Dependencies

        // Injected
        protected GameEventService _eventService;
        protected Rigidbody _rigidbody;
        //

        protected virtual void OnDisable()
        {
            _rigidbody.velocity = Vector3.zero;
        }
    }
}