using Prototype.Data;
using TMPro;
using UnityEngine;

namespace Prototype.View
{
    public class PlayerHealthBarUIView : MonoBehaviour
    {
        [SerializeField] private HealthBarUIView __imgHealthBar;
        [SerializeField] private TextMeshProUGUI textHealthBar;

        protected void SetValue(in object sender, in float value, in float maxValue)
        {
            if (sender is PlayerData)
            {
                textHealthBar.text = $"{value}/{maxValue}";
                __imgHealthBar.MaxValue = maxValue;
                __imgHealthBar.Value = value;
            }
        }
    }
}
