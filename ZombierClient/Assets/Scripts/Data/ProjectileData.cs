using System;

namespace Prototype.Data
{
    [Serializable]
    public class ProjectileData : BaseData
    {
        public int Speed;
        public int Damage;
        //
        public BaseData Sender;
    }
}
