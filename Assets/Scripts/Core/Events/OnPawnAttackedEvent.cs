using SwordHero.Core.Pawn.Adapters;

namespace SwordHero.Core.Events
{
    public struct OnPawnAttackedEvent
    {
        public IPhysicsBody Target { get; }
        public int Damage { get; }
        
        public OnPawnAttackedEvent(IPhysicsBody target, int damage)
        {
            Target = target;
            Damage = damage;
        }
    }
}