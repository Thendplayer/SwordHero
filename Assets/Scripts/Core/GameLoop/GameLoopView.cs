using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SwordHero.Core.GameLoop
{
    public class GameLoopView : MonoBehaviour
    {
        [Header("Screen Elements")]
        [SerializeField] private GameObject _menuScreen;
        [SerializeField] private GameObject _inGameDisplay;
        [SerializeField] private GameObject _tutorialDisplay;
        [SerializeField] private GameObject _itemSelectorDisplay;

        [Header("Gold Display")]
        [SerializeField] private TextMeshProUGUI _goldInGameText;
        [SerializeField] private TextMeshProUGUI _goldInMenuText;

        [Header("Play Button")]
        [SerializeField] private Button _playButton;
        [SerializeField] private TextMeshProUGUI _playButtonText;

        [Header("Shop Items")]
        [SerializeField] private ShopItemView[] _shopItems;

        public Action OnPlayButtonClicked;
        public Action<int> OnItemPurchaseClicked;

        private void Awake()
        {
            _playButton.onClick.AddListener(() => OnPlayButtonClicked?.Invoke());

            for (var i = 0; i < _shopItems.Length; i++)
            {
                var slotIndex = i;
                _shopItems[i].PurchaseButton.onClick.AddListener(() => OnItemPurchaseClicked?.Invoke(slotIndex));
            }
        }

        public void ShowTutorial()
        {
            _tutorialDisplay.SetActive(true);
            _itemSelectorDisplay.SetActive(false);
        }

        public void ShowItemSelector()
        {
            _tutorialDisplay.SetActive(false);
            _itemSelectorDisplay.SetActive(true);
        }

        public void ShowMenu(bool showMenu)
        {
            _menuScreen.SetActive(showMenu);
            _inGameDisplay.SetActive(!showMenu);
        }

        public void SetPlayButtonText(string text)
        {
            _playButtonText.text = text;
        }

        public void SetGoldAmount(int amount)
        {
            var goldText = amount.ToString();
            _goldInGameText.text = goldText;
            _goldInMenuText.text = goldText;
        }

        public void SetupShopSlots(GameLoopModel model, string notAvailableText, string soldOutText)
        {
            if (_shopItems.Length < 2) return;

            SetupHealSlot(model, notAvailableText);
            SetupWeaponSlot(model, notAvailableText, soldOutText);
        }

        private void SetupHealSlot(GameLoopModel model, string notAvailableText)
        {
            var healSlot = _shopItems[(int)ShopSlot.Heal];
            var healData = model.Data.HealItem;

            healSlot.SetItemData(healData.ItemName, healData.ItemPrice, healData.ItemImage);

            if (model.CanAffordSlot(ShopSlot.Heal))
            {
                healSlot.SetState(ShopItemState.Available);
            }
            else
            {
                healSlot.SetState(ShopItemState.NotAvailable, notAvailableText);
            }
        }

        private void SetupWeaponSlot(GameLoopModel model, string notAvailableText, string soldOutText)
        {
            var weaponSlot = _shopItems[(int)ShopSlot.Weapon];

            if (!model.IsCurrentWeaponAvailable())
            {
                weaponSlot.SetState(ShopItemState.NotAvailable, notAvailableText);
                return;
            }

            var currentWeapon = model.GetCurrentWeapon();
            weaponSlot.SetItemData(currentWeapon.ItemName, currentWeapon.ItemPrice, currentWeapon.ItemImage);

            if (model.CanAffordSlot(ShopSlot.Weapon))
            {
                weaponSlot.SetState(ShopItemState.Available);
            }
            else
            {
                weaponSlot.SetState(ShopItemState.NotAvailable, notAvailableText);
            }
        }
    }
}