using Prototype.Data;
using Prototype.Service;
using UnityEngine;
using Zenject;

namespace Prototype.Model
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerModel : MonoBehaviour, IDamaging, IDamageable
    {
        // Public

        [Inject]
        public void Construct(
            MetaData meta,
            GameSessionData session,
            GameEventService eventService,
            WeaponModel weaponModel,
            MarkerDefaulTargetPoint targetPoint,
            TargetHandleModel targetHandle)
        {
            _playerMeta = meta.Player;
            _playerSession = session.Player;
            _eventService = eventService;
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
        public float CritChance { get => _playerSession.CritChance; set => _playerSession.CritChance = value; }
        public float CritMultiplier { get => _playerSession.CritMultiplier; set => _playerSession.CritMultiplier = value; }
        // !IDamaging

        // IDamageable
        public float Health { get => _playerSession.Health; set => _playerSession.Health = value; }
        public float MaxHealth { get => _playerSession.MaxHealth; set => _playerSession.MaxHealth = value; }
        public DescDamage Resists => _playerSession.Resists;
        // !IDamageable


        public int CurrentLevel { get => _playerSession.CurrentLevel; set => _playerSession.CurrentLevel = value; }
        public int CurrentExp { get => _playerSession.CurrentExp; set => _playerSession.CurrentExp = value; }
        public int CurrentLevelExpThreshold => _playerMeta.LevelExpThresholds[CurrentLevel];
        public WeaponModel WeaponModel => _weaponModel;
        public TargetHandleModel TargetHandle => _targetHandle;
        public Transform DefaultTargetPoint => _targetPoint.transform;
        public float Speed
        {
            get => _playerSession.MaxSpeed;
            set => _playerSession.MaxSpeed = value;
        }
        public float RotationSpeed => _rotationSpeed;
        public State CurrentState { get; set; }
        public EnemyModel CurrentTarget { get => _currentTarget; set => _currentTarget = value; }

        // Private 

        // Dependencies

        // Injected
        private GameEventService _eventService;
        private WeaponModel _weaponModel;
        private MarkerDefaulTargetPoint _targetPoint;
        [SerializeField] private TargetHandleModel _targetHandle;
        //
        private PlayerData _playerMeta;
        private PlayerData _playerSession;
        [SerializeField] private float _rotationSpeed = 9f;
        [SerializeField] private EnemyModel _currentTarget;
        private DescDamage _damage;

        private void RecalcDamage()
        {
            foreach (DescDamageType dmg in _playerSession.Weapon.Damage)
            {
                if (_damage.FindIndex(_ => _.Type == dmg.Type) == -1)
                {
                    _damage.Add(new DescDamageType(dmg.Type));
                }

                _damage[dmg.Type] += dmg;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<MarkerLevelExitPoint>(out var exit))
            {
                _eventService.OnPlayerEnteredExit();
            }
        }
    }
}

