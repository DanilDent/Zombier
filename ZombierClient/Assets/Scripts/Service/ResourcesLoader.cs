using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace Prototype.Service
{
    /// <summary>
    /// Provides Unity's static Resources class like API for loading addressable assets
    /// </summary>
    public class ResourcesLoader
    {
        public T Load<T>(string address)
        where T : UnityEngine.Object
        {
            AsyncOperationHandle<T> opHandle = Addressables.LoadAssetAsync<T>(address);
            opHandle.WaitForCompletion();
            if (opHandle.Status != AsyncOperationStatus.Succeeded)
            {
                throw new Exception($"Can't load addressable asset with address {address}");
            }

            return opHandle.Result;
        }

        public T[] LoadAll<T>(params string[] labels)
        where T : UnityEngine.Object
        {
            IEnumerable<string> keys = labels.ToList();
            AsyncOperationHandle<IList<T>> loadOpHandle = Addressables.LoadAssetsAsync<T>(keys, null, Addressables.MergeMode.Intersection);
            loadOpHandle.WaitForCompletion();
            if (loadOpHandle.Status != AsyncOperationStatus.Succeeded)
            {
                throw new Exception($"Can't load addressable asset with labels {labels} using labels Intersection merge mode");
            }
            return loadOpHandle.Result.ToArray();
        }
    }
}