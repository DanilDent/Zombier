using Prototype.Data;
using Prototype.Model;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

using Random = UnityEngine.Random;

namespace Prototype.Controller
{
    public class EnemyAttackController : MonoBehaviour
    {
        // Public

        [Inject]
        public void Construct(List<EnemyModel> enemies, AttackStrategyFactory strategyFactory)
        {
            _enemies = enemies;
            _strategyFactory = strategyFactory;
        }

        // Private

        //Injected
        private List<EnemyModel> _enemies;
        private AttackStrategyFactory _strategyFactory;
        //

        private void OnEnable()
        {
            foreach (var enemy in _enemies)
            {
                DescAttackStrategy.StrategyType strategyType = enemy.AttackStrategies[Random.Range(0, enemy.AttackStrategies.Count)].Type;
                IAttackStrategy strategy = _strategyFactory.Create(strategyType, enemy);
                enemy.CurrentAttackStrategy = strategy;
            }
        }

        private void OnDisable()
        {
            foreach (var enemy in _enemies)
            {
                enemy.CurrentAttackStrategy.Unsubscribe();
                enemy.CurrentAttackStrategy = null;
            }
        }

        private void Update()
        {
            foreach (var enemy in _enemies)
            {
                if (enemy.CurrentState == EnemyModel.State.Attack)
                {
                    enemy.CurrentAttackStrategy.Execute();
                }
            }
        }
    }
}