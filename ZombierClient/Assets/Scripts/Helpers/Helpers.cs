using Prototype.Data;
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

        public static IEnumerator InvokeWithDelay(Action func, float sec)
        {
            yield return new WaitForSeconds(sec);
            func();
        }

        public static IEnumerator InvokeWithDelay(Action<IdData> func, IdData id, float sec)
        {
            yield return new WaitForSeconds(sec);
            func(id);
        }
    }

}