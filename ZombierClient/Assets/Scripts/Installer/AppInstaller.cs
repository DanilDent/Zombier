using Prototype.Data;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class AppInstaller : MonoInstaller
    {
        [SerializeField] private AppData _appData;

        public override void InstallBindings()
        {
            Container.Bind<AppData>().FromInstance(_appData).AsSingle();
            Container.Bind<MetaData>().FromInstance(_appData.Meta).AsSingle();
        }
    }
}
