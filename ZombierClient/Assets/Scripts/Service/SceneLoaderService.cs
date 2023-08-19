using DG.Tweening;
using Prototype.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prototype.Service
{
    // TODO: Separate transition animations logic from scene loading logic
    // for possible reuse in future projects
    public class SceneLoaderService
    {
        // Public

        public SceneLoaderService(AppEventService appEventService)
        {
            _transitionsGraph = new Dictionary<
                Tuple<Scene, Scene>,
                Tuple<Func<Scene, TransitionSettings, IEnumerator>, TransitionSettings>>();
            _appEventService = appEventService;
            _currentScene = Scene.Bootstrap;
            //
            RegisterTransition(Scene.Bootstrap, Scene.MainMenu, LoadWithLoadingScreenAndFooter, new TransitionSettings { ShouldFetchGameBalance = true });

            RegisterTransition(Scene.Game, Scene.Game, LoadWithLoadingScreen);

            RegisterTransition(Scene.MainMenu, Scene.Game, LoadWithLoadingScreen);
            RegisterTransition(Scene.Game, Scene.MainMenu, LoadWithLoadingScreenAndFooter);
            RegisterTransition(Scene.Game, Scene.Results, LoadWithLoadingScreen);
            RegisterTransition(Scene.Results, Scene.MainMenu, LoadWithLoadingScreenAndFooter);

            RegisterTransition(Scene.MainMenu, Scene.Shop, LoadHorizontalTransitionRight);
            RegisterTransition(Scene.MainMenu, Scene.Inventory, LoadHorizontalTransitionLeft);

            RegisterTransition(Scene.Shop, Scene.MainMenu, LoadHorizontalTransitionLeft);
            RegisterTransition(Scene.Shop, Scene.Inventory, LoadHorizontalTransitionLeft);

            RegisterTransition(Scene.Inventory, Scene.MainMenu, LoadHorizontalTransitionRight);
            RegisterTransition(Scene.Inventory, Scene.Shop, LoadHorizontalTransitionRight);
            //
            _appEventService.LoadScene += HandleLoadScene;
        }

        ~SceneLoaderService()
        {
            _appEventService.LoadScene -= HandleLoadScene;
        }

        public float GetLoadingProgress()
        {
            if (_targetOperation != null)
            {
                return Mathf.Clamp(_targetOperation.progress / 0.9f, 0f, 1f);
            }

            return 1f;
        }

        private struct TransitionSettings
        {
            public bool ShouldFetchGameBalance;
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

        // Injected
        private AppEventService _appEventService;
        // Internal variables
        private bool isSceneLoading = false;
        private AsyncOperation _loadingOperation;
        private AsyncOperation _targetOperation;
        private Dictionary<
            Tuple<Scene, Scene>,
            Tuple<Func<Scene, TransitionSettings, IEnumerator>, TransitionSettings>> _transitionsGraph;
        private Scene _currentScene;

        private void HandleLoadScene(object sender, LoadSceneEventArgs e)
        {
            Load(e.To);
        }

        private void RegisterTransition(Scene from, Scene to, Func<Scene, TransitionSettings, IEnumerator> transition, TransitionSettings settings = default)
        {
            Tuple<Scene, Scene> tupleKey = new Tuple<Scene, Scene>(from, to);
            Tuple<Func<Scene, TransitionSettings, IEnumerator>, TransitionSettings> tupleValue =
                new Tuple<Func<Scene, TransitionSettings, IEnumerator>, TransitionSettings>(transition, settings);
            _transitionsGraph.Add(tupleKey, tupleValue);
        }

        private void Load(Scene targetScene)
        {
            if (!isSceneLoading)
            {
                isSceneLoading = true;
                Scene currentScene = _currentScene;
                var tuple = new Tuple<Scene, Scene>(currentScene, targetScene);

                if (_transitionsGraph.ContainsKey(tuple))
                {
                    CoroutineRunner.Instance.StartCoroutine(_transitionsGraph[tuple].Item1(targetScene, _transitionsGraph[tuple].Item2));
                }
                else
                {
                    isSceneLoading = false;
                    Debug.LogWarning($"There is no transition from {currentScene} to {targetScene}");
                }
            }
        }

        #region Transition Coroutines

        private IEnumerator LoadWithLoadingScreen(Scene targetSceneName, TransitionSettings settings = default)
        {
            Scene currentSceneName = _currentScene;

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

            var footerScene = SceneManager.GetSceneByName(Scene.Footer.ToString());
            AsyncOperation footerUnloadOperation = null;
            if (footerScene.isLoaded)
            {
                footerUnloadOperation = SceneManager.UnloadSceneAsync(footerScene.name);
            }

            while (footerUnloadOperation != null && !footerUnloadOperation.isDone)
            {
                yield return null;
            }

            var footerCanvas = UnityEngine.Object.FindObjectOfType<FooterCanvasUIView>();
            if (footerCanvas != null)
            {
                UnityEngine.Object.Destroy(footerCanvas.gameObject);
            }

            SceneManager.UnloadSceneAsync(currentSceneName.ToString());

            _targetOperation = SceneManager.LoadSceneAsync(targetSceneName.ToString(), LoadSceneMode.Additive);
            _targetOperation.allowSceneActivation = false;

            while (_targetOperation.progress < .9f)
            {
                yield return null;
            }

            _targetOperation.allowSceneActivation = true;
            Time.timeScale = 0f;
            yield return loadingScreenRect.DOAnchorPosY(loadingScreenRect.anchoredPosition.y + loadingScreenCanvasRect.rect.height, transitionDuration)
                .SetUpdate(UpdateType.Normal, true)
                .WaitForCompletion();

            _currentScene = targetSceneName;
            SceneManager.UnloadSceneAsync(Scene.Loading.ToString());
            isSceneLoading = false;
            Time.timeScale = 1f;
            UnityEngine.Object.Destroy(CoroutineRunner.Instance.gameObject);
        }

        private IEnumerator LoadHorizontalTransitionRight(Scene targetSceneName, TransitionSettings settings = default)
        {
            Scene currentSceneName = _currentScene;

            var currentScene = SceneManager.GetSceneByName(currentSceneName.ToString());

            RectTransform currentScreenRect = currentScene
                .GetRootGameObjects().Where(_ => _.GetComponentInChildren<ScreenUIViewBase>() != null)
                .Select(_ => _.GetComponentInChildren<ScreenUIViewBase>())
                .FirstOrDefault().GetComponent<RectTransform>();

            RectTransform currentScreenCanvasRect = currentScreenRect.GetComponentInParent<Canvas>()
                .GetComponent<RectTransform>();

            Camera currentSceneCamera = currentScene
                .GetRootGameObjects().Where(_ => _.GetComponentInChildren<Camera>() != null)
                .Select(_ => _.GetComponentInChildren<Camera>())
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
                .GetRootGameObjects().Where(_ => _.GetComponentInChildren<ScreenUIViewBase>() != null)
                .Select(_ => _.GetComponentInChildren<ScreenUIViewBase>())
                .FirstOrDefault().GetComponent<RectTransform>();

            RectTransform targetScreenCanvasRect = targetScreenRect.GetComponentInParent<Canvas>()
                .GetComponent<RectTransform>();

            Camera targetSceneCamera = targetScene
                .GetRootGameObjects().Where(_ => _.GetComponentInChildren<Camera>() != null)
                .Select(_ => _.GetComponentInChildren<Camera>())
                .FirstOrDefault();

            targetSceneCamera.enabled = false;

            float transitionDuration = 0.25f;
            currentScreenRect.DOAnchorPosX(currentScreenRect.anchoredPosition.x - currentScreenCanvasRect.rect.width, transitionDuration);
            yield return targetScreenRect.DOAnchorPosX(targetScreenRect.anchoredPosition.x + targetScreenCanvasRect.rect.width, transitionDuration)
                .From()
                .WaitForCompletion();

            targetSceneCamera.enabled = true;

            _currentScene = targetSceneName;
            SceneManager.UnloadSceneAsync(currentScene.name);
            isSceneLoading = false;
            UnityEngine.Object.Destroy(CoroutineRunner.Instance.gameObject);
        }

        private IEnumerator LoadHorizontalTransitionLeft(Scene targetSceneName, TransitionSettings settings = default)
        {
            Scene currentSceneName = _currentScene;

            var currentScene = SceneManager.GetSceneByName(currentSceneName.ToString());

            RectTransform currentScreenRect = currentScene
                .GetRootGameObjects().Where(_ => _.GetComponentInChildren<ScreenUIViewBase>() != null)
                .Select(_ => _.GetComponentInChildren<ScreenUIViewBase>())
                .FirstOrDefault().GetComponent<RectTransform>();

            RectTransform currentScreenCanvasRect = currentScreenRect.GetComponentInParent<Canvas>()
                .GetComponent<RectTransform>();

            Camera currentSceneCamera = currentScene
                .GetRootGameObjects().Where(_ => _.GetComponentInChildren<Camera>() != null)
                .Select(_ => _.GetComponentInChildren<Camera>())
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
                .GetRootGameObjects().Where(_ => _.GetComponentInChildren<ScreenUIViewBase>() != null)
                .Select(_ => _.GetComponentInChildren<ScreenUIViewBase>())
                .FirstOrDefault().GetComponent<RectTransform>();

            RectTransform targetScreenCanvasRect = targetScreenRect.GetComponentInParent<Canvas>()
                .GetComponent<RectTransform>();

            Camera targetSceneCamera = targetScene
                .GetRootGameObjects().Where(_ => _.GetComponentInChildren<Camera>() != null)
                .Select(_ => _.GetComponentInChildren<Camera>())
                .FirstOrDefault();

            targetSceneCamera.enabled = false;

            float transitionDuration = 0.25f;
            currentScreenRect.DOAnchorPosX(currentScreenRect.anchoredPosition.x + currentScreenCanvasRect.rect.width, transitionDuration);
            yield return targetScreenRect.DOAnchorPosX(targetScreenRect.anchoredPosition.x - targetScreenCanvasRect.rect.width, transitionDuration)
                .From()
                .WaitForCompletion();

            targetSceneCamera.enabled = true;

            _currentScene = targetSceneName;
            SceneManager.UnloadSceneAsync(currentScene.name);
            isSceneLoading = false;
            UnityEngine.Object.Destroy(CoroutineRunner.Instance.gameObject);
        }

        private IEnumerator LoadWithLoadingScreenAndFooter(Scene targetSceneName, TransitionSettings settings = default)
        {
            Scene currentSceneName = _currentScene;

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

            SceneManager.UnloadSceneAsync(currentSceneName.ToString());

            if (settings.ShouldFetchGameBalance)
            {
                LoadingScreenContoller loadingScreenController = loadindScene
                    .GetRootGameObjects().Where(_ => _.GetComponentInChildren<LoadingScreenContoller>() != null)
                    .Select(_ => _.GetComponentInChildren<LoadingScreenContoller>())
                    .FirstOrDefault();

                Task fetchGameBalanceTask = loadingScreenController.FetchGameBalanceAsync();
                yield return new WaitUntil(() => fetchGameBalanceTask.IsCompleted);

                Task loadUserProfileTask = loadingScreenController.LoadUserProfileAsync();
                yield return new WaitUntil(() => loadUserProfileTask.IsCompleted);

                while (!loadingScreenController.IsGameBalanceFetchComplete)
                {
                    yield return null;
                }
            }

            _targetOperation = SceneManager.LoadSceneAsync(targetSceneName.ToString(), LoadSceneMode.Additive);
            _targetOperation.allowSceneActivation = false;

            while (_targetOperation.progress < .9f)
            {
                yield return null;
            }

            _targetOperation.allowSceneActivation = true;

            AsyncOperation footerOperation = SceneManager.LoadSceneAsync(Scene.Footer.ToString(), LoadSceneMode.Additive);
            footerOperation.allowSceneActivation = false;

            while (footerOperation.progress < .9f)
            {
                yield return null;
            }

            footerOperation.allowSceneActivation = true;

            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(2f);
            yield return loadingScreenRect.DOAnchorPosY(loadingScreenRect.anchoredPosition.y + loadingScreenCanvasRect.rect.height, transitionDuration)
                .SetUpdate(UpdateType.Normal, true)
                .WaitForCompletion();

            _currentScene = targetSceneName;
            SceneManager.UnloadSceneAsync(Scene.Loading.ToString());
            isSceneLoading = false;
            Time.timeScale = 1f;
            UnityEngine.Object.Destroy(CoroutineRunner.Instance.gameObject);
        }
        #endregion
    }
}