using Prototype.Data;
using UnityEngine;
using Zenject;

namespace Prototype.Model
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerModel : MonoBehaviour, IDamaging
    {
        // Public

        [Inject]
        public void Construct(
            GameplaySessionData session,
            WeaponModel weaponModel,
            MarkerDefaulTargetPoint targetPoint,
            TargetHandleModel targetHandle)
        {
            _data = session.Player;
            _weaponModel = weaponModel;
            _targetPoint = targetPoint;
            _targetHandle = targetHandle;

            _damage = new DescDamage();
            RecalcDamage();
        }

        public enum State
        {
            NoFight,
            Fight,
            Death,
        };

        // IDamaging
        public DescDamage Damage => _damage;
        public float CritChance { get => _data.CritChance; set => _data.CritChance = value; }
        public float CritMultiplier { get => _data.CritMultiplier; set => _data.CritMultiplier = value; }
        // !IDamaging

        public WeaponModel WeaponModel => _weaponModel;
        public TargetHandleModel TargetHandle => _targetHandle;
        public Transform DefaultTargetPoint => _targetPoint.transform;
        public float Speed
        {
            get => _data.Speed;
            set => _data.Speed = value;
        }
        public float RotationSpeed => _rotationSpeed;
        public State CurrentState { get; set; }
        public EnemyModel CurrentTarget { get => _currentTarget; set => _currentTarget = value; }

        // Private 

        // Dependencies

        // Injected
        private WeaponModel _weaponModel;
        private MarkerDefaulTargetPoint _targetPoint;
        [SerializeField] private TargetHandleModel _targetHandle;
        //
        [SerializeField] private PlayerData _data;
        [SerializeField] private float _rotationSpeed = 9f;
        [SerializeField] private EnemyModel _currentTarget;
        private DescDamage _damage;

        private void RecalcDamage()
        {
            foreach (DescDamageType dmg in _data.Weapon.Damage)
            {
                if (_damage.FindIndex(_ => _.Type == dmg.Type) == -1)
                {
                    _damage.Add(new DescDamageType(dmg.Type));
                }

                _damage[dmg.Type] += dmg;
            }
        }
    }
}

