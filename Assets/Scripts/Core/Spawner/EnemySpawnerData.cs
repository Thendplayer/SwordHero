using System;
using UnityEngine;

namespace SwordHero.Core.Spawner
{
    [Serializable]
    public class EnemySpawnerData
    {
        [Header("Timing")]
        [SerializeField] private float _timeBetweenSpawns = 2f;
        [SerializeField] private float _initialDelay = 1f;

        [Header("Spawning Logic")]
        [SerializeField] private int _maxActiveEnemies = 5;
        [SerializeField] private bool _enableSpawning = true;

        public float TimeBetweenSpawns => _timeBetweenSpawns;
        public float InitialDelay => _initialDelay;
        public int MaxActiveEnemies => _maxActiveEnemies;
        public bool EnableSpawning => _enableSpawning;
    }
}