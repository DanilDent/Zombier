using System;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.Data
{
    [Serializable]
    public class DescDamage : List<DescDamageType>
    {
        public DescDamageType this[DescDamageType.DamageType t]
        {
            get
            {
                return this.FirstOrDefault(_ => _.Type == t);
            }
            set
            {
                int index = this.FindIndex(_ => _.Type == t);
                this[index] = value;
            }
        }
    }
}
