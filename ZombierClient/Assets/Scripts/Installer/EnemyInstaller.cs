using Prototype.Data;
using Prototype.Model;
using Prototype.View;
using Zenject;

namespace Prototype
{
    public class EnemyInstaller : Installer<EnemyInstaller>
    {
        [Inject] private IdData _id;
        [Inject] private EnemyData _dataTemplate;

        public override void InstallBindings()
        {
            Container.BindInstance(_id).AsSingle();
            Container.Bind<EnemyModel>().FromComponentOnRoot().AsSingle();
            Container.BindInstance(_dataTemplate);
            if (_dataTemplate != null)
            {
                Container.Bind<EnemyView>()
                    .FromComponentInNewPrefabResource(_dataTemplate.ViewPrefabAddress)
                    .UnderTransformGroup("View")
                    .AsSingle()
                    .NonLazy();
            }
        }

    }
}
