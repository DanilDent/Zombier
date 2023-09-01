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

        public class Factory : PlaceholderFactory<UnityEngine.Object, WeaponModel> { }

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
        public float FireRateRPM
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

        public WeaponViewDataNameEnum WeaponViewDataName => _weaponViewDataName;

        public DescDamage Damage => _weaponData.Damage;

        public Transform ShootingPoint => _shootingPoint.transform;

        public float Thrust => _weaponData.Thrust;

        public float Recoil => _weaponData.Recoil;

        // Private

        // Injected
        private MarkerShootingPointPlayer _shootingPoint;
        //
        private WeaponData _weaponData;
        [SerializeField] private WeaponViewDataNameEnum _weaponViewDataName;
    }
}