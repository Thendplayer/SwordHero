using VContainer.Unity;

namespace SwordHero.Repositories
{
    public class RepositoryCoordinator : IFixedTickable
    {
        private readonly RepositoryFactory _repositoryFactory;

        public RepositoryCoordinator(RepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public void FixedTick() => _repositoryFactory.Update();
    }
}