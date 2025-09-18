using System;
using MessagePipe;
using SwordHero.Core.Events;
using SwordHero.Core.Pawn.Adapters;
using SwordHero.Core.Pawn.UseCases;
using UnityEngine;
using VContainer.Unity;

namespace SwordHero.Core.Pawn
{
    public abstract class PawnController : IInitializable, IFixedTickable, IDisposable
    {
        protected readonly PawnView _view;
        protected readonly PawnModel _model;

        private readonly AttackClosestTargetUseCase _attackClosestTargetUseCase;
        private readonly ISubscriber<OnPawnAttackedEvent> _onPawnAttacked;

        private IDisposable _onPawnAttackedSubscription;

        protected PawnController(
            PawnView view, 
            PawnModel model,
            ISubscriber<OnPawnAttackedEvent> onPawnAttacked,
            AttackClosestTargetUseCase attackClosestTargetUseCase
        )
        {
            _view = view;
            _model = model;
            _attackClosestTargetUseCase = attackClosestTargetUseCase;
            _onPawnAttacked = onPawnAttacked;
        }

        public virtual void Initialize()
        {
            _onPawnAttackedSubscription = _onPawnAttacked.Subscribe(OnHitReceived);
            
            if (_model.CurrentWeapon != null)
                _view.AddWeapon(_model.CurrentWeapon);
            
            _view.UpdateAnimatorSpeed(_model.AnimationSpeed);
            _view.UpdateLifeDisplay(_model.GetHealthRatio());
        }

        public virtual void Dispose()
        {
            _onPawnAttackedSubscription?.Dispose();
            _view.RemoveWeapon();
        }

        public virtual void FixedTick()
        {
            if (_model.IsDead) return;

            UpdateMovement();
            UpdateAttack();
            UpdateRotation();
        }
        
        protected abstract Layer GetTargetLayer();
        
        protected abstract void HandleDeath();

        private void UpdateMovement()
        {
            _view.MovePosition(_model.Velocity * Time.fixedDeltaTime);
            _view.UpdateBoolAnimationParameter(PawnData.AnimatorParameters.Move, _model.IsMoving);
        }
        
        private void UpdateAttack()
        {
            if (_model.IsMoving || !_model.CanAttack()) return;
            _attackClosestTargetUseCase.Execute(_model.Position, GetTargetLayer(), _model.HitRadius, _model.DamagePerHit, OnAttack);
        }

        private void UpdateRotation()
        {
            if (_model.Velocity.sqrMagnitude <= 0) return;
            
            var targetAngle = Mathf.Atan2(_model.Velocity.x, _model.Velocity.z) * Mathf.Rad2Deg;
            var currentAngle = _view.EulerAngles.y;
            
            var newAngle = _model.CalculateRotationAngle(currentAngle, targetAngle);
            if (newAngle != 0) _view.MoveRotation(new Vector3(0f, newAngle, 0f));
        }
        
        private void OnAttack(Vector3 direction)
        {
            _model.RecordAttack();

            var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            _view.MoveRotation(new Vector3(0f, targetAngle, 0f));
            _view.TriggerAnimationParameter(PawnData.AnimatorParameters.Attack);
        }

        private void OnHitReceived(OnPawnAttackedEvent pawnAttackedEvent)
        {
            if (_model.IsDead || !_model.IsEqual(pawnAttackedEvent.Target)) return;

            _model.SubtractHealthPoints(pawnAttackedEvent.Damage);
            _view.UpdateLifeDisplay(_model.GetHealthRatio());

            if (_model.IsDead)
            {
                HandleDeath();
                _view.TriggerAnimationParameter(PawnData.AnimatorParameters.Die);
                return;
            }

            _view.TriggerAnimationParameter(PawnData.AnimatorParameters.Hit);
        }
    }
}