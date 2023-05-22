using UnityEngine;
using UnityEngine.UI;

namespace Prototype.View
{
    public class FooterUIView : MonoBehaviour
    {
        [SerializeField] private Button _btnInventory;
        [SerializeField] private Button _btnMainMenu;
        [SerializeField] private Button _btnShop;

        private void OnEnable()
        {
            _btnInventory.onClick.AddListener(OnInventory);
            _btnMainMenu.onClick.AddListener(OnMainMenu);
            _btnShop.onClick.AddListener(OnShop);
        }

        private void OnInventory()
        {

        }

        private void OnMainMenu()
        {

        }

        private void OnShop()
        {

        }
    }

}