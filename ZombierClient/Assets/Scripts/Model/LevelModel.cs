using Prototype;
using Prototype.Data;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class LevelModel : MonoBehaviour
{
    [Inject]
    public void Construct(GameplaySessionData session, NavMeshSurface navmesh, MarkerLevelExitPoint exitPoint)
    {
        _session = session;
        Navmesh = navmesh;
        ExitPoint = exitPoint;
    }

    // Injected
    public NavMeshSurface Navmesh { get; private set; }
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
