using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prototype.Service
{
    public static class SceneLoaderService
    {
        public enum Scene
        {
            Game,
            App,
            Loading,
        }


        public static void Load(Scene scene)
        {
            GameObject sc = GameObject.Find("SceneContext");
            if (sc != null)
            {
                UnityEngine.Object.Destroy(sc);
            }

            // Set the loader callback action to load the target scene
            _onLoaderCallback = () =>
            {
                GameObject loadingGameObject = new GameObject("Loading Game Object");
                loadingGameObject.AddComponent<LoadingMonoBehaviour>().StartCoroutine(LoadSceneAsync(scene));
            };

            // Load the loading scene
            SceneManager.LoadScene(Scene.Loading.ToString());
        }

        public static void LoaderCallback()
        {
            // Triggered after the first Update which lets the screen refresh
            // Execute the loader callback action which will load the target scene
            if (_onLoaderCallback != null)
            {
                _onLoaderCallback();
                _onLoaderCallback = null;
            }
        }

        public static float GetLoadingProgress()
        {
            if (_asyncOperation != null)
            {
                return _asyncOperation.progress;
            }

            return 1f;
        }

        private class LoadingMonoBehaviour : MonoBehaviour { }

        private static Action _onLoaderCallback;
        private static AsyncOperation _asyncOperation;

        private static IEnumerator LoadSceneAsync(Scene scene)
        {
            yield return null;

            _asyncOperation = SceneManager.LoadSceneAsync(scene.ToString());

            while (!_asyncOperation.isDone)
            {
                yield return null;
            }
        }
    }
}
