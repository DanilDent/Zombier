using DG.Tweening;
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

            if (IsCrit)
            {
                float scaleTo = Random.Range(1.5f, 2f);
                Sequence scaleSequence = DOTween.Sequence();
                scaleSequence.Append(_rectTransform.DOScale(_rectTransform.localScale * scaleTo, DisplayDuration / 2f));
                scaleSequence.Append(_rectTransform.DOScale(_defaultScale, DisplayDuration / 2f));

                Color toColor = Random.ColorHSV();
                Sequence colorSequence = DOTween.Sequence();
                colorSequence.Append(_text.DOColor(toColor, DisplayDuration / 2f));
                colorSequence.Append(_text.DOColor(_defaultColor, DisplayDuration / 2f));
            }
        }

        private void OnDisable()
        {
            IsCrit = false;
            _rectTransform.localScale = _defaultScale;
            _text.color = _defaultColor;
        }
    }
}
