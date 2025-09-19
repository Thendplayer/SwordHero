using SwordHero.Core.Pawn.Enemy;
using SwordHero.Repositories;
using VContainer.Unity;

namespace SwordHero.Core.Spawner
{
    public class EnemySpawnerController : IFixedTickable
    {
        private readonly EnemySpawnerModel _model;
        private readonly RepositoryRegistry _registry;

        public EnemySpawnerController(EnemySpawnerModel model, RepositoryRegistry registry)
        {
            _model = model;
            _registry = registry;
        }

        public void FixedTick()
        {
            var currentActiveEnemies = _registry.GetActiveCount<EnemyPawnRepository>();

            if (!_model.CanSpawn(currentActiveEnemies)) return;
            var enemy = _registry.SpawnRandom<EnemyPawnRepository>();
            if (enemy != null)
            {
                _model.OnSpawned();
            }
        }

        public void StartSpawning()
        {
            _model.SetSpawningEnabled(true);
        }

        public void StopSpawning()
        {
            _model.SetSpawningEnabled(false);
            _registry.DespawnAll<EnemyPawnRepository>();
        }
    }
}