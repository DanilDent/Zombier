using Prototype.Data;
using Prototype.Model;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class LevelLoadController : MonoBehaviour
    {
        private MarkerLevel _markerLevel;
        private MarkerEntities _markerEntities;

        private PlayerModel _playerModel;
        private LevelModel _levelModel;

        [Inject]
        public void Construct(
            MarkerLevel markerLevel,
            MarkerEntities markerEntities,
            PlayerModel playerModel)
        {
            _markerLevel = markerLevel;
            _markerEntities = markerEntities;
            _playerModel = playerModel;
        }

        public void LoadLevel(in LocationData locationData, int level, in PlayerData playerData)
        {
            var descLevel = locationData.Levels[level];

            var levelPfab = Resources.Load<GameObject>(descLevel.ResourceTag);
            _levelModel = Instantiate(levelPfab, _markerLevel.transform).GetComponent<LevelModel>();

            PositionPlayer(playerData);

            var surface = _levelModel.Navmesh;
            surface.BuildNavMesh();
        }

        private void PositionPlayer(in PlayerData playerData)
        {
            _playerModel.transform.SetParent(_markerEntities.transform);
            _playerModel.transform.position = _levelModel.PlayerSpawnPoint.position;

        }
    }
}
