using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Prototype.ObjectPool
{
    public class MonoObjectPool<T>
    where T : PoolObject
    {
        private PoolObjectFactory<T> _factory;
        private Queue<T> _queue;
        private Transform _transformContainer;
        private T _prefab;

        [Inject]
        public MonoObjectPool(PoolObjectFactory<T> factory)
        {
            _factory = factory;
            _queue = new Queue<T>();
        }

        public void Initialize(T prefab, int count, Transform transformContainer)
        {
            _prefab = prefab;
            _transformContainer = transformContainer;

            for (int i = 0; i < count; ++i)
            {
                T instance = Instantiate(prefab);
                instance.gameObject.SetActive(false);
                _queue.Enqueue(instance);
            }
        }

        public T Create(bool enabled = true)
        {
            return Create(_prefab, enabled);
        }

        public T Create(Vector3 position, Quaternion rotation, bool enabled = true)
        {
            return Create(_prefab, position, rotation, enabled);
        }

        public void Destroy(T instance)
        {
            if (!instance.gameObject.activeInHierarchy)
                return;

            instance.gameObject.SetActive(false);
            _queue.Enqueue(instance);
        }

        public IEnumerator Destroy(T instance, float sec)
        {
            if (!instance.gameObject.activeInHierarchy)
                yield break;

            yield return new WaitForSeconds(sec);
            Destroy(instance);
        }

        public IEnumerator Destroy(T instance, float sec, Action callback)
        {
            if (!instance.gameObject.activeInHierarchy)
                yield break;

            yield return new WaitForSeconds(sec);
            callback();
            Destroy(instance);
        }

        private T Create(T prefab, bool enabled = true)
        {
            T instance;
            if (_queue.Count > 0)
            {
                instance = _queue.Dequeue();
            }
            else
            {
                instance = Instantiate(prefab);
            }

            instance.gameObject.SetActive(enabled);
            return instance;
        }

        private T Create(T prefab, Vector3 position, Quaternion rotation, bool enabled = true)
        {
            T instance;
            if (_queue.Count > 0)
            {
                instance = _queue.Dequeue();
            }
            else
            {
                instance = Instantiate(prefab);
            }

            instance.transform.position = position;
            instance.transform.rotation = rotation;
            instance.gameObject.SetActive(enabled);
            return instance;
        }

        private T Instantiate(T prefab)
        {
            T instance = _factory.Create(prefab);
            instance.transform.SetParent(_transformContainer);
            instance.gameObject.SetActive(false);
            return instance;
        }
    }
}
