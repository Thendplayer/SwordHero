using System;
using MessagePipe;
using SwordHero.Core.Events;
using SwordHero.Core.Pawn.Adapters;
using SwordHero.Core.Pawn.UseCases;
using UnityEngine;

namespace SwordHero.Core.Pawn.Player
{
    public class PlayerPawnController : PawnController
    {
        private readonly ISubscriber<JoystickDragEvent> _onJoystickDrag;
        private readonly ISubscriber<JoystickPressedEvent> _onJoystickPressed;
        private readonly ISubscriber<JoystickReleasedEvent> _onJoystickReleased;
        private readonly ISubscriber<PlayerRespawnEvent> _playerRespawn;
        private readonly ISubscriber<HealItemPurchasedEvent> _healItemPurchased;
        private readonly ISubscriber<WeaponItemPurchasedEvent> _weaponItemPurchased;
        
        private readonly IPublisher<PlayerDeathEvent> _playerDeathPublisher;
        
        private IDisposable _joystickDragSubscription;
        private IDisposable _joystickPressedSubscription;
        private IDisposable _joystickReleasedSubscription;
        private IDisposable _playerRespawnSubscription;
        private IDisposable _healItemPurchasedSubscription;
        private IDisposable _weaponItemPurchasedSubscription;
        
        public PlayerPawnController(
            PawnView view,
            PawnModel model,
            ISubscriber<JoystickDragEvent> onJoystickDrag,
            ISubscriber<JoystickPressedEvent> onJoystickPressed,
            ISubscriber<JoystickReleasedEvent> onJoystickReleased,
            ISubscriber<OnPawnAttackedEvent> onPawnAttacked,
            ISubscriber<PlayerRespawnEvent> playerRespawn,
            IPublisher<PlayerDeathEvent> playerDeathPublisher,
            ISubscriber<HealItemPurchasedEvent> healItemPurchased,
            ISubscriber<WeaponItemPurchasedEvent> weaponItemPurchased,
            AttackClosestTargetUseCase attackClosestTargetUseCase
        ) : base(view, model, onPawnAttacked, attackClosestTargetUseCase)
        {
            _onJoystickDrag = onJoystickDrag;
            _onJoystickPressed = onJoystickPressed;
            _onJoystickReleased = onJoystickReleased;
            _playerRespawn = playerRespawn;
            _healItemPurchased = healItemPurchased;
            _weaponItemPurchased = weaponItemPurchased;

            _playerDeathPublisher = playerDeathPublisher;
        }

        public override void Initialize()
        {
            _joystickDragSubscription = _onJoystickDrag.Subscribe(HandleJoystickDrag);
            _joystickPressedSubscription = _onJoystickPressed.Subscribe(HandleJoystickPressed);
            _joystickReleasedSubscription = _onJoystickReleased.Subscribe(HandleJoystickReleased);
            _playerRespawnSubscription = _playerRespawn.Subscribe(HandlePlayerRespawn);
            _healItemPurchasedSubscription = _healItemPurchased.Subscribe(HandleHealItemPurchased);
            _weaponItemPurchasedSubscription = _weaponItemPurchased.Subscribe(HandleWeaponItemPurchased);

            _view.SetAsCameraTarget();
            base.Initialize();
        }

        public override void Dispose()
        {
            _joystickDragSubscription?.Dispose();
            _joystickPressedSubscription?.Dispose();
            _joystickReleasedSubscription?.Dispose();
            _playerRespawnSubscription?.Dispose();
            _healItemPurchasedSubscription?.Dispose();
            _weaponItemPurchasedSubscription?.Dispose();
            base.Dispose();
        }

        protected override Layer GetTargetLayer() => Layer.Enemy;
        
        protected override void HandleDeath()
        {
            _playerDeathPublisher.Publish(new PlayerDeathEvent());
        }

        private void HandlePlayerRespawn(PlayerRespawnEvent respawnEvent)
        {
            _model.Reset();
            _view.ResetAnimatorToEntry();
            _view.RemoveWeapon();
            _view.AddWeapon(_model.CurrentWeapon);
        }

        public void StopMovement()
        {
            _model.SetMoving(false);
        }

        private void HandleJoystickDrag(JoystickDragEvent dragEvent)
        {
            _model.SetVelocity(dragEvent.Direction, dragEvent.Magnitude);
        }

        private void HandleJoystickPressed(JoystickPressedEvent pressedEvent)
        {
            _model.SetMoving(true);
        }
        
        private void HandleJoystickReleased(JoystickReleasedEvent releasedEvent)
        {
            _model.SetMoving(false);
        }

        private void HandleHealItemPurchased(HealItemPurchasedEvent healEvent)
        {
            _model.AddHealthPoints(healEvent.HealAmount);
            _view.UpdateLifeDisplay(_model.GetHealthRatio());
        }

        private void HandleWeaponItemPurchased(WeaponItemPurchasedEvent weaponEvent)
        {
            _model.SetNewWeapon(weaponEvent.WeaponRecipe.Data);
            _view.RemoveWeapon();
            _view.AddWeapon(_model.CurrentWeapon);
        }
    }
}