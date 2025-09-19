using System.Collections.Generic;

namespace SwordHero.Repositories
{
    public interface IPoolUpdatable
    {
        void Update();
    }

    public interface IPoolDespawnable
    {
        bool TryDespawn(IPoolableRepository repository);
    }

    public interface IPoolSpawnable<T> : IPoolUpdatable, IPoolDespawnable where T : IPoolableRepository
    {
        T Spawn();
        void Despawn(T repository);
        IReadOnlyCollection<T> GetActiveInstances();
    }
}