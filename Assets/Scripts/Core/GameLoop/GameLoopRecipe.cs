using SwordHero.Core.GameLoop.UseCases;
using SwordHero.Repositories;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace SwordHero.Core.GameLoop
{
    [CreateAssetMenu(menuName = "Recipes/GameLoop", fileName = "GameLoopRecipe")]
    public class GameLoopRecipe : StandardRecipe
    {
        [SerializeField] private GameLoopView _view;
        [SerializeField] private GameLoopData _data;

        public override void Register(IContainerBuilder builder)
        {
            builder.RegisterInstance(_data);
            builder.RegisterComponentInNewPrefab(_view, Lifetime.Singleton);
            builder.Register<GameLoopModel>(Lifetime.Singleton);
            builder.Register<GameLoopController>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ControlGamePauseUseCase>(Lifetime.Singleton);
        }
    }
}