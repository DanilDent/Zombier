using Zenject;

namespace Prototype
{
    public class EnemyInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<MarkerTargetPoint>().FromComponentInChildren().AsSingle();
        }
    }
}
