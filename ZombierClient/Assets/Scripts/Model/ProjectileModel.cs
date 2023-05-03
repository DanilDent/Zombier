using Prototype.SO;
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
        public int Speed => _projectileSO.Speed;

        // Private

        // Dependencies

        // Injected
        private Rigidbody _rigidbody;
        //
        [SerializeField] private ProjectileSO _projectileSO;
    }
}