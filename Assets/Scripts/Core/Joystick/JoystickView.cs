using UnityEngine;

namespace SwordHero.Core.Joystick
{
    public class JoystickView : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private RectTransform _joystickHandle;

        public RectTransform RectTransform => _rectTransform;

        public void SetPosition(Vector2 position) => _rectTransform.anchoredPosition = position;
        public void SetHandlePosition(Vector2 position) => _joystickHandle.anchoredPosition = position;
        public void SetActive(bool isActive) => gameObject.SetActive(isActive);
    }
}