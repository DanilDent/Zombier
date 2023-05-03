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
        public class Factory : PlaceholderFactory<EnemySO, EnemyModel> { }

        [Inject]
        public void Construct(EnemySO SO, MarkerView gfx, EnemyView.Factory viewFactory, NavMeshAgent agent)
        {
            _viewParentTransform = gfx;
            _viewFactory = viewFactory;
            _agent = agent;

            _health = SO.Health;
            _speed = SO.Speed;

            SetView(SO.EnemyViewPrefab);
        }

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

        // Private

        // Dependencies 

        // From factory
        private int _health;
        private int _speed;

        // From DI container
        private MarkerView _viewParentTransform;
        private EnemyView.Factory _viewFactory;
        private NavMeshAgent _agent;

        private void SetView(EnemyView viewPrefab)
        {
            EnemyView viewInstance = _viewFactory.Create(viewPrefab);
            viewInstance.transform.SetParent(_viewParentTransform.transform);
        }
    }
}

