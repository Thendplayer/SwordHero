namespace SwordHero.Core.Events
{
    public struct GoldEarnedEvent
    {
        public int Amount { get; }

        public GoldEarnedEvent(int amount)
        {
            Amount = amount;
        }
    }
}