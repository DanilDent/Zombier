using Prototype.Data;
using Prototype.Model;
using Prototype.ObjectPool;
using Prototype.Service;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

using Random = UnityEngine.Random;

namespace Prototype.Controller
{
    public class EnemyAttackController : MonoBehaviour
    {
        // Public

        [Inject]
        public void Construct(List<EnemyModel> enemies, AttackStrategyFactory strategyFactory)
        {
            _enemies = enemies;
            _strategyFactory = strategyFactory;
        }

        // Private

        //Injected
        private List<EnemyModel> _enemies;
        private AttackStrategyFactory _strategyFactory;
        //

        private void Start()
        {
            foreach (var enemy in _enemies)
            {
                DescAttackStrategy.StrategyType strategyType = enemy.AttackStrategies[Random.Range(0, enemy.AttackStrategies.Count)].Type;
                IAttackStrategy strategy = _strategyFactory.Create(strategyType, enemy);
                enemy.CurrentAttackStrategy = strategy;
            }
        }

        private void Update()
        {
            foreach (var enemy in _enemies)
            {
                if (enemy.CurrentState == EnemyModel.State.Attack)
                {
                    enemy.CurrentAttackStrategy.Execute();
                }
            }
        }
    }

    public interface IAttackStrategy
    {
        public void Execute();
    }

    public class MeleeAttackStrategy : IAttackStrategy
    {
        // Public

        public MeleeAttackStrategy(EnemyModel enemy, PlayerModel player, GameplayEventService eventService)
        {
            // Injected
            _enemy = enemy;
            _player = player;
            _eventService = eventService;

            // Initialization
            int secInMin = 60;
            float rps = (float)_enemy.AttackRateRpm / secInMin;
            _attackTimerMax = 1f / rps;

            // Events
            _eventService.EnemyAttackAnimationEvent += HandleAttackAnimationEvent;
        }

        ~MeleeAttackStrategy()
        {
            _eventService.EnemyAttackAnimationEvent -= HandleAttackAnimationEvent;
        }

        public class Factory : PlaceholderFactory<EnemyModel, MeleeAttackStrategy> { }

        public void Execute()
        {
            _attackTimer -= Time.deltaTime;
            if (_attackTimer < 0)
            {
                _attackTimer = _attackTimerMax;

                if (Vector3.Distance(_enemy.transform.position, _player.transform.position) < _enemy.AttackRange)
                {
                    _eventService.OnEnemyAttack(new GameplayEventService.EnemyAttackEventArgs { EntityId = _enemy.Id });
                }
            }
        }

        // Private

        // Injected
        private GameplayEventService _eventService;
        private PlayerModel _player;
        // From factory
        private EnemyModel _enemy;
        //
        private float _attackTimer;
        private float _attackTimerMax;

        private void HandleAttackAnimationEvent(object sender, GameplayEventService.EnemyAttackAnimationEventArgs e)
        {
            if (_enemy.Id == e.EntityId)
            {
                if (Vector3.Distance(_enemy.transform.position, _player.transform.position) < _enemy.AttackRange)
                {
                    _eventService.OnDamaged(new GameplayEventService.DamagedEventArgs { Attacker = _enemy, Defender = _player });
                }
            }
        }
    }

    public class RangeAttackStrategy : IAttackStrategy
    {
        // Public

        public RangeAttackStrategy(
            EnemyModel enemy,
            PlayerModel player,
            GameplayEventService eventService,
            MonoObjectPool<EnemyProjectileModel> projectilePool)
        {
            _player = player;
            _enemy = enemy;
            _eventService = eventService;
            _projectilePool = projectilePool;

            // Initialization
            int secInMin = 60;
            float rps = (float)_enemy.AttackRateRpm / secInMin;
            _attackTimerMax = 1f / rps;

            // Events
            _eventService.EnemyAttackAnimationEvent += HandleAttackAnimationEvent;
        }

        ~RangeAttackStrategy()
        {
            _eventService.EnemyAttackAnimationEvent -= HandleAttackAnimationEvent;
        }

        public class Factory : PlaceholderFactory<EnemyModel, RangeAttackStrategy> { }

        public void Execute()
        {
            _attackTimer -= Time.deltaTime;

            if (_attackTimer < 0)
            {
                _attackTimer = _attackTimerMax;

                if (Vector3.Distance(_enemy.transform.position, _player.transform.position) < _enemy.AttackRange)
                {
                    _eventService.OnEnemyAttack(new GameplayEventService.EnemyAttackEventArgs { EntityId = _enemy.Id });
                }
            }
        }

        // Private

        // Injected
        private GameplayEventService _eventService;
        private PlayerModel _player;
        private MonoObjectPool<EnemyProjectileModel> _projectilePool;
        // From factory
        private EnemyModel _enemy;
        //
        private float _attackTimer;
        private float _attackTimerMax;

        private void HandleAttackAnimationEvent(object sender, GameplayEventService.EnemyAttackAnimationEventArgs e)
        {
            if (_enemy.Id == e.EntityId)
            {
                Shoot();
            }
        }

        private void Shoot()
        {
            float playerHeight = 1f;
            Vector3 targetPosition = _player.transform.position + Vector3.up * (playerHeight * .5f);
            Vector3 shootDir = (targetPosition - _enemy.ShootingPoint.position).normalized;
            Quaternion rot = Quaternion.LookRotation(shootDir);

            EnemyProjectileModel projectile = _projectilePool.Create(_enemy.ProjectilePrefab, _enemy.ShootingPoint.position, rot);
            projectile.Sender = _enemy;

            projectile.Rigidbody.AddForce(shootDir * _enemy.Thrust, ForceMode.Impulse);
            float torqueMultiplier = 1000f;
            projectile.Rigidbody.AddTorque(Random.insideUnitSphere * torqueMultiplier);
        }
    }

    public class AttackStrategyFactory : PlaceholderFactory<DescAttackStrategy.StrategyType, EnemyModel, IAttackStrategy> { }

    public class AttackStrategyByStrategyTypeFactory : IFactory<DescAttackStrategy.StrategyType, EnemyModel, IAttackStrategy>
    {
        private MeleeAttackStrategy.Factory _meleeAttackStrategyFactory;
        private RangeAttackStrategy.Factory _rangeAttackStrategyFactory;

        public AttackStrategyByStrategyTypeFactory(
            MeleeAttackStrategy.Factory meleeAttackStrategyFactory,
            RangeAttackStrategy.Factory rangeAttackStrategyFactory)
        {
            _meleeAttackStrategyFactory = meleeAttackStrategyFactory;
            _rangeAttackStrategyFactory = rangeAttackStrategyFactory;
        }

        public IAttackStrategy Create(DescAttackStrategy.StrategyType type, EnemyModel enemy)
        {
            switch (type)
            {
                case DescAttackStrategy.StrategyType.Melee:
                    return _meleeAttackStrategyFactory.Create(enemy);
                case DescAttackStrategy.StrategyType.Range:
                    return _rangeAttackStrategyFactory.Create(enemy);
                default:
                    throw new NotImplementedException($"Creating of attack strategies of type {type} is not supported.");
            }
        }
    }

}