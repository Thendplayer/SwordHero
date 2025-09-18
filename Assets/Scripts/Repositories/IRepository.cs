using VContainer;

namespace SwordHero.Repositories
{
    public interface IRepository
    {
        void Register(IContainerBuilder builder);
    }
}