using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Anim Scaling Config Data", menuName = "Data/Anim Scaling Config Data")]
    public class AnimScalingConfigData : SerializedScriptableObject
    {
        public Dictionary<string, Tuple<string, float>> Multipliers => _multipliers;

        [OdinSerialize] private Dictionary<string, Tuple<string, float>> _multipliers;
    }
}
