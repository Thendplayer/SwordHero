using Cinemachine;
using MessagePipe;
using SwordHero.AssetManagement;
using SwordHero.Core.Events;
using SwordHero.Core.Pawn.Enemy;
using SwordHero.Core.Spawner;
using SwordHero.Repositories;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace SwordHero.Core
{
    public class GameLifetimeScope : LifetimeScope
    {
        [Header("References")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private AddressableCatalog _addressableCatalog;
        [SerializeField] private StandardRecipe[] _standardRecipes;
        [SerializeField] private PoolableRecipe[] _poolableRecipes;

        private bool _containerBuild = false;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterAssetManagement(builder);
            RegisterEvents(builder);
            RegisterInstances(builder);
            RegisterRepositories(builder);

            builder.RegisterBuildCallback(_ => _containerBuild = true);
        }
        
        public bool IsContainerBuild() => _containerBuild;

        private void RegisterAssetManagement(IContainerBuilder builder)
        {
            builder.RegisterInstance(_addressableCatalog);
            builder.Register<AddressableAssetLoader>(Lifetime.Singleton).AsImplementedInterfaces();
        }

        private void RegisterEvents(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<JoystickDragEvent>(options);
            builder.RegisterMessageBroker<JoystickPressedEvent>(options);
            builder.RegisterMessageBroker<JoystickReleasedEvent>(options);
            builder.RegisterMessageBroker<OnPawnAttackedEvent>(options);
            builder.RegisterMessageBroker<HealItemPurchasedEvent>(options);
            builder.RegisterMessageBroker<WeaponItemPurchasedEvent>(options);
            builder.RegisterMessageBroker<PlayerDeathEvent>(options);
            builder.RegisterMessageBroker<PlayerRespawnEvent>(options);
            builder.RegisterMessageBroker<GoldEarnedEvent>(options);
            builder.RegisterMessageBroker<ExtractionPointTriggeredEvent>(options);
        }

        private void RegisterInstances(IContainerBuilder builder)
        {
            builder.RegisterInstance(_canvas).As<Canvas>();
            builder.RegisterInstance(Camera.main).As<Camera>();
            builder.RegisterInstance(_virtualCamera);
        }
        
        private void RegisterRepositories(IContainerBuilder builder)
        {
            foreach (var recipe in _standardRecipes)
            {
                recipe.Register(builder);
            }

            builder.Register<RepositoryFactory>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<RepositoryCoordinator>(Lifetime.Singleton).AsImplementedInterfaces();
            
            builder.Register<SpawnerService<EnemyPawnRepository>>(Lifetime.Singleton).AsSelf();
            
            builder.RegisterBuildCallback(RegisterPoolableRecipes);
        }

        private void RegisterPoolableRecipes(IObjectResolver resolver)
        {
            RegisterRecipesForType<EnemyPawnRepository>(resolver);
        }

        private void RegisterRecipesForType<T>(IObjectResolver resolver) where T : IPoolableRepository
        {
            var spawnerService = resolver.Resolve<SpawnerService<T>>();

            foreach (var recipe in _poolableRecipes)
            {
                var repository = recipe.GetRepository();
                if (repository is T) 
                    spawnerService.RegisterSpawner(() => (T)recipe.GetRepository());
            }
        }
    }
}