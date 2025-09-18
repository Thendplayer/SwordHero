using SwordHero.Core.Pawn.Enemy;
using SwordHero.Repositories;
using VContainer.Unity;

namespace SwordHero.Core.Spawner
{
    public class EnemySpawnerController : IFixedTickable
    {
        private readonly EnemySpawnerModel _model;
        private readonly SpawnerService<EnemyPawnRepository> _spawnerService;

        public EnemySpawnerController(EnemySpawnerModel model, SpawnerService<EnemyPawnRepository> spawnerService)
        {
            _model = model;
            _spawnerService = spawnerService;
        }

        public void FixedTick()
        {
            var currentActiveEnemies = _spawnerService.GetTotalActive();

            if (!_model.CanSpawn(currentActiveEnemies)) return;
            var enemy = _spawnerService.Spawn();
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
            _spawnerService.DespawnAll();
        }

        public int GetActiveEnemyCount() => _spawnerService.GetTotalActive();
    }
}