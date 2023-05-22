using Prototype.Data;
using UnityEngine;
using Zenject;

namespace Prototype.Model
{
    public class WeaponModel : MonoBehaviour
    {
        // Public 

        [Inject]
        public void Construct(GameSessionData session, MarkerShootingPointPlayer shootingPoint)
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
                return _weaponData.AttackRateRPM;
            }
            set
            {
                _weaponData.AttackRateRPM = value;
            }
        }
        public Transform ShootingPoint => _shootingPoint.transform;
        public PlayerProjectileModel ProjectilePrefab => Resources.Load<PlayerProjectileModel>(_weaponData.ProjectileData.AssetPath);
        public float Thrust => _weaponData.Thrust;
        public float Recoil => _weaponData.Recoil;

        // Private

        // Dependecies

        // Injected
        private MarkerShootingPointPlayer _shootingPoint;
        //
        private WeaponData _weaponData;
    }
}

