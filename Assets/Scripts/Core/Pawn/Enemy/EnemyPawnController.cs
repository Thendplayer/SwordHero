using MessagePipe;
using SwordHero.Core.Events;
using SwordHero.Core.Pawn.Adapters;
using SwordHero.Core.Pawn.UseCases;

namespace SwordHero.Core.Pawn.Enemy
{
    public class EnemyPawnController : PawnController
    {
        private readonly SetRandomSpawnPositionUseCase _setRandomSpawnPositionUseCase;
        private readonly SetTargetDirectionUseCase _setTargetDirectionUseCase;
        private readonly DespawnPoolablePawnInScopedLifetimeUseCase _despawnPoolablePawnInScopedLifetimeUseCase;
        private readonly IPublisher<GoldEarnedEvent> _goldEarnedPublisher;

        public EnemyPawnController(
            PawnView view,
            PawnModel model,
            ISubscriber<OnPawnAttackedEvent> onPawnAttacked,
            AttackClosestTargetUseCase attackClosestTargetUseCase,
            SetRandomSpawnPositionUseCase setRandomSpawnPositionUseCase,
            SetTargetDirectionUseCase setTargetDirectionUseCase,
            DespawnPoolablePawnInScopedLifetimeUseCase despawnPoolablePawnInScopedLifetimeUseCase,
            IPublisher<GoldEarnedEvent> goldEarnedPublisher
        ) : base(view, model, onPawnAttacked, attackClosestTargetUseCase)
        {
            _setRandomSpawnPositionUseCase = setRandomSpawnPositionUseCase;
            _setTargetDirectionUseCase = setTargetDirectionUseCase;
            _despawnPoolablePawnInScopedLifetimeUseCase = despawnPoolablePawnInScopedLifetimeUseCase;
            _goldEarnedPublisher = goldEarnedPublisher;
        }

        public override void Initialize()
        {
            _setRandomSpawnPositionUseCase.Execute(_model.SetPosition);
            base.Initialize();
        }

        public override void FixedTick()
        {
            CalculateMovement();
            base.FixedTick();
        }

        protected override Layer GetTargetLayer() => Layer.Player;

        protected override void HandleDeath()
        {
            _goldEarnedPublisher.Publish(new GoldEarnedEvent(_model.GoldValue));
            _despawnPoolablePawnInScopedLifetimeUseCase.Execute();
        }

        private void CalculateMovement()
        {
            _setTargetDirectionUseCase.Execute(
                _model.Position, 
                _model.HitRadius, 
                GetTargetLayer(), 
                out var targetReached,
                out var direction
            );

            if (targetReached)
            {
                _model.SetMoving(false);
                return;
            }
            
            _model.SetVelocity(direction);
            _model.SetMoving(true);
        }
    }
}