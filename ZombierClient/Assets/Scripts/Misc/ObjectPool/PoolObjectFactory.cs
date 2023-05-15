using Zenject;

namespace Prototype.ObjectPool
{
    public class PoolObjectFactory<T> : PlaceholderFactory<UnityEngine.Object, T>
        where T : PoolObject
    {
    }
}
