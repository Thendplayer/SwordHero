using Cysharp.Threading.Tasks;
using SwordHero.AssetManagement;
using SwordHero.Core.UI;
using UnityEngine;
using VContainer;
using System;

namespace SwordHero.Core
{
    public class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private GameLifetimeScope _gameLifetimeScope;
        [SerializeField] private LoadingView _loadingView;

        private async void Awake()
        {
            _loadingView.Show();
            _loadingView.UpdateProgress(0f);

            await WaitForContainerBuild();
            _loadingView.UpdateProgress(0.5f);

            await LoadStartupAssets();
            _loadingView.UpdateProgress(1f);

            _loadingView.Hide();
            enabled = false;
        }

        private async UniTask WaitForContainerBuild()
        {
            while (!_gameLifetimeScope.IsContainerBuild())
                await UniTask.Yield();
        }

        private async UniTask LoadStartupAssets()
        {
            var assetLoader = _gameLifetimeScope.Container.Resolve<IAssetLoader>();
            var progress = new Progress<float>(p => _loadingView.UpdateProgress(0.5f + p * 0.5f));

            await assetLoader.LoadStartupAssetsAsync(progress);
        }
    }
}