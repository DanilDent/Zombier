using UnityEngine;
using UnityEngine.UI;

namespace Prototype.View
{
    [RequireComponent(typeof(Image))]
    public class HealthBarUIView : MonoBehaviour
    {
        [SerializeField] private Image _img;

        private float _value;
        private float _maxValue;
        private float _percent;

        public float Value
        {
            get => _value;
            set
            {
                _value = value;
                _img.fillAmount = (float)_value / _maxValue;
            }
        }

        public float MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                _img.fillAmount = (float)_value / _maxValue;
            }
        }

        public float Percent
        {
            get => _percent;
            set
            {
                _percent = value;
                _img.fillAmount = Mathf.Clamp(_percent, 0f, 1f);
                _value = _maxValue * _percent;
            }
        }
    }
}
