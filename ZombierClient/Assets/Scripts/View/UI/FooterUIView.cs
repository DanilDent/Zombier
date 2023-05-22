using Prototype.Service;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Prototype.View
{
    public class FooterUIView : MonoBehaviour
    {
        [Inject]
        public void Construct(AppEventService appEventService)
        {
            _appEventService = appEventService;
        }

        // Injected
        private AppEventService _appEventService;
        // From inspector
        [SerializeField] private Button _btnInventory;
        [SerializeField] private Button _btnMainMenu;
        [SerializeField] private Button _btnShop;

        private void OnEnable()
        {
            _btnInventory.onClick.AddListener(OnInventory);
            _btnMainMenu.onClick.AddListener(OnMainMenu);
            _btnShop.onClick.AddListener(OnShop);
        }

        private void OnDisable()
        {
            _btnInventory.onClick.RemoveAllListeners();
            _btnMainMenu.onClick.RemoveAllListeners();
            _btnShop.onClick.RemoveAllListeners();
        }

        private void OnInventory()
        {
            _appEventService.OnLoadScene(new LoadSceneEventArgs { To = Scene.Inventory });
        }

        private void OnMainMenu()
        {
            _appEventService.OnLoadScene(new LoadSceneEventArgs { To = Scene.MainMenu });
        }

        private void OnShop()
        {
            _appEventService.OnLoadScene(new LoadSceneEventArgs { To = Scene.Shop });
        }
    }

}