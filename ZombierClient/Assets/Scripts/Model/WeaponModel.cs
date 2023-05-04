using UnityEngine;
using Zenject;

namespace Prototype.Model
{
    public class WeaponModel : MonoBehaviour
    {
        // Public 

        [Inject]
        public void Construct(MarkerWeaponEndPoint shootingPoint)
        {
            _shootingPoint = shootingPoint;
        }

        // Properties

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
        public Transform WeaponEndPoint => _shootingPoint.transform;
        public ProjectileModel ProjectilePrefab => _weaponSO.ProjectileData.Prefab;

        // Private

        // Dependecies

        // Injected
        private MarkerWeaponEndPoint _shootingPoint;
        //
        [SerializeField] private Data.WeaponData _weaponSO;
    }
}

