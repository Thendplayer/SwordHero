using VContainer;

namespace SwordHero.Repositories
{
    public interface IPoolableRepository
    {
        bool IsActive { get; }
        void Spawn();
        void Despawn();
        void Update();
        void Register(IContainerBuilder builder);
    }
}