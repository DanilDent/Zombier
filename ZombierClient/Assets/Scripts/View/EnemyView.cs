using Prototype.Data;
using Prototype.Service;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Prototype.View
{
    public class EnemyView : MonoBehaviour
    {
        [Inject]
        public void Construct(IdData id, GameplayEventService eventService, Animator animator)
        {
            _id = id;
            _eventService = eventService;
            _animator = animator;
        }

        public class Factory : PlaceholderFactory<UnityEngine.Object, EnemyView> { }


        private void Start()
        {
            _velocityHash = Animator.StringToHash("Velocity");
            _hitDirXHash = Animator.StringToHash("HitDirX");
            _hitDirZHash = Animator.StringToHash("HitDirZ");
            _hitTriggerHash = Animator.StringToHash("HitTrigger");

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
        }

        private void OnDisable()
        {
            _eventService.EnemyMoved -= HandleMovementAnimation;
            _eventService.EnemyHit -= HandleHitAnimation;
        }

        [SerializeField] private List<string> _startStateNames;

        // Injected
        private IdData _id;
        private GameplayEventService _eventService;
        private Animator _animator;
        //
        private int _velocityHash;
        private int _hitDirXHash;
        private int _hitDirZHash;
        private int _hitTriggerHash;

        private void HandleMovementAnimation(object sender, GameplayEventService.EnemyMovedEventArgs e)
        {
            if (_id == e.Id)
            {
                _animator.SetFloat(_velocityHash, e.Value);
            }
        }

        private void HandleHitAnimation(object sender, GameplayEventService.EnemyHitEventArgs e)
        {
            if (_id == e.EntityId)
            {
                Vector3 localHitDir = transform.InverseTransformDirection(e.HitDirection);

                float hitDirX = localHitDir.x;
                float hitDirZ = localHitDir.z;

                hitDirX = Mathf.Clamp(hitDirX, -1f, 1f);
                hitDirZ = Mathf.Clamp(hitDirZ, -1f, 0f);

                _animator.SetFloat(_hitDirXHash, hitDirX);
                _animator.SetFloat(_hitDirZHash, hitDirZ);
                _animator.SetTrigger(_hitTriggerHash);
            }
        }
    }

}