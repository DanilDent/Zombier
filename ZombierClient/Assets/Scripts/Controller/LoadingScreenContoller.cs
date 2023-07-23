using Prototype.View;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class LoadingScreenContoller : MonoBehaviour
{
    [Inject]
    public void Construct(GameConfigDBService gameConfigDb, LoadingScreenUIView loadingScreenUIVew)
    {
        _gameConfigDb = gameConfigDb;
        _loadingScreenUIView = loadingScreenUIVew;
    }

    public async Task FetchGameBalanceAsync()
    {
        _loadingScreenUIView.SetLoadingState(LoadingScreenUIView.LoadingState.DownloadingGameBalance);

        await _gameConfigDb.FetchGameBalanceConfig();
        _gameConfigDb.GetTestGameBalanceJsonString();
        IsGameBalanceFetchComplete = true;

        _loadingScreenUIView.SetLoadingState(LoadingScreenUIView.LoadingState.LoadingScene);
    }

    public bool IsGameBalanceFetchComplete { get; private set; }

    // Injected
    private GameConfigDBService _gameConfigDb;
    private LoadingScreenUIView _loadingScreenUIView;
}
