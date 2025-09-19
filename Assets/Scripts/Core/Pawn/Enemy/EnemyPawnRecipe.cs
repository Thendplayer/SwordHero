using SwordHero.Core.Pawn.Adapters;
using SwordHero.Core.Pawn.UseCases;
using SwordHero.Repositories;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace SwordHero.Core.Pawn.Enemy
{
    [CreateAssetMenu(menuName = "Recipes/Enemy", fileName = "EnemyRecipe")]
    public class EnemyPawnRecipe : PoolableRecipe
    {
        [SerializeField] private PawnView _view;
        [SerializeField] private PawnData _data;

        public override IPoolableRepository GetRepository()
        {
            return new EnemyPawnRepository(_view, _data);
        }
    }
    
    public class EnemyPawnRepository : IPoolableRepository
    {
        private class References
        {
            public EnemyPawnController Controller;
            public PawnModel Model;
            public PawnView View;
        }

        private readonly PawnView _view;
        private readonly PawnData _data;

        private IObjectResolver _resolver;
        private References _references = null;
        private bool _isActive = false;

        public bool IsActive => _isActive;

        public EnemyPawnRepository(PawnView view, PawnData data)
        {
            _view = view;
            _data = data;
        }

        public void Register(IContainerBuilder builder)
        {
            builder.RegisterInstance(this).As<IPoolableRepository>();

            builder.RegisterInstance(_data);
            builder.RegisterComponentInNewPrefab(_view, Lifetime.Scoped);
            builder.Register<IPhysicsBody>(resolver =>
            {
                var pawnView = resolver.Resolve<PawnView>();
                return pawnView.GetComponent<RigidbodyAdapter>();
            }, Lifetime.Scoped);

            builder.Register<PawnModel>(Lifetime.Scoped);
            builder.Register<EnemyPawnController>(Lifetime.Scoped).AsSelf();
            
            builder.Register<AttackClosestTargetUseCase>(Lifetime.Scoped);
            builder.Register<SetRandomSpawnPositionUseCase>(Lifetime.Scoped);
            builder.Register<SetTargetDirectionUseCase>(Lifetime.Scoped);
            builder.Register<DespawnPoolablePawnUseCase>(Lifetime.Scoped);

            builder.RegisterBuildCallback(resolver => _resolver = resolver);
        }

        public void Spawn()
        {
            if (_isActive) return;

            _references ??= new References
            {
                Controller = _resolver.Resolve<EnemyPawnController>(),
                Model = _resolver.Resolve<PawnModel>(),
                View = _resolver.Resolve<PawnView>()
            };

            _references.Model.Reset(_data);
            _references.View.gameObject.SetActive(true);
            _references.Controller.Initialize();
            _isActive = true;
        }

        public void Update()
        {
            if (!_isActive) return;
            _references.Controller.FixedTick();
        }

        public void Despawn()
        {
            if (!_isActive) return;

            _references.View.gameObject.SetActive(false);
            _references.Controller.Dispose();
            _isActive = false;
        }
    }
}