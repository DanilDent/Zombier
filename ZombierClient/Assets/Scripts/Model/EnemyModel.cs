using Prototype.Controller;
using Prototype.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Prototype.Model
{
    /// <summary>
    /// Provides access to enemy non-view related gameplay runtime data
    /// </summary>
    public class EnemyModel : MonoBehaviour, IDamaging, IDamageable
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

            _damage = new DescDamage();
            RecalcDamage();
        }

        public class Factory : PlaceholderFactory<IdData, EnemyData, EnemyModel> { }

        public enum State
        {
            Idle = 0,
            Chase,
            Attack,
            Dead,
        };

        public IdData Id => _id;

        // IDamaging
        public DescDamage Damage => _damage;
        public float CritChance { get => _data.CritChance; set => _data.CritChance = value; }
        public float CritMultiplier { get => _data.CritMultiplier; set => _data.CritMultiplier = value; }
        // !IDamaging
        // IDamageable
        public float Health { get => _data.Health; set => _data.Health = value; }
        public float MaxHealth { get => _data.MaxHealth; set => _data.MaxHealth = value; }
        public DescDamage Resists => _data.Resists;
        // !IDamageable

        public float MaxSpeed => _data.MaxSpeed;
        public float AttackRange => _data.Weapon.AttackRange;
        public List<DescAttackStrategy> AttackStrategies => _data.AttackStrategies;
        // Gameplay properties
        public State CurrentState { get; set; }
        public IAttackStrategy CurrentAttackStrategy { get; set; }
        public NavMeshAgent Agent => _agent;
        public Rigidbody Rigidbody => _rigidbody;
        public Transform TargetPoint => _targetPoint.transform;
        public Transform ShootingPoint => _shootingPoint.transform;
        public float AttackRateRpm => _data.Weapon.AttackRateRPM;
        public EnemyProjectileModel ProjectilePrefab => _data.Weapon.ProjectileData.Prefab as EnemyProjectileModel;
        public float Thrust => _data.Weapon.Thrust;
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
        private MarkerShootingPointEnemy _shootingPoint;
        private DescDamage _damage;

        private void Start()
        {
            _shootingPoint = GetComponentInChildren<MarkerShootingPointEnemy>();
        }

        private void RecalcDamage()
        {
            foreach (DescDamageType dmg in _data.Weapon.Damage)
            {
                if (_damage.FindIndex(_ => _.Type == dmg.Type) == -1)
                {
                    _damage.Add(new DescDamageType(dmg.Type));
                }

                _damage[dmg.Type] += dmg;
            }
        }
    }
}

