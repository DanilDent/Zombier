using System;

namespace Prototype.Data
{
    [Serializable]
    public struct DescAttackStrategy
    {
        public enum StrategyType
        {
            Melee = 0,
            Range,
        }

        public StrategyType Type;
    }
}
