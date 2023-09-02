using Prototype.Model;
using UnityEngine;

namespace Prototype.Misc
{
    public class DisableIfTargetNotInRange : MonoBehaviour
    {
        public Transform Target
        {
            private get => _target;
            set
            {
                _target = value;
            }
        }

        public void SetDistanceComp()
        {
            _distanceCheckGO = GetComponentInChildren<EnemyModel>().transform;
        }

        [SerializeField] private float _range = 20;
        [SerializeField] private Transform _target;
        private Transform _distanceCheckGO;

        private void Update()
        {
            if (_target == null || _distanceCheckGO == null)
            {
                return;
            }

            if (Vector3.Distance(_distanceCheckGO.position, _target.position) < _range)
            {
                SetActiveChildren(true);
            }
            else
            {
                SetActiveChildren(false);
            }
        }

        private void SetActiveChildren(bool enabled)
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(enabled);
            }
        }
    }
}
