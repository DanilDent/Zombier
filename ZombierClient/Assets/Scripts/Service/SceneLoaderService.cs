using System;
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

        private static Action _onLoaderCallback;

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
                SceneManager.LoadScene(scene.ToString());
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
    }
}
