using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SwordHero.Core.GameLoop
{
    public enum ShopItemState
    {
        Available,
        NotAvailable,
        SoldOut
    }

    [Serializable]
    public class ShopItemView
    {
        [SerializeField] private GameObject _itemDisplay;
        [SerializeField] private GameObject _statusDisplay;
        [SerializeField] private Button _purchaseButton;
        [SerializeField] private TextMeshProUGUI _itemNameText;
        [SerializeField] private TextMeshProUGUI _itemPriceText;
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] private Image _itemImage;

        public Button PurchaseButton => _purchaseButton;

        public void SetState(ShopItemState state, string statusText = "")
        {
            switch (state)
            {
                case ShopItemState.Available:
                    _itemDisplay.SetActive(true);
                    _statusDisplay.SetActive(false);
                    _purchaseButton.interactable = true;
                    break;
                case ShopItemState.NotAvailable:
                case ShopItemState.SoldOut:
                    _itemDisplay.SetActive(false);
                    _statusDisplay.SetActive(true);
                    _statusText.text = statusText;
                    _purchaseButton.interactable = false;
                    break;
            }
        }

        public void SetItemData(string itemName, int itemPrice, Sprite itemSprite)
        {
            _itemNameText.text = itemName;
            _itemPriceText.text = itemPrice.ToString();
            _itemImage.sprite = itemSprite;
        }
    }
}