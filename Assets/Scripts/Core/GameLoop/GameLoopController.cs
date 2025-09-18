using System;
using MessagePipe;
using SwordHero.Core.Events;
using SwordHero.Core.GameLoop.UseCases;
using VContainer.Unity;

namespace SwordHero.Core.GameLoop
{
    public class GameLoopController : IInitializable, IDisposable
    {
        private readonly GameLoopModel _model;
        private readonly GameLoopView _view;
        
        private readonly IPublisher<HealItemPurchasedEvent> _healPurchasedPublisher;
        private readonly IPublisher<WeaponItemPurchasedEvent> _weaponPurchasedPublisher;
        private readonly IPublisher<PlayerRespawnEvent> _playerRespawnPublisher;

        private readonly ISubscriber<PlayerDeathEvent> _playerDeathSubscriber;
        private readonly ISubscriber<GoldEarnedEvent> _goldEarnedSubscriber;
        private readonly ISubscriber<ExtractionPointTriggeredEvent> _extractionPointSubscriber;
        
        private readonly ControlGamePauseUseCase _controlGamePauseUseCase;

        private IDisposable _playerDeathSubscription;
        private IDisposable _goldEarnedSubscription;
        private IDisposable _extractionPointSubscription;

        public GameLoopController(
            GameLoopModel model,
            GameLoopView view,
            IPublisher<HealItemPurchasedEvent> healPurchasedPublisher,
            IPublisher<WeaponItemPurchasedEvent> weaponPurchasedPublisher,
            IPublisher<PlayerRespawnEvent> playerRespawnPublisher,
            ISubscriber<PlayerDeathEvent> playerDeathSubscriber,
            ISubscriber<GoldEarnedEvent> goldEarnedSubscriber,
            ISubscriber<ExtractionPointTriggeredEvent> extractionPointSubscriber,
            ControlGamePauseUseCase controlGamePauseUseCase
        )
        {
            _model = model;
            _view = view;
            
            _healPurchasedPublisher = healPurchasedPublisher;
            _weaponPurchasedPublisher = weaponPurchasedPublisher;
            _playerRespawnPublisher = playerRespawnPublisher;

            _playerDeathSubscriber = playerDeathSubscriber;
            _goldEarnedSubscriber = goldEarnedSubscriber;
            _extractionPointSubscriber = extractionPointSubscriber;

            _controlGamePauseUseCase = controlGamePauseUseCase;
        }

        public void Initialize()
        {
            _view.OnPlayButtonClicked += OnPlayButtonClicked;
            _view.OnItemPurchaseClicked += OnItemPurchaseClicked;

            _playerDeathSubscription = _playerDeathSubscriber.Subscribe(OnPlayerDeath);
            _goldEarnedSubscription = _goldEarnedSubscriber.Subscribe(OnGoldEarned);
            _extractionPointSubscription = _extractionPointSubscriber.Subscribe(OnExtractionPointTriggered);

            UpdateUI();
        }

        public void Dispose()
        {
            _view.OnPlayButtonClicked -= OnPlayButtonClicked;
            _view.OnItemPurchaseClicked -= OnItemPurchaseClicked;

            _playerDeathSubscription?.Dispose();
            _goldEarnedSubscription?.Dispose();
            _extractionPointSubscription?.Dispose();
        }

        private void OnPlayButtonClicked()
        {
            if (_model.IsDead)
            {
                _playerRespawnPublisher.Publish(new PlayerRespawnEvent());
                _model.ResetPurchases();
            }

            _model.StartGame();
            UpdateUI();
        }

        private void OnPlayerDeath(PlayerDeathEvent playerDeathEvent) => EndGame(true);

        private void OnGoldEarned(GoldEarnedEvent goldEarnedEvent) => AddGold(goldEarnedEvent.Amount);

        private void OnExtractionPointTriggered(ExtractionPointTriggeredEvent extractionEvent)
        {
            _model.OpenMenu();
            UpdateUI();
        }

        public void EndGame(bool playerDied = false)
        {
            _model.EndGame(playerDied);
            UpdateUI();
        }

        public void AddGold(int amount)
        {
            _model.AddGold(amount);
            _view.SetGoldAmount(_model.Gold);
            UpdateShopDisplay();
        }

        private void OnItemPurchaseClicked(int slotIndex)
        {
            var slot = (ShopSlot)slotIndex;

            if (!_model.CanAffordSlot(slot))
                return;

            var purchaseSuccessful = false;
            switch (slot)
            {
                case ShopSlot.Heal:
                    purchaseSuccessful = _model.PurchaseSlot(slot);
                    if (purchaseSuccessful)
                    {
                        var healEvent = new HealItemPurchasedEvent(_model.Data.HealItem.HealAmount, _model.Data.HealItem.ItemPrice);
                        _healPurchasedPublisher.Publish(healEvent);
                    }
                    break;
                case ShopSlot.Weapon:
                    var currentWeapon = _model.GetCurrentWeapon();
                    purchaseSuccessful = _model.PurchaseSlot(slot);
                    if (purchaseSuccessful)
                    {
                        var weaponEvent = new WeaponItemPurchasedEvent(currentWeapon.WeaponRecipe, currentWeapon.ItemPrice);
                        _weaponPurchasedPublisher.Publish(weaponEvent);
                    }
                    break;
            }

            if (!purchaseSuccessful) return;
            _view.SetGoldAmount(_model.Gold);
            UpdateShopDisplay();
        }

        private void UpdateUI()
        {
            var isInGame = _model.CurrentState == GameState.InGame;
            _view.ShowMenu(!isInGame);

            switch (_model.CurrentState)
            {
                case GameState.Start:
                    _view.ShowTutorial();
                    break;
                case GameState.Menu:
                    _view.ShowItemSelector();
                    break;
            }

            _controlGamePauseUseCase.Execute(isInGame);

            _view.SetPlayButtonText(_model.GetPlayButtonText());
            _view.SetGoldAmount(_model.Gold);
            UpdateShopDisplay();
        }

        private void UpdateShopDisplay()
        {
            _view.SetupShopSlots(_model, _model.Data.NotAvailableText, _model.Data.SoldOutText);
        }
    }
}