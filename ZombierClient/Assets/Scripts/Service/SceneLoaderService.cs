using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoaderService
{
    public enum Scene
    {
        MainMenu,
        Game,
        Loading,
        Results
    }

    private static bool isSceneLoading = false;
    private static int _loadingFrames = 10;
    private static AsyncOperation _loadingOperation;
    private static AsyncOperation _targetOperation;

    public static int LoadingFrames
    {
        get { return _loadingFrames; }
        set { _loadingFrames = value; }
    }

    public static void Load(Scene targetScene)
    {
        if (!isSceneLoading)
        {
            isSceneLoading = true;
            CoroutineRunner.Instance.StartCoroutine(LoadSceneCoroutine(targetScene));
        }
    }

    public static float GetLoadingProgress()
    {
        if (_targetOperation != null)
        {
            return _targetOperation.progress;
        }

        return 1f;
    }

    private static IEnumerator LoadSceneCoroutine(Scene targetScene)
    {
        GameObject sceneContextObject = GameObject.Find("SceneContext");
        if (sceneContextObject != null)
        {
            Object.Destroy(sceneContextObject);
        }

        float progress = 0f;

        _loadingOperation = SceneManager.LoadSceneAsync(Scene.Loading.ToString());
        _loadingOperation.allowSceneActivation = false;

        progress = _loadingOperation.progress;
        while (_loadingOperation.progress < .9f)
        {
            progress = _loadingOperation.progress;
            yield return null;
        }

        _loadingOperation.allowSceneActivation = true;

        _targetOperation = SceneManager.LoadSceneAsync(targetScene.ToString(), LoadSceneMode.Additive);
        _targetOperation.allowSceneActivation = false;

        progress = _targetOperation.progress;
        while (_targetOperation.progress < .9f)
        {
            progress = _targetOperation.progress;
            yield return null;
        }

        _targetOperation.allowSceneActivation = true;

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(5f);

        SceneManager.UnloadSceneAsync(Scene.Loading.ToString());
        isSceneLoading = false;
        Time.timeScale = 1f;
        Object.Destroy(CoroutineRunner.Instance.gameObject);
    }

    private class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner instance;
        public static CoroutineRunner Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject gameObject = new GameObject("CoroutineRunner");
                    instance = gameObject.AddComponent<CoroutineRunner>();
                    DontDestroyOnLoad(gameObject);
                }
                return instance;
            }
        }
    }
}
