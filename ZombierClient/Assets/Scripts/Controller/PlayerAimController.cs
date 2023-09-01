using Prototype.Model;
using Prototype.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class PlayerAimController : MonoBehaviour
    {
        // Public 

        [Inject]
        public void Construct(
           GameEventService eventService,
           PlayerModel player,
           List<EnemyModel> enemies)
        {
            _eventService = eventService;
            _player = player;
            _enemies = enemies;
        }

        // Private 

        // Dependencies

        // Injected
        private GameEventService _eventService;
        private PlayerModel _player;
        [SerializeField] private List<EnemyModel> _enemies;
        //
        private IEnumerator _targetTransitionCoroutine = null;
        [SerializeField] private float _targetTranstiionMultiplier = 9f;
        [SerializeField] private float _stateUpdateRate = 0.1f;

        private void OnEnable()
        {
            _eventService.EnemyPreDestroyed += HandleEnemyPreDestroyed;

            StartCoroutine(UpdatePlayerState());
        }

        private IEnumerator UpdatePlayerState()
        {
            while (true)
            {
                EnemyModel currentTarget = _player.CurrentTarget;
                if (TryFindClosestEnemy(_player, out EnemyModel enemy))
                {
                    // Found at least one closest enemy 
                    if (currentTarget == enemy)
                    {
                        // Target has not changed
                        if (Vector3.Distance(_player.transform.position, enemy.transform.position) < _player.Weapon.AttackRange)
                        {
                            // Target is still in attack range
                        }
                        else
                        {
                            // Target is not in attack range anymore
                            _player.CurrentState = PlayerModel.State.NoFight;
                            UpdateCurrentTarget(null);
                            _eventService.OnPlayerStopFight();
                        }
                    }
                    else if (currentTarget == null)
                    {
                        // Player does not have a target yet
                        if (Vector3.Distance(_player.transform.position, enemy.transform.position) < _player.Weapon.AttackRange)
                        {
                            // Enemy is in range of current weapon
                            _player.CurrentState = PlayerModel.State.Fight;
                            UpdateCurrentTarget(enemy);
                            _eventService.OnPlayerStartFight();
                        }
                        else
                        {
                            // Enemy is not in range of current weapon
                        }
                    }
                    else
                    {
                        // Player has other target
                        if (Vector3.Distance(_player.transform.position, enemy.transform.position) < _player.Weapon.AttackRange)
                        {
                            // Enemy is in range of current weapon
                            UpdateCurrentTarget(enemy);
                        }
                        else
                        {
                            // Enemy is not in range of current weapon
                            _player.CurrentState = PlayerModel.State.NoFight;
                            UpdateCurrentTarget(null);
                            _eventService.OnPlayerStopFight();
                        }
                    }
                }
                else
                {
                    // No enemies on the level
                    if (currentTarget != null)
                    {
                        // Current target is invalid
                        _player.CurrentState = PlayerModel.State.NoFight;
                        UpdateCurrentTarget(null);
                        _eventService.OnPlayerStopFight();
                    }
                    else
                    {
                        // Player doesn't have a target
                        _player.CurrentState = PlayerModel.State.NoFight;
                        UpdateCurrentTarget(null);
                        _eventService.OnPlayerStopFight();
                    }
                }

                yield return new WaitForSeconds(_stateUpdateRate);
            }
        }

        private bool TryFindClosestEnemy(in PlayerModel playerModel, out EnemyModel value)
        {
            value = null;
            var closestDist = float.MaxValue;
            foreach (var enemyModel in _enemies)
            {
                var dist = Vector3.Distance(playerModel.transform.position, enemyModel.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    value = enemyModel;
                }
            }

            return value != null;
        }

        private void UpdateCurrentTarget(EnemyModel target)
        {
            _player.CurrentTarget = target;
            if (_player.CurrentTarget != null)
            {
                UpdateTargetHandle(_player.CurrentTarget.TargetPoint);
            }
            else
            {
                UpdateTargetHandle(_player.DefaultTargetPoint);
            }
        }

        private void UpdateTargetHandle(Transform parent)
        {
            _player.TargetHandle.transform.SetParent(parent);
            _player.TargetHandle.transform.localEulerAngles = Vector3.zero;

            if (_targetTransitionCoroutine != null)
            {
                StopCoroutine(_targetTransitionCoroutine);
            }
            _targetTransitionCoroutine = SmoothTransitionToNewPositionCoroutine(_player.TargetHandle, Vector3.zero);
            StartCoroutine(_targetTransitionCoroutine);
        }

        private IEnumerator SmoothTransitionToNewPositionCoroutine(TargetHandleModel handle, Vector3 targetPos)
        {
            Vector3 currentPos = handle.transform.localPosition;
            yield return null;
            while (currentPos != targetPos)
            {
                currentPos = handle.transform.localPosition;
                handle.transform.localPosition = Vector3.Lerp(currentPos, targetPos, _targetTranstiionMultiplier * Time.deltaTime);
                yield return null;
            }

            handle.transform.localPosition = Vector3.zero;
        }

        private void HandleEnemyPreDestroyed(object sender, EventArgs e)
        {
            if (_player.CurrentTarget == null)
            {
                UpdateTargetHandle(_player.DefaultTargetPoint);
            }
            else
            {
                UpdateTargetHandle(_player.CurrentTarget.TargetPoint);
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            _targetTransitionCoroutine = null;

            _player.CurrentState = PlayerModel.State.Death;
            _player.CurrentTarget = null;
            _player.TargetHandle.transform.SetParent(_player.DefaultTargetPoint);
            _player.TargetHandle.transform.localEulerAngles = Vector3.zero;
            _player.TargetHandle.transform.localPosition = Vector3.zero;

            _eventService.EnemyPreDestroyed -= HandleEnemyPreDestroyed;

            _eventService.OnPlayerStopFight();

        }
    }
}
