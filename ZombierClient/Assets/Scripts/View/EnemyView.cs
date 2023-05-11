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
        }

        private void OnDisable()
        {
            _eventService.EnemyMoved -= HandleMovementAnimation;
        }

        [SerializeField] private List<string> _startStateNames;

        // Injected
        private IdData _id;
        private GameplayEventService _eventService;
        private Animator _animator;
        //
        private int _velocityHash;

        private void HandleMovementAnimation(object sender, GameplayEventService.EnemyMovedEventArgs e)
        {
            if (_id == e.Id)
            {
                _animator.SetFloat(_velocityHash, e.Value);
            }
        }
    }

}