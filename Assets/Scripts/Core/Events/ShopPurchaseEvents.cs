using SwordHero.Core.Weapons;

namespace SwordHero.Core.Events
{
    public struct HealItemPurchasedEvent
    {
        public int HealAmount { get; }
        public int Price { get; }

        public HealItemPurchasedEvent(int healAmount, int price)
        {
            HealAmount = healAmount;
            Price = price;
        }
    }

    public struct WeaponItemPurchasedEvent
    {
        public WeaponRecipe WeaponRecipe { get; }
        public int Price { get; }

        public WeaponItemPurchasedEvent(WeaponRecipe weaponRecipe, int price)
        {
            WeaponRecipe = weaponRecipe;
            Price = price;
        }
    }
}