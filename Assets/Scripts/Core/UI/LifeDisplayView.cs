using UnityEngine;
using UnityEngine.UI;

namespace SwordHero.Core.UI
{
    [RequireComponent(typeof(Canvas))]
    public class LifeDisplayView : MonoBehaviour
    {
        [SerializeField] private Image _lifeBar;
        private Camera _camera;
        
        private Quaternion _lastCameraRotation;

        private void Awake()
        {
            _camera = Camera.main;
            GetComponent<Canvas>().worldCamera = _camera;
        }
        
        // TODO: This runs every frame and is expensive - should cache and only update when camera/position changes
        private void LateUpdate()
        {
            if (_camera == null) return;
            transform.LookAt(transform.position + _camera.transform.rotation * Vector3.back, Vector3.up);
        }

        public bool IsActive() => gameObject.activeSelf;
        public void Activate() => gameObject.SetActive(true);
        
        public void UpdateLife(float hp)
        {
            _lifeBar.fillAmount = hp;
            if (hp >= 1 || hp <= 0) gameObject.SetActive(false);
        }
    }
}