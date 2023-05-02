using Cinemachine;
using Prototype.Controller;
using Prototype.Data;
using Prototype.Model;
using Prototype.Service;
using Prototype.SO;
using Prototype.View;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using Zenject;

namespace Prototype
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameplaySessionData _session;

        [SerializeField] private LevelModel LevelPrefab;
        [SerializeField] private PlayerModel PlayerPrefab;
        [SerializeField] private EnemyModel EnemyPrefab;

        public override void InstallBindings()
        {
            Config.CreateGameMetaData(out var meta);
            Config.CreateUserData(meta, out var user);
            Config.CreateGameplaySessionData(user, out _session);

            // Game config
            Container.Bind<GameplaySessionData>().FromInstance(_session).AsSingle();

            // Camera
            Container.Bind<CinemachineVirtualCamera>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesTo<CameraController>().AsSingle();

            // UI 
            Container.Bind<FloatingJoystick>().FromComponentInHierarchy().AsSingle();
            Container.Bind<GameplayHudUIView>().FromComponentInHierarchy().AsSingle();

            // Services
            Container.Bind<GameplayInputService>().AsSingle();
            Container.Bind<GameplayEventService>().AsSingle();

            //// Game entities

            // Level
            Container.Bind<LevelModel>()
                .FromComponentInNewPrefab(LevelPrefab)
                .WithGameObjectName(LevelPrefab.gameObject.name)
                .UnderTransform(GetMarker<MarkerLevel>)
                .AsSingle();

            Container.Bind<MarkerPlayerSpawnPoint>().FromComponentInChildren().AsSingle();
            Container.Bind<NavMeshSurface>().FromComponentInChildren().AsSingle();

            // Player
            Container.Bind<PlayerModel>()
                .FromComponentInNewPrefab(PlayerPrefab)
                .WithGameObjectName("Player")
                .UnderTransform(GetMarker<MarkerEntities>)
                .AsSingle();

            Container.Bind<WeaponModel>().FromComponentInChildren().AsSingle();
            Container.Bind<TargetHandleModel>().FromComponentInChildren().AsSingle();
            Container.Bind<Rig>().FromComponentInChildren().AsSingle();
            Container.Bind<MarkerDefaulTargetPoint>().FromComponentInChildren().AsSingle();

            // Enemy
            Container.BindFactory<EnemySO, EnemyModel, EnemyModel.Factory>()
                .FromComponentInNewPrefab(EnemyPrefab)
                .WithGameObjectName("Enemy")
                .UnderTransform(GetMarker<MarkerEnemies>);

            Container.BindFactory<UnityEngine.Object, EnemyView, EnemyView.Factory>()
                .FromFactory<PrefabFactory<EnemyView>>();

            Container.Bind<MarkerTargetPoint>().FromComponentInChildren().AsSingle();
            Container.Bind<MarkerView>().FromComponentInHierarchy().AsSingle();

            //// !Game entities

            //// Game controllers

            Container.Bind<EnemySpawnController>().AsSingle();

            /// !GameControllers
        }

        private Transform GetMarker<T>(InjectContext context)
            where T : UnityEngine.Component
        {
            return transform.GetComponentInChildren<T>().transform;
        }
    }
}
