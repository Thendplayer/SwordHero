using System;
using System.Collections.Generic;
using System.Linq;
using VContainer.Unity;

namespace SwordHero.Repositories
{
    internal class Repository<T> : IPoolSpawnable<T>, IDisposable where T : IPoolableRepository
    {
        private readonly Func<T> _repositoryFactory;
        private readonly LifetimeScope _parentScope;
        private readonly Stack<T> _availableInstances;
        private readonly HashSet<T> _activeInstances;
        private readonly Dictionary<T, LifetimeScope> _repositoryToScope;

        public Repository(Func<T> repositoryFactory, LifetimeScope parentScope, int poolSize)
        {
            _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            _parentScope = parentScope ?? throw new ArgumentNullException(nameof(parentScope));

            _availableInstances = new Stack<T>(poolSize);
            _activeInstances = new HashSet<T>();
            _repositoryToScope = new Dictionary<T, LifetimeScope>();

            for (var i = 0; i < poolSize; i++)
            {
                var repository = CreateNewRepository();
                _availableInstances.Push(repository);
            }
        }

        private T CreateNewRepository()
        {
            var repository = _repositoryFactory();
            var childScope = _parentScope.CreateChild(builder => repository.Register(builder));

            _repositoryToScope[repository] = childScope;
            return repository;
        }

        public T Spawn()
        {
            var repository = _availableInstances.Count > 0 ? _availableInstances.Pop() : CreateNewRepository();
            _activeInstances.Add(repository);
            repository.Spawn();
            return repository;
        }

        public void Update()
        {
            foreach (var repository in _activeInstances.ToArray())
            {
                if (repository.IsActive)
                {
                    repository.Update();
                }
            }
        }

        public void Despawn(T repository)
        {
            if (repository == null || !_activeInstances.Contains(repository))
                return;

            repository.Despawn();
            _activeInstances.Remove(repository);
            _availableInstances.Push(repository);
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

        public IReadOnlyCollection<T> GetActiveInstances()
        {
            return _activeInstances.ToList().AsReadOnly();
        }

        public void Dispose()
        {
            foreach (var repository in _activeInstances.Concat(_availableInstances))
            {
                if (_repositoryToScope.TryGetValue(repository, out var scope))
                {
                    scope.Dispose();
                }
            }

            _activeInstances.Clear();
            _availableInstances.Clear();
            _repositoryToScope.Clear();
        }
    }
}