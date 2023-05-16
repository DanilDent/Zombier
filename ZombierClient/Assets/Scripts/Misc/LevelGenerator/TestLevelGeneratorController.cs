using Prototype.Data;
using Prototype.MeshCombine;
using UnityEngine;

namespace Prototype.LevelGeneration
{
    public class TestLevelGeneratorController : MonoBehaviour
    {
        [SerializeField] private MeshCombiner _meshCombiner;
        [SerializeField] private LocationData _locationData;
        [SerializeField] private LevelData _levelData;
        [SerializeField] private LevelGeneratorData _levelGeneratorData;

        private LevelGenerator _levelGenerator;

        private void OnEnable()
        {
            _levelGenerator = new LevelGenerator(_meshCombiner, _levelGeneratorData, _locationData, _levelData);
            _levelGenerator.GenerateLevel();
        }

        private void OnDisable()
        {
            _levelGenerator = null;
        }
    }
}
