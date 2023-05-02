using Prototype.SO;
using Prototype.View;
using UnityEngine;
using Zenject;

namespace Prototype.Model
{
    /// <summary>
    /// Stores enemy non-view related game data
    /// </summary>
    public class EnemyModel : MonoBehaviour
    {
        public class Factory : PlaceholderFactory<EnemySO, EnemyModel> { }

        // Dependencies 

        // From factory
        private int _health;
        private int _speed;

        // From DI container
        private MarkerView _viewParentTransform;
        private EnemyView.Factory _viewFactory;

        [Inject]
        public void Construct(EnemySO SO, MarkerView gfx, EnemyView.Factory viewFactory)
        {
            _viewParentTransform = gfx;
            _viewFactory = viewFactory;

            _health = SO.Health;
            _speed = SO.Speed;

            SetView(SO.EnemyViewPrefab);
        }

        private void SetView(EnemyView viewPrefab)
        {
            EnemyView viewInstance = _viewFactory.Create(viewPrefab);
            viewInstance.transform.SetParent(_viewParentTransform.transform);
        }
    }
}

