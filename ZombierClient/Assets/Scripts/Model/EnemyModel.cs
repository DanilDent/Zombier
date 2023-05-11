using Prototype.Data;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Prototype.Model
{
    /// <summary>
    /// Provides access to enemy non-view related gameplay runtime data
    /// </summary>
    public class EnemyModel : MonoBehaviour, IDamageable
    {
        // Public

        [Inject]
        public void Construct(
            IdData id,
            EnemyData dataTemplate,
            NavMeshAgent agent,
            Rigidbody rigidbody,
            MarkerTargetPoint targetPoint)
        {
            _id = id;
            _data = Instantiate(dataTemplate);
            _agent = agent;
            _rigidbody = rigidbody;
            _targetPoint = targetPoint;
        }

        public class Factory : PlaceholderFactory<IdData, EnemyData, EnemyModel> { }

        public IdData Id => _id;
        // IDamageable
        public float Health { get => _data.Health; set => _data.Health = value; }
        public DescDamage Resists { get => _data.Resists; }
        //
        public float MaxSpeed => _data.MaxSpeed;
        public float AttackRange => _data.Weapon.AttackRange;
        // Gameplay properties
        public NavMeshAgent Agent => _agent;
        public Rigidbody Rigidbody => _rigidbody;
        public Transform TargetPoint => _targetPoint.transform;
        public float RotationMultiplier => _rotationMultiplier;
        public float Acceleration => _acceleration;
        public float Deceleration => _deceleration;
        public Vector3 CurrentMovement { get; set; }
        public float CurrentSpeed { get; set; }
        public float MovingForce { get; set; }
        public float StoppingForce { get; set; }
        public bool IsMoving()
        {
            return CurrentMovement.magnitude > 0;
        }

        // Private

        // Dependencies 

        // Injected
        private IdData _id;
        private NavMeshAgent _agent;
        private Rigidbody _rigidbody;
        private MarkerTargetPoint _targetPoint;
        // From factory
        private EnemyData _data;
        //
        [SerializeField] private float _rotationMultiplier;
        [SerializeField] private float _acceleration;
        [SerializeField] private float _deceleration;
    }
}

