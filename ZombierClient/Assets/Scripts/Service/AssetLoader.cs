using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Prototype.Service
{
    public class AssetLoader<T>
    {
        private readonly Dictionary<string, AsyncOperationHandle<T>> _loadAssetOpHandles;
        private readonly List<string> _addresses;

        public AssetLoader(List<string> addresses)
        {
            _loadAssetOpHandles = new Dictionary<string, AsyncOperationHandle<T>>();
            _addresses = addresses;
        }

        public void Load()
        {
            foreach (string address in _addresses)
            {
                _loadAssetOpHandles.Add(address, Addressables.LoadAssetAsync<T>(address));
            }

            foreach (var opHandle in _loadAssetOpHandles.Values)
            {
                opHandle.WaitForCompletion();
            }

            foreach (var keyValue in _loadAssetOpHandles)
            {
                if (keyValue.Value.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError($"Can't load asset with address {keyValue.Key}");
                }
            }
        }

        public void Release()
        {
            foreach (var opHandle in _loadAssetOpHandles.Values)
            {
                if (opHandle.IsValid())
                {
                    Addressables.Release(opHandle);
                }
            }
        }

        public T Get(string address)
        {
            if (!_loadAssetOpHandles.ContainsKey(address))
            {
                throw new System.Exception($"The key {address} is not represented in dictionary");
            }
            return _loadAssetOpHandles[address].Result;
        }

        public TCast Get<TCast>(string address)
        where TCast : class
        {
            if (!_loadAssetOpHandles.ContainsKey(address))
            {
                throw new System.Exception($"The key {address} is not represented in dictionary");
            }
            return _loadAssetOpHandles[address].Result as TCast;
        }

        public TComponent GetComponent<TComponent>(string address)
        where TComponent : Component
        {
            var result = _loadAssetOpHandles[address].Result;
            if (result is GameObject cast)
            {
                return cast.GetComponentInChildren<TComponent>();
            }

            return null;
        }
    }
}
