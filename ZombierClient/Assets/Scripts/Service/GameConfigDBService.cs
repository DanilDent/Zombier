using Firebase.Extensions;
using Firebase.RemoteConfig;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class GameConfigDBService
{
    public string GetTestGameBalanceJsonString()
    {
        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        string json = remoteConfig.GetValue("GameBalance").StringValue;
        Debug.Log($"Game config json: {json}");
        return json;
    }

    public async Task FetchGameBalanceConfig()
    {
        Debug.Log("Fetching game balance...");
        await FetchDataAsync();
        Debug.Log("Game balance fetching complete");
    }

    // Start a fetch request.
    // FetchAsync only fetches new data if the current data is older than the provided
    // timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
    // By default the timespan is 12 hours, and for production apps, this is a good
    // number. For this example though, it's set to a timespan of zero, so that
    // changes in the console will always show up immediately.
    private Task FetchDataAsync()
    {
        Debug.Log("Fetching data...");
        Task fetchTask =
        FirebaseRemoteConfig.DefaultInstance.FetchAsync(
            TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }

    private void FetchComplete(Task fetchTask)
    {
        if (!fetchTask.IsCompleted)
        {
            Debug.LogError("Retrieval hasn't finished.");
            return;
        }

        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        var info = remoteConfig.Info;
        if (info.LastFetchStatus != LastFetchStatus.Success)
        {
            Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
            return;
        }

        // Fetch successful. Parameter values must be activated to use.
        remoteConfig.ActivateAsync()
          .ContinueWithOnMainThread(
            task =>
            {
                Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");
            });
    }
}
