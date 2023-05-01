using System;
using System.Collections.Generic;

namespace Prototype.Data
{
    [Serializable]
    public sealed class GameMetaData
    {
        public SharedData Data;
        public Dictionary<IdData, ProjectileData> Projectiles = new Dictionary<IdData, ProjectileData>();
    }
}

