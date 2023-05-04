using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoObjectPool<T>
    where T : MonoBehaviour
{
    private T _prefab;
    private Queue<T> _queue;
    private Transform _container;

    public MonoObjectPool(T prefab, Transform container)
    {
        _prefab = prefab;
        _container = container;
    }

    public MonoObjectPool(T prefab, Transform container, int count)
    {
        _queue = new Queue<T>();
        _prefab = prefab;
        _container = container;

        for (int i = 0; i < count; ++i)
        {
            T instance = Instantiate();
            _queue.Enqueue(instance);
        }
    }

    public T Create(Vector3 position, Quaternion rotation)
    {
        T instance;
        if (_queue.Count > 0)
        {
            instance = _queue.Dequeue();
        }
        else
        {
            instance = Instantiate();
        }

        instance.gameObject.SetActive(true);
        instance.transform.position = position;
        instance.transform.rotation = rotation;
        return instance;
    }

    public void Destroy(T instance)
    {
        instance.gameObject.SetActive(false);
        _queue.Enqueue(instance);
    }

    public IEnumerator Destroy(T instance, float sec)
    {
        yield return new WaitForSeconds(sec);
        Destroy(instance);
    }

    private T Instantiate()
    {
        T instance = GameObject.Instantiate<T>(_prefab, _container);
        instance.gameObject.SetActive(false);
        return instance;
    }
}
