using Prototype.Controller;
using Prototype.Data;
using Prototype.Service;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class AppInstaller : MonoInstaller
    {
        // From inspector
        [SerializeField] private bool _useFirebaseConfig = false;
        [SerializeField] private bool _useFirestoreDataBase = true;
        // Resolved by app installer 
        private AppData _appData;

        public override void InstallBindings()
        {
            _appData = new AppData();

            // App data
            if (_useFirebaseConfig)
            {
                // Use data from firebase remote config
                Container.Bind<AppData>().FromInstance(_appData).AsSingle();
                Container.Bind<UserData>().FromInstance(_appData.User).AsSingle();
            }
            else if (_useFirestoreDataBase)
            {
                Container.Bind<AppData>().FromInstance(_appData).AsSingle();
                Container.Bind<UserData>().FromInstance(_appData.User).AsSingle();
            }

            // !App data

            // Services
            Container.Bind<GameConfigDBService>().AsSingle().NonLazy();
            Container.Bind<AppEventService>().AsSingle().NonLazy();
            Container.Bind<SceneLoaderService>().AsSingle().NonLazy();
            Container.Bind<GameplaySessionConfigurator>().AsSingle();
            Container.Bind<UsersDbService>().AsSingle().NonLazy();
            Container.Bind<AuthenticationService>().AsSingle().NonLazy();
            // !Services

            // Controllers
            Container.Bind<AppController>().FromComponentInHierarchy().AsSingle();
            // !Controllers
        }
    }
}
