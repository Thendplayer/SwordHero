using System;
using Cinemachine;
using SwordHero.Core.UI;
using SwordHero.Core.Weapons.Adapters;
using UnityEngine;
using VContainer;

namespace SwordHero.Core.Pawn
{
    public class PawnView : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _weaponHolder;
        [SerializeField] private LifeDisplayView _lifeDisplay;
        [Inject] private CinemachineVirtualCamera _virtualCamera;
        
        public Vector3 EulerAngles => _rigidbody.transform.eulerAngles;
        public Action OnCollisionTriggered;

        private void Awake()
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | 
                                     RigidbodyConstraints.FreezeRotationZ |
                                     RigidbodyConstraints.FreezePositionY;
        }

        private void OnCollisionEnter(Collision collision)
        {
            var collisionRigidbody = collision.rigidbody;
            if (collisionRigidbody == null || collisionRigidbody.isKinematic) return;
            OnCollisionTriggered?.Invoke();
        }

        public void AddWeapon(IWeapon weapon) => weapon.Instantiate(_weaponHolder);
        public void RemoveWeapon()
        {
            if (_weaponHolder != null && _weaponHolder.childCount > 0)
            {
                Destroy(_weaponHolder.GetChild(0).gameObject);
            }
        }

        public void UpdateLifeDisplay(float hp)
        {
            if (!_lifeDisplay.IsActive())
                _lifeDisplay.Activate();
            
            _lifeDisplay.UpdateLife(hp);
        }

        public void SetAsCameraTarget() => _virtualCamera.Follow = transform;
        public void MovePosition(Vector3 direction) => _rigidbody.MovePosition(_rigidbody.position + direction);
        public void MoveRotation(Vector3 rotation) => _rigidbody.MoveRotation(Quaternion.Euler(rotation));
        public void UpdateBoolAnimationParameter(int parameterHash, bool value) => _animator.SetBool(parameterHash, value);
        public void TriggerAnimationParameter(int parameterHash) => _animator.SetTrigger(parameterHash);
        public void UpdateAnimatorSpeed(float speed) => _animator.speed = speed;
        
        public void ResetAnimatorToEntry()
        {
            _animator.Rebind();
            _animator.Update(0f);
        }
    }
}