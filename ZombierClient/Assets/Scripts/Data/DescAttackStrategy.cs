using Firebase.Firestore;
using System;

namespace Prototype.Data
{
    [Serializable]
    [FirestoreData]
    public struct DescAttackStrategy
    {
        [FirestoreData]
        public enum StrategyType
        {
            Melee = 0,
            Range,
        }

        [FirestoreProperty]
        public StrategyType Type { get; set; }
    }
}
