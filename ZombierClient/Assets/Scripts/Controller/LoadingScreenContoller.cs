using Prototype.Service;
using Prototype.View;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class LoadingScreenContoller : MonoBehaviour
{
    [Inject]
    public void Construct(
        GameConfigDBService gameConfigDb,
        LoadingScreenUIView loadingScreenUIVew,
        UsersDbService usersDb)
    {
        _gameConfigDb = gameConfigDb;
        _loadingScreenUIView = loadingScreenUIVew;
        _usersDb = usersDb;
    }

    public async Task FetchGameBalanceAsync()
    {
        _loadingScreenUIView.SetLoadingState(LoadingScreenUIView.LoadingState.DownloadingGameBalance);

        await _gameConfigDb.FetchGameBalanceConfig();
        _gameConfigDb.UpdateGameBalance();
        IsGameBalanceFetchComplete = true;

        _loadingScreenUIView.SetLoadingState(LoadingScreenUIView.LoadingState.LoadingScene);
    }

    public async Task LoadUserProfileAsync()
    {
        Debug.Log("Loading user profile...");
        await _usersDb.LoadUserAsync();
        Debug.Log("User profile load completed");
    }

    public bool IsGameBalanceFetchComplete { get; private set; }

    // Injected
    private GameConfigDBService _gameConfigDb;
    private UsersDbService _usersDb;
    private LoadingScreenUIView _loadingScreenUIView;
}
