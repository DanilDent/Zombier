using Prototype.Data;
using Prototype.LevelGeneration;
using UnityEngine;
using Zenject;

namespace Prototype.Factory
{
    public class ProceduralLevelFactory : IFactory<LevelModel>
    {
        private readonly DiContainer _container;
        private readonly LevelGeneratorData _generatorConfig;
        private readonly LocationData _locationData;
        private readonly LevelData _levelData;
        private readonly MarkerLevel _markerLevel;

        public ProceduralLevelFactory(
            DiContainer container,
            LevelGeneratorData generatorConfig,
            LocationData locationData,
            LevelData levelData,
            MarkerLevel markerLevel)
        {
            _container = container;
            _generatorConfig = generatorConfig;
            _locationData = locationData;
            _levelData = levelData;
            _markerLevel = markerLevel;
        }

        public LevelModel Create()
        {
            LevelGenerator levelGenerator = new LevelGenerator(_generatorConfig, _locationData, _levelData);
            GameObject levelGameObject = levelGenerator.GenerateLevel();
            levelGameObject.transform.SetParent(_markerLevel.transform);
            return _container.InstantiateComponent<LevelModel>(levelGameObject);
        }
    }
}
