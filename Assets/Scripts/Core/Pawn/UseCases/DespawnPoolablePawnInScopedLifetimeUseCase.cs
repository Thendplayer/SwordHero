using SwordHero.Repositories;

namespace SwordHero.Core.Pawn.UseCases
{
    public class DespawnPoolablePawnInScopedLifetimeUseCase
    {
        private readonly RepositoryFactory _factory;
        private readonly IPoolableRepository _repository;

        public DespawnPoolablePawnInScopedLifetimeUseCase(RepositoryFactory factory, IPoolableRepository repository)
        {
            _factory = factory;
            _repository = repository;
        }
        
        public void Execute()
        {
            _factory.Despawn(_repository);
        }
    }
}