using System;
using System.Collections.Generic;
using System.Linq;
using VContainer;
using VContainer.Unity;

namespace SwordHero.Repositories
{
    internal class RepositoryFactoryPool<T> : IFactoryPool<T>, IDisposable where T : IRepository, IPoolableRepository
    {
        private class PooledRepository
        {
            public T Repository { get; set; }
            public LifetimeScope ChildScope { get; set; }
        }

        private readonly Func<T> _repositoryFactory;
        private readonly LifetimeScope _parentScope;
        private readonly Stack<PooledRepository> _availableInstances;
        private readonly HashSet<PooledRepository> _activeInstances;

        public RepositoryFactoryPool(Func<T> repositoryFactory, LifetimeScope parentScope, int poolSize)
        {
            _repositoryFactory = repositoryFactory;
            _parentScope = parentScope;

            _availableInstances = new Stack<PooledRepository>(poolSize);
            _activeInstances = new HashSet<PooledRepository>();

            for (var i = 0; i < poolSize; i++)
            {
                var pooledInstance = CreateNewPooledRepository();
                _activeInstances.Add(pooledInstance);
            }
        }

        public T Spawn()
        {
            var pooledInstance = _availableInstances.Count > 0 ? _availableInstances.Pop() : CreateNewPooledRepository();

            _activeInstances.Add(pooledInstance);
            pooledInstance.Repository.Spawn();

            return pooledInstance.Repository;
        }
        
        public void Update()
        {
            foreach (var pooledInstance in _activeInstances.ToArray())
            {
                if (pooledInstance.Repository.IsActive)
                    pooledInstance.Repository.Update();
            }
        }

        public void Despawn(T repository)
        {
            if (repository == null) return;

            var pooledInstance = FindPooledInstance(repository);
            if (pooledInstance == null || !_activeInstances.Contains(pooledInstance))
            {
                return;
            }

            repository.Despawn();

            _activeInstances.Remove(pooledInstance);
            _availableInstances.Push(pooledInstance);
        }

        public bool TryDespawn(IPoolableRepository repository)
        {
            if (repository is T typedRepository)
            {
                Despawn(typedRepository);
                return true;
            }
            return false;
        }
        
        public void Dispose()
        {
            foreach (var instance in _availableInstances)
            {
                DestroyPooledRepository(instance);
            }

            foreach (var instance in _activeInstances)
            {
                DestroyPooledRepository(instance);
            }

            _availableInstances.Clear();
            _activeInstances.Clear();
        }

        public IReadOnlyCollection<T> GetActiveInstances()
        {
            return _activeInstances.Select(instance => instance.Repository).ToList().AsReadOnly();
        }

        private PooledRepository CreateNewPooledRepository()
        {
            var repositoryInstance = _repositoryFactory();

            var childScope = _parentScope.CreateChild(builder =>
            {
                builder.RegisterInstance(repositoryInstance).As<IPoolableRepository>();
                repositoryInstance.Register(builder);
            });

            return new PooledRepository
            {
                Repository = repositoryInstance,
                ChildScope = childScope
            };
        }

        private void DestroyPooledRepository(PooledRepository pooledInstance)
        {
            if (pooledInstance == null) return;
            pooledInstance.ChildScope?.Dispose();
        }

        private PooledRepository FindPooledInstance(T repository)
        {
            foreach (var instance in _activeInstances)
            {
                if (ReferenceEquals(instance.Repository, repository))
                    return instance;
            }

            return null;
        }
    }
}