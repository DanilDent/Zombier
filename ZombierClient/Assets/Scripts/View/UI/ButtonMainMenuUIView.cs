using Prototype.Service;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.View
{
    public class ButtonMainMenuUIView : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveAllListeners();
        }

        private void OnClick()
        {
            SceneLoaderService.Load(SceneLoaderService.Scene.MainMenu);
        }
    }
}
