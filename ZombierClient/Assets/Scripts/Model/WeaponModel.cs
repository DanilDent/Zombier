using Prototype.Data;
using UnityEngine;
using Zenject;

namespace Prototype.Model
{
    public class WeaponModel : MonoBehaviour
    {
        // Public 

        [Inject]
        public void Construct(GameplaySessionData session, MarkerWeaponEndPoint shootingPoint)
        {
            _weaponData = session.Player.Weapon;
            _shootingPoint = shootingPoint;
        }

        // Properties

        public float AttackRange
        {
            get
            {
                return _weaponData.AttackRange;
            }
            set
            {
                _weaponData.AttackRange = value;
            }
        }
        public int FireRateRPM
        {
            get
            {
                return _weaponData.FireRateRPM;
            }
            set
            {
                _weaponData.FireRateRPM = value;
            }
        }
        public Transform WeaponEndPoint => _shootingPoint.transform;
        public ProjectileModel ProjectilePrefab => _weaponData.ProjectileData.Prefab;
        public float Thrust => _weaponData.Thrust;
        public float Recoil => _weaponData.Recoil;

        // Private

        // Dependecies

        // Injected
        private MarkerWeaponEndPoint _shootingPoint;
        //
        private WeaponData _weaponData;
    }
}

