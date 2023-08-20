using System;
using System.Collections;
using UnityEngine;

namespace Prototype
{
    public static class Helpers
    {
        public static TComp CreateSceneObject<TComp>(Transform parent)
                where TComp : Component
        {
            var obj = new GameObject();
            obj.transform.parent = parent.transform;
            obj.name = typeof(TComp).Name;
            var comp = obj.AddComponent<TComp>();
            return comp;
        }

        public static IEnumerator InvokeWithDelay(Action action, float sec)
        {
            yield return new WaitForSeconds(sec);
            action();
        }

        public static IEnumerator InvokeWithDelay<T>(Action<T> action, T param, float sec)
        {
            yield return new WaitForSeconds(sec);
            action(param);
        }

        public static bool TryRandom(float probability)
        {
            probability = Mathf.Clamp01(probability);
            return UnityEngine.Random.Range(0f, 1f) < probability;
        }


    }

    public class CoroutineRunner : MonoBehaviour
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