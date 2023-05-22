using DG.Tweening;
using Prototype.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prototype.Service
{
    public class SceneLoaderService
    {
        private bool isSceneLoading = false;
        private AsyncOperation _loadingOperation;
        private AsyncOperation _targetOperation;
        private Dictionary<Tuple<Scene, Scene>, Func<Scene, IEnumerator>> _transitionsGraph;
        private AppEventService _appEventService;

        public SceneLoaderService(AppEventService appEventService)
        {
            _transitionsGraph = new Dictionary<Tuple<Scene, Scene>, Func<Scene, IEnumerator>>();
            _appEventService = appEventService;
            //
            RegisterTransition(Scene.MainMenu, Scene.Game, LoadWithLoadingScreen);
            RegisterTransition(Scene.Game, Scene.MainMenu, LoadWithLoadingScreen);
            RegisterTransition(Scene.Game, Scene.Results, LoadWithLoadingScreen);
            RegisterTransition(Scene.Results, Scene.MainMenu, LoadWithLoadingScreen);
            RegisterTransition(Scene.Game, Scene.Game, LoadWithLoadingScreen);
            //
            _appEventService.LoadScene += HandleLoadScene;
        }

        ~SceneLoaderService()
        {
            _appEventService.LoadScene -= HandleLoadScene;
        }

        private void HandleLoadScene(object sender, LoadSceneEventArgs e)
        {
            Load(e.To);
        }

        private void RegisterTransition(Scene from, Scene to, Func<Scene, IEnumerator> transition)
        {
            Tuple<Scene, Scene> tupleKey = new Tuple<Scene, Scene>(from, to);
            _transitionsGraph.Add(tupleKey, transition);
        }

        private void Load(Scene targetScene)
        {
            if (!isSceneLoading)
            {
                isSceneLoading = true;
                Scene currentScene = (Scene)Enum.Parse(typeof(Scene), SceneManager.GetActiveScene().name);
                var tuple = new Tuple<Scene, Scene>(currentScene, targetScene);
                CoroutineRunner.Instance.StartCoroutine(_transitionsGraph[tuple](targetScene));
            }
        }

        public float GetLoadingProgress()
        {
            if (_targetOperation != null)
            {
                return _targetOperation.progress;
            }

            return 1f;
        }

        private IEnumerator LoadWithLoadingScreen(Scene targetSceneName)
        {
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

            Camera loadingSceneCamera = loadindScene
                .GetRootGameObjects().Where(_ => _.GetComponentInChildren<Camera>() != null)
                .Select(_ => _.GetComponentInChildren<Camera>())
                .FirstOrDefault().GetComponent<Camera>();

            loadingSceneCamera.enabled = false;

            float transitionDuration = 0.5f;
            yield return loadingScreenRect.DOAnchorPosY(loadingScreenRect.anchoredPosition.y - loadingScreenCanvasRect.rect.height, transitionDuration)
                .From()
                .WaitForCompletion();

            loadingSceneCamera.enabled = true;

            SceneManager.UnloadSceneAsync(currentScene.ToString());

            _targetOperation = SceneManager.LoadSceneAsync(targetSceneName.ToString(), LoadSceneMode.Additive);
            _targetOperation.allowSceneActivation = false;

            while (_targetOperation.progress < .9f)
            {
                yield return null;
            }

            _targetOperation.allowSceneActivation = true;

            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(2f);
            yield return loadingScreenRect.DOAnchorPosY(loadingScreenRect.anchoredPosition.y + loadingScreenCanvasRect.rect.height, transitionDuration)
                .SetUpdate(UpdateType.Normal, true)
                .WaitForCompletion();

            SceneManager.UnloadSceneAsync(Scene.Loading.ToString());
            isSceneLoading = false;
            Time.timeScale = 1f;
            UnityEngine.Object.Destroy(CoroutineRunner.Instance.gameObject);
        }

        public IEnumerator LoadHorizontalTransitionLeft(Scene targetSceneName)
        {
            Scene currentSceneName = (Scene)Enum.Parse(typeof(Scene), SceneManager.GetActiveScene().name);


            var currentScene = SceneManager.GetSceneByName(targetSceneName.ToString());

            RectTransform currentScreenRect = currentScene
                .GetRootGameObjects().Where(_ => _.GetComponent<ScreenUIViewBase>() != null)
                .Select(_ => _.GetComponent<ScreenUIViewBase>())
                .FirstOrDefault().GetComponent<RectTransform>();

            RectTransform currentScreenCanvasRect = currentScreenRect.GetComponentInParent<Canvas>()
                .GetComponent<RectTransform>();

            Camera currentSceneCamera = currentScene
                .GetRootGameObjects().Where(_ => _.GetComponent<Camera>() != null)
                .Select(_ => _.GetComponent<Camera>())
                .FirstOrDefault();


            _targetOperation = SceneManager.LoadSceneAsync(targetSceneName.ToString(), LoadSceneMode.Additive);
            _targetOperation.allowSceneActivation = false;

            while (_targetOperation.progress < .9f)
            {
                yield return null;
            }

            _targetOperation.allowSceneActivation = true;

            yield return new WaitUntil(() => SceneManager.GetSceneByName(targetSceneName.ToString()).isLoaded);

            var targetScene = SceneManager.GetSceneByName(targetSceneName.ToString());

            RectTransform targetScreenRect = targetScene
                .GetRootGameObjects().Where(_ => _.GetComponent<ScreenUIViewBase>() != null)
                .Select(_ => _.GetComponent<ScreenUIViewBase>())
                .FirstOrDefault().GetComponent<RectTransform>();

            RectTransform targetScreenCanvasRect = targetScreenRect.GetComponentInParent<Canvas>()
                .GetComponent<RectTransform>();

            Camera targetSceneCamera = targetScene
                .GetRootGameObjects().Where(_ => _.GetComponent<Camera>() != null)
                .Select(_ => _.GetComponent<Camera>())
                .FirstOrDefault();

            targetSceneCamera.enabled = false;

            float transitionDuration = 0.5f;
            currentScreenRect.DOAnchorPosX(currentScreenRect.anchoredPosition.x - currentScreenCanvasRect.rect.width, transitionDuration);
            yield return targetScreenRect.DOAnchorPosX(targetScreenRect.anchoredPosition.x + targetScreenCanvasRect.rect.width, transitionDuration)
                .From()
                .WaitForCompletion();

            targetSceneCamera.enabled = true;

            SceneManager.UnloadSceneAsync(currentScene.name);
            isSceneLoading = false;
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

}