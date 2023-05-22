using UnityEngine;

namespace Prototype.View
{
    public class FooterCanvasUIView : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
