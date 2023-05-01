using Prototype.Data;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class GameplayController : MonoBehaviour
    {
        private GameplaySessionData _session;
        private LevelLoadController _levelLoader;

        [Inject]
        public void Construct(GameplaySessionData session, LevelLoadController levelLoader)
        {
            _levelLoader = levelLoader;
            _session = session;
        }

        private void Start()
        {
            var locationId = _session.IdCurrentLocation;
            var locationData = _session.Data.Locations.Find(x => x.IdSession.Equals(locationId));
            _levelLoader.LoadLevel(locationData, _session.CurrentStage, _session.Data.Player);
        }
    }
}
