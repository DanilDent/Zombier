using Prototype.Data;
using Prototype.SO;
using UnityEngine;
using Zenject;

namespace Prototype.Model
{
    public class WeaponModel : MonoBehaviour
    {
        // Public 

        [Inject]
        public void Construct(GameplaySessionData session, MarkerShootingPoint shootingPoint)
        {
            _session = session;
            _shootingPoint = shootingPoint;
        }
        // Properties
        public IdData Id
        {
            get => _id;
            private set { _id = value; }
        }
        public float AttackRange
        {
            get
            {
                return _weaponSO.AttackRange;
            }
            set
            {
                _weaponSO.AttackRange = value;
            }
        }
        public int FireRateRPM
        {
            get
            {
                return _weaponSO.FireRateRPM;
            }
            set
            {
                _weaponSO.FireRateRPM = value;
            }
        }
        public Transform ShootingPoint => _shootingPoint.transform;
        public ProjectileModel ProjectilePrefab => _weaponSO.ProjectileSO.Prefab;

        // Private

        // Dependecies

        // Injected
        private GameplaySessionData _session;
        private MarkerShootingPoint _shootingPoint;
        //
        [SerializeField] private WeaponSO _weaponSO;
        [SerializeField] private IdData _id;
    }
}

