using System;

namespace Prototype.Data
{
    [Serializable]
    public class WeaponData : BaseData
    {
        public int FireRateRPM;
        public float AttackRange;
        public IdData IdProjectile;
    }

}