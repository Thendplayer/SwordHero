using SwordHero.Repositories;
using UnityEngine;
using VContainer;

namespace SwordHero.Core.Spawner
{
    [CreateAssetMenu(menuName = "Recipes/EnemySpawner", fileName = "EnemySpawnerRecipe")]
    public class EnemySpawnerRecipe : StandardRecipe
    {
        [SerializeField] private EnemySpawnerData _data;

        public override void Register(IContainerBuilder builder)
        {
            builder.RegisterInstance(_data);
            builder.Register<EnemySpawnerModel>(Lifetime.Singleton);
            builder.Register<EnemySpawnerController>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        }
    }
}