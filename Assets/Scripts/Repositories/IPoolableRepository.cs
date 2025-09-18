namespace SwordHero.Repositories
{
    public interface IPoolableRepository : IRepository
    {
        bool IsActive { get; }
        void Spawn();
        void Despawn();
        void Update();
    }
}