using Prototype.Model;
using Prototype.Service;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class PlayerMovementController : MonoBehaviour
    {
        private GameInputService _inputService;
        private GameEventService _eventService;
        private PlayerModel _playerModel;

        private Vector3 _currentMovement;
        private CharacterController _characterController;

        [Inject]
        public void Construct(
            GameInputService inputService,
            GameEventService eventService,
            PlayerModel playerModel)
        {
            _inputService = inputService;
            _eventService = eventService;
            _playerModel = playerModel;
        }

        private void Awake()
        {
            _characterController = _playerModel.GetComponent<CharacterController>();
        }

        private void Update()
        {
            _currentMovement = new Vector3(_inputService.Direction.x, 0f, _inputService.Direction.y);

            HandleMovement();
            switch (_playerModel.CurrentState)
            {
                case PlayerModel.State.NoFight:
                    HandleRotationNoFight();
                    break;
                case PlayerModel.State.Fight:
                    HandleRotationFight();
                    break;
            }
        }

        private void HandleMovement()
        {
            HandleGravity();
            _characterController.Move(_currentMovement * _playerModel.Speed * Time.deltaTime);
            _eventService.OnPlayerMoved(new GameEventService.PlayerMovedEventArgs { Movement = new Vector3(_currentMovement.x, 0f, _currentMovement.z) });
        }

        private void HandleRotationNoFight()
        {
            Vector3 positionToLookAt;

            positionToLookAt.x = _currentMovement.x;
            positionToLookAt.y = 0;
            positionToLookAt.z = _currentMovement.z;

            if (positionToLookAt != Vector3.zero)
            {
                Quaternion currentRotation = _playerModel.transform.rotation;
                Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
                _playerModel.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _playerModel.RotationSpeed * Time.deltaTime);
            }

        }

        private void HandleRotationFight()
        {
            if (_playerModel.CurrentTarget != null)
            {
                Vector3 lookDirection = _playerModel.CurrentTarget.transform.position - _playerModel.transform.position;
                lookDirection = lookDirection.normalized;

                Vector3 positionToLookAt;
                positionToLookAt.x = lookDirection.x;
                positionToLookAt.y = 0;
                positionToLookAt.z = lookDirection.z;

                Quaternion currentRotation = _playerModel.transform.rotation;
                Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);

                _playerModel.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _playerModel.RotationSpeed * Time.deltaTime);
            }
        }

        private void HandleGravity()
        {
            float gravity = _characterController.isGrounded ? -.05f : -9.8f;
            _currentMovement.y = gravity;
        }

        private void OnDisable()
        {
            _eventService.OnPlayerMoved(new GameEventService.PlayerMovedEventArgs { Movement = Vector3.zero });
        }
    }
}
