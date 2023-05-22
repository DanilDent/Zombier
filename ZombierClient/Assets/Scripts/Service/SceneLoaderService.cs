using DG.Tweening;
using Prototype.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private static AsyncOperation _loadingOperation;
    private static AsyncOperation _targetOperation;
    private static Dictionary<Tuple<Scene, Scene>, Func<Scene, IEnumerator>> _transitionsGraph;

    static SceneLoaderService()
    {
        _transitionsGraph = new Dictionary<Tuple<Scene, Scene>, Func<Scene, IEnumerator>>();

        RegisterTransition(Scene.MainMenu, Scene.Game, LoadWithLoadingScreen);
        RegisterTransition(Scene.Game, Scene.MainMenu, LoadWithLoadingScreen);
        RegisterTransition(Scene.Game, Scene.Results, LoadWithLoadingScreen);
        RegisterTransition(Scene.Results, Scene.MainMenu, LoadWithLoadingScreen);
        RegisterTransition(Scene.Game, Scene.Game, LoadWithLoadingScreen);
    }

    private static void RegisterTransition(Scene from, Scene to, Func<Scene, IEnumerator> transition)
    {
        Tuple<Scene, Scene> tupleKey = new Tuple<Scene, Scene>(from, to);
        _transitionsGraph.Add(tupleKey, transition);
    }

    public static void Load(Scene targetScene)
    {
        if (!isSceneLoading)
        {
            isSceneLoading = true;
            Scene currentScene = (Scene)Enum.Parse(typeof(Scene), SceneManager.GetActiveScene().name);
            var tuple = new Tuple<Scene, Scene>(currentScene, targetScene);
            CoroutineRunner.Instance.StartCoroutine(_transitionsGraph[tuple](targetScene));
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
            UnityEngine.Object.Destroy(sceneContextObject);
        }

        _loadingOperation = SceneManager.LoadSceneAsync(Scene.Loading.ToString());
        _loadingOperation.allowSceneActivation = false;

        while (_loadingOperation.progress < .9f)
        {
            yield return null;
        }

        _loadingOperation.allowSceneActivation = true;

        _targetOperation = SceneManager.LoadSceneAsync(targetScene.ToString(), LoadSceneMode.Additive);
        _targetOperation.allowSceneActivation = false;

        while (_targetOperation.progress < .9f)
        {
            yield return null;
        }

        _targetOperation.allowSceneActivation = true;

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(2f);

        SceneManager.UnloadSceneAsync(Scene.Loading.ToString());
        isSceneLoading = false;
        Time.timeScale = 1f;
        UnityEngine.Object.Destroy(CoroutineRunner.Instance.gameObject);
    }

    private static IEnumerator LoadWithLoadingScreen(Scene targetScene)
    {
        GameObject sceneContextObject = GameObject.Find("SceneContext");
        if (sceneContextObject != null)
        {
            UnityEngine.Object.Destroy(sceneContextObject);
        }

        Scene currentScene = (Scene)Enum.Parse(typeof(Scene), SceneManager.GetActiveScene().name);

        _loadingOperation = SceneManager.LoadSceneAsync(Scene.Loading.ToString(), LoadSceneMode.Additive);
        _loadingOperation.allowSceneActivation = false;

        while (_loadingOperation.progress < .9f)
        {
            yield return null;
        }

        _loadingOperation.allowSceneActivation = true;

        yield return new WaitUntil(() => SceneManager.GetSceneByName(Scene.Loading.ToString()).isLoaded);

        var loadindScene = SceneManager.GetSceneByName(Scene.Loading.ToString());

        RectTransform loadingScreenRect = loadindScene
            .GetRootGameObjects().Where(_ => _.GetComponentInChildren<LoadingScreenUIView>() != null)
            .Select(_ => _.GetComponentInChildren<LoadingScreenUIView>())
            .FirstOrDefault().GetComponent<RectTransform>();
        RectTransform loadingScreenCanvasRect = loadingScreenRect.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        yield return loadingScreenRect.DOAnchorPosY(loadingScreenRect.anchoredPosition.y - loadingScreenCanvasRect.rect.height, duration: 1f)
            .From()
            .WaitForCompletion();

        SceneManager.UnloadSceneAsync(currentScene.ToString());

        _targetOperation = SceneManager.LoadSceneAsync(targetScene.ToString(), LoadSceneMode.Additive);
        _targetOperation.allowSceneActivation = false;

        while (_targetOperation.progress < .9f)
        {
            yield return null;
        }

        _targetOperation.allowSceneActivation = true;

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(5f);
        yield return loadingScreenRect.DOAnchorPosY(loadingScreenRect.anchoredPosition.y + loadingScreenCanvasRect.rect.height, duration: 1f)
            .SetUpdate(UpdateType.Normal, true)
            .WaitForCompletion();

        SceneManager.UnloadSceneAsync(Scene.Loading.ToString());
        isSceneLoading = false;
        Time.timeScale = 1f;
        UnityEngine.Object.Destroy(CoroutineRunner.Instance.gameObject);
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
