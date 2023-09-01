using UnityEngine;

namespace Prototype.Data
{
    public class WeaponViewDataContainer : MonoBehaviour
    {
        public WeaponViewData GetWeaponViewDataByName(WeaponViewDataNameEnum name)
        {
            switch (name)
            {
                case WeaponViewDataNameEnum.Pistol:
                    return _pistolViewData;
                case WeaponViewDataNameEnum.Rifle:
                    return _rifleViewData;
            }

            throw new System.Exception($"Unkown weapon view data name {name}");
        }

        [SerializeField] private WeaponViewData _pistolViewData;
        [SerializeField] private WeaponViewData _rifleViewData;
    }
}
