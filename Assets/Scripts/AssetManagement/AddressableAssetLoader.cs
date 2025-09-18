using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace SwordHero.AssetManagement
{
    public class AddressableAssetLoader : IAssetLoader
    {
        private readonly AddressableCatalog _catalog;
        private readonly IObjectResolver _container;
        private readonly Dictionary<string, AsyncOperationHandle> _cache = new();

        public AddressableAssetLoader(AddressableCatalog catalog, IObjectResolver container)
        {
            _catalog = catalog;
            _container = container;
        }

        public async UniTask LoadStartupAssetsAsync(IProgress<float> progress = null, CancellationToken ct = default)
        {
            var startupEntries = _catalog.GetStartupEntries();
            if (startupEntries.Length == 0)
            {
                progress?.Report(1f);
                return;
            }

            for (var i = 0; i < startupEntries.Length; i++)
            {
                await LoadAssetInternal(startupEntries[i].reference, ct);
                progress?.Report((float)(i + 1) / startupEntries.Length);
            }

            Debug.Log($"Preloaded {startupEntries.Length} startup assets");
        }

        public async UniTask<T> LoadAssetAsync<T>(AssetReference reference, CancellationToken ct = default) where T : Object
        {
            var handle = await LoadAssetInternal(reference, ct);
            return handle.Result as T;
        }

        public async UniTask<GameObject> InstantiateAsync(AssetReferenceGameObject prefabRef, Transform parent = null, CancellationToken ct = default)
        {
            var prefab = await LoadAssetAsync<GameObject>(prefabRef, ct);
            var instance = Object.Instantiate(prefab, parent);

            _container.InjectGameObject(instance);

            return instance;
        }

        public void Dispose()
        {
            foreach (var handle in _cache.Values)
            {
                if (handle.IsValid()) 
                    Addressables.Release(handle);
            }

            _cache.Clear();
        }

        private async UniTask<AsyncOperationHandle> LoadAssetInternal(AssetReference reference, CancellationToken ct)
        {
            var key = reference.AssetGUID;

            if (_cache.TryGetValue(key, out var cached) && cached.IsValid())
                return cached;

            var handle = Addressables.LoadAssetAsync<Object>(reference);
            _cache[key] = handle;

            await handle.ToUniTask(cancellationToken: ct);
            return handle;
        }
    }
}