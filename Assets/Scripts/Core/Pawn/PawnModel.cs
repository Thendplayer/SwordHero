using JetBrains.Annotations;
using SwordHero.Core.Pawn.Adapters;
using SwordHero.Core.Weapons;
using SwordHero.Core.Weapons.Adapters;
using UnityEngine;

namespace SwordHero.Core.Pawn
{
    public class PawnModel
    {
        private readonly PawnData _originalData;
        private readonly IPhysicsBody _physicsBody;
        private readonly float _minAngleForRotation;
        private readonly float _rotationSmoothTime;
        
        private Vector3 _velocity;
        private float _maxHealth;
        private float _rotationSpeed;
        private bool _isMoving;
        private float _lastAttackTime;

        public Vector3 Velocity => _velocity;
        public Vector3 Position => _physicsBody.Position;
        public bool IsMoving => _isMoving;
        public bool IsDead => CurrentHealth <= 0;
        public int GoldValue => _originalData.GoldValue;
        
        [CanBeNull] public IWeapon CurrentWeapon { get; private set; }
        public int CurrentHealth { get; private set; }
        public float MovementSpeed { get; private set; }
        public float AnimationSpeed { get; private set; }
        public float HitRadius { get; private set; }
        public int DamagePerHit { get; private set; }
        public float AttackCooldown { get; private set; }

        public PawnModel(PawnData data, IPhysicsBody physicsBody)
        {
            _originalData = data;
            _minAngleForRotation = data.MinAngleForRotation;
            _rotationSmoothTime = data.RotationSmoothTime;

            _physicsBody = physicsBody;

            Reset(data);
        }
        
        public void Reset() => Reset(_originalData);

        public void Reset(PawnData data)
        {
            _rotationSpeed = 0f;
            _velocity = Vector3.zero;
            _isMoving = false;
            _lastAttackTime = Time.time;

            _maxHealth = CurrentHealth = data.BaseHealthPoints;
            SetNewWeapon(data.InitialWeaponData);
            SetPosition(new Vector3(0, Position.y, 0));
        }
        
        public void SetNewWeapon(WeaponData weaponData)
        {
            CurrentWeapon = weaponData.Weapon;
            MovementSpeed = weaponData.MovementSpeed;
            AnimationSpeed = weaponData.AnimationSpeed;
            HitRadius = weaponData.HitRadius;
            DamagePerHit = weaponData.Damage;
            AttackCooldown = weaponData.AttackCooldown;
        }
        
        public void SetPosition(Vector3 position) => _physicsBody.SetPosition(position);
        
        public void SetVelocity(Vector2 direction, float magnitude = 1)
        {
            var input = direction * magnitude;
            var inputDirection = new Vector3(-input.x, 0f, -input.y);
            _velocity = inputDirection * MovementSpeed;
        }
        
        public void SetMoving(bool isMoving)
        {
            _isMoving = isMoving;
            if (!isMoving)
            {
                _velocity = Vector3.zero;
                _rotationSpeed = 0f;
            }
        }

        public float CalculateRotationAngle(float currentAngle, float targetAngle)
        {
            var angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle));
            if (!(angleDifference > _minAngleForRotation)) return 0;

            return Mathf.SmoothDampAngle(
                currentAngle,
                targetAngle,
                ref _rotationSpeed,
                _rotationSmoothTime
            );
        }

        public bool IsEqual(IPhysicsBody physicsBody) => _physicsBody.Equals(physicsBody);

        public void AddHealthPoints(int addition)
        {
            CurrentHealth += addition;
            if (CurrentHealth > _maxHealth)
                _maxHealth = CurrentHealth;
        }

        public void SubtractHealthPoints(int addition)
        {
            CurrentHealth -= addition;
            if (CurrentHealth <= 0) 
                CurrentHealth = 0;
        }

        public float GetHealthRatio() => CurrentHealth / _maxHealth;

        public bool CanAttack() => Time.time >= _lastAttackTime + AttackCooldown;

        public void RecordAttack() => _lastAttackTime = Time.time;
    }
}