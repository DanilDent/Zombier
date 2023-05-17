using Prototype.Service;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.View
{
    public class MainMenuScreenUIView : MonoBehaviour
    {
        [SerializeField] private Button _playButton;

        private void OnEnable()
        {
            _playButton.onClick.AddListener(OnPlay);
        }

        private void OnPlay()
        {
            SceneLoaderService.Load(SceneLoaderService.Scene.Game);
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveAllListeners();
        }
    }
}
