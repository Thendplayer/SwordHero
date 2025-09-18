using UnityEngine;

namespace SwordHero.Core.Spawner
{
    public class EnemySpawnerModel
    {
        private readonly EnemySpawnerData _data;
        private float _nextSpawnTime;
        private bool _isSpawningEnabled;

        public EnemySpawnerModel(EnemySpawnerData data)
        {
            _data = data;
            _nextSpawnTime = Time.time + _data.InitialDelay;
            _isSpawningEnabled = _data.EnableSpawning;
        }

        public bool CanSpawn(int currentActiveEnemies)
        {
            if (!_data.EnableSpawning || !_isSpawningEnabled) return false;
            if (currentActiveEnemies >= _data.MaxActiveEnemies) return false;
            return Time.time >= _nextSpawnTime;
        }

        public void SetSpawningEnabled(bool enabled) => _isSpawningEnabled = enabled;

        public void OnSpawned() => _nextSpawnTime = Time.time + _data.TimeBetweenSpawns;
    }
}