using SwordHero.Repositories;

namespace SwordHero.Core.Pawn.UseCases
{
    public class DespawnPoolablePawnUseCase
    {
        private readonly RepositoryRegistry _registry;
        private readonly IPoolableRepository _repository;

        public DespawnPoolablePawnUseCase(RepositoryRegistry registry, IPoolableRepository repository)
        {
            _registry = registry;
            _repository = repository;
        }
        
        public void Execute()
        {
            _registry.Despawn(_repository);
        }
    }
}