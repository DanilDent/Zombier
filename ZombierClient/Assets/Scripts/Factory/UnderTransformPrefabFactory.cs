using UnityEngine;
using Zenject;

public class UnderTransformPrefabFactory<P1, T> : IFactory<UnityEngine.Object, P1, T>
    where T : Component
{
    private readonly DiContainer _container;
    private readonly Transform _transform;

    public UnderTransformPrefabFactory(DiContainer container, Transform transform)
    {
        _container = container;
        _transform = transform;
    }

    public T Create(UnityEngine.Object prefab, P1 param1)
    {
        T instance = (T)_container.InstantiatePrefabForComponentExplicit(
               typeof(T), prefab, InjectUtil.CreateArgListExplicit(param1));
        instance.gameObject.transform.SetParent(_transform);
        return instance;
    }
}
