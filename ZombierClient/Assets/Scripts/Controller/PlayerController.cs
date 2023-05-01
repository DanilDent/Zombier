//using Prototype.Model;
//using Prototype.Service;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Zenject;

//namespace Prototype.Controller
//{
//    public class PlayerController : MonoBehaviour
//    {
//        private GameplayInputService _inputService;
//        private GameplayEventService _eventService;
//        private PlayerModel _playerModel;
//        private List<EnemyModel> _enemyModels;

//        private Vector3 _currentMovement;
//        private EnemyModel _currentTarget;
//        private IEnumerator _targetTransitionCoroutine = null;
//        [SerializeField] private float _targetTranstiionMultiplier = 9f;

//        [Inject]
//        public void Construct(
//            GameplayInputService inputService,
//            GameplayEventService eventService,
//            PlayerModel playerModel,
//            List<EnemyModel> enemyModels)
//        {
//            _inputService = inputService;
//            _eventService = eventService;
//            _playerModel = playerModel;
//            _enemyModels = enemyModels;
//        }

//        private void Update()
//        {
//            _currentMovement = new Vector3(_inputService.Direction.x, 0f, _inputService.Direction.y);
//            UpdatePlayerState();
//            HandleMovement();
//            switch (_playerModel.CurrentState)
//            {
//                case PlayerModel.State.NoFight:
//                    HandleRotationNoFight();
//                    break;
//                case PlayerModel.State.Fight:
//                    HandleRotationFight();
//                    break;
//            }
//        }

//        private void HandleMovement()
//        {
//            HandleGravity();

//            CharacterController characterController = _playerModel.GetComponent<CharacterController>();
//            characterController.Move(_currentMovement * _playerModel.Speed * Time.deltaTime);
//            _eventService.OnPlayerMoved(new GameplayEventService.PlayerMovedEventArgs { Movement = new Vector3(_currentMovement.x, 0f, _currentMovement.z) });
//        }

//        private void HandleRotationNoFight()
//        {
//            Vector3 positionToLookAt;

//            positionToLookAt.x = _currentMovement.x;
//            positionToLookAt.y = 0;
//            positionToLookAt.z = _currentMovement.z;

//            if (IsMoving())
//            {
//                Quaternion currentRotation = _playerModel.transform.rotation;
//                Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);

//                _playerModel.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _playerModel.RotationSpeed * Time.deltaTime);
//            }

//        }

//        private void HandleRotationFight()
//        {
//            if (_currentTarget != null)
//            {
//                Vector3 lookDirection = _currentTarget.transform.position - _playerModel.transform.position;
//                lookDirection = lookDirection.normalized;

//                Vector3 positionToLookAt;
//                positionToLookAt.x = lookDirection.x;
//                positionToLookAt.y = 0;
//                positionToLookAt.z = lookDirection.z;

//                Quaternion currentRotation = _playerModel.transform.rotation;
//                Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);

//                _playerModel.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _playerModel.RotationSpeed * Time.deltaTime);
//            }
//        }

//        private void HandleGravity()
//        {
//            float gravity = -9.8f;
//            _currentMovement.y = gravity;
//        }

//        private void UpdatePlayerState()
//        {
//            if (TryFindClosestEnemy(_playerModel, out EnemyModel enemy))
//            {
//                // Found at least one closest enemy 
//                if (_currentTarget == enemy)
//                {
//                    // Target has not changed
//                    if (Vector3.Distance(_playerModel.transform.position, enemy.transform.position) < _playerModel.WeaponModel.AttackRange)
//                    {
//                        // Target is still in attack range
//                    }
//                    else
//                    {
//                        // Target is not in attack range anymore
//                        _playerModel.CurrentState = PlayerModel.State.NoFight;
//                        UpdateCurrentTarget(null);
//                        _eventService.OnPlayerStopFight();
//                    }
//                }
//                else if (_currentTarget == null)
//                {
//                    // Player does not have a target yet
//                    if (Vector3.Distance(_playerModel.transform.position, enemy.transform.position) < _playerModel.WeaponModel.AttackRange)
//                    {
//                        // Enemy is in range of current weapon
//                        _playerModel.CurrentState = PlayerModel.State.Fight;
//                        UpdateCurrentTarget(enemy);
//                        _eventService.OnPlayerStartFight();
//                    }
//                    else
//                    {
//                        // Enemy is not in range of current weapon
//                    }
//                }
//                else
//                {
//                    // Player has other target
//                    if (Vector3.Distance(_playerModel.transform.position, enemy.transform.position) < _playerModel.WeaponModel.AttackRange)
//                    {
//                        // Enemy is in range of current weapon
//                        UpdateCurrentTarget(enemy);
//                    }
//                    else
//                    {
//                        // Enemy is not in range of current weapon
//                        _playerModel.CurrentState = PlayerModel.State.NoFight;
//                        UpdateCurrentTarget(null);
//                        _eventService.OnPlayerStopFight();
//                    }
//                }
//            }
//            else
//            {
//                // No enemies on the level
//                if (_currentTarget != null)
//                {
//                    // Current target is invalid
//                    _playerModel.CurrentState = PlayerModel.State.NoFight;
//                    UpdateCurrentTarget(null);
//                    _eventService.OnPlayerStopFight();
//                }
//            }
//        }

//        private bool IsMoving()
//        {
//            return _currentMovement.x != 0f || _currentMovement.y != 0f;
//        }

//        private bool TryFindClosestEnemy(in PlayerModel playerModel, out EnemyModel value)
//        {
//            value = null;
//            var closestDist = float.MaxValue;
//            foreach (var enemyModel in _enemyModels)
//            {
//                var dist = Vector3.Distance(playerModel.transform.position, enemyModel.transform.position);
//                if (dist < closestDist)
//                {
//                    closestDist = dist;
//                    value = enemyModel;
//                }
//            }

//            return value != null;
//        }

//        private void UpdateCurrentTarget(EnemyModel target)
//        {
//            _currentTarget = target;
//            if (_currentTarget != null)
//            {
//                UpdateTargetModel(_currentTarget.TargetPoint.transform);
//            }
//        }

//        private void UpdateTargetModel(Transform parent)
//        {
//            _playerModel.TargetHandle.transform.SetParent(parent);
//            _playerModel.TargetHandle.transform.localEulerAngles = Vector3.zero;

//            if (_targetTransitionCoroutine != null)
//            {
//                StopCoroutine(_targetTransitionCoroutine);
//            }
//            _targetTransitionCoroutine = SmoothTransitionToNewPositionCoroutine(_playerModel.TargetHandle, Vector3.zero);
//            StartCoroutine(_targetTransitionCoroutine);
//        }

//        private IEnumerator SmoothTransitionToNewPositionCoroutine(TargetHandle model, Vector3 targetPos)
//        {
//            Vector3 currentPos = model.transform.localPosition;
//            yield return null;
//            while (currentPos != targetPos)
//            {
//                currentPos = model.transform.localPosition;
//                model.transform.localPosition = Vector3.Lerp(currentPos, targetPos, _targetTranstiionMultiplier * Time.deltaTime);
//                yield return null;
//            }

//            model.transform.localPosition = Vector3.zero;
//        }
//    }
//}
