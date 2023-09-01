using UnityEngine;

namespace Prototype.Data
{
    public class WeaponViewData : MonoBehaviour
    {
        public WeaponViewDataNameEnum Name => _name;
        public Transform HandAim => _handAim;
        public Vector3 BodyAimOffset => _bodyAimOffset;
        public Vector3 HandAimOffset => _handAimOffset;
        public Transform LeftHand => _leftHand;
        public Transform RightHand => _rightHand;
        public Transform LeftHandHint => _leftHandHint;
        public Transform RightHandHint => _rightHandHint;
        public Transform RightHandTarget => _rightHandTarget;
        public Transform LeftHandTarget => _leftHandTarget;
        public AnimatorOverrideController AnimatorController => _animatorController;

        [SerializeField] private WeaponViewDataNameEnum _name;
        [SerializeField] private Transform _handAim;
        [SerializeField] private Vector3 _bodyAimOffset;
        [SerializeField] private Vector3 _handAimOffset;
        [SerializeField] private Transform _leftHand;
        [SerializeField] private Transform _rightHand;
        [SerializeField] private Transform _leftHandHint;
        [SerializeField] private Transform _rightHandHint;
        [SerializeField] private Transform _rightHandTarget;
        [SerializeField] private Transform _leftHandTarget;
        [SerializeField] private AnimatorOverrideController _animatorController;
    }
}

