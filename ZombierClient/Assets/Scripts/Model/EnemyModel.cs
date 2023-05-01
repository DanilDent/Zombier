using Prototype.Data;
using UnityEngine;
using Zenject;

namespace Prototype.Model
{
    public class EnemyModel : MonoBehaviour
    {
        private MarkerTargetPoint _targetPoint;

        [Inject]
        public void Construct(MarkerTargetPoint targetPoint)
        {
            _targetPoint = targetPoint;
        }

        public IdData Id { get; private set; }
        public MarkerTargetPoint TargetPoint
        {
            get => _targetPoint;
            private set => _targetPoint = value;
        }
    }
}

