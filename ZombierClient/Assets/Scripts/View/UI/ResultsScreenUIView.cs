using Prototype.Service;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.View
{
    public class ResultsScreenUIView : MonoBehaviour
    {
        [SerializeField] private Button _btnMainMenu;

        private void Awake()
        {
            _btnMainMenu.onClick.AddListener(OnMainMenu);
        }

        private void OnMainMenu()
        {
            SceneLoaderService.Load(SceneLoaderService.Scene.App);
        }
    }
}