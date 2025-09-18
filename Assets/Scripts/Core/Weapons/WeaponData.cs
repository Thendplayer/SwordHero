using System;
using JetBrains.Annotations;
using SwordHero.Core.Weapons.Adapters;
using UnityEngine;

namespace SwordHero.Core.Weapons
{
    [Serializable]
    public class WeaponData
    {
        [SerializeField] [CanBeNull] private WeaponAdapter _weapon;
        [SerializeField] private int _damage;
        [SerializeField] private float _movementSpeed;
        [SerializeField] private float _animationSpeed = 1;
        [SerializeField] private float _hitRadius;
        [SerializeField] private float _attackCooldown;

        public IWeapon Weapon => _weapon;
        public int Damage => _damage;
        public float MovementSpeed => _movementSpeed;
        public float AnimationSpeed => _animationSpeed;
        public float HitRadius => _hitRadius;
        public float AttackCooldown => _attackCooldown;
    }
}