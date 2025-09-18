namespace SwordHero.Core.GameLoop
{
    public enum GameState
    {
        Start,
        Menu,
        InGame
    }

    public enum ShopSlot
    {
        Heal = 0,
        Weapon = 1
    }

    public class GameLoopModel
    {
        private readonly GameLoopData _data;
        private GameState _currentState;
        private bool _isDead;
        private int _gold;
        private int _currentWeaponIndex;
        private bool _isHealItemAvailable;

        public GameState CurrentState => _currentState;
        public bool IsDead => _isDead;
        public int Gold => _gold;
        public GameLoopData Data => _data;

        public GameLoopModel(GameLoopData data)
        {
            _data = data;
            _currentState = GameState.Start;
            _isDead = false;
            _gold = 0;
            _currentWeaponIndex = 0;
            _isHealItemAvailable = true;
        }

        public void StartGame()
        {
            _currentState = GameState.InGame;
            _isDead = false;
        }

        public void ResetPurchases()
        {
            _currentWeaponIndex = 0;
            _isHealItemAvailable = true;
        }
        
        public void OpenMenu()
        {
            _currentState = GameState.Menu;
            _isDead = false;
        }

        public void EndGame(bool playerDied)
        {
            _currentState = GameState.Start;
            _isDead = playerDied;
            _gold = 0;
        }
        
        public string GetPlayButtonText() => _isDead ? _data.ReviveButtonText : _data.GoButtonText;

        public void AddGold(int amount) => _gold += amount;
        
        public bool IsCurrentWeaponAvailable() => _currentWeaponIndex < _data.WeaponItems.Length;
        
        public WeaponShopItemData GetCurrentWeapon()
        {
            return !IsCurrentWeaponAvailable() ? null : _data.WeaponItems[_currentWeaponIndex];
        }

        public int GetCurrentWeaponPrice()
        {
            var currentWeapon = GetCurrentWeapon();
            return currentWeapon?.ItemPrice ?? 0;
        }
        
        public bool CanAffordSlot(ShopSlot slot)
        {
            switch (slot)
            {
                case ShopSlot.Heal:
                    return _isHealItemAvailable && _gold >= _data.HealItem.ItemPrice;
                case ShopSlot.Weapon:
                    return IsCurrentWeaponAvailable() && _gold >= GetCurrentWeaponPrice();
                default:
                    return false;
            }
        }

        public bool PurchaseSlot(ShopSlot slot)
        {
            if (!CanAffordSlot(slot))
                return false;

            switch (slot)
            {
                case ShopSlot.Heal:
                    _gold -= _data.HealItem.ItemPrice;
                    return true;
                case ShopSlot.Weapon:
                    _gold -= GetCurrentWeaponPrice();
                    _currentWeaponIndex++;
                    return true;
                default:
                    return false;
            }
        }
    }
}