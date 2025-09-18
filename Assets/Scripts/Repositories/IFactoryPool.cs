using System.Collections.Generic;

namespace SwordHero.Repositories
{
    public interface IFactoryPoolBase
    {
        void Update();
        bool TryDespawn(IPoolableRepository repository);
    }

    public interface IFactoryPool<T> : IFactoryPoolBase where T : IRepository, IPoolableRepository
    {
        T Spawn();
        void Despawn(T repository);
        IReadOnlyCollection<T> GetActiveInstances();
    }
}