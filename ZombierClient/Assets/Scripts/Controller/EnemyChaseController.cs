using Prototype.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class EnemyChaseController : MonoBehaviour
    {
        // Public

        [Inject]
        public void Construct(PlayerModel player, List<EnemyModel> enemies)
        {
            _player = player;
            _enemies = enemies;
        }

        // Private

        private void Start()
        {
            foreach (var enemy in _enemies)
            {
                enemy.Agent.stoppingDistance = enemy.AttackRange;
            }

            StartCoroutine(UpdateChaseDestination());
        }

        private IEnumerator UpdateChaseDestination()
        {
            while (true)
            {
                foreach (var enemy in _enemies)
                {
                    switch (enemy.CurrentState)
                    {
                        case EnemyModel.State.Idle:
                            // Enemy is waiting
                            if (Vector3.Distance(enemy.transform.position, _player.transform.position) < _chaseRange)
                            {
                                // Enemy first detected player in chase range
                                enemy.Agent.SetDestination(_player.transform.position);
                                enemy.CurrentState = EnemyModel.State.Chase;
                            }
                            break;
                        case EnemyModel.State.Chase:
                            // Enemy is chasing
                            if (Vector3.Distance(enemy.transform.position, _player.transform.position) < _chaseRange)
                            {
                                // Enemy continues chasing the player
                                enemy.Agent.SetDestination(_player.transform.position);
                            }
                            else if (Vector3.Distance(enemy.transform.position, _player.transform.position) > _chaseRange)
                            {
                                // Player leaved chase range
                                enemy.Agent.SetDestination(enemy.transform.position);
                                enemy.CurrentState = EnemyModel.State.Idle;
                            }

                            if (Vector3.Distance(enemy.transform.position, _player.transform.position) < enemy.AttackRange)
                            {
                                // Player is in attack range 
                                enemy.CurrentState = EnemyModel.State.Attack;
                            }
                            break;
                        case EnemyModel.State.Attack:
                            // Enemy is attacking
                            if (Vector3.Distance(enemy.transform.position, _player.transform.position) > enemy.AttackRange)
                            {
                                // Player leaved attack range
                                enemy.CurrentState = EnemyModel.State.Chase;
                            }
                            break;
                        case EnemyModel.State.Dead:
                            // Enemy is dead
                            break;
                        default:
                            throw new NotImplementedException($"Enemy {enemy} is in unknown state {enemy.CurrentState}");
                    }
                    Debug.DrawRay(enemy.Agent.destination, Vector3.up * 2f, Color.red, _destUpdateRate);
                }

                yield return new WaitForSeconds(_destUpdateRate);
            }
        }

        // Injected
        private PlayerModel _player;
        private List<EnemyModel> _enemies;
        //
        [SerializeField] private float _chaseRange;
        [SerializeField] private float _destUpdateRate = 0.1f;
    }
}
