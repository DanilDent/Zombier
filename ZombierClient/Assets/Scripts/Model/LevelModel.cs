using Prototype;
using Prototype.Data;
using UnityEngine;
using Zenject;

public class LevelModel : MonoBehaviour
{
    [Inject]
    public void Construct(GameplaySessionData session, MarkerLevelExitPoint exitPoint)
    {
        _session = session;
        ExitPoint = exitPoint;
    }

    // Injected
    public MarkerLevelExitPoint ExitPoint { get; private set; }
    // Properties
    public EnemySpawnData EnemySpawnData
    {
        get => _session.Location.Levels[_session.CurrentLevelIndex].EnemySpawnData;
        private set => _session.Location.Levels[_session.CurrentLevelIndex].EnemySpawnData = value;
    }

    // Private
    private GameplaySessionData _session;
}
