using Prototype.Data;
using Prototype.Model;
using Zenject;

namespace Prototype.Controller
{
    public class AttackStrategyFactory : PlaceholderFactory<DescAttackStrategy.StrategyType, EnemyModel, IAttackStrategy> { }
}