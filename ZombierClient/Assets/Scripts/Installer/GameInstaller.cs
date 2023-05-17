using Cinemachine;
using Prototype.Controller;
using Prototype.Data;
using Prototype.Extensions;
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
        [SerializeField] private GameplaySessionData _session;
        //
        [SerializeField] private int _projectilePoolSize = 10;

        public override void InstallBindings()
        {
            // Config
            Container.Bind<GameplaySessionData>().FromInstance(_session).AsSingle();
            // !Config

            // Unity components
            Container.Bind<Animator>().FromComponentInChildren().AsTransient();
            Container.Bind<CharacterController>().FromComponentInChildren().AsTransient();
            Container.Bind<Rigidbody>().FromComponentInChildren().AsTransient();
            Container.Bind<NavMeshAgent>().FromComponentInChildren().AsTransient();
            Container.Bind<MarkerTargetPoint>().FromComponentInChildren().AsTransient();
            Container.Bind<MarkerView>().FromComponentInChildren().AsTransient();
            Container.Bind<MarkerShootingPointPlayer>().FromComponentInChildren().AsTransient();
            Container.Bind<MarkerGround>().FromComponentInHierarchy().AsCached();

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
                        var prefab = _session.DamageTextUIPrefab;
                        var tranfromContainer = transform.GetComponentInChildren<MarkerUIPool>().transform;
                        pool.Initialize(prefab, _projectilePoolSize, tranfromContainer);
                    }
                })
                .NonLazy();
            // !UI

            // Services
            Container.Bind<GameInputService>().AsSingle();
            Container.Bind<GameUIEventService>().AsSingle();
            Container.Bind<GameEventService>().AsSingle();
            // !Services

            //// Game entities

            // Level
            Container.Bind<MarkerLevelExitPoint>().FromComponentInChildren().AsSingle();
            GameObject levelGameObject = GenerateLevel();
            Container.Bind<LevelModel>()
                .FromNewComponentOn(levelGameObject)
                .AsSingle()
                .NonLazy();

            // Player
            Container.Bind<PlayerModel>()
                .FromComponentInNewPrefab(_session.Player.PlayerPrefab)
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

            Container.Bind<MonoObjectPool<PlayerProjectileModel>>()
                .AsSingle()
                .OnInstantiated((ctx, obj) =>
                {
                    if (obj is MonoObjectPool<PlayerProjectileModel> pool)
                    {
                        var prefab = _session.Player.Weapon.ProjectileData.Prefab as PlayerProjectileModel;
                        var tranfromContainer = transform.GetComponentInChildren<MarkerProjectiles>().transform;
                        pool.Initialize(prefab, _projectilePoolSize, tranfromContainer);
                    }
                })
                .NonLazy();

            Container.Bind<MonoObjectPool<EnemyProjectileModel>>()
                .AsSingle()
                .OnInstantiated((ctx, obj) =>
                 {
                     if (obj is MonoObjectPool<EnemyProjectileModel> pool)
                     {
                         var prefab = _session.EnemyProjectilePrefab;
                         var tranfromContainer = transform.GetComponentInChildren<MarkerProjectiles>().transform;
                         pool.Initialize(prefab, _projectilePoolSize, tranfromContainer);
                     }
                 })
                .NonLazy();


            // Enemy
            Container.BindFactory<IdData, EnemyData, EnemyModel, EnemyModel.Factory>()
                .FromSubContainerResolve()
                .ByNewPrefabInstaller<EnemyInstaller>(_session.EnemyPrefab)
                .UnderTransform(GetMarker<MarkerEnemies>());

            Container.BindFactory<UnityEngine.Object, EnemyView, EnemyView.Factory>().
                    FromFactory<PrefabFactory<EnemyView>>();

            Container.BindFactory<EnemyModel, MeleeAttackStrategy, MeleeAttackStrategy.Factory>();
            Container.BindFactory<EnemyModel, RangeAttackStrategy, RangeAttackStrategy.Factory>();
            Container.BindFactory<DescAttackStrategy.StrategyType, EnemyModel, IAttackStrategy, AttackStrategyFactory>()
                .FromFactory<AttackStrategyByStrategyTypeFactory>();

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
                        var prefab = _session.HitVFXPrefab;
                        var tranfromContainer = transform.GetComponentInChildren<MarkerVFX>().transform;
                        pool.Initialize(prefab, _projectilePoolSize, tranfromContainer);
                    }
                })
                .NonLazy();
            Container.Bind<MonoObjectPool<DeathBloodSplashVFXView>>()
               .AsSingle()
               .OnInstantiated((ctx, obj) =>
               {
                   if (obj is MonoObjectPool<DeathBloodSplashVFXView> pool)
                   {
                       var prefab = _session.DeathVFXPrefab;
                       var tranfromContainer = transform.GetComponentInChildren<MarkerVFX>().transform;
                       pool.Initialize(prefab, _projectilePoolSize, tranfromContainer);
                   }
               })
               .NonLazy();

            // Gameplay Controllers
            Container.Bind<EnemySpawnController>().FromComponentInHierarchy(true).AsSingle();
            Container.Bind<PlayerMovementController>().FromComponentInHierarchy(true).AsSingle();
            Container.Bind<PlayerAimController>().FromComponentInHierarchy(true).AsSingle();
            Container.Bind<PlayerShootController>().FromComponentInHierarchy(true).AsSingle();
            Container.Bind<DealDamageController>().FromComponentInHierarchy(true).AsSingle();
            Container.Bind<EnemyAttackController>().FromComponentInHierarchy(true).AsSingle();
            Container.Bind<EnemyChaseController>().FromComponentInHierarchy(true).AsSingle();
            Container.Bind<EnemyMovementController>().FromComponentInHierarchy(true).AsSingle();
            Container.Bind<SpawnDamageTextUIController>().FromComponentInHierarchy(true).AsSingle();
            Container.Bind<VFXController>().FromComponentInHierarchy(true).AsSingle();
            // !Gameplay Controllers

            //// !Game entities
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

        private GameObject GenerateLevel()
        {
            LevelGenerator levelGenerator = new LevelGenerator(
                _session.LevelGeneratorConfig,
                _session.Location,
                _session.Location.Levels[_session.CurrentLevelIndex]);

            GameObject levelInstance = levelGenerator.GenerateLevel();
            levelInstance.transform.SetParent(GetMarker<MarkerLevel>());
            return levelInstance;
        }
    }
}
