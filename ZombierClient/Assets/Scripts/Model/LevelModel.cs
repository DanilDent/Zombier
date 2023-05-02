using Prototype;
using Prototype.SO;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class LevelModel : MonoBehaviour
{
    [Inject]
    public void Construct(NavMeshSurface navmesh, MarkerLevelExitPoint exitPoint)
    {
        Navmesh = navmesh;
        ExitPoint = exitPoint;
    }

    // Injected
    public NavMeshSurface Navmesh { get; private set; }
    public MarkerLevelExitPoint ExitPoint { get; private set; }
    // Properties
    public EnemySpawnSO EnemySpawnSO
    {
        get => _enemySpawnSO;
        private set => _enemySpawnSO = value;
    }

    // Private
    [SerializeField] private EnemySpawnSO _enemySpawnSO;
}
