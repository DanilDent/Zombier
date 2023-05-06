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


        private void Start()
        {
            _velocityHash = Animator.StringToHash("Velocity");

            float offset = Random.Range(0f, 1f);
            int layers = 0;
            _animator.Play(START_STATE_NAME, layers, offset);
        }

        private void OnEnable()
        {
            _eventService.EnemyMoved += HandleMovementAnimation;
        }

        private void OnDisable()
        {
            _eventService.EnemyMoved -= HandleMovementAnimation;
        }

        private const string START_STATE_NAME = "Base Layer.Locomotion";

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