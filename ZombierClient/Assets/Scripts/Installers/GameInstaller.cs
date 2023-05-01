using Cinemachine;
using Prototype.Controller;
using Prototype.Data;
using Prototype.Model;
using Prototype.Service;
using Prototype.View;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private PlayerModel PlayerPrefab;

        [SerializeField] private GameplaySessionData _session;

        public override void InstallBindings()
        {
            Config.CreateGameMetaData(out var meta);
            Config.CreateUserData(meta, out var user);
            Config.CreateGameplaySessionData(user, out _session);

            Container.Bind<GameplaySessionData>().FromInstance(_session).AsSingle();

            Container.Bind<FloatingJoystick>().FromComponentInHierarchy().AsSingle();
            Container.Bind<GameplayHudUIView>().FromComponentInHierarchy().AsSingle();

            Container.Bind<GameplayInputService>().AsSingle();
            Container.Bind<GameplayEventService>().AsSingle();

            Container.Bind<LevelLoadController>().FromComponentInHierarchy().AsSingle();

            Container.Bind<MarkerLevel>().FromComponentInHierarchy().AsSingle();
            Container.Bind<MarkerEntities>().FromComponentInHierarchy().AsSingle();

            Container.Bind<PlayerModel>().FromSubContainerResolve().ByNewContextPrefab(PlayerPrefab).AsSingle();
            Container.Bind<CinemachineVirtualCamera>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesTo<CameraController>().AsSingle();
        }


    }
}
