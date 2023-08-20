using Prototype;
using Prototype.Data;
using UnityEngine;
using Zenject;

public class LevelModel
{
    [Inject]
    public void Construct(GameSessionData session)
    {
        _session = session;
    }

    // Injected
    public MarkerLevelExitPoint ExitPoint { get; set; }
    // Properties
    public GameObject LevelView { get; set; }

    public EnemySpawnData EnemySpawnData
    {
        get => _session.Location.Levels[_session.CurrentLevelIndex].EnemySpawnData;
        private set => _session.Location.Levels[_session.CurrentLevelIndex].EnemySpawnData = value;
    }

    // Private
    private GameSessionData _session;
}
