using Prototype.Model;
using Prototype.Service;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class MeleeAttackStrategy : IAttackStrategy
    {
        // Public

        public MeleeAttackStrategy(EnemyModel enemy, PlayerModel player, GameEventService eventService)
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

        public class Factory : PlaceholderFactory<EnemyModel, MeleeAttackStrategy> { }

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
        // From factory
        private EnemyModel _enemy;
        //
        private float _attackTimer;
        private float _attackTimerMax;

        private void HandleAttackAnimationEvent(object sender, GameEventService.EnemyAttackAnimationEventArgs e)
        {
            if (_enemy.Id == e.EntityId)
            {
                if (Vector3.Distance(_enemy.transform.position, _player.transform.position) < _enemy.AttackRange)
                {
                    _eventService.OnAttacked(new GameEventService.AttackedEventArgs { Attacker = _enemy, Defender = _player });
                }
            }
        }
    }
}