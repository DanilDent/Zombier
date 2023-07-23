using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class LoadingScreenContoller : MonoBehaviour
{
    [Inject]
    public void Construct(GameConfigDBService gameConfigDb)
    {
        _gameConfigDb = gameConfigDb;
    }

    public async Task FetchGameBalanceAsync()
    {
        await _gameConfigDb.FetchGameBalanceConfig();
        _gameConfigDb.GetTestGameBalanceJsonString();
        IsGameBalanceFetchComplete = true;
    }

    public bool IsGameBalanceFetchComplete { get; private set; }

    // Injected
    private GameConfigDBService _gameConfigDb;
}
