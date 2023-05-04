using Cinemachine;
using Prototype.Controller;
using Prototype.Data;
using Prototype.Model;
using Prototype.Service;
using Prototype.View;
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
        [SerializeField] private EnemyModel EnemyPrefab;

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

            // Weapons
            Container.Bind<MarkerProjectiles>().FromComponentInHierarchy().AsSingle();
            Container.BindFactory<UnityEngine.Object, ProjectileModel, ProjectileModel.Factory>()
                .FromFactory<PrefabFactory<ProjectileModel>>();
            Container.Bind<Rigidbody>().FromComponentInChildren().AsTransient();
            Container.Bind<MarkerWeaponEndPoint>().FromComponentInChildren().AsTransient();

            // Enemy
            Container.BindFactory<Data.EnemyData, EnemyModel, EnemyModel.Factory>()
                .FromComponentInNewPrefab(EnemyPrefab)
                .WithGameObjectName("Enemy")
                .UnderTransform(this.GetMarker<MarkerEnemies>);

            Container.BindFactory<UnityEngine.Object, EnemyView, EnemyView.Factory>()
                .FromFactory<PrefabFactory<EnemyView>>();

            Container.Bind<List<EnemyModel>>().AsSingle();

            Container.Bind<MarkerTargetPoint>().FromComponentInChildren().AsTransient();
            Container.Bind<MarkerView>().FromComponentInChildren().AsTransient();
            Container.Bind<NavMeshAgent>().FromComponentInChildren().AsTransient();

            //// !Game entities

            //// Game controllers

            Container.Bind<EnemySpawnController>().FromComponentInHierarchy().AsSingle();

            ////////////
            Container.Bind<MonoObjectPool<ProjectileModel>>().AsSingle();
            ////////////

            /// !GameControllers
        }

        private Transform GetMarker<T>(InjectContext context)
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
