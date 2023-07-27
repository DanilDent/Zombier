using Prototype.Data;
using Prototype.Service;
using System.Collections.Generic;
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
            GameSessionData session,
            GameEventService eventService,
            WeaponModel weaponModel,
            MarkerDefaulTargetPoint targetPoint,
            TargetHandleModel targetHandle,
            AppData appData)
        {
            _playerSession = session.Player;
            _eventService = eventService;
            _weaponModel = weaponModel;
            _targetPoint = targetPoint;
            _targetHandle = targetHandle;
            _gameBalance = appData.GameBalance;

            _damage = new DescDamage();
            _damage = RecalcDamage();

            MaxHealth = _gameBalance.Player.PlayerConfig.MaxHealth;
        }

        public enum State
        {
            NoFight,
            Fight,
            Death,
        };

        public List<string> AppliedBuffs => _playerSession.AppliedBuffs;

        // IDamaging
        public DescDamage Damage => _damage;

        public float CritChance { get => _playerSession.CritChance; set => _playerSession.CritChance = value; }
        public float CritMultiplier { get => _playerSession.CritMultiplier; set => _playerSession.CritMultiplier = value; }
        // !IDamaging

        // IDamageable
        public float Health
        {
            get
            {
                return MaxHealth * _playerSession.HealthRatio;
            }
            set
            {
                float ratio = (float)value / MaxHealth;
                _playerSession.HealthRatio = ratio;
            }
        }

        public float HealthRatio => _playerSession.HealthRatio;

        public float MaxHealth { get; set; }
        public DescDamage Resists => _playerSession.Resists;
        // !IDamageable


        public int CurrentLevel { get => _playerSession.CurrentLevel; set => _playerSession.CurrentLevel = value; }
        public int SavedLevelUpCounter { get => _playerSession.SavedLevelUpCounter; set => _playerSession.SavedLevelUpCounter = value; }
        public int CurrentExp { get => _playerSession.CurrentExp; set => _playerSession.CurrentExp = value; }
        public int CurrentLevelExpThreshold => _playerSession.LevelExpThresholds[CurrentLevel - 1];
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
        private GameBalanceData _gameBalance;
        private GameEventService _eventService;
        private WeaponModel _weaponModel;
        private MarkerDefaulTargetPoint _targetPoint;
        [SerializeField] private TargetHandleModel _targetHandle;
        //
        private PlayerData _playerSession;
        [SerializeField] private float _rotationSpeed = 9f;
        [SerializeField] private EnemyModel _currentTarget;
        private DescDamage _damage;

        private DescDamage RecalcDamage()
        {
            DescDamage damage = new DescDamage();
            foreach (DescDamageType dmg in _playerSession.Weapon.Damage)
            {
                if (damage.FindIndex(_ => _.Type == dmg.Type) == -1)
                {
                    damage.Add(new DescDamageType(dmg.Type));
                }

                damage[dmg.Type] += dmg;
            }
            return damage;
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

