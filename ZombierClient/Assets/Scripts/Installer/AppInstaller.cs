using Prototype.Controller;
using Prototype.Data;
using Prototype.Service;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class AppInstaller : MonoInstaller
    {
        [SerializeField] private AppData _appData;
        [SerializeField] private bool _useEditorConfig = true;

        public override void InstallBindings()
        {
            // App data
            if (_useEditorConfig)
            {
                Container.Bind<AppData>().FromInstance(_appData).AsSingle();
                Container.Bind<MetaData>().FromInstance(_appData.Meta).AsSingle();
            }
            else
            {

            }

            // !App data

            Container.Bind<AppEventService>().AsSingle().NonLazy();
            Container.Bind<SceneLoaderService>().AsSingle().NonLazy();

            Container.BindInterfacesTo<AppController>().AsSingle().NonLazy();
        }
    }
}
