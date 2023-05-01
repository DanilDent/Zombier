using System;

namespace Prototype.Data
{
    [Serializable]
    public sealed class GameplaySessionData
    {
        public IdData IdCurrentLocation;
        public int CurrentStage;
        // Shared data
        public SharedData Data;
    }
}

