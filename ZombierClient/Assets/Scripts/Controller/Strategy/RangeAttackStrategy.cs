using Prototype.Model;
using Prototype.ObjectPool;
using Prototype.Service;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class RangeAttackStrategy : IAttackStrategy
    {
        // Public

        public RangeAttackStrategy(
            EnemyModel enemy,
            PlayerModel player,
            GameEventService eventService,
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
            Subscribe();
        }

        public void Subscribe()
        {
            _eventService.EnemyAttackAnimationEvent += HandleAttackAnimationEvent;
        }

        public void Unsubscribe()
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
                    _eventService.OnEnemyAttack(new GameEventService.EnemyAttackEventArgs { EntityId = _enemy.Id });
                }
            }
        }

        // Private

        // Injected
        private GameEventService _eventService;
        private PlayerModel _player;
        private MonoObjectPool<EnemyProjectileModel> _projectilePool;
        // From factory
        private EnemyModel _enemy;
        //
        private float _attackTimer;
        private float _attackTimerMax;

        private void HandleAttackAnimationEvent(object sender, GameEventService.EnemyAttackAnimationEventArgs e)
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

            EnemyProjectileModel projectile = _projectilePool.Create(_enemy.ShootingPoint.position, rot);
            projectile.Sender = _enemy;

            float randomThrustMultiplier = 1.5f;
            projectile.Rigidbody.AddForce(shootDir * Random.Range(_enemy.Thrust, _enemy.Thrust * randomThrustMultiplier), ForceMode.Impulse);
            float torqueMultiplier = 1000f;
            projectile.Rigidbody.AddTorque(Random.insideUnitSphere * torqueMultiplier);
        }
    }
}