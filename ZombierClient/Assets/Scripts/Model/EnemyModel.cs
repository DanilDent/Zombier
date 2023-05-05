using Prototype.Data;
using Prototype.View;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Prototype.Model
{
    /// <summary>
    /// Stores enemy non-view related gameplay runtime data
    /// </summary>
    public class EnemyModel : MonoBehaviour, IDamageable
    {
        // Public

        [Inject]
        public void Construct(
            EnemyData enemyData,
            MarkerView gfx,
            EnemyView.Factory viewFactory,
            NavMeshAgent agent,
            MarkerTargetPoint targetPoint)
        {
            _viewParentTransform = gfx;
            _viewFactory = viewFactory;
            _agent = agent;
            _targetPoint = targetPoint;

            _data = Instantiate(enemyData);
            SetView(enemyData.EnemyViewPrefab);
        }

        public class Factory : PlaceholderFactory<EnemyData, EnemyModel> { }

        public NavMeshAgent Agent
        {
            get
            {
                return _agent;
            }
            private set
            {
                _agent = value;
            }
        }

        // IDamageable
        public float Health { get => _data.Health; set => _data.Health = value; }
        public DescDamage Resists { get => _data.Resists; }
        //
        public Transform TargetPoint => _targetPoint.transform;

        // Private

        // Dependencies 

        // Injected
        private MarkerView _viewParentTransform;
        private EnemyView.Factory _viewFactory;
        private NavMeshAgent _agent;
        private MarkerTargetPoint _targetPoint;
        // From factory
        private EnemyData _data;

        private void SetView(EnemyView viewPrefab)
        {
            EnemyView viewInstance = _viewFactory.Create(viewPrefab);
            viewInstance.transform.SetParent(_viewParentTransform.transform);
        }
    }
}

