using System;
using System.Collections;
using UnityEngine;

namespace Prototype.ObjectPool
{
    public interface IMonoObjectPool<T> where T : PoolObject
    {
        T Create(bool enabled = true);
        T Create(Vector3 position, Quaternion rotation, bool enabled = true);
        void Destroy(T instance);
        IEnumerator Destroy(T instance, float sec);
        IEnumerator Destroy(T instance, float sec, Action callback);
        void Initialize(T prefab, int count, Transform transformContainer);
        void Initialize(T prefab, int count, Transform transformContainer, Action<T> onInstantiated);
    }
}