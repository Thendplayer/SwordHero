using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer.Unity;
using Random = UnityEngine.Random;

namespace SwordHero.Repositories
{
    public class RepositoryRegistry : IFixedTickable, IDisposable
    {
        private readonly LifetimeScope _lifetimeScope;
        private readonly List<object> _pools;
        private readonly Dictionary<Type, List<object>> _poolsByType;
        private readonly Dictionary<IPoolableRepository, object> _repositoryToPool;
        private bool _isDisposed = false;

        public RepositoryRegistry(LifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;

            _pools = new List<object>();
            _poolsByType = new Dictionary<Type, List<object>>();
            _repositoryToPool = new Dictionary<IPoolableRepository, object>();
        }

        public void Register<T>(Func<T> repositoryFactory, int poolSize = 0) where T : IPoolableRepository
        {
            var type = typeof(T);
            var pool = new Repository<T>(repositoryFactory, _lifetimeScope, poolSize);
            _pools.Add(pool);

            if (!_poolsByType.ContainsKey(type))
                _poolsByType[type] = new List<object>();

            _poolsByType[type].Add(pool);

            foreach (var instance in pool.GetActiveInstances())
            {
                _repositoryToPool[instance] = pool;
            }
        }
        
        public T Spawn<T>(bool useRandom = false) where T : IPoolableRepository
        {
            var type = typeof(T);
            if (!_poolsByType.TryGetValue(type, out var typePools))
                throw new InvalidOperationException($"Repository type '{type.Name}' not registered. Call Register<{type.Name}>() first.");

            var pools = typePools.OfType<IPoolSpawnable<T>>().ToList();
            var selectedPool = useRandom ?
                pools.Skip(Random.Range(0, pools.Count)).FirstOrDefault() :
                pools.FirstOrDefault();

            var repository = selectedPool.Spawn();
            if (repository != null)
                _repositoryToPool[repository] = selectedPool;

            return repository;
        }

        public T SpawnRandom<T>() where T : IPoolableRepository => Spawn<T>(useRandom: true);

        public void Despawn(IPoolableRepository entity)
        {
            if (!_repositoryToPool.TryGetValue(entity, out var poolObj))
            {
                Debug.LogWarning($"[RepositoryRegistry] Attempted to despawn untracked repository of type '{entity.GetType().Name}'. This may indicate a double-despawn or the repository was not spawned by this registry.");
                return;
            }

            if (poolObj is IPoolDespawnable pool && pool.TryDespawn(entity))
            {
                _repositoryToPool.Remove(entity);
            }
        }

        public int GetActiveCount<T>() where T : IPoolableRepository
        {
            var type = typeof(T);
            if (!_poolsByType.TryGetValue(type, out var typePools))
                return 0;

            return typePools.OfType<IPoolSpawnable<T>>().Sum(pool => pool.GetActiveInstances().Count);
        }

        public void DespawnAll<T>() where T : IPoolableRepository
        {
            var type = typeof(T);
            if (!_poolsByType.TryGetValue(type, out var typePools))
                return;

            foreach (var poolObj in typePools)
            {
                if (poolObj is not IPoolSpawnable<T> pool) continue;

                var activeInstances = pool.GetActiveInstances().ToList();
                foreach (var instance in activeInstances)
                {
                    Despawn(instance);
                }
            }
        }

        public IReadOnlyCollection<T> GetActiveInstances<T>() where T : IPoolableRepository
        {
            var type = typeof(T);
            if (!_poolsByType.TryGetValue(type, out var typePools))
                return new List<T>().AsReadOnly();

            var allInstances = new List<T>();
            foreach (var poolObj in typePools)
            {
                if (poolObj is IPoolSpawnable<T> pool)
                {
                    allInstances.AddRange(pool.GetActiveInstances());
                }
            }

            return allInstances.AsReadOnly();
        }

        public void FixedTick()
        {
            foreach (var poolObj in _pools)
            {
                if (poolObj is IPoolUpdatable updatable)
                {
                    updatable.Update();
                }
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            try
            {
                foreach (var poolObj in _pools)
                {
                    if (poolObj is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[RepositoryRegistry] Error disposing pools: {ex.Message}");
            }
            finally
            {
                _pools.Clear();
                _poolsByType.Clear();
                _repositoryToPool.Clear();
                _isDisposed = true;
            }
        }
    }
}