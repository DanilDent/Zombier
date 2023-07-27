using Cinemachine;
using Prototype.Controller;
using Prototype.Data;
using Prototype.Extensions;
using Prototype.Factory;
using Prototype.LevelGeneration;
using Prototype.Model;
using Prototype.ObjectPool;
using Prototype.Service;
using Prototype.View;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;
using Zenject;

namespace Prototype
{
    public class GameInstaller : MonoInstaller
    {
        // Injected
        [Inject] private AppData _appData;
        // From inspector
        [SerializeField] private GameConfigData _gameConfig;
        // Resolved by installer itself
        private GameSessionData _currentGameSession;
        private AssetLoader<GameObject> _assetLoader;

        public override void InstallBindings()
        {
            // Game data
            _currentGameSession = _appData.User.GameSession.Copy();
            Container.Bind<GameSessionData>().FromInstance(_currentGameSession).AsSingle();
            // !Game data

            LoadPrefabs(new List<string>()
            {
                _currentGameSession.Player.PlayerPrefabAddress,
                _currentGameSession.Player.Weapon.ProjectileData.PrefabAddress,
                //
                _gameConfig.EnemyContextAddress,
                _gameConfig.EnemyProjectilePrefabAddress,
                _gameConfig.HitVFXPrefabAddress,
                _gameConfig.DeathVFXPrefabAddress,
                _gameConfig.DamageTextUIPrefabAddress,
                _gameConfig.ExpTextUIPrefabAddress,
            });

            // Unity components
            Container.Bind<Animator>().FromComponentInChildren().AsTransient();
            Container.Bind<CharacterController>().FromComponentInChildren().AsTransient();
            Container.Bind<Rigidbody>().FromComponentInChildren().AsTransient();
            Container.Bind<NavMeshAgent>().FromComponentInChildren().AsTransient();
            // Markers
            Container.Bind<MarkerTargetPoint>().FromComponentInChildren().AsTransient();
            Container.Bind<MarkerView>().FromComponentInChildren().AsTransient();
            Container.Bind<MarkerShootingPointPlayer>().FromComponentInChildren().AsTransient();
            Container.Bind<MarkerGround>().FromComponentInHierarchy().AsCached();
            Container.Bind<MarkerLevel>().FromComponentInHierarchy().AsSingle();

            Container.Bind<Camera>().FromComponentInHierarchy().AsSingle();
            // !Unity components

            // Unity UI components
            Container.Bind<Image>().FromMethod((InjectContext ctx) =>
            {
                if (ctx.ObjectInstance is Component cast)
                {
                    Transform root = cast.transform;
                    return root.SearchComponent<MarkerFiller>().GetComponent<Image>();
                }

                return null;
            })
            .AsTransient()
            .WhenInjectedInto<EnemyHealthBarUIView>();

            Container.Bind<TextMeshProUGUI>()
                .FromComponentInChildren()
                .AsTransient()
                .WhenInjectedInto<DamageTextUIView>();

            Container.Bind<RectTransform>().FromComponentInChildren()
                .AsTransient()
                .WhenInjectedInto<DamageTextUIView>();

            Container.Bind<TextMeshProUGUI>()
                .FromComponentInChildren()
                .AsTransient()
                .WhenInjectedInto<ExpTextUIView>();

            Container.Bind<RectTransform>().FromComponentInChildren()
                .AsTransient()
                .WhenInjectedInto<ExpTextUIView>();
            // !Unity UI components

            // Camera
            Container.Bind<CinemachineVirtualCamera>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesTo<CameraController>().AsSingle();
            // !Camera

            // UI 
            Container.Bind<FloatingJoystick>().FromComponentInHierarchy().AsSingle();

            Container.BindFactory<UnityEngine.Object, DamageTextUIView, PoolObjectFactory<DamageTextUIView>>()
                .FromFactory<PrefabFactory<DamageTextUIView>>();

            Container.Bind<MonoObjectPool<DamageTextUIView>>()
                .AsSingle()
                .OnInstantiated((ctx, obj) =>
                {
                    if (obj is MonoObjectPool<DamageTextUIView> pool)
                    {
                        var prefab = _assetLoader.GetComponent<DamageTextUIView>(_gameConfig.DamageTextUIPrefabAddress);
                        var transformContainer = transform.GetComponentInChildren<MarkerUIPool>().transform;
                        pool.Initialize(prefab, _gameConfig.ProjectilesPoolSize, transformContainer);
                    }
                })
                .NonLazy();

            Container.BindFactory<UnityEngine.Object, ExpTextUIView, PoolObjectFactory<ExpTextUIView>>()
                .FromFactory<PrefabFactory<ExpTextUIView>>();

            Container.Bind<MonoObjectPool<ExpTextUIView>>()
                .AsSingle()
                .OnInstantiated((ctx, obj) =>
                {
                    if (obj is MonoObjectPool<ExpTextUIView> pool)
                    {
                        var prefab = _assetLoader.GetComponent<ExpTextUIView>(_gameConfig.ExpTextUIPrefabAddress);
                        var transformContainer = transform.GetComponentInChildren<MarkerUIPool>().transform;
                        pool.Initialize(prefab, _gameConfig.ProjectilesPoolSize, transformContainer);
                    }
                })
                .NonLazy();
            // !UI

            // Services
            Container.Bind<GameInputService>().AsSingle();
            Container.Bind<GameEventService>().AsSingle();
            // !Services

            //// Game entities

            // Level
            Container.Bind<MarkerLevelExitPoint>().FromComponentInChildren().AsSingle();

            Container.Bind<LevelGeneratorData>().FromInstance(_gameConfig.LevelGeneratorConfig).AsSingle();
            Container.Bind<LocationData>().FromInstance(_currentGameSession.Location).AsSingle();
            Container.Bind<LevelData>().FromInstance(_currentGameSession.Location.Levels[_currentGameSession.CurrentLevelIndex]);
            Container.Bind<LevelModel>().FromFactory<ProceduralLevelFactory>().AsSingle().NonLazy();

            // Player
            Container.Bind<PlayerModel>()
                .FromComponentInNewPrefab(_assetLoader.Get(_currentGameSession.Player.PlayerPrefabAddress))
                .WithGameObjectName("Player")
                .UnderTransform(GetMarker<MarkerEntities>)
                .AsSingle()
                .OnInstantiated<PlayerModel>(SetPlayerPositionToZero);

            Container.Bind<WeaponModel>().FromComponentInChildren().AsSingle();
            Container.Bind<TargetHandleModel>().FromComponentInChildren().AsSingle();
            Container.Bind<Rig>().FromComponentInChildren().AsSingle();
            Container.Bind<MarkerDefaulTargetPoint>().FromComponentInChildren().AsSingle();

            // Weapon
            Container.Bind<MarkerProjectiles>().FromComponentInHierarchy().AsSingle();

            Container.BindFactory<UnityEngine.Object, PlayerProjectileModel, PoolObjectFactory<PlayerProjectileModel>>()
                .FromFactory<PrefabFactory<PlayerProjectileModel>>();
            Container.BindFactory<UnityEngine.Object, EnemyProjectileModel, PoolObjectFactory<EnemyProjectileModel>>()
                .FromFactory<PrefabFactory<EnemyProjectileModel>>();

            Container.Bind<IMonoObjectPool<PlayerProjectileModel>>()
                .WithId("DefaultPlayerProjectileObjectPool")
                .To<MonoObjectPool<PlayerProjectileModel>>()
                .AsCached()
                .OnInstantiated((ctx, obj) =>
                {
                    if (obj is MonoObjectPool<PlayerProjectileModel> pool)
                    {
                        var prefab = _assetLoader.GetComponent<PlayerProjectileModel>(_currentGameSession.Player.Weapon.ProjectileData.PrefabAddress);
                        var transformContainer = transform.GetComponentInChildren<MarkerProjectiles>().transform;
                        pool.Initialize(prefab, _gameConfig.ProjectilesPoolSize, transformContainer);
                    }
                })
                .NonLazy();

            Container.Bind<IMonoObjectPool<PlayerProjectileModel>>()
                .WithId("BouncePlayerProjectileObjectPool")
                .To<MonoObjectPool<PlayerProjectileModel>>()
                .AsCached()
                .OnInstantiated((ctx, obj) =>
                {
                    if (obj is MonoObjectPool<PlayerProjectileModel> pool)
                    {
                        var prefab = _assetLoader.GetComponent<PlayerProjectileModel>(_currentGameSession.Player.Weapon.ProjectileData.PrefabAddress);
                        var transformContainer = transform.GetComponentInChildren<MarkerProjectiles>().transform;
                        pool.Initialize(prefab, _gameConfig.ProjectilesPoolSize, transformContainer, (instance) =>
                        {
                            instance.GetComponent<Collider>().material = _gameConfig.ProjectileBouncePhysMat;
                            instance.Init(isBouncing: true);
                        });
                    }
                })
                .NonLazy();

            Container.Bind<MonoObjectPool<EnemyProjectileModel>>()
                .AsSingle()
                .OnInstantiated((ctx, obj) =>
                 {
                     if (obj is MonoObjectPool<EnemyProjectileModel> pool)
                     {
                         var prefab = _assetLoader.GetComponent<EnemyProjectileModel>(_gameConfig.EnemyProjectilePrefabAddress);
                         var transformContainer = transform.GetComponentInChildren<MarkerProjectiles>().transform;
                         pool.Initialize(prefab, _gameConfig.ProjectilesPoolSize, transformContainer);
                     }
                 })
                .NonLazy();


            // Enemy
            Container.BindFactory<IdData, EnemyData, EnemyModel, EnemyModel.Factory>()
                .FromSubContainerResolve()
                .ByNewPrefabInstaller<EnemyInstaller>(_assetLoader.Get(_gameConfig.EnemyContextAddress))
                .UnderTransform(GetMarker<MarkerEnemies>())
                .OnInstantiated<GameObject>((ctx, go) =>
                {
                    go.transform.position = Vector3.zero;
                });

            Container.BindFactory<UnityEngine.Object, EnemyView, EnemyView.Factory>().
                    FromFactory<PrefabFactory<EnemyView>>();

            Container.Bind<List<EnemyModel>>().AsSingle();

            // VFX
            Container.Bind<ParticleSystem>().FromComponentsInChildren().AsTransient();
            Container.BindFactory<UnityEngine.Object, HitBloodSplashVFXView, PoolObjectFactory<HitBloodSplashVFXView>>()
                .FromFactory<PrefabFactory<HitBloodSplashVFXView>>();
            Container.BindFactory<UnityEngine.Object, DeathBloodSplashVFXView, PoolObjectFactory<DeathBloodSplashVFXView>>()
                .FromFactory<PrefabFactory<DeathBloodSplashVFXView>>();
            Container.Bind<MonoObjectPool<HitBloodSplashVFXView>>()
                .AsSingle()
                .OnInstantiated((ctx, obj) =>
                {
                    if (obj is MonoObjectPool<HitBloodSplashVFXView> pool)
                    {
                        var prefab = _assetLoader.GetComponent<HitBloodSplashVFXView>(_gameConfig.HitVFXPrefabAddress);
                        var transformContainer = transform.GetComponentInChildren<MarkerVFX>().transform;
                        pool.Initialize(prefab, _gameConfig.ProjectilesPoolSize, transformContainer);
                    }
                })
                .NonLazy();
            Container.Bind<MonoObjectPool<DeathBloodSplashVFXView>>()
               .AsSingle()
               .OnInstantiated((ctx, obj) =>
               {
                   if (obj is MonoObjectPool<DeathBloodSplashVFXView> pool)
                   {
                       var prefab = _assetLoader.GetComponent<DeathBloodSplashVFXView>(_gameConfig.DeathVFXPrefabAddress);
                       var transformContainer = transform.GetComponentInChildren<MarkerVFX>().transform;
                       pool.Initialize(prefab, _gameConfig.ProjectilesPoolSize, transformContainer);
                   }
               })
               .NonLazy();

            //// !Game entities

            //// Buffs && Effects

            Container.BindFactory<string, Buff, BuffFactory>().FromFactory<BuffFromIdFactory>();
            Container.BindFactory<EffectConfig, EffectModel, EffectModel.Factory>();

            /// !Buffs && Effects


            // Gameplay Controllers
            Container.Bind<EnemySpawnController>().FromComponentInHierarchy(true).AsSingle();
            Container.Bind<PlayerMovementController>().FromComponentInHierarchy(true).AsSingle();
            Container.Bind<PlayerAimController>().FromComponentInHierarchy(true).AsSingle();
            Container.Bind<PlayerShootController>().FromComponentInHierarchy(true).AsSingle();
            Container.Bind<PlayerLevelUpController>().FromComponentInHierarchy(true).AsSingle();
            Container.Bind<DealDamageController>().FromComponentInHierarchy(true).AsSingle();
            Container.Bind<EnemyAIController>().FromComponentInHierarchy(true).AsSingle();
            Container.Bind<SpawnWorldCanvasUIText>().FromComponentInHierarchy(true).AsSingle();
            Container.Bind<VFXController>().FromComponentInHierarchy(true).AsSingle();
            Container.Bind<ApplyBuffController>().FromComponentInHierarchy(true).AsSingle();
            Container.Bind<EffectsController>().FromComponentInHierarchy(true).AsSingle();
            // !Gameplay Controllers
        }

        private void OnDestroy()
        {
            _assetLoader.Release();
        }

        private void LoadPrefabs(List<string> addresses)
        {
            _assetLoader = new AssetLoader<GameObject>(addresses);
            _assetLoader.Load();
        }

        private Transform GetMarker<T>(InjectContext context)
            where T : UnityEngine.Component
        {
            return transform.GetComponentInChildren<T>().transform;
        }

        private Transform GetMarker<T>()
           where T : UnityEngine.Component
        {
            return transform.GetComponentInChildren<T>().transform;
        }

        private void SetPlayerPositionToZero(InjectContext context, PlayerModel player)
        {
            player.transform.position = Vector3.zero;
        }
    }
}
