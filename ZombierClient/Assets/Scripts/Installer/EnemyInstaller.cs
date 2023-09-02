using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using Prototype.Data;
using Prototype.Misc;
using Prototype.Model;
using Prototype.Service;
using Prototype.View;
using System;
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
        [Inject] private PlayerModel _player;
        // Resolved by installer
        private AssetLoader<GameObject> _assetLoader;
        private GameObject _contextGO;

        public override void InstallBindings()
        {
            Container.BindInstance(_id).AsSingle();

            string idStr = _id.ToString();
            int nCharsToDisplay = 8;

            if (_dataTemplate != null)
            {
                Container.BindInstance(_dataTemplate);

                _assetLoader = new AssetLoader<GameObject>(new List<string>()
                {
                    _dataTemplate.ModelPrefabAddress,
                    _dataTemplate.ViewPrefabAddress
                });
                _assetLoader.Load();

                Container.Bind<EnemyModel>()
                    .FromComponentInNewPrefab(_assetLoader.Get(_dataTemplate.ModelPrefabAddress))
                    .AsSingle()
                    .OnInstantiated<EnemyModel>((ctx, model) =>
                    {
                        _contextGO = model.transform.parent.gameObject;
                        _contextGO.name = $"Enemy#{idStr.Substring(0, Math.Min(idStr.Length, nCharsToDisplay))}Context";
                        AddDisablingComponent();
                    })
                    .NonLazy();

                Container.Bind<EnemyView>()
                    .FromComponentInNewPrefab(_assetLoader.Get(_dataTemplate.ViewPrefabAddress))
                    .UnderTransform(GetMarker<MarkerView>)
                    .AsSingle()
                    .NonLazy();

                Container.Bind<FSMOwner>().FromComponentInChildren().AsSingle();
                Container.Bind<Blackboard>().FromComponentInChildren().AsSingle();
            }
        }

        private Transform GetMarker<T>(InjectContext context)
           where T : UnityEngine.Component
        {
            return _contextGO?.transform.GetComponentInChildren<T>().transform;
        }

        private void OnDestroy()
        {
            _assetLoader?.Release();
        }

        private void AddDisablingComponent()
        {
            var comp = _contextGO.AddComponent<DisableIfTargetNotInRange>();
            comp.SetDistanceComp();
            comp.Target = _player.transform;
        }
    }
}
