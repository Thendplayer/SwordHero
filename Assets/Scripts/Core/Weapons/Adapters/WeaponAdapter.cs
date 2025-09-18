using UnityEngine;

namespace SwordHero.Core.Weapons.Adapters
{
    public interface IWeapon
    {
        void Instantiate(Transform parent);
    }

    public class WeaponAdapter : MonoBehaviour, IWeapon
    {
        public void Instantiate(Transform parent) => Instantiate(this, parent);
    }
}