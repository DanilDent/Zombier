using Prototype.Misc;

namespace Prototype.Data
{
    public class EnemySpawnTypeData : IWeighted
    {
        public EnemyData EnemyData { get; set; }
        public float Weight { get; set; }
    }
}