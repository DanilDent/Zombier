using Prototype.Data;
using Prototype.ObjectPool;
using UnityEngine;
using Zenject;

namespace Prototype.Model
{
    public class ProjectileModel : PoolObject
    {
        // Public

        [Inject]
        public void Construct(MonoObjectPool<ProjectileModel> pool, Rigidbody rigidbody)
        {
            _pool = pool;
            _rigidbody = rigidbody;
        }

        public Rigidbody Rigidbody => _rigidbody;

        // Private

        // Dependencies

        // Injected
        private MonoObjectPool<ProjectileModel> _pool;
        private Rigidbody _rigidbody;
        //
        [SerializeField] private ProjectileData _projectileSO;

        private void OnCollisionEnter(Collision collision)
        {
            _pool.Destroy(this);
        }

        private void OnDisable()
        {
            _rigidbody.velocity = Vector3.zero;
        }
    }
}