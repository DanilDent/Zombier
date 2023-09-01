using Prototype.Data;
using Prototype.Model;
using Prototype.Service;
using Prototype.UnityComponent;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Zenject;

namespace Prototype.View
{
    public class PlayerView : MonoBehaviour
    {
        // Public 

        [Inject]
        public void Construct(
            GameEventService eventService,
            GameConfigData gameConfig,
            PlayerModel playerModel,
            Rig aimRig)
        {
            _eventService = eventService;
            _gameConfig = gameConfig;
            _playerModel = playerModel;
            _aimRig = aimRig;
        }

        public void OnPlayerShootAnimationEvent()
        {
            _eventService.OnPlayerShootAnimationEvent();
        }

        // Private 

        // Dependencies

        // Injected
        private GameEventService _eventService;
        private GameConfigData _gameConfig;
        private PlayerModel _playerModel;
        private Rig _aimRig;
        //

        private enum State
        {
            NoAim,
            Aim,
            Death,
        };


        private Animator _animator;
        private int _baseLayerIndex;
        private int _aimLayerIndex;
        private int _aimMovementLayerIndex;

        private int _velocityHash;
        private int _velocityXHash;
        private int _velocityZHash;
        private int _shootTriggerHash;
        private int _deathTriggerHash;
        private int _reviveTriggerHash;
        private int _animMultiplierHash;

        private State _state;

        [SerializeField] private float _aimTransitionMultiplier = 15f;
        private IEnumerator _smoothLayerTransitionCoroutine = null;

        // From inspector
        [SerializeField] private WeaponViewDataContainer _weaponViewDataContainer;
        [SerializeField] private Transform _handAim;
        [SerializeField] private Transform _leftHand;
        [SerializeField] private Transform _rightHand;
        [SerializeField] private Transform _leftHandHint;
        [SerializeField] private Transform _rightHandHint;
        [SerializeField] private Transform _rightHandTarget;
        [SerializeField] private Transform _leftHandTarget;
        [SerializeField] private MultiAimConstraint _bodyAimConstraint;
        [SerializeField] private MultiAimConstraint _handAimConstraint;
        //
        [SerializeField] private Transform _pistolPosition;
        [SerializeField] private Transform _riflePosition;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            _velocityHash = Animator.StringToHash("Velocity");
            _velocityXHash = Animator.StringToHash("VelocityX");
            _velocityZHash = Animator.StringToHash("VelocityZ");
            _shootTriggerHash = Animator.StringToHash("ShootTrigger");
            _deathTriggerHash = Animator.StringToHash("DeathTrigger");
            _reviveTriggerHash = Animator.StringToHash("ReviveTrigger");
            _animMultiplierHash = Animator.StringToHash("AnimMultiplier");

            _baseLayerIndex = _animator.GetLayerIndex("Base Layer");
            _aimLayerIndex = _animator.GetLayerIndex("Aim");
            _aimMovementLayerIndex = _animator.GetLayerIndex("Aim movement");
        }

        private void OnEnable()
        {
            _eventService.PlayerMoved += HandleMovementAnimations;
            _eventService.PlayerStartFight += HandleNoAimToAimTransition;
            _eventService.PlayerStopFight += HandleAimToNoAimTransition;
            _eventService.PlayerShoot += HandleShootAnimation;
            _eventService.PlayerDeath += HandlePlayerDeath;
            _eventService.PlayerRevive += HandlePlayerRevive;
            _eventService.WeaponInstantiated += HandleWeaponInstantiated;
        }

        private void OnDisable()
        {
            _eventService.PlayerMoved -= HandleMovementAnimations;
            _eventService.PlayerStartFight -= HandleNoAimToAimTransition;
            _eventService.PlayerStopFight -= HandleAimToNoAimTransition;
            _eventService.PlayerShoot -= HandleShootAnimation;
            _eventService.PlayerDeath -= HandlePlayerDeath;
            _eventService.PlayerRevive -= HandlePlayerRevive;
            _eventService.WeaponInstantiated -= HandleWeaponInstantiated;
        }

        private void HandleWeaponInstantiated(object sender, GameEventService.WeaponInstantiatedEventArgs e)
        {
            WeaponViewData weaponViewData = _weaponViewDataContainer.GetWeaponViewDataByName(e.WeaponInstance.WeaponViewDataName);

            _handAim.transform.localPosition = weaponViewData.HandAim.transform.localPosition;
            _leftHand.transform.localPosition = weaponViewData.LeftHand.transform.localPosition;
            _rightHand.transform.localPosition = weaponViewData.RightHand.transform.localPosition;

            _leftHandHint.transform.localPosition = weaponViewData.LeftHandHint.transform.localPosition;
            _leftHandHint.transform.localRotation = weaponViewData.LeftHandHint.transform.localRotation;

            _rightHandHint.transform.localPosition = weaponViewData.RightHandHint.transform.localPosition;
            _rightHandHint.transform.localRotation = weaponViewData.RightHandHint.transform.localRotation;

            _rightHandTarget.transform.localPosition = weaponViewData.RightHandTarget.transform.localPosition;
            _rightHandTarget.transform.localRotation = weaponViewData.RightHandTarget.transform.localRotation;

            _leftHandTarget.transform.localPosition = weaponViewData.LeftHandTarget.transform.localPosition;
            _leftHandTarget.transform.localRotation = weaponViewData.LeftHandTarget.transform.localRotation;

            _bodyAimConstraint.data.offset = weaponViewData.BodyAimOffset;
            _handAimConstraint.data.offset = weaponViewData.HandAimOffset;

            _animator.runtimeAnimatorController = (RuntimeAnimatorController)weaponViewData.AnimatorController;

            switch (e.WeaponInstance.WeaponViewDataName)
            {
                case WeaponViewDataNameEnum.Pistol:
                    e.WeaponInstance.GetComponent<BindTransforms>().VirtualParentTransform = _pistolPosition;
                    break;
                case WeaponViewDataNameEnum.Rifle:
                    e.WeaponInstance.GetComponent<BindTransforms>().VirtualParentTransform = _riflePosition;
                    break;
                default:
                    throw new System.Exception($"Unknown weapon view data name {e.WeaponInstance.WeaponViewDataName}");

            }
        }

        private void HandleMovementAnimations(object sender, GameEventService.PlayerMovedEventArgs e)
        {
            switch (_state)
            {
                case State.NoAim:
                    _animator.SetFloat(_velocityHash, e.Movement.magnitude);
                    UpdateAnimScaling();
                    break;
                case State.Aim:
                    Vector3 movementInLocalSpace = transform.InverseTransformDirection(e.Movement);
                    _animator.SetFloat(_velocityXHash, movementInLocalSpace.x);
                    _animator.SetFloat(_velocityZHash, movementInLocalSpace.z);
                    UpdateAnimScaling();
                    break;
                case State.Death:
                    break;
            }
        }

        private void HandleNoAimToAimTransition(object sender, EventArgs e)
        {
            _state = State.Aim;
            float targetWeight = 1f;
            _animator.SetFloat(_velocityHash, 0f);
            SetAimingRigAndLayersWeight(targetWeight);
        }

        private void HandleAimToNoAimTransition(object sender, EventArgs e)
        {
            _state = State.NoAim;
            float targetWeight = 0f;
            SetAimingRigAndLayersWeight(targetWeight);
        }

        private void HandleShootAnimation(object sender, EventArgs e)
        {
            _animator.SetTrigger(_shootTriggerHash);
        }

        private void HandlePlayerDeath(object sender, EventArgs e)
        {
            SwitchToDeathState();
            _animator.SetTrigger(_deathTriggerHash);
        }

        private void HandlePlayerRevive(object sender, EventArgs e)
        {
            _state = State.NoAim;
            _animator.SetTrigger(_reviveTriggerHash);
        }

        private void SwitchToDeathState()
        {
            _state = State.Death;
            float targetWeight = 0f;
            SetAimingRigAndLayersWeight(targetWeight);
        }

        private void SetAimingRigAndLayersWeight(float targetValue)
        {
            _aimRig.weight = targetValue;

            if (_smoothLayerTransitionCoroutine != null)
            {
                StopCoroutine(_smoothLayerTransitionCoroutine);
            }
            _smoothLayerTransitionCoroutine = SmoothTransitionToAimLayers(targetValue);
            StartCoroutine(_smoothLayerTransitionCoroutine);
        }

        private IEnumerator SmoothTransitionToAimLayers(float targetValue)
        {
            float aimLayerWeight = _animator.GetLayerWeight(_aimLayerIndex);
            float aimMovementLayerWeight = _animator.GetLayerWeight(_aimMovementLayerIndex);

            yield return null;

            while (!Mathf.Approximately(aimLayerWeight, targetValue) && !Mathf.Approximately(aimMovementLayerWeight, targetValue))
            {
                aimLayerWeight = _animator.GetLayerWeight(_aimLayerIndex);
                aimMovementLayerWeight = _animator.GetLayerWeight(_aimMovementLayerIndex);

                _animator.SetLayerWeight(_aimLayerIndex, Mathf.Lerp(aimLayerWeight, targetValue, _aimTransitionMultiplier * Time.deltaTime));
                _animator.SetLayerWeight(_aimMovementLayerIndex, Mathf.Lerp(aimMovementLayerWeight, targetValue, _aimTransitionMultiplier * Time.deltaTime));

                yield return null;
            }

            _animator.SetLayerWeight(_aimLayerIndex, targetValue);
            _animator.SetLayerWeight(_aimMovementLayerIndex, targetValue);
        }

        private void UpdateAnimScaling()
        {
            var animName = GetCurrentAnimationClipName();
            float multiplier = 1f;
            if (_gameConfig.AnimScaling.Multipliers.ContainsKey(animName))
            {
                float multiplierFromConfig = _gameConfig.AnimScaling.Multipliers[animName].Item2;
                float multiplierFromModel = (float)typeof(PlayerModel).GetProperties()
                    .FirstOrDefault(_ => _.Name.Equals(_gameConfig.AnimScaling.Multipliers[animName].Item1)).GetValue(_playerModel);
                _animator.SetFloat(_animMultiplierHash, multiplierFromConfig * multiplierFromModel);
            }
            else
            {
                _animator.SetFloat(_animMultiplierHash, multiplier);
            }
        }

        private string GetCurrentAnimationClipName()
        {
            AnimatorClipInfo[] clipInfo = _animator
                .GetCurrentAnimatorClipInfo(_state == State.NoAim ? _baseLayerIndex : _aimMovementLayerIndex);
            if (clipInfo.Length > 0)
            {
                return clipInfo[0].clip.name;
            }

            return string.Empty;
        }
    }
}