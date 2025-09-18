using System;
using System.Collections.Generic;
using System.Linq;
using VContainer.Unity;

namespace SwordHero.Repositories
{
    public class RepositoryFactory : IDisposable
    {
        private readonly LifetimeScope _lifetimeScope;
        private readonly List<IFactoryPoolBase> _factories = new();
        private readonly Dictionary<IPoolableRepository, IFactoryPoolBase> _repositoryToFactory = new();

        public RepositoryFactory(LifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public void Register<TRepository>(Func<TRepository> repositoryFactory, int poolSize = 0)
            where TRepository : IPoolableRepository
        {
            var factory = new RepositoryFactoryPool<TRepository>(repositoryFactory, _lifetimeScope, poolSize);
            _factories.Add(factory);

            foreach (var instance in factory.GetActiveInstances())
            {
                _repositoryToFactory[instance] = factory;
            }
        }

        public T Spawn<T>() where T : IPoolableRepository
        {
            foreach (var factoryBase in _factories)
            {
                if (factoryBase is IFactoryPool<T> factory)
                {
                    var repository = factory.Spawn();
                    _repositoryToFactory[repository] = factoryBase;
                    return repository;
                }
            }
            return default;
        }

        public T SpawnRandom<T>() where T : IPoolableRepository
        {
            var matchingFactories = _factories.OfType<IFactoryPool<T>>().ToList();
            if (matchingFactories.Count == 0) return default;

            var randomFactory = matchingFactories[UnityEngine.Random.Range(0, matchingFactories.Count)];
            var repository = randomFactory.Spawn();
            _repositoryToFactory[repository] = randomFactory;
            return repository;
        }
        
        public void Update()
        {
            foreach (var factory in _factories)
            {
                factory.Update();
            }
        }

        public void Despawn(IPoolableRepository entity)
        {
            if (entity == null) return;

            if (_repositoryToFactory.TryGetValue(entity, out var factory) && factory.TryDespawn(entity))
            {
                _repositoryToFactory.Remove(entity);
            }
        }

        public IReadOnlyCollection<T> GetActiveInstances<T>() where T : IPoolableRepository
        {
            var allInstances = new List<T>();
            foreach (var factoryBase in _factories)
            {
                if (factoryBase is IFactoryPool<T> factory)
                {
                    allInstances.AddRange(factory.GetActiveInstances());
                }
            }
            return allInstances.AsReadOnly();
        }

        public void Dispose()
        {
            foreach (var factory in _factories)
            {
                if (factory is IDisposable disposable)
                    disposable.Dispose();
            }

            _factories.Clear();
            _repositoryToFactory.Clear();
        }
    }
}