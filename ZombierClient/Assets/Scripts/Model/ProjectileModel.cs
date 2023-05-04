using Prototype.Data;
using UnityEngine;
using Zenject;

namespace Prototype.Model
{
    public class ProjectileModel : MonoBehaviour
    {
        // Public

        [Inject]
        public void Construct(Rigidbody rigidbody)
        {
            _rigidbody = rigidbody;
        }

        public class Factory : PlaceholderFactory<UnityEngine.Object, ProjectileModel> { }

        public Rigidbody Rigidbody => _rigidbody;
        public float Thrust => _projectileSO.Thrust;

        // Private

        // Dependencies

        // Injected
        private Rigidbody _rigidbody;
        //
        [SerializeField] private ProjectileData _projectileSO;

        private void OnCollisionEnter(Collision collision)
        {
            Destroy(gameObject);
        }
    }
}