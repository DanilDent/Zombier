using Prototype.Data;
using Prototype.Model;
using Prototype.Service;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Prototype.View
{
    public class EnemyView : MonoBehaviour
    {
        // Public

        [Inject]
        public void Construct(IdData id, GameEventService eventService, Animator animator)
        {
            _id = id;
            _eventService = eventService;
            _animator = animator;
        }

        public class Factory : PlaceholderFactory<UnityEngine.Object, EnemyView> { }

        public void OnAttackAnimationEvent()
        {
            _eventService.OnEnemyAttackAnimationEvent(new GameEventService.EnemyAttackAnimationEventArgs { EntityId = _id });
        }

        public void OnDeathAnimationEvent()
        {
            _eventService.OnEnemyDeathAnimationEvent(new GameEventService.EnemyDeathAnimationEventArgs { EntityId = _id });
        }

        public void OnHitEndAnimationEvent()
        {

        }

        // Private

        private void Awake()
        {
            _attackTriggersHashes = new List<int>();

            _velocityHash = Animator.StringToHash("Velocity");
            _hitTriggerHash = Animator.StringToHash("HitTrigger");
            _attackTriggersHashes.Add(Animator.StringToHash("Attack0Trigger"));
            _attackTriggersHashes.Add(Animator.StringToHash("Attack1Trigger"));
            _deathTrigger = Animator.StringToHash("DeathTrigger");

            float offset = Random.Range(0f, 1f);
            foreach (var fullStateName in _startStateNames)
            {
                string layerName = fullStateName.Substring(0, fullStateName.IndexOf('.'));
                int layerIndex = _animator.GetLayerIndex(layerName);
                _animator.Play(fullStateName, layerIndex, offset);
            }
        }

        private void OnEnable()
        {
            _eventService.EnemyMoved += HandleMovementAnimation;
            _eventService.EnemyHit += HandleHitAnimation;
            _eventService.EnemyAttack += HandleEnemyAttackAnimation;
            _eventService.EnemyDeath += HandleDeathAnimaiton;
        }

        private void OnDisable()
        {
            _eventService.EnemyMoved -= HandleMovementAnimation;
            _eventService.EnemyHit -= HandleHitAnimation;
            _eventService.EnemyAttack -= HandleEnemyAttackAnimation;
            _eventService.EnemyDeath -= HandleDeathAnimaiton;
        }

        [SerializeField] private List<string> _startStateNames;

        // Injected
        private IdData _id;
        private GameEventService _eventService;
        private Animator _animator;
        //
        private int _velocityHash;
        private int _hitTriggerHash;
        private List<int> _attackTriggersHashes;
        private int _deathTrigger;

        private void HandleMovementAnimation(object sender, GameEventService.EnemyMovedEventArgs e)
        {
            if (_id == e.Id)
            {
                _animator.SetFloat(_velocityHash, e.Value);
            }
        }

        private void HandleHitAnimation(object sender, GameEventService.EnemyHitEventArgs e)
        {
            if (_id == e.EntityId)
            {
                _animator.SetTrigger(_hitTriggerHash);
            }
        }

        private void HandleEnemyAttackAnimation(object sender, GameEventService.EnemyAttackEventArgs e)
        {
            if (_id == e.EntityId)
            {
                int attackTriggerHash = _attackTriggersHashes[Random.Range(0, _attackTriggersHashes.Count)];
                _animator.SetTrigger(attackTriggerHash);
            }
        }

        private void HandleDeathAnimaiton(object sender, GameEventService.EnemyDeathEventArgs e)
        {
            if (e.Entity is EnemyModel cast && _id == cast.Id)
            {
                _animator.SetTrigger(_deathTrigger);
            }
        }
    }

}