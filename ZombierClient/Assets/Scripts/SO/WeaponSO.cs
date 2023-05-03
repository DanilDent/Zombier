using UnityEngine;

namespace Prototype.SO
{
    [CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon Data")]
    public class WeaponSO : ScriptableObject
    {
        public int FireRateRPM;
        public float AttackRange;
        public ProjectileSO ProjectileSO;
    }
}