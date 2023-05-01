using Prototype.Data;
using Prototype.Extensions;
using Prototype.Model;
using UnityEngine.Animations.Rigging;
using Zenject;

namespace Prototype
{
    public class PlayerInstaller : MonoInstaller
    {
        [Inject] private GameplaySessionData _session;

        public override void InstallBindings()
        {
            Container.Bind<PlayerModel>().FromComponentInHierarchy().AsSingle();
            Container.Bind<WeaponModel>().FromMethod(GetCurrentWeaponModel).AsSingle();
            Container.Bind<TargetHandle>().FromComponentInChildren().AsSingle();

            Container.Bind<Rig>().FromComponentInHierarchy().AsSingle();

            Container.Bind<MarkerDefaulTargetPoint>().FromComponentInChildren().AsSingle();
        }

        private WeaponModel GetCurrentWeaponModel()
        {
            return transform.SearchComponent<WeaponModel>(x =>
            {
                if (x.TryGetComponent<WeaponModel>(out var component))
                {
                    if (component.Id.Equals(_session.Data.Player.IdCurrentWeapon))
                    {
                        return true;
                    }
                }

                return false;
            });
        }
    }
}
