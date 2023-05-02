using Prototype.Data;
using UnityEngine;
using Zenject;

namespace Prototype.Model
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerModel : MonoBehaviour
    {
        private GameplaySessionData _session;
        private WeaponModel _weaponModel;
        private MarkerDefaulTargetPoint _targetPoint;
        private TargetHandleModel _targetHandle;

        [SerializeField] private float _rotationSpeed = 9f;
        [SerializeField] private float _speed = 5.28f;

        private EnemyModel _currentTarget;


        [Inject]
        public void Construct(
            GameplaySessionData session,
            WeaponModel weaponModel,
            MarkerDefaulTargetPoint targetPoint,
            TargetHandleModel targetModel)
        {
            _session = session;
            _weaponModel = weaponModel;
            _targetPoint = targetPoint;
            _targetHandle = targetModel;
        }


        public IdData Id { get; private set; }

        public WeaponModel WeaponModel => _weaponModel;

        public TargetHandleModel TargetHandle => _targetHandle;

        public int Health
        {
            get => _session.Data.Player.Health;
            set => _session.Data.Player.Health = value;
        }

        [SerializeField]
        public float Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                _session.Data.Player.Speed = _speed;
            }
        }

        public float RotationSpeed => _rotationSpeed;

        public enum State
        {
            NoFight,
            Fight,
            Death,
        };

        public State CurrentState { get; set; }
        public EnemyModel CurrentTarget { get; set; }

        public bool IsAlive()
        {
            return Health > 0;
        }
    }
}

