using Prototype.ObjectPool;
using TMPro;
using UnityEngine;
using Zenject;

namespace Prototype.View
{
    public class DamageTextUIView : PoolObject
    {
        // Public 

        [Inject]
        public void Construct(TextMeshProUGUI text, RectTransform rectTransform)
        {
            _text = text;
            _rectTransform = rectTransform;

            _defaultScale = _rectTransform.localScale;
        }

        public float DisplayDuration { get; set; }
        public bool IsCrit { get; set; }

        public void SetTextValue(float value)
        {
            _text.text = ((int)value).ToString();
        }

        // Private

        // Injected
        private TextMeshProUGUI _text;
        private RectTransform _rectTransform;
        //
        private Vector3 _defaultScale;

        private void OnEnable()
        {
            if (IsCrit)
            {
                _rectTransform.localScale *= 2f;
            }
        }

        private void OnDisable()
        {
            IsCrit = false;
            _rectTransform.localScale = _defaultScale;
        }
    }
}
