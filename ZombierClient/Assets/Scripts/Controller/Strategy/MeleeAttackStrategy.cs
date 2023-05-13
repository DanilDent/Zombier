using Prototype.Model;
using Prototype.Service;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
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
                    _eventService.OnAttacked(new GameplayEventService.AttackedEventArgs { Attacker = _enemy, Defender = _player });
                }
            }
        }
    }
}