using System;
using SwordHero.Core.Weapons;
using UnityEngine;

namespace SwordHero.Core.GameLoop
{
    [Serializable]
    public class HealItemData
    {
        [SerializeField] private string _itemName = "Health";
        [SerializeField] private int _itemPrice;
        [SerializeField] private int _healAmount;
        [SerializeField] private Sprite _itemImage;

        public string ItemName => _itemName;
        public int ItemPrice => _itemPrice;
        public int HealAmount => _healAmount;
        public Sprite ItemImage => _itemImage;
    }

    [Serializable]
    public class WeaponShopItemData
    {
        [SerializeField] private string _itemName;
        [SerializeField] private int _itemPrice;
        [SerializeField] private Sprite _itemImage;
        [SerializeField] private WeaponRecipe _weaponRecipe;

        public string ItemName => _itemName;
        public int ItemPrice => _itemPrice;
        public Sprite ItemImage => _itemImage;
        public WeaponRecipe WeaponRecipe => _weaponRecipe;
    }

    [Serializable]
    public class GameLoopData
    {
        [Header("Button Texts")]
        [SerializeField] private string _goButtonText = "Go!";
        [SerializeField] private string _reviveButtonText = "Revive!";

        [Header("Shop Status Texts")]
        [SerializeField] private string _soldOutText = "Sold Out";
        [SerializeField] private string _notAvailableText = "Not Available";

        [Header("Shop Items")]
        [SerializeField] private HealItemData _healItem;
        [SerializeField] private WeaponShopItemData[] _weaponItems;

        public string GoButtonText => _goButtonText;
        public string ReviveButtonText => _reviveButtonText;
        public string SoldOutText => _soldOutText;
        public string NotAvailableText => _notAvailableText;
        public HealItemData HealItem => _healItem;
        public WeaponShopItemData[] WeaponItems => _weaponItems;
    }
}