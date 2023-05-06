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
            EnemyData enemyData,
            NavMeshAgent agent,
            CharacterController characterController,
            MarkerTargetPoint targetPoint)
        {
            _agent = agent;
            _characterController = characterController;
            _targetPoint = targetPoint;

            _data = Instantiate(enemyData);
        }

        public class Factory : PlaceholderFactory<UnityEngine.Object, EnemyData, EnemyModel> { }


        // IDamageable
        public float Health { get => _data.Health; set => _data.Health = value; }
        public DescDamage Resists { get => _data.Resists; }
        //
        public float Speed => _data.Speed;
        public float AttackRange => _data.Weapon.AttackRange;
        // Gameplay properties
        public CharacterController CharacterController => _characterController;
        public NavMeshAgent Agent => _agent;
        public Transform TargetPoint => _targetPoint.transform;
        public float RotationMultiplier => _rotationMultiplier;
        public float Acceleration => _acceleration;
        public float Deceleration => _deceleration;
        public Vector3 CurrentMovement { get; set; }
        public float CurrentSpeed { get; set; }
        public bool IsMoving()
        {
            return CurrentMovement.x != 0 || CurrentMovement.z != 0;
        }

        // Private

        // Dependencies 

        // Injected
        private NavMeshAgent _agent;
        private CharacterController _characterController;
        private MarkerTargetPoint _targetPoint;
        // From factory
        private EnemyData _data;
        //
        [SerializeField] private float _rotationMultiplier;
        [SerializeField] private float _acceleration;
        [SerializeField] private float _deceleration;
    }
}

