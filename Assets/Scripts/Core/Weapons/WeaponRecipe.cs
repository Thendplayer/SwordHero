using UnityEngine;

namespace SwordHero.Core.Weapons
{
    [CreateAssetMenu(menuName = "Recipes/Weapon", fileName = "WeaponRecipe")]
    public class WeaponRecipe : ScriptableObject
    {
        [SerializeField] private WeaponData _data;

        public WeaponData Data => _data;
    }
}