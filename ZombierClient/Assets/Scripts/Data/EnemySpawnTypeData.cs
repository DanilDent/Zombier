using Prototype.Misc;

namespace Prototype.Data
{
    public class EnemySpawnTypeData : IWeighted
    {
        public string EnemyId { get; set; }
        public float Weight { get; set; }
    }
}