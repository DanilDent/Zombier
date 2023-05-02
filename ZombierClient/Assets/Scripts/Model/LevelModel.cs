using Prototype;
using Prototype.SO;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class LevelModel : MonoBehaviour
{
    public MarkerPlayerSpawnPoint PlayerSpawnPoint;
    public NavMeshSurface Navmesh;
    public EnemySpawnSO EnemySpawnSO;

    [Inject]
    public void Construct(MarkerPlayerSpawnPoint playerSpawnPoint, NavMeshSurface navmesh)
    {
        PlayerSpawnPoint = playerSpawnPoint;
        Navmesh = navmesh;
    }
}
