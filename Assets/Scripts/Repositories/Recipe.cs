using UnityEngine;
using VContainer;

namespace SwordHero.Repositories
{
    public abstract class StandardRecipe : ScriptableObject
    {
        public abstract void Register(IContainerBuilder builder);
    }

    public abstract class PoolableRecipe : ScriptableObject
    {
        public abstract IPoolableRepository GetRepository();
    }
}