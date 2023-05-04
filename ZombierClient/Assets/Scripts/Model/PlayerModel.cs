using Prototype.Data;
using UnityEngine;
using Zenject;

namespace Prototype.Model
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerModel : MonoBehaviour
    {
        // Public

        [Inject]
        public void Construct(
            GameplaySessionData session,
            WeaponModel weaponModel,
            MarkerDefaulTargetPoint targetPoint,
            TargetHandleModel targetHandle)
        {
            _session = session;
            _weaponModel = weaponModel;
            _targetPoint = targetPoint;
            _targetHandle = targetHandle;
        }

        public enum State
        {
            NoFight,
            Fight,
            Death,
        };

        public WeaponModel WeaponModel => _weaponModel;
        public TargetHandleModel TargetHandle => _targetHandle;
        public Transform DefaultTargetPoint => _targetPoint.transform;

        public float Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                _session.Player.Speed = _speed;
            }
        }
        public float RotationSpeed => _rotationSpeed;
        public State CurrentState { get; set; }
        public EnemyModel CurrentTarget { get => _currentTarget; set => _currentTarget = value; }

        // Private 

        // Dependencies

        // Injected
        private GameplaySessionData _session;
        private WeaponModel _weaponModel;
        private MarkerDefaulTargetPoint _targetPoint;
        [SerializeField] private TargetHandleModel _targetHandle;
        //
        [SerializeField] private float _rotationSpeed = 9f;
        [SerializeField] private float _speed = 5.28f;
        [SerializeField] private EnemyModel _currentTarget;
    }
}

