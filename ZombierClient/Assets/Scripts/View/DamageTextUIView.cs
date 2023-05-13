using Prototype.ObjectPool;
using TMPro;
using Zenject;

namespace Prototype.View
{
    public class DamageTextUIView : PoolObject
    {
        [Inject]
        public void Construct(TextMeshProUGUI text)
        {
            _text = text;
        }

        public float DisplayDuration { get; set; }

        public void SetTextValue(float value)
        {
            _text.text = ((int)value).ToString();
        }

        private TextMeshProUGUI _text;
    }
}
