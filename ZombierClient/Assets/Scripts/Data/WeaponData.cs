using System;

namespace Prototype.Data
{
    [Serializable]
    public class WeaponData : BaseData
    {
        public float AttackRange;
        public float FireRateRPM;
        public IdData IdProjectile;
    }

}