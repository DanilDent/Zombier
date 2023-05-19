using Prototype;
using Prototype.Data;
using UnityEngine;
using Zenject;

public class LevelModel : MonoBehaviour
{
    [Inject]
    public void Construct(GameSessionData session, MarkerLevelExitPoint exitPoint)
    {
        _session = session;
        ExitPoint = exitPoint;
    }

    public class Factory : PlaceholderFactory<LevelModel> { }

    // Injected
    public MarkerLevelExitPoint ExitPoint { get; private set; }
    // Properties
    public EnemySpawnData EnemySpawnData
    {
        get => _session.Location.Levels[_session.CurrentLevelIndex].EnemySpawnData;
        private set => _session.Location.Levels[_session.CurrentLevelIndex].EnemySpawnData = value;
    }

    // Private
    private GameSessionData _session;
}
