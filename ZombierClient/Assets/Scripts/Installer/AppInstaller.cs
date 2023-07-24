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
        [SerializeField] private AppData _appData;
        [SerializeField] private bool _useEditorConfig = true;
        // Resolved by app installer 
        private SerializationService _serializationService;

        public override void InstallBindings()
        {
            _serializationService = new SerializationService();

            // App data
            if (_useEditorConfig)
            {
                // Use data from editor's SO assets
                Container.Bind<AppData>().FromInstance(_appData).AsSingle();
                Container.Bind<MetaData>().FromInstance(_appData.Meta).AsSingle();
                Container.Bind<UserData>().FromInstance(_appData.User).AsSingle();
            }
            else
            {
                if (!_serializationService.AppDataExists())
                {
                    _serializationService.SerializeAppData(_appData);
                    Debug.Log($"Created new app data in {_serializationService.PersistentAppDataPath}");
                }

                // Use data from data base
                AppData appData = _serializationService.DeserializeAppData();
                Container.Bind<AppData>().FromInstance(appData).AsSingle();
                Container.Bind<MetaData>().FromInstance(appData.Meta).AsSingle();
                Container.Bind<UserData>().FromInstance(appData.User).AsSingle();
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
