using Prototype.Data;
using Prototype.View;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Prototype.Model
{
    /// <summary>
    /// Stores enemy non-view related game data
    /// </summary>
    public class EnemyModel : MonoBehaviour
    {
        // Public

        [Inject]
        public void Construct(
            EnemyData SO,
            MarkerView gfx,
            EnemyView.Factory viewFactory,
            NavMeshAgent agent,
            MarkerTargetPoint targetPoint)
        {
            _viewParentTransform = gfx;
            _viewFactory = viewFactory;
            _agent = agent;
            _targetPoint = targetPoint;

            _health = SO.Health;
            _speed = SO.Speed;

            SetView(SO.EnemyViewPrefab);
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
        public Transform TargetPoint => _targetPoint.transform;

        // Private

        // Dependencies 

        // Injected
        private MarkerView _viewParentTransform;
        private EnemyView.Factory _viewFactory;
        private NavMeshAgent _agent;
        private MarkerTargetPoint _targetPoint;
        // From factory
        private int _health;
        private int _speed;

        private void SetView(EnemyView viewPrefab)
        {
            EnemyView viewInstance = _viewFactory.Create(viewPrefab);
            viewInstance.transform.SetParent(_viewParentTransform.transform);
        }
    }
}

