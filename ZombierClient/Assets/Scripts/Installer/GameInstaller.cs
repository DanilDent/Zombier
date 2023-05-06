using Cinemachine;
using Prototype.Controller;
using Prototype.Data;
using Prototype.Factory;
using Prototype.Model;
using Prototype.ObjectPool;
using Prototype.Service;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
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

            // Camera
            Container.Bind<CinemachineVirtualCamera>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesTo<CameraController>().AsSingle();

            // UI 
            Container.Bind<FloatingJoystick>().FromComponentInHierarchy().AsSingle();

            // Services
            Container.Bind<GameplayInputService>().AsSingle();
            Container.Bind<GameplayEventService>().AsSingle();

            //// Game entities

            // Level
            Container.Bind<LevelModel>()
                .FromComponentInNewPrefab(_session.Location.Levels[_session.CurrentLevelIndex].LevelPrefab)
                .UnderTransform(GetMarker<MarkerLevel>)
                .AsSingle()
                .OnInstantiated<LevelModel>(BuildNavmesh)
                .NonLazy();
            Container.Bind<NavMeshSurface>().FromComponentInChildren().AsSingle();
            Container.Bind<MarkerLevelExitPoint>().FromComponentInChildren().AsSingle();

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

            Container.BindFactory<UnityEngine.Object, ProjectileModel, PoolObjectFactory<ProjectileModel>>()
                .FromFactory<PrefabFactory<ProjectileModel>>();

            Container.Bind<MonoObjectPool<ProjectileModel>>()
                .AsSingle()
                .OnInstantiated((ctx, obj) =>
                {
                    if (obj is MonoObjectPool<ProjectileModel> pool)
                    {
                        var prefab = _session.Player.Weapon.ProjectileData.Prefab;
                        var tranfromContainer = transform.GetComponentInChildren<MarkerProjectiles>().transform;
                        pool.Initialize(prefab, _projectilePoolSize, tranfromContainer);
                    }
                })
                .NonLazy();

            Container.Bind<Rigidbody>().FromComponentInChildren().AsTransient();
            Container.Bind<MarkerWeaponEndPoint>().FromComponentInChildren().AsTransient();

            // Enemy
            //Container.BindFactory<Data.EnemyData, EnemyModel, EnemyModel.Factory>()
            //    .FromComponentInNewPrefab(EnemyPrefab)
            //    .WithGameObjectName("Enemy")
            //    .UnderTransform(this.GetMarker<MarkerEnemies>);

            Container.Bind<Transform>().FromMethod(() => GetMarker<MarkerEnemies>())
                .WhenInjectedInto<UnderTransformPrefabFactory<EnemyData, EnemyModel>>();

            Container.BindFactory<UnityEngine.Object, EnemyData, EnemyModel, EnemyModel.Factory>()
                .FromFactory<UnderTransformPrefabFactory<EnemyData, EnemyModel>>();

            Container.Bind<List<EnemyModel>>().AsSingle();

            Container.Bind<MarkerTargetPoint>().FromComponentInChildren().AsTransient();
            Container.Bind<MarkerView>().FromComponentInChildren().AsTransient();
            Container.Bind<NavMeshAgent>().FromComponentInChildren().AsTransient();

            // VFX
            Container.Bind<ParticleSystem>().FromComponentsInChildren().AsTransient();

            //// !Game entities

            // Unity components
            Container.Bind<Animator>().FromComponentInChildren().AsTransient();
            Container.Bind<CharacterController>().FromComponentInChildren().AsTransient();

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

        private void BuildNavmesh(InjectContext context, LevelModel level)
        {
            if (level.Navmesh != null)
            {
                level.Navmesh.BuildNavMesh();
            }
        }

        private void SetPlayerPositionToZero(InjectContext context, PlayerModel player)
        {
            player.transform.position = Vector3.zero;
        }
    }
}
