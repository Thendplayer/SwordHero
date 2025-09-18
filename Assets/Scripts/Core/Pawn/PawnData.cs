using System;
using SwordHero.Core.Weapons;
using UnityEngine;

namespace SwordHero.Core.Pawn
{
    [Serializable]
    public class PawnData
    {
        public static class AnimatorParameters
        {
            public static readonly int Move = Animator.StringToHash("Move");
            public static readonly int Attack = Animator.StringToHash("Attack");
            public static readonly int Hit = Animator.StringToHash("Hit");
            public static readonly int Die = Animator.StringToHash("Die");
        }

        [Header("Rotation")]
        [SerializeField] private float _rotationSmoothTime = 0.06f;
        [SerializeField] private float _minAngleForRotation = 5f;
        
        [Header("Stats")]
        [SerializeField] private int _baseHealthPoints = 10;
        [SerializeField] private int _goldValue = 50;
        
        [Header("Weapon")]
        [SerializeField] private WeaponRecipe _initialWeaponRecipe;

        public float MinAngleForRotation => _minAngleForRotation;
        public float RotationSmoothTime => _rotationSmoothTime;
        public int BaseHealthPoints => _baseHealthPoints;
        public int GoldValue => _goldValue;
        public WeaponData InitialWeaponData => _initialWeaponRecipe.Data;
    }
}