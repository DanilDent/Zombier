using System;

namespace Prototype.Data
{
    [Serializable]
    public class PlayerData : BaseData
    {
        public int Health;
        public int MaxHealth;
        public float Speed;
        public IdData IdCurrentWeapon;
    }
}