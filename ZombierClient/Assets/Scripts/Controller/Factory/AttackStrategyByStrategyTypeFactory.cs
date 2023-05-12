using Prototype.Data;
using Prototype.Model;
using System;
using Zenject;

namespace Prototype.Controller
{
    public class AttackStrategyByStrategyTypeFactory : IFactory<DescAttackStrategy.StrategyType, EnemyModel, IAttackStrategy>
    {
        private MeleeAttackStrategy.Factory _meleeAttackStrategyFactory;
        private RangeAttackStrategy.Factory _rangeAttackStrategyFactory;

        public AttackStrategyByStrategyTypeFactory(
            MeleeAttackStrategy.Factory meleeAttackStrategyFactory,
            RangeAttackStrategy.Factory rangeAttackStrategyFactory)
        {
            _meleeAttackStrategyFactory = meleeAttackStrategyFactory;
            _rangeAttackStrategyFactory = rangeAttackStrategyFactory;
        }

        public IAttackStrategy Create(DescAttackStrategy.StrategyType type, EnemyModel enemy)
        {
            switch (type)
            {
                case DescAttackStrategy.StrategyType.Melee:
                    return _meleeAttackStrategyFactory.Create(enemy);
                case DescAttackStrategy.StrategyType.Range:
                    return _rangeAttackStrategyFactory.Create(enemy);
                default:
                    throw new NotImplementedException($"Creating of attack strategies of type {type} is not supported.");
            }
        }
    }
}