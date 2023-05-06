using Prototype.Data;
using Prototype.Model;
using Prototype.Service;
using Prototype.View;
using Zenject;

namespace Prototype
{
    public class EnemyInstaller : Installer<EnemyInstaller>
    {
        [Inject] private EnemyData _dataTemplate;

        public override void InstallBindings()
        {
            Container.BindInstance(IdProviderService.GetNewId()).AsSingle();
            Container.Bind<EnemyModel>().FromComponentOnRoot().AsSingle();
            Container.BindInstance(_dataTemplate);
            if (_dataTemplate != null)
            {
                Container.Bind<EnemyView>()
                .FromComponentInNewPrefab(_dataTemplate.ViewPrefab)
                .UnderTransformGroup("View")
                .AsSingle()
                .NonLazy();
            }
        }

    }
}
