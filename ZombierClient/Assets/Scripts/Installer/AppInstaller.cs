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
        [SerializeField] private bool _useFirebaseConfig = true;
        // Resolved by app installer 
        private AppData _appData;
        private SerializationService _serializationService;

        public override void InstallBindings()
        {
            _serializationService = new SerializationService();
            _appData = new AppData();

            // App data
            if (_useFirebaseConfig)
            {
                // Use data from firebase remote config
                Container.Bind<AppData>().FromInstance(_appData).AsSingle();
                Container.Bind<UserData>().FromInstance(_appData.User).AsSingle();
            }
            else
            {
                if (!_serializationService.AppDataFolderExists())
                {
                    _serializationService.SerializeUserData(_appData.User);
                    Debug.Log($"Created new app data in {_serializationService.PersistentAppDataPath}");
                }

                // Use data from data base
                UserData userData = _serializationService.DeserializeUserData();
                _appData.User = userData;
                Container.Bind<AppData>().FromInstance(_appData).AsSingle();
                Container.Bind<UserData>().FromInstance(_appData.User).AsSingle();
            }
            // !App data

            // Services
            Container.Bind<GameConfigDBService>().AsSingle().NonLazy();
            Container.Bind<AppEventService>().AsSingle().NonLazy();
            Container.Bind<SceneLoaderService>().AsSingle().NonLazy();
            Container.Bind<SerializationService>().FromInstance(_serializationService).AsSingle();
            Container.Bind<GameplaySessionConfigurator>().AsSingle();
            // !Services

            // Controllers
            Container.Bind<AppController>().FromComponentInHierarchy().AsSingle();
            // !Controllers
        }
    }
}
