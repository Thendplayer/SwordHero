using SwordHero.Core.ExtractionPoint.UseCases;
using SwordHero.Core.Pawn.Adapters;
using SwordHero.Core.Pawn.UseCases;
using SwordHero.Repositories;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace SwordHero.Core.Pawn.Player
{
    [CreateAssetMenu(menuName = "Recipes/Player", fileName = "PlayerRecipe")]
    public class PlayerPawnRecipe : StandardRecipe
    {
        [SerializeField] private PawnView _view;
        [SerializeField] private PawnData _data;

        public override void Register(IContainerBuilder builder)
        {
            builder.RegisterInstance(_data);
            builder.RegisterComponentInNewPrefab(_view, Lifetime.Singleton);
            builder.Register<IPhysicsBody>(resolver =>
            {
                var pawnView = resolver.Resolve<PawnView>();
                return pawnView.GetComponent<RigidbodyAdapter>();
            }, Lifetime.Singleton);
            
            builder.Register<PawnModel>(Lifetime.Singleton);
            builder.Register<PlayerPawnController>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();

            builder.Register<SetRandomSpawnPositionUseCase>(Lifetime.Singleton);
            builder.Register<AttackClosestTargetUseCase>(Lifetime.Singleton);
            builder.Register<SetExtractionPointPositionUseCase>(Lifetime.Singleton);
        }
    }
}