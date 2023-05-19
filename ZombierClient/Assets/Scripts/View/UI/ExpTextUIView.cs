using DG.Tweening;
using Prototype.ObjectPool;
using TMPro;
using UnityEngine;
using Zenject;

namespace Prototype.View
{
    public class ExpTextUIView : PoolObject
    {
        // Public 

        [Inject]
        public void Construct(TextMeshProUGUI text, RectTransform rectTransform)
        {
            _text = text;
            _rectTransform = rectTransform;
        }

        public float DisplayDuration { get; set; }
        public bool IsCrit { get; set; }

        public void SetTextValue(float value)
        {
            _text.text = ((int)value).ToString() + " " + EXP_TEXT;
        }

        // Private

        private const string EXP_TEXT = "EXP";
        // Injected
        private TextMeshProUGUI _text;
        private RectTransform _rectTransform;
        // From inspector
        [SerializeField] private float _flyDistance;
        //
        private Vector3 _defaultScale;
        private Color _defaultColor;

        private void OnEnable()
        {
            _defaultScale = _rectTransform.localScale;
            _defaultColor = _text.color;

            _rectTransform.DOMoveY(_rectTransform.position.y + _flyDistance, DisplayDuration);
        }

        private void OnDisable()
        {
            IsCrit = false;
            _rectTransform.localScale = _defaultScale;
            _text.color = _defaultColor;
        }
    }
}
