namespace SwordHero.Repositories
{
    public class SpawnerService<T> where T : IPoolableRepository
    {
        private readonly RepositoryFactory _factory;

        public SpawnerService(RepositoryFactory factory)
        {
            _factory = factory;
        }

        public void RegisterSpawner(System.Func<T> repositoryFactory, int poolSize = 0)
        {
            _factory.Register(repositoryFactory, poolSize);
        }

        public T Spawn()
        {
            return _factory.SpawnRandom<T>();
        }

        public void DespawnAll()
        {
            var instances = _factory.GetActiveInstances<T>();
            foreach (var instance in instances)
            {
                _factory.Despawn(instance);
            }
        }

        public int GetTotalActive() => _factory.GetActiveInstances<T>().Count;
    }
}