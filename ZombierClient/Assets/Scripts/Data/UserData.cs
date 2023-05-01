using System;

namespace Prototype.Data
{
    [Serializable]
    public sealed class UserData
    {
        public SharedData Data;
        public GameplaySessionData Session;
    }
}

