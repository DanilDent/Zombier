using Prototype.Data;
using Prototype.Service;
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


        private void OnEnable()
        {
            _eventService.EnemyMoved += HandleMovementAnimation;
        }

        private void OnDisable()
        {
            _eventService.EnemyMoved -= HandleMovementAnimation;
        }

        [SerializeField] private IdData _id;
        private GameplayEventService _eventService;
        private Animator _animator;

        private void HandleMovementAnimation(object sender, GameplayEventService.EnemyMovedEventArgs e)
        {
            if (_id == e.Id)
            {
                _animator.SetFloat("Velocity", e.Value);
            }
        }
    }

}