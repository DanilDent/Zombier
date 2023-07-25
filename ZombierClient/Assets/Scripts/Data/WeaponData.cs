using Sirenix.Serialization;

namespace Prototype.Data
{
    public class WeaponData
    {
        public float AttackRateRPM;
        public float AttackRange;
        public float Thrust = 3f;
        public float Recoil = 0.1f;
        [OdinSerialize] public DescDamage Damage;
        public ProjectileData ProjectileData;
    }
}