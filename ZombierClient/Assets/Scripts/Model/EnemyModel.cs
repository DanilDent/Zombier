using Prototype.Data;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Prototype.Model
{
    /// <summary>
    /// Provides access to enemy non-view related gameplay runtime data
    /// </summary>
    public class EnemyModel : MonoBehaviour, IDamageable
    {
        // Public

        [Inject]
        public void Construct(
            EnemyData enemyData,
            NavMeshAgent agent,
            MarkerTargetPoint targetPoint)
        {
            _agent = agent;
            _targetPoint = targetPoint;

            _data = Instantiate(enemyData);
        }

        public class Factory : PlaceholderFactory<UnityEngine.Object, EnemyData, EnemyModel> { }

        // IDamageable
        public float Health { get => _data.Health; set => _data.Health = value; }
        public DescDamage Resists { get => _data.Resists; }
        //
        public NavMeshAgent Agent => _agent;
        public Transform TargetPoint => _targetPoint.transform;

        // Private

        // Dependencies 

        // Injected
        private NavMeshAgent _agent;
        private MarkerTargetPoint _targetPoint;
        // From factory
        private EnemyData _data;
    }
}

