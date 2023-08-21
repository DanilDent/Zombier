using Prototype.Data;
using Prototype.Model;
using Prototype.Service;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

using Random = UnityEngine.Random;

namespace Prototype.View
{
    public class EnemyView : MonoBehaviour
    {
        // Public

        [Inject]
        public void Construct(
            IdData id,
            GameEventService eventService,
            Animator animator,
            GameConfigData gameConfig,
            EnemyModel enemyModel)
        {
            _id = id;
            _eventService = eventService;
            _animator = animator;
            _gameConfig = gameConfig;
            _enemyModel = enemyModel;
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
            _eventService.OnEnemyHitEnd(new GameEventService.EnemyHitEventArgs { EntityId = _id });
        }

        public void OnAttackEndAnimationEvent()
        {
            _eventService.OnEnemyAttackEnd(new GameEventService.EnemyAttackEventArgs { EntityId = _id });
        }

        // Private

        private void Awake()
        {
            _attackTriggersHashes = new List<int>();

            _velocityHash = Animator.StringToHash("Velocity");
            _hitTriggerHash = Animator.StringToHash("HitTrigger");
            _attackTriggersHashes.Add(Animator.StringToHash("Attack0Trigger"));
            _attackTriggersHashes.Add(Animator.StringToHash("Attack1Trigger"));
            _deathTriggerHash = Animator.StringToHash("DeathTrigger");
            _animMultiplierHash = Animator.StringToHash("AnimMultiplier");

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

        private void Start()
        {
            UpdateAnimScaling();
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
        private GameConfigData _gameConfig;
        private EnemyModel _enemyModel;
        //
        private int _velocityHash;
        private int _hitTriggerHash;
        private List<int> _attackTriggersHashes;
        private int _deathTriggerHash;
        private int _animMultiplierHash;

        private void HandleMovementAnimation(object sender, GameEventService.EnemyMovedEventArgs e)
        {
            if (_id == e.Id)
            {
                _animator.SetFloat(_velocityHash, e.Value);
                UpdateAnimScaling();
            }
        }

        private void HandleHitAnimation(object sender, GameEventService.EnemyHitEventArgs e)
        {
            if (_id == e.EntityId)
            {
                if (e.PlayAnimation)
                {
                    _animator.SetTrigger(_hitTriggerHash);
                    UpdateAnimScaling();
                }
            }
        }

        private void HandleEnemyAttackAnimation(object sender, GameEventService.EnemyAttackEventArgs e)
        {
            if (_id == e.EntityId)
            {
                int attackTriggerHash = _attackTriggersHashes[Random.Range(0, _attackTriggersHashes.Count)];
                _animator.SetTrigger(attackTriggerHash);
                UpdateAnimScaling();
            }
        }

        private void HandleDeathAnimaiton(object sender, GameEventService.EnemyDeathEventArgs e)
        {
            if (e.Entity is EnemyModel cast && _id == cast.Id)
            {
                _animator.SetTrigger(_deathTriggerHash);
                UpdateAnimScaling();
            }
        }

        private string GetCurrentAnimationClipName()
        {
            AnimatorClipInfo[] clipInfo = _animator.GetCurrentAnimatorClipInfo(0);
            if (clipInfo.Length > 0)
            {
                return clipInfo[0].clip.name;
            }

            return string.Empty;
        }

        private void UpdateAnimScaling()
        {
            var animName = GetCurrentAnimationClipName();
            float multiplier = 1f;
            if (_gameConfig.AnimScaling.Multipliers.ContainsKey(animName))
            {
                float multiplierFromConfig = _gameConfig.AnimScaling.Multipliers[animName].Item2;
                float multiplierFromModel = (float)typeof(EnemyModel).GetProperties()
                    .FirstOrDefault(_ => _.Name.Equals(_gameConfig.AnimScaling.Multipliers[animName].Item1)).GetValue(_enemyModel);
                _animator.SetFloat(_animMultiplierHash, multiplierFromConfig * multiplierFromModel);
            }
            else
            {
                _animator.SetFloat(_animMultiplierHash, multiplier);
            }
        }
    }

}