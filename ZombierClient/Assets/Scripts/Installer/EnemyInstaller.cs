using Prototype.Data;
using Prototype.Model;
using Prototype.Service;
using Prototype.View;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class EnemyInstaller : Installer<EnemyInstaller>
    {
        // Injected
        [Inject] private IdData _id;
        [Inject] private EnemyData _dataTemplate;
        // Resolved by installer
        private AssetLoader<GameObject> _assetLoader;

        public override void InstallBindings()
        {
            _assetLoader = new AssetLoader<GameObject>(new List<string>()
            {
                _dataTemplate.ViewPrefabAddress
            });
            _assetLoader.Load();

            Container.BindInstance(_id).AsSingle();
            Container.Bind<EnemyModel>().FromComponentOnRoot().AsSingle();
            Container.BindInstance(_dataTemplate);
            if (_dataTemplate != null)
            {
                //Container.Bind<EnemyView>()
                //    .FromNewComponentOnNewPrefab(_dataTemplate.ViewPrefabAddress)
                //    .UnderTransformGroup("View")
                //    .AsSingle()
                //    .NonLazy();
                Container.Bind<EnemyView>()
                    .FromComponentInNewPrefab(_assetLoader.Get(_dataTemplate.ViewPrefabAddress))
                    .UnderTransformGroup("View")
                    .AsSingle()
                    .NonLazy();
            }
        }

        private void OnDestroy()
        {
            _assetLoader.Release();
        }
    }
}
