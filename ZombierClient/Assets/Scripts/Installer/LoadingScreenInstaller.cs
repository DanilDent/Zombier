using Prototype.View;
using Zenject;

namespace Prototype
{
    public class LoadingScreenInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<LoadingScreenUIView>().FromComponentInHierarchy().AsSingle();
        }
    }
}
