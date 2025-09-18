using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading;
using Object = UnityEngine.Object;

namespace SwordHero.AssetManagement
{
    public interface IAssetLoader : IDisposable
    {
        UniTask LoadStartupAssetsAsync(IProgress<float> progress = null, CancellationToken ct = default);
        UniTask<T> LoadAssetAsync<T>(AssetReference reference, CancellationToken ct = default) where T : Object;
        UniTask<GameObject> InstantiateAsync(AssetReferenceGameObject prefabRef, Transform parent = null, CancellationToken ct = default);
    }
}