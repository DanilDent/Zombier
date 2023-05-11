using Prototype.Model;
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
                    if (Vector3.Distance(enemy.transform.position, _player.transform.position) < _chaseRange)
                    {
                        enemy.Agent.SetDestination(_player.transform.position);
                    }
                    else
                    {
                        enemy.Agent.SetDestination(enemy.transform.position);
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
