using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using Prototype.Data;
using Prototype.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Prototype.Model
{
    /// <summary>
    /// Provides access to enemy non-view related gameplay runtime data
    /// </summary>
    public class EnemyModel : MonoBehaviour, IDamaging, IDamageable, IEffectable
    {
        // Public

        [Inject]
        public void Construct(
            IdData id,
            EnemyData dataTemplate,
            NavMeshAgent agent,
            MarkerTargetPoint targetPoint,
            FSMOwner fsmOwner,
            Blackboard blackboard)
        {
            _id = id;
            _data = dataTemplate.Copy();
            _agent = agent;
            _targetPoint = targetPoint;
            _fsmOwner = fsmOwner;
            _blackboard = blackboard;

            _damage = new DescDamage();
            RecalcDamage();
            AppliableEffects = new List<EffectConfig>();
            AppliedEffects = new List<EffectConfig>();

            CurrentState = HumanoidState.Idle;
        }

        public class Factory : PlaceholderFactory<IdData, EnemyData, EnemyModel> { }

        public IdData Id => _id;

        public HumanoidState CurrentState { get; set; }

        public List<EffectConfig> AppliableEffects { get; set; }

        public List<EffectConfig> AppliedEffects { get; set; }

        // IDamaging
        public DescDamage Damage => _damage;
        public float CritChance { get => _data.CritChance; set => _data.CritChance = value; }
        public float CritMultiplier { get => _data.CritMultiplier; set => _data.CritMultiplier = value; }
        public List<EffectConfig> DamagingEffects { get; private set; }
        // !IDamaging
        // IDamageable
        public float Health { get => _data.Health; set => _data.Health = value; }
        public float MaxHealth { get => _data.MaxHealth; set => _data.MaxHealth = value; }
        public DescDamage Resists => _data.Resists;
        // !IDamageable

        public float MaxSpeed => _data.MaxSpeed;
        public int ExpReward => _data.ExpReward;
        public float AttackRange => _data.EnemyAttack.AttackRange;
        public float Attack0SpeedMultiplier => _data.EnemyAttack.Attack0SpeedMultiplier;
        public float Attack1SpeedMultiplier => _data.EnemyAttack.Attack1SpeedMultiplier;
        public float HitFromFront0Speed => _data.HitFromFront0Speed;
        // Gameplay properties
        public NavMeshAgent Agent => _agent;
        public Transform TargetPoint => _targetPoint.transform;
        public Transform ShootingPoint => _shootingPoint.transform;
        public float AttackRateRpm => _data.EnemyAttack.AttackRateRPM;
        public float Thrust => _data.EnemyAttack.Thrust;
        public float RotationMultiplier => _rotationMultiplier;
        public float Acceleration => _acceleration;
        public float Deceleration => _deceleration;
        public Vector3 CurrentMovement { get; set; }
        public float CurrentSpeed { get; set; }
        public float MovingForce { get; set; }
        public float StoppingForce { get; set; }
        public bool IsMoving => CurrentMovement.magnitude > 0;
        public float AttackTimerMax
        {
            get
            {
                int secInMin = 60;
                float rps = (float)AttackRateRpm / secInMin;
                float attackTimerMax = 1f / rps;

                return attackTimerMax;
            }
        }
        public FSMOwner FSMOwner => _fsmOwner;
        public Blackboard Blackboard => _blackboard;

        // Private

        // Dependencies 

        // Injected
        private IdData _id;
        private NavMeshAgent _agent;
        private MarkerTargetPoint _targetPoint;
        private FSMOwner _fsmOwner;
        private Blackboard _blackboard;
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
            foreach (DescDamageType dmg in _data.EnemyAttack.Damage)
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

